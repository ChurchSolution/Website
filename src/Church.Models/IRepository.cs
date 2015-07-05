// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepository.cs" company="Church">
//   Copyright (c) Rui Min. All rights reserved.
// </copyright>
// <summary>
//   Defines the IRepository interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IRepository
    {
        Task<IQueryable<WeeklyBulletin>> GetBulletinsAsync();

        /// <summary>
        /// Gets the default bulletin.
        /// </summary>
        /// <param name="cultureName">The culture name.</param>
        /// <returns>The <see cref="WeeklyBulletin"/>.</returns>
        Task<WeeklyBulletin> GetDefaultBulletinAsync(string cultureName);

        Task<WeeklyBulletin> GetBulletinByDateAsync(string cultureName, DateTime date);

        Task<WeeklyBulletin> AddOrUpdateBulletinAsync(DateTime date, string fileUrl, string planText, string culture);

        /// <summary>
        /// Gets a list of incidents.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>The <see cref="IQueryable"/> of <see cref="IIncident"/>.</returns>
        IQueryable<IIncident> GetIncidents(DateTime date);

        #region Sermon

        /// <summary>
        /// Gets the list of sermons.
        /// </summary>
        /// <returns> The <see cref="IQueryable{ISermon}"/>.</returns>
        IQueryable<Sermon> GetSermons();

        /// <summary>
        /// Adds a sermon.
        /// </summary>
        /// <param name="sermon">The sermon.</param>
        /// <returns>The <see cref="Task{ISermon}"/>.</returns>
        Task<Sermon> AddSermonAsync(Sermon sermon);

        /// <summary>
        /// Updates a sermon.
        /// </summary>
        /// <param name="sermon">The sermon.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UpdateSermonAsync(Sermon sermon);

        /// <summary>
        /// Deletes a sermon.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task DeleteSermonAsync(Guid id);

        #endregion

        #region Material

        /// <summary>
        /// Gets the list of materials.
        /// </summary>
        /// <returns> The <see cref="IQueryable{IMaterial}"/>.</returns>
        IQueryable<Material> GetMaterials();

        /// <summary>
        /// Adds a material.
        /// </summary>
        /// <param name="material">The material.</param>
        /// <returns>The <see cref="Task{IMaterial}"/>.</returns>
        Task<Material> AddMaterialAsync(Material material);

        /// <summary>
        /// Updates a material.
        /// </summary>
        /// <param name="material">The material.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UpdateMaterialAsync(Material material);

        /// <summary>
        /// Deletes a material.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task DeleteMaterialAsync(Guid id);

        #endregion

        #region Hymn

        IQueryable<Hymn> GetHymns();

        Task<Hymn> AddHymnAsync(Hymn hymn);

        Task UpdateHymnAsync(Hymn hymn);

        Task DeleteHymnAsync(Guid id);

        #endregion
    }
}
