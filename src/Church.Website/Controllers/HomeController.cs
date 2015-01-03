namespace Church.Website.Controllers
{
    using Church.Website.Models;
    using Resources;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;

    public class HomeController : Controller
    {
        private FrameworkEntities db = new FrameworkEntities();

        public ActionResult Index()
        {
            this.ViewBag.Title = string.Format(Framework.SiteWide_Title_Format, Framework.HomeIndex_Title);

            var recentDate = DateTime.Now.AddDays(Utilities.Configuration.NumberOfIncidentDays.Value);
            this.ViewBag.Incidents = this.db.Incidents.Where(i => i.Time > recentDate).OrderByDescending(e => e.Time).ToArray();

            var culture = CultureInfo.CurrentUICulture.Name;
            var bulletin = this.db.Bulletins.OrderByDescending(b => b.Date).FirstOrDefault(b => culture.Equals(b.Culture, StringComparison.Ordinal));
            var factory = Utilities.CreateFactory(CultureInfo.CreateSpecificCulture(bulletin.Culture));
            var weeklyBulletin = factory.CreateBulletin(bulletin.Date, bulletin.FileUrl, bulletin.PlainText);
            this.ViewBag.Bulletin = weeklyBulletin;

            //var speaker = churchBulletin.Speaker;
            //var title = churchBulletin.MessageTitle;
            //var date = churchBulletin.Date;
            //this.ViewBag.Sermon = this.db.Sermons.FirstOrDefault(s => speaker.Equals(s.Speaker) && title.Equals(s.Title) && date.Equals(s.Date.Value));

            return View();
        }

        public ActionResult Bulletin(DateTime? date = null)
        {
            this.ViewBag.Title = string.Format(Framework.SiteWide_Title_Format, Framework.HomeBulletin_Title);

            var culture = CultureInfo.CurrentUICulture.Name;
            var bulletin = date.HasValue ? this.db.Bulletins.FirstOrDefault(b => date.Value == b.Date) : null ??
                this.db.Bulletins.OrderByDescending(b => b.Date).FirstOrDefault(b => culture.Equals(b.Culture, StringComparison.Ordinal)) ??
                this.db.Bulletins.OrderByDescending(b => b.Date).FirstOrDefault();

            var factory = Utilities.CreateFactory(CultureInfo.CreateSpecificCulture(bulletin.Culture));
            this.ViewBag.Bulletin = null == bulletin ? null : factory.CreateBulletin(bulletin.Date, bulletin.FileUrl, bulletin.PlainText);

            return View();
        }

        public ActionResult Bible()
        {
            ViewBag.Title = string.Format(Framework.SiteWide_Title_Format, Framework.HomeBible_Title);

            return View();
        }

        public ActionResult Library()
        {
            ViewBag.Title = string.Format(Framework.SiteWide_Title_Format, Framework.HomeLibrary_Title);

            return View();
        }
        public ActionResult About()
        {
            ViewBag.Title = string.Format(Framework.SiteWide_Title_Format, Framework.HomeAbout_Title);

            return View();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
