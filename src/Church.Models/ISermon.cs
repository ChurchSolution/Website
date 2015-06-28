// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISermon.cs" company="Church">
//   Copyright (c) Rui Min. All rights reserved.
// </copyright>
// <summary>
//   Defines the ISermon type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models
{
    using System;

    /// <summary>
    /// Provides an interface for Sermon instance.
    /// </summary>
    public interface ISermon
    {
        /// <summary>
        /// Gets the id.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets the date.
        /// </summary>
        DateTime? Date { get; }

        /// <summary>
        /// Gets the speaker.
        /// </summary>
        string Speaker { get; }

        /// <summary>
        /// Gets the title.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets the file url.
        /// </summary>
        string FileUrl { get; }
    }
}
