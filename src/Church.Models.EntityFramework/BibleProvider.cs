// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BibleProvider.cs" company="Church">
//   Copyright (c) Rui Min. All rights reserved.
// </copyright>
// <summary>
//   Defines the BibleProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models.EntityFramework
{
    using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

    /// <summary>
    /// Provides the Bible data provider.
    /// </summary>
    public class BibleProvider : IBibleProvider, IDisposable
    {
        /// <summary>
        /// The entities.
        /// </summary>
        private readonly BibleEntities entities;

        /// <summary>
        /// Initializes a new instance of the <see cref="BibleProvider"/> class.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        internal BibleProvider(BibleEntities entities)
        {
            this.entities = entities;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BibleProvider"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The <see cref="BibleProvider"/>.</returns>
        public static BibleProvider Create(string connectionString)
        {
            var entities = new BibleEntities(connectionString);

            return new BibleProvider(entities);
        }

        /// <summary>
        /// Disposes the resources used by the repository.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Gets bibles.
        /// </summary>
        /// <returns>The <see cref="IQueryable"/> of <see cref="Models.Bible"/>.</returns>
        public IQueryable<Models.Bible> GetBibles()
        {
            return
                this.entities.Bibles.Select(
                    bible =>
                    new Models.Bible
                        {
                            Culture = bible.Culture,
                            Id = bible.Id,
                            Language = bible.Language,
                            Version = bible.Version,
                        });
        }

        /// <summary>
        /// Gets a Bible by its ID.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <returns>The <see cref="Models.Bible"/>.</returns>
        public async Task<Models.Bible> GetBibleAsync(Guid id)
        {
            var bible = await this.entities.Bibles.FirstAsync(b => b.Id == id);
            return bible.ToModel();
        }

        /// <summary>
        /// Gets the default bible in a culture.
        /// </summary>
        /// <param name="cultureName">The culture name.</param>
        /// <returns>The <see cref="Models.Bible"/>.</returns>
        public async Task<Models.Bible> GetBibleAsync(string cultureName)
        {
            var bible = await this.entities.Bibles.FirstOrDefaultAsync(b => cultureName.Equals(b.Culture, StringComparison.Ordinal) && b.IsDefault)
                ?? await this.entities.Bibles.OrderBy(b => b.Culture).FirstAsync(b => b.IsDefault);
            return bible.ToModel();
        }

        /// <summary>
        /// Gets the Bible verses.
        /// </summary>
        /// <param name="bibleId">The bible id.</param>
        /// <param name="bookNameOrAbbreviation">The book name or abbreviation.</param>
        /// <param name="chapterOrder">The chapter order.</param>
        /// <param name="orders">The orders.</param>
        /// <returns>The <see cref="IEnumerable{T}"/> of <see cref="Models.BibleVerse"/>.</returns>
        public async Task<IEnumerable<Models.BibleVerse>> GetVersesAsync(Guid bibleId, string bookNameOrAbbreviation, int chapterOrder, IEnumerable<int> orders)
        {
            var bible = await this.entities.Bibles.FirstAsync(b => b.Id == bibleId);
            var bibleBook = bible.BibleBooks.First(book => book.Abbreviation == bookNameOrAbbreviation || book.Name == bookNameOrAbbreviation);
            var bibleChapter = bibleBook.BibleChapters.First(chapter => chapter.Order == chapterOrder);
            var indexSet =
                new HashSet<int>(
                    (orders ?? Enumerable.Range(1, bibleChapter.VerseStrings.Count())).Where(
                        i => i <= bibleChapter.VerseStrings.Count()));

            return bibleChapter.GetVerses().Where(v => indexSet.Contains(v.Order));
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
    }
}
