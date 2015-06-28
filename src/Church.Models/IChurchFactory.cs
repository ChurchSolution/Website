// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IChurchFactory.cs" company="Church">
//     Copyright (c) Rui Min. All rights reserved.
// </copyright>
// <summary>
//   Defines the interface for Churchs.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models
{
    using System;

    /// <summary>
    /// Provides the interface for church.
    /// </summary>
    public interface IChurchFactory
    {
        /// <summary>
        /// Creates a bulletin from bulletin text.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="date">The date.</param>
        /// <param name="fileUri">The file uri.</param>
        /// <param name="plainText"> The plain text.</param>
        /// <returns> The <see cref="WeeklyBulletin"/>.</returns>
        WeeklyBulletin CreateBulletin(string culture, DateTime date, string fileUri, string plainText);
    }
}
