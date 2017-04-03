// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BulletinTextBuilder.cs" company="Church">
//   Copyright (c) Rui Min. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models
{
    using Church.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    public abstract class BulletinTextBuilder
    {
        protected Dictionary<string, string> handlers;

        protected BulletinTextBuilder(CultureInfo culture)
        {
            this.handlers = new Dictionary<string, string>();
            this.Culture = culture;

            var members = this.GetType().GetMembers(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.GetCustomAttributes(typeof(SectionSeparatorAttribute), false).Any());
            foreach (var m in members)
            {
                var attribute = m.GetCustomAttribute(typeof(SectionSeparatorAttribute), false) as SectionSeparatorAttribute;
                Trace.Assert(attribute != null, m.Name + " has none of SectionSeparatorAttribute.");
                attribute.StartingStrings.ToList().ForEach(s => this.handlers.Add(s, m.Name));
            }
        }

        public CultureInfo Culture { get; private set; }

        public virtual T Make<T>(string plainText) where T : IWeeklyBulletinProperties, new()
        {
            var lines = plainText.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.None);

            var properties = new T();
            this.Fill(properties, lines);
            ExceptionUtilities.ThrowFormatExceptionIfFalse(properties.Verify(), "Could not load all necessary properties.");

            return properties;
        }

        protected void Fill(IWeeklyBulletinProperties properties, IEnumerable<string> lines)
        {
            var lineList = new List<string>();
            string handler = null;
            string nextHandler = null;
            foreach (var line in lines)
            {
                if ((nextHandler = this.GetMethod(line)) != null)
                {
                    if (handler != null)
                    {
                        var remaining = this.ProcessSection(handler, properties, lineList);
                        if (remaining.Any())
                        {
                            this.Fill(properties, remaining);
                        }
                        else
                        {
                            handler = null;
                        }
                        lineList.Clear();
                    }

                    handler = nextHandler;
                }

                lineList.Add(line);
            }

            if (handler != null)
            {
                this.ProcessSection(handler, properties, lineList);
            }
        }

        protected static decimal ParseNumber(string numberInString)
        {
            Trace.Assert(null != numberInString);

            decimal res;
            if (!decimal.TryParse(numberInString, out res))
            {
                res = 0;
            }

            return res;
        }

        protected static decimal ParseMoney(string moneyInString)
        {
            Trace.Assert(null != moneyInString);

            var numberInString = moneyInString.TrimStart('$').Replace(",", String.Empty);

            return ParseNumber(numberInString);
        }

        protected IEnumerable<string> ProcessWordTable(IEnumerable<string> lines, Func<string, string> preprocess, int numberOfCells, Action<string[]> processCells)
        {
            Debug.Assert(null != lines);
            string allLines = string.Join("\n", lines);
            if (null != preprocess)
            {
                allLines = preprocess(allLines);
            }

            string[] allCells = allLines.Split('\r');
            int totalNumber = (allCells.Length - 1) / numberOfCells * numberOfCells;

            int order = 0;
            string[] cells = new string[numberOfCells];
            int loop = 0;
            for (; loop < totalNumber; loop++)
            {
                cells[order] = allCells[loop].Trim().Replace("\n", "\r\n");
                if (++order == numberOfCells)
                {
                    processCells(cells);
                    order = 0;
                }
            }

            List<string> lstCell = new List<string>();
            bool hasMore = false;
            while (loop < allCells.Length)
            {
                if (hasMore)
                {
                    lstCell.AddRange(allCells[loop].Split('\n'));
                }
                else if (!string.IsNullOrEmpty(allCells[loop].Trim()))
                {
                    hasMore = true;
                    lstCell.AddRange(allCells[loop].Split('\n'));
                }
                loop++;
            }
            return lstCell;
        }

        private string GetMethod(string line)
        {
            // TODO: For better performance, we'd better use prefix tree
            foreach (var handler in this.handlers)
            {
                if (line.StartsWith(handler.Key))
                {
                    return handler.Value;
                }
            }

            return null;
        }

        private IEnumerable<string> ProcessSection(string methodName, IWeeklyBulletinProperties properties, IList<string> lines)
        {
            try
            {
                return this.GetType().InvokeMember(methodName,
                         BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod,
                         null, this, new object[] { properties, lines }) as IEnumerable<string>;
            }
            catch (Exception exp)
            {
                throw new FormatException(
                    string.Format("Found errors in executing '{0}' while parsing '{1}'.", methodName, string.Join("\n", lines)),
                    exp);
            }
        }
    }
}
