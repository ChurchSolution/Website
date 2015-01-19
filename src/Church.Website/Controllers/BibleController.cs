namespace Church.Website.Controllers
{
    using Church.Model;
    using Church.Website.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
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
        public async Task<HttpResponseMessage> GetBibleAsync(string book, int chapter, IEnumerable<int> indexes, Guid? id)
        {
            const char SplitChar = '|';

            ExceptionUtilities.ThrowArgumentNullExceptionIfEmpty(book, "book");
            ExceptionUtilities.ThrowArgumentNullExceptionIfEmpty(chapter, "chapter");

            var bibles = this.entities.Bibles.OrderBy(b => b.Culture);
            var bible = id.HasValue ? await this.entities.GetBibleAsync(id.Value) :
                await this.entities.GetDefaultBibleAsync(CultureInfo.CurrentUICulture.Name);
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
                Versions = bibles.Select(b => new { Id = b.Id, Text = b.Language + b.Version }),
                SelectedVersion = bible.Id,
                Books = books.OrderBy(b => b.Order).Select(b => new { Id = b.Order, Text = b.Name }),
                SelectedBook = selectedBook.Order,
                Chapters = chapters.OrderBy(c => c.Order).Select(c => new { Id = c.Order, Text = c.Order.ToString() }),
                SelectedChapter = selectedChapter.Order,
                Verses = selectedChapter.VerseStrings.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Select(v =>
                {
                    var segments = v.Split(SplitChar);
                    Trace.Assert(2 == segments.Length, string.Format("The verse string '{0}' is incorrect.", v));
                    int order;
                    Trace.Assert(int.TryParse(segments[0], out order), string.Format("The order '{0}' is not an integer", segments[0]));
                    return new { Id = order, Text = segments[1] };
                }).Where(v => indexSet.Contains(v.Id)),
            };

            return this.Request.CreateResponse(model);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetVersesAsync(string abbreviation, Guid? id)
        {
            ExceptionUtilities.ThrowArgumentNullExceptionIfEmpty(abbreviation, "abbreviation");

            // TODO Need to handle ":" in abbreviation

            var culture = id.HasValue ? (await this.entities.GetBibleAsync(id.Value)).Culture : CultureInfo.CurrentUICulture.Name;
            var pattern = await this.GetVersePatternAsync(culture);
            var match = Regex.Match(abbreviation, pattern, RegexOptions.IgnoreCase);
            ExceptionUtilities.ThrowFormatExceptionIfFalse(match.Success && match.Groups.Count >= 3, "Found an incorrect abbreviation '{0}'", abbreviation);

            var book = match.Groups[1].Value;
            var chapter = int.Parse(match.Groups[2].Value);
            var others = match.Groups[3].Value;

            // TODO Considering to move to Bible handler for multiple languages.
            return await this.GetBibleAsync(book, chapter, this.GetIndexes(others.Split(',', '，'), abbreviation), id);
        }

        [HttpGet]
        public async Task<string> GetVersePatternAsync(string culture)
        {
            if(string.IsNullOrWhiteSpace(culture))
            {
                culture = CultureInfo.CurrentUICulture.Name;
            }

            var bible = await this.entities.GetDefaultBibleAsync(culture);
            var bookNames = bible.BibleBooks.SelectMany(b => new[] { b.Name, b.Abbreviation });

            // TODO Considering to move to Bible handler for multiple languages.
            return "(" + string.Join("|", bookNames) + ")[ ]*([0-9]+)[ ]*[:：]([ ]*[0-9]+(-[ ]*[0-9]+)?([，,][ ]*[0-9]+(-[ ]*[0-9]+)?)*)";
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
