// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Hymn.cs" company="Church">
//   Copyright (c) Rui Min. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models
{
    using System;

    /// <summary>
    /// Provides the Hymn model.
    /// </summary>
    public class Hymn
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the lyrics.
        /// </summary>
        public string Lyrics { get; set; }

        /// <summary>
        /// Gets or sets the links.
        /// </summary>
        public string Links { get; set; }

        /// <summary>
        /// Gets or sets the culture.
        /// </summary>
        public string Culture { get; set; }
    }
}
