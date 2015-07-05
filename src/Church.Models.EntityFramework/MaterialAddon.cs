// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MaterialAddon.cs" company="Church">
//   Copyright (c) Rui Min. All rights reserved.
// </copyright>
// <summary>
//   Defines the Material type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models.EntityFramework
{
    /// <summary>
    /// Provides the sermon class from Material model.
    /// </summary>
    public partial class Material
    {
        /// <summary>
        /// Creates a material from the Sermon model.
        /// </summary>
        /// <param name="material">The material.</param>
        /// <returns>The <see cref="Sermon"/>.</returns>
        public static Material Create(Models.Material material)
        {
            return new Material
            {
                Id = material.Id,
                Type = material.Type,
                Date = material.Date,
                Authors = material.Authors,
                Title = material.Title,
                FileUrl = material.FileUrl
            };
        }

        /// <summary>
        /// Converts to Material model class.
        /// </summary>
        /// <returns>The <see cref="Models.Material"/>.</returns>
        public Models.Material ToModel()
        {
            return new Models.Material
            {
                Id = this.Id,
                Type = this.Type,
                Date = this.Date,
                Authors = this.Authors,
                Title = this.Title,
                FileUrl = this.FileUrl
            };
        }
    }
}
