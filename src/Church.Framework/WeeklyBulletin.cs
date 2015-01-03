//-----------------------------------------------------------------------------
// <copyright file="WeeklyBulletin.cs">
//     Copyright (c) Rui Min. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Church.Model
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    public class WeeklyBulletin
    {
        private WeeklyBulletin()
        {
        }

        public DateTime Date { get; private set; }

        public string FileUri { get; private set; }

        public CultureInfo Culture { get; private set; }

        public IWeeklyBulletinProperties Properties { get; private set; }

        public static WeeklyBulletin Create(DateTime date, string fileUri, CultureInfo culture, IWeeklyBulletinProperties properties)
        {
            return new WeeklyBulletin { Date = date, FileUri = fileUri, Culture = culture, Properties = properties };
        }
    }
}
