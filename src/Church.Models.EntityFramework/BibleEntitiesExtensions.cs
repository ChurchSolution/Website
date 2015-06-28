// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BibleEntitiesExtensions.cs" company="Church">
//   Copyright (c) Rui Min. All rights reserved.
// </copyright>
// <summary>
//   Defines the BibleEntitiesExtension type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides the extensions for Bible entities.
    /// </summary>
    public static class BibleEntitiesExtensions
    {
        /// <summary>
        /// Converts to Bible model.
        /// </summary>
        /// <param name="bible">The bible.</param>
        /// <returns>The <see cref="Models.Bible"/>.</returns>
        public static Models.Bible ToModel(this Bible bible)
        {
            return new Models.Bible
                       {
                           Books = bible.BibleBooks.OrderBy(b => b.Order).Select(b => b.ToModel()),
                           Culture = bible.Culture,
                           Id = bible.Id,
                           Language = bible.Language,
                           Version = bible.Version,
                       };
        }

        /// <summary>
        /// Converts to the Bible book model.
        /// </summary>
        /// <param name="bibleBook">The Bible book.</param>
        /// <returns>The <see cref="Models.BibleBook"/>.</returns>
        public static Models.BibleBook ToModel(this BibleBook bibleBook)
        {
            return new Models.BibleBook
                       {
                           BibleId = bibleBook.BibleId,
                           Abbreviation = bibleBook.Abbreviation,
                           Chapters =
                               bibleBook.BibleChapters.OrderBy(c => c.Order).Select(c => c.ToModel()),
                           Name = bibleBook.Name,
                           Order = bibleBook.Order,
                       };
        }

        /// <summary>
        /// Converts to the Bible chapter model.
        /// </summary>
        /// <param name="bibleChapter">The Bible chapter.</param>
        /// <returns>The <see cref="Models.BibleChapter"/>.</returns>
        public static Models.BibleChapter ToModel(this BibleChapter bibleChapter)
        {
            return new Models.BibleChapter
                       {
                           BibleId = bibleChapter.BibleBook.BibleId,
                           BookAbbreviation = bibleChapter.BibleBook.Abbreviation,
                           Order = bibleChapter.Order,
                           Verses = bibleChapter.GetVerses(),
                       };
        }

        /// <summary>
        /// Gets the verses from Bible chapter.
        /// </summary>
        /// <param name="bibleChapter">The bible chapter.</param>
        /// <returns>The <see cref="IEnumerable{BibleVerse}"/>.</returns>
        public static IEnumerable<Models.BibleVerse> GetVerses(this BibleChapter bibleChapter)
        {
            const char SplitChar = '|';

            return
                bibleChapter.VerseStrings.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Select(
                    verse =>
                        {
                            var segments = verse.Split(SplitChar);
                            return new BibleVerse
                                       {
                                           BibleId = bibleChapter.BibleBook.BibleId,
                                           BookAbbreviation = bibleChapter.BibleBook.Abbreviation,
                                           ChapterOrder = bibleChapter.Order,
                                           Order = int.Parse(segments[0]),
                                           Text = segments[1]
                                       };
                        });
        }
    }
}