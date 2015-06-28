// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BibleBook.cs" company="Church">
//   Copyright (c) Rui Min. All rights reserved.
// </copyright>
// <summary>
//   Defines the BibleBook type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides the Bible book structure.
    /// </summary>
    public class BibleBook
    {
        /// <summary>
        /// Gets or sets the Bible id.
        /// </summary>
        public Guid BibleId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the abbreviation.
        /// </summary>
        public string Abbreviation { get; set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the chapters.
        /// </summary>
        public IEnumerable<BibleChapter> Chapters { get; set; }
    }
}
