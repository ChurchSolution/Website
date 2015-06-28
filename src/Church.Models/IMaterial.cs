// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMaterial.cs" company="Church">
//   Copyright (c) Rui Min. All rights reserved.
// </copyright>
// <summary>
//   Defines the IMaterial type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models
{
    using System;

    /// <summary>
    /// Provides an interface for Material instance.
    /// </summary>
    public interface IMaterial
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
        /// Gets the authors.
        /// </summary>
        string Authors { get; }

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
