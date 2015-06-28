namespace Church.Website.Controllers
{
    using Church.Models;
    using Church.Website.Models;
    using System;
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
        private readonly IRepository repository;

        public BulletinsController()
            : this(Church.Models.EntityFramework.Repository.Create(Utilities.FrameworkEntitiesConnectionString, Utilities.Configuration.ChurchWebsiteLibrary))
        {
        }

        internal BulletinsController(IRepository repository)
        {
            this.repository = repository;
        }

        // GET api/Bulletins
        [HttpGet]
        public Task<IQueryable<WeeklyBulletin>> GetBulletins()
        {
            return this.repository.GetBulletinsAsync();
        }

        // GET api/Bulletins/5
        [HttpGet]
        [ResponseType(typeof(WeeklyBulletin))]
        public async Task<HttpResponseMessage> GetAsync(int id)
        {
            var bulletin = await this.repository.GetDefaultBulletinAsync(CultureInfo.CurrentUICulture.Name);
            if (bulletin == null)
            {
                return this.Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return this.Request.CreateResponse(HttpStatusCode.OK, bulletin);
        }

        // GET api/Bulletins?date={date}
        [HttpGet]
        [ResponseType(typeof(WeeklyBulletin))]
        public async Task<HttpResponseMessage> GetAsync(DateTime date)
        {
            var bulletin = await this.repository.GetBulletinByDateAsync(CultureInfo.CurrentUICulture.Name, date);
            if (bulletin == null)
            {
                return this.Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return this.Request.CreateResponse(HttpStatusCode.OK, bulletin);
        }

        // POST api/Bulletins
        [Authorize(Roles = "Administrators")]
        [ResponseType(typeof(WeeklyBulletin))]
        public async Task<HttpResponseMessage> PostAsync([FromBody]BulletinRequest request)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid model.");
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

            var bulletin = await this.repository.AddOrUpdateBulletinAsync(request.Date, fileUrl, request.TextFileContent, CultureInfo.CurrentUICulture.Name);

            return this.Request.CreateResponse(HttpStatusCode.Created, bulletin);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var disposable = this.repository as IDisposable;
                if(disposable!=null)
                {
                    disposable.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}