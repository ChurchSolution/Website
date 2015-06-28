// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BibleChapter.cs" company="Church">
//   Copyright (c) Rui Min. All rights reserved.
// </copyright>
// <summary>
//   Defines the BibleChapter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides Bible chapter structure.
    /// </summary>
    public class BibleChapter
    {
        /// <summary>
        /// Gets or sets the Bible id.
        /// </summary>
        public Guid BibleId { get; set; }

        /// <summary>
        /// Gets or sets the book abbreviation.
        /// </summary>
        public string BookAbbreviation { get; set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the verses.
        /// </summary>
        public IEnumerable<BibleVerse> Verses { get; set; }
    }
}
