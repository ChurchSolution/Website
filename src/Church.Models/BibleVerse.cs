// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BibleVerse.cs" company="Church">
//   Copyright (c) Rui Min. All rights reserved.
// </copyright>
// <summary>
//   Defines the BibleVerse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models
{
    using System;

    /// <summary>
    /// Provides Bible verse structure.
    /// </summary>
    public class BibleVerse
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
        /// Gets or sets the chapter order.
        /// </summary>
        public int ChapterOrder { get; set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text { get; set; }
    }
}
