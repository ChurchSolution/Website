namespace Church.Website.Controllers
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Description;
    using System.IO;
    using System.Web;

    using Church;
    using Church.Website.Models;

    public class BulletinsController : ApiController
    {
        private FrameworkEntities entities;

        public BulletinsController()
            : this(new FrameworkEntities())
        {
        }

        internal BulletinsController(FrameworkEntities entities)
        {
            this.entities = entities;
        }

        // GET api/Bulletins
        public IQueryable<Bulletin> GetBulletins()
        {
            return entities.Bulletins;
        }

        // GET api/Bulletins/5
        //[ResponseType(typeof(Bulletin))]
        public IHttpActionResult Get(int id)
        {
            var bulletin = entities.Bulletins.OrderByDescending(b => b.Date).FirstOrDefault(b => b.Culture == CultureInfo.CurrentUICulture.Name);
            if (bulletin == null)
            {
                return NotFound();
            }

            return this.GetBulletin(bulletin);
        }

        // GET api/Bulletins?date={date}
        [ResponseType(typeof(Bulletin))]
        public IHttpActionResult Get(DateTime date)
        {
            var bulletin = entities.Bulletins.FirstOrDefault(b => b.Date == date && b.Culture == CultureInfo.CurrentUICulture.Name);
            if (bulletin == null)
            {
                return NotFound();
            }

            return this.GetBulletin(bulletin);
        }

        private IHttpActionResult GetBulletin(Bulletin bulletin)
        {
            var factory = Utilities.CreateFactory(CultureInfo.CreateSpecificCulture(bulletin.Culture));
            var weeklyBulletin = factory.CreateBulletin(bulletin.Date, bulletin.FileUrl, bulletin.PlainText);

            //var date = builder.Bulletin.Date;
            //var title = builder.Bulletin.MessageTitle;
            //var speaker = builder.Bulletin.Speaker;

            //var sermon = entities.Sermons.FirstOrDefault(s => s.Date == date
            //    && s.Title.Equals(title, StringComparison.CurrentCultureIgnoreCase)
            //    && s.Title.Equals(speaker, StringComparison.CurrentCultureIgnoreCase));

            //var model = new
            //{
            //    Bulletin = builder.Bulletin,
            //    Sermon = sermon,
            //};

            return Ok(weeklyBulletin);
        }

        // POST api/Bulletins
        [Authorize(Roles = "Administrators")]
        [ResponseType(typeof(Bulletin))]
        public IHttpActionResult Post([FromBody]BulletinRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string fileUrl = string.Empty;
            var printFile = BinaryFile.FromBase64(request.PrintFileContent);
            if (null != printFile)
            {
                printFile.Filename = string.Join(
                        Utilities.Configuration.BulletinFilename.Separator,
                        Utilities.Configuration.BulletinFilename.Prefix,
                        request.Date.ToString(Utilities.Configuration.BulletinFilename.DateFormat))
                        + "." + printFile.Extension;
                printFile.Save(HttpContext.Current.Server.MapPath(".." + Utilities.Configuration.ContentFolder.BulletinFolder));
                fileUrl = Utilities.Configuration.ContentFolder.BulletinFolder + "/" + printFile.Filename;
            }

            var bulletin = entities.Bulletins.FirstOrDefault(b => b.Date == request.Date && b.Culture == CultureInfo.CurrentUICulture.Name);
            if (null == bulletin)
            {
                bulletin = new Bulletin
                {
                    PlainText = request.TextFileContent,
                    Date = request.Date,
                    Culture = CultureInfo.CurrentUICulture.Name,
                    Id = Guid.NewGuid(),
                    FileUrl = fileUrl,
                };
                entities.Bulletins.Add(bulletin);
            }
            else
            {
                bulletin.PlainText = request.TextFileContent;
                bulletin.FileUrl = fileUrl;
                entities.Entry(bulletin).State = EntityState.Modified;
            }

            try
            {
                entities.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (BulletinExists(bulletin.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = bulletin.Id }, bulletin);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                entities.Dispose();
            }

            base.Dispose(disposing);
        }

        private bool BulletinExists(Guid id)
        {
            return entities.Bulletins.Count(e => e.Id == id) > 0;
        }
    }
}