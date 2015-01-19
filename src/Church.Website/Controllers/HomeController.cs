namespace Church.Website.Controllers
{
    using Church.Website.Models;
    using Resources;
    using System;
    using System.Data.Entity;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

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
            this.ViewBag.Title = string.Format(Framework.SiteWide_Title_Format, Framework.HomeIndex_Title);

            var recentDate = DateTime.Now.AddDays(Utilities.Configuration.NumberOfIncidentDays.Value);
            this.ViewBag.Incidents = this.entities.Incidents.Where(i => i.Time > recentDate).OrderByDescending(e => e.Time).ToArray();
            this.ViewBag.Bulletin = await this.entities.GetDefaultBulletinAsync(CultureInfo.CurrentUICulture.Name);

            return this.View();
        }

        public async Task<ActionResult> Bulletin(DateTime? date = null)
        {
            this.ViewBag.Title = string.Format(Framework.SiteWide_Title_Format, Framework.HomeBulletin_Title);

            this.ViewBag.Bulletin = date.HasValue ?
                await this.entities.GetBulletinByDateAsync(date.Value, CultureInfo.CurrentUICulture.Name) :
                null ?? await this.entities.GetDefaultBulletinAsync(CultureInfo.CurrentUICulture.Name);

            return this.View();
        }
    
        public ActionResult Bible()
        {
            this.ViewBag.Title = string.Format(Framework.SiteWide_Title_Format, Framework.HomeBible_Title);

            return this.View();
        }

        public ActionResult Library()
        {
            this.ViewBag.Title = string.Format(Framework.SiteWide_Title_Format, Framework.HomeLibrary_Title);

            return this.View();
        }

        public ActionResult About()
        {
            this.ViewBag.Title = string.Format(Framework.SiteWide_Title_Format, Framework.HomeAbout_Title);

            return this.View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.entities.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
