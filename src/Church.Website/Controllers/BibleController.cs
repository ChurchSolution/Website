namespace Church.Website.Controllers
{
    using Church.Model;
    using Church.Website.Models;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web.Http;

    public class BibleController : ApiController
    {
        private BibleEntities entities;

        public BibleController()
            : this(new BibleEntities())
        {
        }

        internal BibleController(BibleEntities entities)
        {
            this.entities = entities;
        }

        [HttpGet]
        public IHttpActionResult GetBible(string book, int chapter, IEnumerable<int> indexes, string id)
        {
            const char SplitChar = '|';

            ExceptionUtilities.ThrowArgumentNullExceptionIfEmpty(book, "book");
            ExceptionUtilities.ThrowArgumentNullExceptionIfEmpty(chapter, "chapter");

            var bibles = entities.Bibles.OrderBy(b => b.Culture);
            var bible = string.IsNullOrEmpty(id) ? entities.Bibles.FirstOrDefault(b => b.Culture == CultureInfo.CurrentUICulture.Name && b.IsDefault) ?? bibles.First(b => b.IsDefault) :
                bibles.First(b => b.Id.ToString() == id);
            var books = bible.BibleBooks;
            int bookOrder;
            var selectedBook = int.TryParse(book, out bookOrder) ? books.First(b => b.Order == bookOrder) :
                books.First(b => b.Name.Equals(book, StringComparison.OrdinalIgnoreCase) || b.Abbreviation.Equals(book, StringComparison.OrdinalIgnoreCase));
            var chapters = selectedBook.BibleChapters;
            var selectedChapter = chapters.First(c => c.Order == chapter);
            var indexSet = new HashSet<int>((indexes ?? Enumerable.Range(1, selectedChapter.VerseStrings.Length))
                .Where(i => i <= selectedChapter.VerseStrings.Length));

            var model = new
            {
                Versions = bibles.OrderBy(b => b.Culture).Select(b => new
                {
                    Id = b.Id,
                    Name = b.Language + b.Version
                }),
                SelectedVersion = bible.Id,
                Books = books.OrderBy(b => b.Order).Select(b => new
                {
                    Id = b.Order,
                    Name = b.Name
                }),
                SelectedBook = selectedBook.Order,
                Chapters = chapters.OrderBy(c => c.Order).Select(c => new
                {
                    Id = c.Order,
                    Name = c.Order
                }),
                SelectedChapter = selectedChapter.Order,
                Verses = selectedChapter.VerseStrings.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Select(v =>
                {
                    var segments = v.Split(SplitChar);
                    Trace.Assert(2 == segments.Length, string.Format("The verse string '{0}' is incorrect.", v));
                    int order;
                    Trace.Assert(int.TryParse(segments[0], out order), string.Format("The order '{0}' is not an integer", segments[0]));
                    return new { Order = order, Value = segments[1] };
                }).Where(v => indexSet.Contains(v.Order)),
            };

            return this.Ok(model);
        }

        [HttpGet]
        public IHttpActionResult GetVerses(string abbreviation, string id)
        {
            ExceptionUtilities.ThrowArgumentNullExceptionIfEmpty(abbreviation, "abbreviation");

            // TODO Need to handle ":" in abbreviation

            var culture = string.IsNullOrWhiteSpace(id)? CultureInfo.CurrentUICulture.Name:this.entities.Bibles.First(b => b.Id.ToString() == id).Culture;
            var pattern = this.GetVersePattern(culture);
            var match = Regex.Match(abbreviation, pattern, RegexOptions.IgnoreCase);
            ExceptionUtilities.ThrowFormatExceptionIfFalse(match.Success && match.Groups.Count >= 3, "Found an incorrect abbreviation '{0}'", abbreviation);

            var book = match.Groups[1].Value;
            var chapter = int.Parse(match.Groups[2].Value);
            var others = match.Groups[3].Value;

            // TODO Considering to move to Bible handler for multiple languages.
            return this.GetBible(book, chapter, this.GetIndexes(others.Split(',', '，'), abbreviation), id);
        }

        [HttpGet]
        public string GetVersePattern(string culture)
        {
            if(string.IsNullOrWhiteSpace(culture))
            {
                culture = CultureInfo.CurrentUICulture.Name;
            }

            var bibles = this.entities.Bibles.OrderBy(b => b.Culture);
            var bible = this.entities.Bibles.FirstOrDefault(b => b.Culture == culture && b.IsDefault) ?? bibles.First(b => b.IsDefault);

            // TODO Considering to move to Bible handler for multiple languages.
            return "(" + string.Join("|", bible.BibleBooks.SelectMany(b => new[] { b.Name, b.Abbreviation })) + ")[ ]*([0-9]+)[ ]*[:：]([ ]*[0-9]+(-[ ]*[0-9]+)?([，,][ ]*[0-9]+(-[ ]*[0-9]+)?)*)";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                entities.Dispose();
            }

            base.Dispose(disposing);
        }

        private IEnumerable<int> GetIndexes(IEnumerable<string> segments, string abbreviation)
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
                        ExceptionUtilities.ThrowFormatExceptionIfFalse(from <= to, "Found an incorrect abbreviation '{0}'", abbreviation);
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
