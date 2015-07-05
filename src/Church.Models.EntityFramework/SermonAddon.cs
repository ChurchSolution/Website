// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SermonAddon.cs" company="Church">
//   Copyright (c) Rui Min. All rights reserved.
// </copyright>
// <summary>
//   Defines the Sermon type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models.EntityFramework
{
    /// <summary>
    /// Provides the sermon class for sermon model.
    /// </summary>
    public partial class Sermon
    {
        /// <summary>
        /// Creates a sermon from the Sermon model.
        /// </summary>
        /// <param name="sermon">The sermon.</param>
        /// <returns>The <see cref="Sermon"/>.</returns>
        public static Sermon Create(Models.Sermon sermon)
        {
            return new Sermon
                       {
                           Id = sermon.Id,
                           Type = sermon.Type,
                           Date = sermon.Date,
                           Speaker = sermon.Speaker,
                           Title = sermon.Title,
                           FileUrl = sermon.FileUrl
                       };
        }

        /// <summary>
        /// Converts to Sermon model class.
        /// </summary>
        /// <returns>The <see cref="Models.Sermon"/>.</returns>
        public Models.Sermon ToModel()
        {
            return new Models.Sermon
            {
                Id = this.Id,
                Type = this.Type,
                Date = this.Date,
                Speaker = this.Speaker,
                Title = this.Title,
                FileUrl = this.FileUrl
            };
        }
    }
}
