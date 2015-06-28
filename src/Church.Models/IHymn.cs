// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHymn.cs" company="Church">
//   Copyright (c) Rui Min. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models
{
    using System;

    /// <summary>
    /// Provides the Hymn interface.
    /// </summary>
    public interface IHymn
    {
        /// <summary>
        /// Gets the id.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the source.
        /// </summary>
        string Source { get; }

        /// <summary>
        /// Gets the lyrics.
        /// </summary>
        string Lyrics { get; }

        /// <summary>
        /// Gets the links.
        /// </summary>
        string Links { get; }

        /// <summary>
        /// Gets the culture.
        /// </summary>
        string Culture { get; }
    }
}
