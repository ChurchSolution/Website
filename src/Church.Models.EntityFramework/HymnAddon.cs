// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HymnAddon.cs" company="Church">
//   Copyright (c) Rui Min. All rights reserved.
// </copyright>
// <summary>
//   Defines the Hymn type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models.EntityFramework
{
    /// <summary>
    /// Provides the sermon class from Hymn model.
    /// </summary>
    public partial class Hymn
    {
        /// <summary>
        /// Creates a hymn from the Hymn model.
        /// </summary>
        /// <param name="hymn">The hymn.</param>
        /// <returns>The <see cref="Hymn"/>.</returns>
        public static Hymn Create(Models.Hymn hymn)
        {
            return new Hymn
            {
                Id = hymn.Id,
                Name = hymn.Name,
                Source = hymn.Source,
                Lyrics = hymn.Lyrics,
                Links = hymn.Links,
                Culture = hymn.Culture
            };
        }

        /// <summary>
        /// Converts to Hymn model class.
        /// </summary>
        /// <returns>The <see cref="Models.Hymn"/>.</returns>
        public Models.Hymn ToModel()
        {
            return new Models.Hymn
            {
                Id = this.Id,
                Name = this.Name,
                Source = this.Source,
                Lyrics = this.Lyrics,
                Links = this.Links,
                Culture = this.Culture
            };
        }
    }
}
