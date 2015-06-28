// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Bible.cs" company="Church">
//   Copyright (c) Rui Min. All rights reserved.
// </copyright>
// <summary>
//   Default the Bible class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides the Bible structure.
    /// </summary>
    public class Bible
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the culture.
        /// </summary>
        public string Culture { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the books.
        /// </summary>
        public IEnumerable<BibleBook> Books { get; set; }
    }
}