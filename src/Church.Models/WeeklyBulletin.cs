// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeeklyBulletin.cs" company="Church">
//   Copyright (c) Rui Min. All rights reserved.
// </copyright>
// <summary>
//   Defines the weekly bulletin.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Provides the weekly bulletin.
    /// </summary>
    public class WeeklyBulletin
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="WeeklyBulletin"/> class from being created.
        /// </summary>
        private WeeklyBulletin()
        {
        }

        /// <summary>
        /// Gets the date.
        /// </summary>
        public DateTime Date { get; private set; }

        /// <summary>
        /// Gets the file uri.
        /// </summary>
        public string FileUri { get; private set; }

        /// <summary>
        /// Gets the culture.
        /// </summary>
        public CultureInfo Culture { get; private set; }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        public IWeeklyBulletinProperties Properties { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeeklyBulletin"/> class.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="fileUri">The file uri.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="properties">The properties.</param>
        /// <returns>The <see cref="WeeklyBulletin"/>.</returns>
        public static WeeklyBulletin Create(DateTime date, string fileUri, CultureInfo culture, IWeeklyBulletinProperties properties)
        {
            return new WeeklyBulletin
            {
                Date = date,
                FileUri = fileUri,
                Culture = culture,
                Properties = properties,
            };
        }
    }
}
