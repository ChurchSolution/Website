// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Repository.cs" company="Church">
//   Copyright (c) Rui Min. All rights reserved.
// </copyright>
// <summary>
//   Defines the Repository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides the implementation of the repository interface.
    /// </summary>
    public class Repository : IRepository, IDisposable
    {
        /// <summary>
        /// The entities.
        /// </summary>
        private readonly FrameworkEntities entities;

        /// <summary>
        /// The factory.
        /// </summary>
        private readonly Lazy<IChurchFactory> factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository"/> class.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <param name="factory">The factory.</param>
        internal Repository(FrameworkEntities entities, Lazy<IChurchFactory> factory)
        {
            this.entities = entities;
            this.factory = factory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="dllName">The DLL name.</param>
        /// <returns>The <see cref="Repository"/>.</returns>
        public static Repository Create(string connectionString, string dllName)
        {
            var entities = new FrameworkEntities(connectionString);
            var factory = new Lazy<IChurchFactory>(() => CreateFactory(dllName));

            return new Repository(entities, factory);
        }

        /// <summary>
        /// Disposes the resources used by the repository.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        public Task<IQueryable<WeeklyBulletin>> GetBulletinsAsync()
        {
            return Task.FromResult(this.entities.Bulletins.Select(b => GetBulletin(b)));
        }

        /// <summary>
        /// Gets the default bulletin.
        /// </summary>
        /// <param name="cultureName">The culture name.</param>
        /// <returns>The <see cref="WeeklyBulletin"/>.</returns>
        public async Task<WeeklyBulletin> GetDefaultBulletinAsync(string cultureName)
        {
            var bulletin =
                await
                Task.Run(
                    () =>
                    this.entities.Bulletins.OrderByDescending(b => b.Date)
                        .FirstOrDefault(b => cultureName.Equals(b.Culture, StringComparison.Ordinal))
                    ?? this.entities.Bulletins.OrderByDescending(b => b.Date).FirstOrDefault());

            return GetBulletin(bulletin);
        }

        public async Task<WeeklyBulletin> GetBulletinByDateAsync(string cultureName, DateTime date)
        {
            var bulletin =
                await
                Task.Run(
                    () =>
                    this.entities.Bulletins.FirstOrDefault(
                        b => b.Date == date && cultureName.Equals(b.Culture, StringComparison.Ordinal)));

            return GetBulletin(bulletin);
        }

        public async Task<WeeklyBulletin> AddOrUpdateBulletinAsync(
            DateTime date,
            string fileUrl,
            string planText,
            string culture)
        {
            var bulletin = this.entities.Bulletins.FirstOrDefault(b => b.Date == date && b.Culture == culture);
            if (null == bulletin)
            {
                bulletin = new Bulletin
                               {
                                   PlainText = planText,
                                   Date = date,
                                   Culture = CultureInfo.CurrentUICulture.Name,
                                   Id = Guid.NewGuid(),
                                   FileUrl = fileUrl,
                               };
                this.entities.Bulletins.Add(bulletin);
            }
            else
            {
                bulletin.PlainText = planText;
                bulletin.FileUrl = fileUrl;
                this.entities.Entry(bulletin).State = EntityState.Modified;
            }

            await this.entities.SaveChangesAsync();

            return this.GetBulletin(bulletin);
        }

        public IQueryable<IIncident> GetIncidents(DateTime date)
        {
            return this.entities.Incidents.Where(i => i.Time > date);
        }

        #region Sermon

        /// <summary>
        /// Gets the list of sermons.
        /// </summary>
        /// <returns> The <see cref="IQueryable{ISermon}"/>.</returns>
        public IQueryable<Models.Sermon> GetSermons()
        {
            return
                this.entities.Sermons.Select(
                    sermon =>
                    new Models.Sermon
                        {
                            Id = sermon.Id,
                            Type = sermon.Type,
                            Date = sermon.Date,
                            Speaker = sermon.Speaker,
                            Title = sermon.Title,
                            FileUrl = sermon.FileUrl
                        });
        }

        /// <summary>
        /// Adds a sermon.
        /// </summary>
        /// <param name="sermon">The sermon.</param>
        /// <returns>The <see cref="Task{ISermon}"/>.</returns>
        public async Task<Models.Sermon> AddSermonAsync(Models.Sermon sermon)
        {
            var newSermon = Sermon.Create(sermon);
            this.entities.Sermons.Add(newSermon);
            await this.entities.SaveChangesAsync();

            return newSermon.ToModel();
        }

        /// <summary>
        /// Updates a sermon.
        /// </summary>
        /// <param name="sermon">The sermon.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task UpdateSermonAsync(Models.Sermon sermon)
        {
            if (sermon == null)
            {
                throw new ArgumentNullException("sermon");
            }

            var oldSermon = await this.GetSermonAsync(sermon.Id);

            oldSermon.Type = sermon.Type;
            oldSermon.Date = sermon.Date;
            oldSermon.Speaker = sermon.Speaker;
            oldSermon.Title = sermon.Title;
            oldSermon.FileUrl = sermon.FileUrl;
            await this.entities.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a sermon.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task DeleteSermonAsync(Guid id)
        {
            var sermon = await this.GetSermonAsync(id);

            this.entities.Sermons.Remove(sermon);
            await this.entities.SaveChangesAsync();
        }

        #endregion

        #region Material

        /// <summary>
        /// Gets the list of materials.
        /// </summary>
        /// <returns> The <see cref="IQueryable{IMaterial}"/>.</returns>
        public IQueryable<Models.Material> GetMaterials()
        {
            return
                this.entities.Materials.Select(
                    material =>
                    new Models.Material
                        {
                            Id = material.Id,
                            Type = material.Type,
                            Date = material.Date,
                            Authors = material.Authors,
                            Title = material.Title,
                            FileUrl = material.FileUrl
                        });
        }

        /// <summary>
        /// Adds a material.
        /// </summary>
        /// <param name="material">The material.</param>
        /// <returns>The <see cref="Task{IMaterial}"/>.</returns>
        public async Task<Models.Material> AddMaterialAsync(Models.Material material)
        {
            var newMaterial = Material.Create(material);
            this.entities.Materials.Add(newMaterial);
            await this.entities.SaveChangesAsync();

            return newMaterial.ToModel();
        }

        /// <summary>
        /// Updates a material.
        /// </summary>
        /// <param name="material">The material.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task UpdateMaterialAsync(Models.Material material)
        {
            if (material == null)
            {
                throw new ArgumentNullException("material");
            }

            var oldMaterial = await this.GetMaterialAsync(material.Id);

            oldMaterial.Type = material.Type;
            oldMaterial.Date = material.Date;
            oldMaterial.Authors = material.Authors;
            oldMaterial.Title = material.Title;
            oldMaterial.FileUrl = material.FileUrl;
            await this.entities.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a material.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task DeleteMaterialAsync(Guid id)
        {
            var material = await this.GetMaterialAsync(id);

            this.entities.Materials.Remove(material);
            await this.entities.SaveChangesAsync();
        }

        #endregion

        public async Task<IHymn> AddHymnAsync(IHymn hymn)
        {
            var newHymn = new Hymn
                              {
                                  Culture = hymn.Culture,
                                  Id = Guid.NewGuid(),
                                  Lyrics = hymn.Lyrics,
                                  Links = hymn.Links,
                                  Name = hymn.Name,
                                  Source = hymn.Source,
                              };
            this.entities.Hymns.Add(newHymn);
            await this.entities.SaveChangesAsync();

            return newHymn;
        }

        public IQueryable<IHymn> GetHymns()
        {
            return this.entities.Hymns;
        }

        public async Task UpdateHymnsAsync(IHymn hymn)
        {
            if (hymn == null)
            {
                throw new ArgumentNullException("hymn");
            }

            var oldHymn = await this.GetHymnAsync(hymn.Id);

            oldHymn.Name = hymn.Name;
            oldHymn.Source = hymn.Source;
            oldHymn.Culture = hymn.Culture;
            oldHymn.Links = hymn.Links;
            oldHymn.Lyrics = hymn.Lyrics;
            await this.entities.SaveChangesAsync();
        }

        public async Task DeleteHymnsAsync(Guid id)
        {
            var hymn = await this.GetHymnAsync(id);

            this.entities.Hymns.Remove(hymn);
            await this.entities.SaveChangesAsync();
        }

        /// <summary>
        /// Implements the dispose pattern.
        /// </summary>
        /// <param name="disposing">A value indicating whether the managed resources should be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.entities.Dispose();
            }
        }

        /// <summary>
        /// Creates the church factory.
        /// </summary>
        /// <param name="dllName">The DLL name.</param>
        /// <returns>The <see cref="IChurchFactory"/>.</returns>
        private static IChurchFactory CreateFactory(string dllName)
        {
            // Read from disk if the assembly is not loaded yet
            var assembly =
                AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name.Equals(dllName))
                ?? Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, dllName) + ".dll");
            var type =
                assembly.GetTypes()
                    .First(t => typeof(IChurchFactory).IsAssignableFrom(t) && 0 == (TypeAttributes.Interface & t.Attributes));
            return Activator.CreateInstance(type) as IChurchFactory;
        }

        /// <summary>
        /// Gets the weekly bulletin from the stored bulletin.
        /// </summary>
        /// <param name="bulletin">The bulletin.</param>
        /// <returns>The <see cref="WeeklyBulletin"/>.</returns>
        private WeeklyBulletin GetBulletin(Bulletin bulletin)
        {
            if (null == bulletin)
            {
                return null;
            }

            var weeklyBulletin = this.factory.Value.CreateBulletin(
                bulletin.Culture,
                bulletin.Date,
                bulletin.FileUrl,
                bulletin.PlainText);
            return weeklyBulletin;
        }

        private async Task<Sermon> GetSermonAsync(Guid id)
        {
            var sermon = await this.entities.Sermons.SingleOrDefaultAsync(h => h.Id.Equals(id));
            if (sermon == null)
            {
                throw new KeyNotFoundException(string.Format("Cannot find the sermon '{0}", id));
            }

            return sermon;
        }

        private async Task<Material> GetMaterialAsync(Guid id)
        {
            var material = await this.entities.Materials.SingleOrDefaultAsync(h => h.Id.Equals(id));
            if (material == null)
            {
                throw new KeyNotFoundException(string.Format("Cannot find the material '{0}", id));
            }

            return material;
        }

        private async Task<Hymn> GetHymnAsync(Guid id)
        {
            var hymn = await this.entities.Hymns.SingleOrDefaultAsync(h => h.Id.Equals(id));
            if (hymn == null)
            {
                throw new KeyNotFoundException(string.Format("Cannot find the hymn '{0}", id));
            }

            return hymn;
        }
    }
}
