// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeController.cs" company="Church">
//   Copyright (c) Rui Min. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Website.Controllers
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using Church.Models;
    using Church.Website.Models;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    using Resources;

    /// <summary>
    /// Provides the home controller.
    /// </summary>
    [CultureFilter]
    public class HomeController : Controller
    {
        /// <summary>
        /// The repository.
        /// </summary>
        private readonly IRepository repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        public HomeController()
            : this(
                Church.Models.EntityFramework.Repository.Create(
                    Utilities.FrameworkEntitiesConnectionString,
                    Utilities.Configuration.ChurchWebsiteLibrary))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        internal HomeController(IRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Gets the index page.
        /// </summary>
        /// <returns>The index page.</returns>
        public async Task<ActionResult> Index()
        {
            await this.LoadDefaultBibleIdAsync();

            var recentDate = DateTime.Now.AddDays(Utilities.Configuration.NumberOfIncidentDays.Value);
            this.ViewBag.Incidents = this.repository.GetIncidents(recentDate).OrderByDescending(e => e.Time).ToArray();
            this.ViewBag.Bulletin = await this.repository.GetDefaultBulletinAsync(CultureInfo.CurrentUICulture.Name);

            return this.ViewWithTitle(Framework.HomeIndex_Title);
        }

        /// <summary>
        /// Gets the bulletin page.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>The bulletin page.</returns>
        public async Task<ActionResult> Bulletin(DateTime? date = null)
        {
            await this.LoadDefaultBibleIdAsync();

            this.ViewBag.Bulletin = date.HasValue ?
                await this.repository.GetBulletinByDateAsync(CultureInfo.CurrentUICulture.Name, date.Value) :
                await this.repository.GetDefaultBulletinAsync(CultureInfo.CurrentUICulture.Name);

            return this.ViewWithTitle(Framework.HomeBulletin_Title);
        }

        /// <summary>
        /// Gets the bible page.
        /// </summary>
        /// <returns>The bible page.</returns>
        public async Task<ActionResult> Bible()
        {
            var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            using (
                var bibleProvider =
                    Church.Models.EntityFramework.BibleProvider.Create(Utilities.BibleEntitiesConnectionString))
            {
                this.ViewBag.BibleVersions = JsonConvert.SerializeObject(bibleProvider.GetBibles().ToArray(), jsonSerializerSettings);

                var bible = await bibleProvider.GetBibleAsync(CultureInfo.CurrentUICulture.Name);
                this.ViewBag.BibleId = bible.Id;
            }

            return this.ViewWithTitle(Framework.HomeBible_Title);
        }

        /// <summary>
        /// Gets the library page.
        /// </summary>
        /// <returns>The library page.</returns>
        public ActionResult Library()
        {
            return this.ViewWithTitle(Framework.HomeLibrary_Title);
        }

        /// <summary>
        /// Gets the about page.
        /// </summary>
        /// <returns>The about page.</returns>
        public ActionResult About()
        {
            return this.ViewWithTitle(Framework.HomeAbout_Title);
        }

        /// <summary>
        /// Disposes the resources used by the controller.
        /// </summary>
        /// <param name="disposing">A value indicating whether the managed resource is disposed. </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var disposable = this.repository as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Loads the default Bible ID.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task LoadDefaultBibleIdAsync()
        {
            using (
                var bibleProvider =
                    Church.Models.EntityFramework.BibleProvider.Create(Utilities.BibleEntitiesConnectionString))
            {
                var bible = await bibleProvider.GetBibleAsync(CultureInfo.CurrentUICulture.Name);
                this.ViewBag.BibleId = bible.Id;
            }
        }

        /// <summary>
        /// Returns the view with title.
        /// </summary>
        /// <param name="resourceKey">The resource key.</param>
        /// <returns>The view with title.</returns>
        private ViewResult ViewWithTitle(string resourceKey)
        {
            this.ViewBag.Title = string.Format(Framework.SiteWide_Title_Format, resourceKey);

            return this.View();
        }
    }
}
