// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Bible.cs" company="Church">
//   Copyright (c) Rui Min. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;

    public class Bible
    {
        public string Version { get; internal set; }
        public string Language { get; internal set; }
        public bool IsDefault { get; internal set; }
        public string Culture { get; internal set; }
        public IEnumerable<BibleBook> Books { get { return this._books.OrderBy(b => b.Key).Select(b => b.Value); } }

        internal Dictionary<int, BibleBook> _books;
        public Bible()
        {
            this._books = new Dictionary<int, BibleBook>();
        }

        public static Bible Create(string version, string language, bool isDefault, CultureInfo cultureInfo)
        {
            return new Bible
            {
                Version = version,
                Language = language,
                IsDefault = isDefault,
                Culture = cultureInfo.Name
            };
        }

        public BibleBook CreateBook(int order, string name, string abbreviation)
        {
            var book = new BibleBook
            {
                Bible = this,
                Order = order,
                Name = name,
                Abbreviation = abbreviation
            };
            this._books.Add(book.Order, book);
            return book;
        }
    }

    public class BibleBook
    {
        public Bible Bible { get; internal set; }
        public int Order { get; internal set; }
        public string Name { get; internal set; }
        public string Abbreviation { get; internal set; }
        public IEnumerable<BibleChapter> Chapters { get { return this._chapters.OrderBy(c => c.Key).Select(c => c.Value); } }

        internal Dictionary<int, BibleChapter> _chapters;
        public BibleBook()
        {
            this._chapters = new Dictionary<int,BibleChapter>();
        }

        public BibleChapter CreateChapter(int order)
        {
            var chapter= new BibleChapter
            {
                Book = this,
                Order = order
            };
            this._chapters.Add(chapter.Order, chapter);

            return chapter;
        }
    }

    public class BibleChapter
    {
        public BibleBook Book { get; internal set; }
        public int Order { get; internal set; }
        public IEnumerable<string> Verses { get { return this._lstVerse; } }

        internal List<string> _lstVerse;

        public BibleChapter()
        {
            this._lstVerse = new List<string>();
        }

        public void AddVerse(int indexer, string text)
        {
            Trace.Assert(indexer == this._lstVerse.Count + 1);
            this._lstVerse.Add(text);
        }
    }
}