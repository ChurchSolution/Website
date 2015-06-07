namespace Church.Website.Controllers
{
    using Church.Models;
    using Church.Website.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Description;

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
        [HttpGet]
        public Task<IQueryable<Bulletin>> GetBulletins()
        {
            return Task.FromResult<IQueryable<Bulletin>>(this.entities.Bulletins);
        }

        // GET api/Bulletins/5
        [HttpGet]
        [ResponseType(typeof(Bulletin))]
        public async Task<HttpResponseMessage> GetAsync(int id)
        {
            var bulletin = await this.entities.GetDefaultBulletinAsync(CultureInfo.CurrentUICulture.Name);
            if (bulletin == null)
            {
                return this.Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return this.Request.CreateResponse(HttpStatusCode.OK, bulletin);
        }

        // GET api/Bulletins?date={date}
        [HttpGet]
        [ResponseType(typeof(Bulletin))]
        public async Task<HttpResponseMessage> GetAsync(DateTime date)
        {
            var bulletin = await this.entities.GetBulletinByDateAsync(date, CultureInfo.CurrentUICulture.Name);
            if (bulletin == null)
            {
                return this.Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return this.Request.CreateResponse(HttpStatusCode.OK, bulletin);
        }

        // POST api/Bulletins
        [Authorize(Roles = "Administrators")]
        [ResponseType(typeof(Bulletin))]
        public IHttpActionResult Post([FromBody]BulletinRequest request)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(ModelState);
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

            var bulletin = this.entities.Bulletins.FirstOrDefault(b => b.Date == request.Date && b.Culture == CultureInfo.CurrentUICulture.Name);
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
                this.entities.Bulletins.Add(bulletin);
            }
            else
            {
                bulletin.PlainText = request.TextFileContent;
                bulletin.FileUrl = fileUrl;
                this.entities.Entry(bulletin).State = EntityState.Modified;
            }

            try
            {
                this.entities.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (BulletinExists(bulletin.Id))
                {
                    return this.Conflict();
                }
                else
                {
                    throw;
                }
            }

            return this.CreatedAtRoute("DefaultApi", new { id = bulletin.Id }, bulletin);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.entities.Dispose();
            }

            base.Dispose(disposing);
        }

        private bool BulletinExists(Guid id)
        {
            return this.entities.Bulletins.Count(e => e.Id == id) > 0;
        }
    }
}