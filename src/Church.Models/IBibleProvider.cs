// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBibleProvider.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the IBibleProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides the Bible data.
    /// </summary>
    public interface IBibleProvider
    {
        /// <summary>
        /// Gets bibles.
        /// </summary>
        /// <returns>The <see cref="IQueryable"/> of <see cref="Models.Bible"/>.</returns>
        IQueryable<Bible> GetBibles();

        /// <summary>
        /// Gets a Bible by its ID.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <returns>The <see cref="Models.Bible"/>.</returns>
        Task<Bible> GetBibleAsync(Guid id);

        /// <summary>
        /// Gets the default bible in a culture.
        /// </summary>
        /// <param name="cultureName">The culture name.</param>
        /// <returns>The <see cref="Models.Bible"/>.</returns>
        Task<Bible> GetBibleAsync(string cultureName);

        /// <summary>
        /// Gets the Bible verses.
        /// </summary>
        /// <param name="bibleId">The bible id.</param>
        /// <param name="bookNameOrAbbreviation">The book abbreviation.</param>
        /// <param name="chapterOrder">The chapter order.</param>
        /// <param name="orders">The orders.</param>
        /// <returns>The <see cref="IEnumerable{T}"/> of <see cref="Models.BibleVerse"/>.</returns>
        Task<IEnumerable<BibleVerse>> GetVersesAsync(Guid bibleId, string bookNameOrAbbreviation, int chapterOrder, IEnumerable<int> orders);
    }
}
