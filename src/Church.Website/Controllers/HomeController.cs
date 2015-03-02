namespace Church.Website.Controllers
{
    using Church.Website.Models;
    using Resources;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [CultureFilter]
    public class HomeController : Controller
    {
        private FrameworkEntities entities;
        
        public HomeController()
            : this(new FrameworkEntities())
        {
        }

        internal HomeController(FrameworkEntities entities)
        {
            this.entities = entities;
        }

        public async Task<ActionResult> Index()
        {
            var recentDate = DateTime.Now.AddDays(Utilities.Configuration.NumberOfIncidentDays.Value);
            this.ViewBag.Incidents = this.entities.Incidents.Where(i => i.Time > recentDate).OrderByDescending(e => e.Time).ToArray();
            this.ViewBag.Bulletin = await this.entities.GetDefaultBulletinAsync(CultureInfo.CurrentUICulture.Name);

            return this.ViewWithTitle(Framework.HomeIndex_Title);
        }

        public async Task<ActionResult> Bulletin(DateTime? date = null)
        {
            this.ViewBag.Bulletin = date.HasValue ?
                await this.entities.GetBulletinByDateAsync(date.Value, CultureInfo.CurrentUICulture.Name) :
                await this.entities.GetDefaultBulletinAsync(CultureInfo.CurrentUICulture.Name);

            return this.ViewWithTitle(Framework.HomeBulletin_Title);
        }
    
        public ActionResult Bible()
        {
            return this.ViewWithTitle(Framework.HomeBible_Title);
        }

        public ActionResult Library()
        {
            return this.ViewWithTitle(Framework.HomeLibrary_Title);
        }

        public ActionResult About()
        {
            return this.ViewWithTitle(Framework.HomeAbout_Title);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.entities.Dispose();
            }

            base.Dispose(disposing);
        }

        private ViewResult ViewWithTitle(string resourceKey)
        {
            this.ViewBag.Title = string.Format(Framework.SiteWide_Title_Format, resourceKey);

            return this.View();
        }
    }
}
