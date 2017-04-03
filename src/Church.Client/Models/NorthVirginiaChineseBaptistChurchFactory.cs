// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NorthVirginiaChineseBaptistChurchFactory.cs" company="Church">
//     Copyright (c) Rui Min. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models
{
    using Church.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Provides a factory for the north virginia chinese baptist church.
    /// </summary>
    public class NorthVirginiaChineseBaptistChurchFactory : IChurchFactory
    {
        /// <summary>
        /// The builders.
        /// </summary>
        private readonly IDictionary<string, BulletinTextBuilder> builders;

        /// <summary>
        /// Initializes a new instance of the <see cref="NorthVirginiaChineseBaptistChurchFactory"/> class.
        /// </summary>
        public NorthVirginiaChineseBaptistChurchFactory()
        {
            this.builders = new Dictionary<string, BulletinTextBuilder>(StringComparer.OrdinalIgnoreCase)
                                {
                                    {
                                        "zh-CN",
                                        new NorthVirginiaChineseBaptistChurchCnBulletinBuilder()
                                    },
                                    {
                                        "zh-TW",
                                        new NorthVirginiaChineseBaptistChurchTwBulletinBuilder()
                                    }
                                };
        }

        /// <summary>
        /// Creates a bulletin from bulletin text.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="date">The date.</param>
        /// <param name="fileUri">The file uri.</param>
        /// <param name="plainText">The plain text.</param>
        /// <returns> The <see cref="WeeklyBulletin"/>.</returns>
        public WeeklyBulletin CreateBulletin(string culture, DateTime date, string fileUri, string plainText)
        {
            ExceptionUtilities.ThrowArgumentNullExceptionIfEmpty(plainText, "plainText");

            ExceptionUtilities.ThrowFormatExceptionIfFalse(
                this.builders.TryGetValue(culture, out BulletinTextBuilder builder),
                "Could not handle the bulletin in the culture '{0}'",
                culture);

            var properties = builder.Make<NorthVirginiaChineseBaptistChurchBulletin>(plainText);
            ExceptionUtilities.ThrowFormatExceptionIfFalse(
                properties.Date == date,
                "The date entered ({0}) doesn't match the date on the bulletin ({1}).",
                properties.Date?.ToString(),
                date.ToShortDateString());

            return WeeklyBulletin.Create(date, fileUri, CultureInfo.CreateSpecificCulture(culture), properties);
        }
    }
}
