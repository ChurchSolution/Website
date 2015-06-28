// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BibleController.cs" company="Church">
//   Copyright (c) Rui Min. All rights reserved.
// </copyright>
// <summary>
//   Defines the BibleController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Website.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;

    using Church.Models;
    using Church.Website.Models;

    /// <summary>
    /// Provide the Bible controller.
    /// </summary>
    public class BibleController : ApiController
    {
        /// <summary>
        /// The provider.
        /// </summary>
        private readonly IBibleProvider provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="BibleController"/> class.
        /// </summary>
        public BibleController()
            : this(Church.Models.EntityFramework.BibleProvider.Create(Utilities.BibleEntitiesConnectionString))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BibleController"/> class.
        /// </summary>
        /// <param name="provider">The provider.s</param>
        internal BibleController(IBibleProvider provider)
        {
            this.provider = provider;
        }

        /// <summary>
        /// Gets verses from a Bible chapter.
        /// </summary>
        /// <param name="bibleId">The bible id.</param>
        /// <param name="order">The book order.</param>
        /// <param name="chapterOrder">The chapter order.</param>
        /// <returns>The <see cref="HttpResponseMessage"/>.</returns>
        public async Task<HttpResponseMessage> GetChapterAsync(Guid bibleId, int order, int chapterOrder)
        {
            var versions = this.provider.GetBibles().ToArray();
            var version = await this.provider.GetBibleAsync(bibleId);
            var books = version.Books.Select(b => new { b.Name, b.Order }).ToArray();
            var book = version.Books.FirstOrDefault(b => b.Order == order) ?? version.Books.First();
            var chapters = book.Chapters.Select(c => new { c.Order }).ToArray();
            var chapter = book.Chapters.FirstOrDefault(c => c.Order == chapterOrder) ?? book.Chapters.First();
            var verses = chapter.Verses.ToArray();

            var model =
                new
                    {
                        Versions = versions,
                        SelectedVersion = version.Id,
                        Books = books,
                        SelectedBook = book.Order,
                        Chapters = chapters,
                        SelectedChapter = chapter.Order,
                        Verses = verses
                    };

            return this.Request.CreateResponse(model);
        }

        /// <summary>
        /// Gets the verse pattern.
        /// </summary>
        /// <param name="bibleId">The Bible ID.</param>
        /// <returns>The <see cref="HttpResponseMessage"/>.</returns>
        [HttpGet]
        [ResponseType(typeof(string))]
        public async Task<HttpResponseMessage> GetAbbreviationsAsync(Guid bibleId)
        {
            var bible = await this.provider.GetBibleAsync(bibleId);
            var verserPattern = bible.GetAbbreviations();
            return this.Request.CreateResponse(verserPattern);
        }

        /// <summary>
        /// Gets the verses async.
        /// </summary>
        /// <param name="bibleId">The Bible ID.</param>
        /// <param name="abbreviation">The abbreviation.</param>
        /// <returns>The <see cref="HttpResponseMessage"/>.</returns>
        [HttpGet]
        [ResponseType(typeof(IEnumerable<BibleVerse>))]
        [FormatExceptionFilter]
        public async Task<HttpResponseMessage> GetAbbreviationAsync(Guid bibleId, string abbreviation)
        {
            ExceptionUtilities.ThrowArgumentNullExceptionIfEmpty(abbreviation, "abbreviation");

            // TODO Need to handle ":" in abbreviation

            var bible = await this.provider.GetBibleAsync(bibleId);
            var pattern = bible.GetAbbreviations();
            var match = Regex.Match(abbreviation, pattern, RegexOptions.IgnoreCase);
            ExceptionUtilities.ThrowFormatExceptionIfFalse(
                match.Success && match.Groups.Count >= 3,
                Resources.Framework.Bible_IncorrectAbbreviation,
                abbreviation);

            var book = match.Groups[1].Value;
            var chapter = int.Parse(match.Groups[2].Value);
            var others = match.Groups[3].Value;

            // TODO Considering to move to Bible handler for multiple languages.
            var verses = await this.provider.GetVersesAsync(bibleId, book, chapter, GetIndexes(others.Split(',', '，'), abbreviation));
            return this.Request.CreateResponse(verses);
        }

        /// <summary>
        /// Disposes the resources used by the controller.
        /// </summary>
        /// <param name="disposing">A value indicating whether the managed resource is disposed. </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var disposable = this.provider as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets indexes in an abbreviation.
        /// </summary>
        /// <param name="segments">The segments.</param>
        /// <param name="abbreviation">The abbreviation.</param>
        /// <returns>The <see cref="IEnumerable{T}"/> of <see cref="int"/>.</returns>
        /// <exception cref="FormatException">While the abbreviation is in a wrong format.</exception>
        private static IEnumerable<int> GetIndexes(IEnumerable<string> segments, string abbreviation)
        {
            foreach (var segment in segments)
            {
                var parts = segment.Split('-');
                switch (parts.Length)
                {
                    case 1:
                        yield return int.Parse(parts[0]);
                        break;
                    case 2:
                        var from = int.Parse(parts[0]);
                        var to = int.Parse(parts[1]);
                        ExceptionUtilities.ThrowFormatExceptionIfFalse(
                            from <= to,
                            "Found an incorrect abbreviation '{0}'",
                            abbreviation);
                        for (int loop = from; loop <= to; loop++)
                        {
                            yield return loop;
                        }
                        break;
                    default:
                        throw new FormatException(string.Format("Found an incorrect abbreviation '{0}'", abbreviation));
                }
            }
        }
    }
}
