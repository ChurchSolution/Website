namespace Church.Website.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using Church.Website.Models;

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

        // GET api/Bible?culture=
        public IHttpActionResult GetBible(string culture)
        {
            if (string.IsNullOrEmpty(culture))
            {
                culture = CultureInfo.CurrentUICulture.Name;
            }

            var bibles = entities.Bibles.OrderBy(b => b.Culture);
            var bible = entities.Bibles.FirstOrDefault(b => b.Culture == culture && b.IsDefault)
                ?? bibles.First(b => b.IsDefault);

            return this.GetBible(bible.Id);
        }

        // GET api/Bible/{id}
        public IHttpActionResult GetBible(Guid id, int bookOrder = 1, int chapterOrder = 1)
        {
            const char SplitChar = '|';

            var bibles = entities.Bibles.OrderBy(b => b.Culture);
            var bible = bibles.First(b => b.Id==id);
            var books = bible.BibleBooks;
            var book = books.First(b => b.Order == bookOrder);
            var chapters = book.BibleChapters;
            var chapter = chapters.First(c => c.Order == chapterOrder);

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
                SelectedBook = book.Order,
                Chapters = chapters.OrderBy(c => c.Order).Select(c => new
                {
                    Id = c.Order,
                    Name = c.Order
                }),
                SelectedChapter = chapter.Order,
                Verses = chapter.VerseStrings.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Select(v =>
                {
                    var segments = v.Split(SplitChar);
                    return new { Order = segments[0], Value = segments[1] };
                }),
            };

            return this.Ok(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                entities.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
