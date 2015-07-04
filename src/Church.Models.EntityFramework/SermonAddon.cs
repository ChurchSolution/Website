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
    /// Provides the sermon class from ISermon interface.
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
    }
}
