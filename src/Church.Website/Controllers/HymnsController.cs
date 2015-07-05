// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HymnsController.cs" company="Church">
//   Copyright (c) Rui Min. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Website.Controllers
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http.Description;
    using System.Web.OData;

    using Church.Models;
    using Church.Website.Models;

    /// <summary>
    /// Provides the hymns controller.
    /// </summary>
    public class HymnsController : ODataController
    {
        /// <summary>
        /// The repository.
        /// </summary>
        private readonly IRepository repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="HymnsController"/> class.
        /// </summary>
        public HymnsController()
            : this(
                Church.Models.EntityFramework.Repository.Create(
                    Utilities.FrameworkEntitiesConnectionString,
                    Utilities.Configuration.ChurchWebsiteLibrary))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HymnsController"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository from unit tests.
        /// </param>
        internal HymnsController(IRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Gets a list of hymns.
        /// </summary>
        /// <returns>The <see cref="IQueryable{IHymn}"/> on hymns.</returns>
        [EnableQuery]
        public IQueryable<Hymn> Get()
        {
            return this.repository.GetHymns();
        }

        /// <summary>
        /// Gets a hymn.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="HttpResponseMessage"/> with the <see cref="Hymn"/>.</returns>
        [EnableQuery]
        public async Task<Hymn> GetAsync(Guid key)
        {
            var hymn = await this.repository.GetHymns().SingleAsync(h => h.Id.Equals(key));
            return hymn;
        }

        /// <summary>
        /// Creates a hymn.
        /// </summary>
        /// <param name="hymn">The hymn.</param>
        /// <returns>The <see cref="HttpResponseMessage"/> with the <see cref="Hymn"/>.</returns>
        [ResponseType(typeof(Hymn))]
        public async Task<HttpResponseMessage> PostAsync(Hymn hymn)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            await this.repository.AddHymnAsync(hymn);
            return this.Request.CreateResponse(HttpStatusCode.Created, hymn);
        }

        /// <summary>
        /// Patches a hymn.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="hymn">The hymn.</param>
        /// <returns>The <see cref="HttpResponseMessage"/> with the <see cref="Hymn"/>.</returns>
        [ResponseType(typeof(Hymn))]
        public async Task<HttpResponseMessage> PatchAsync([FromODataUri] Guid key, Delta<Hymn> hymn)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            var hymnToBeUpdated = await this.repository.GetHymns().SingleAsync(s => s.Id.Equals(key));
            hymn.Patch(hymnToBeUpdated);
            await this.repository.UpdateHymnAsync(hymnToBeUpdated);
            return this.Request.CreateResponse(HttpStatusCode.Accepted, hymnToBeUpdated);
        }

        /// <summary>
        /// Updates a hymn.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="hymn">The hymn.</param>
        /// <returns>The <see cref="HttpResponseMessage"/> with the <see cref="Hymn"/>.</returns>
        [ResponseType(typeof(Hymn))]
        public async Task<HttpResponseMessage> PutAsync([FromODataUri] Guid key, Hymn hymn)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            if (key != hymn.Id)
            {
                var message = string.Format(
                    "The key '{0}' in the URL doesn't match the one '{1}' in the request body.",
                    key,
                    hymn.Id);
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
            }

            await this.repository.UpdateHymnAsync(hymn);
            return this.Request.CreateResponse(HttpStatusCode.Accepted, hymn);
        }

        /// <summary>
        /// Deletes a hymn.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="HttpResponseMessage"/>.</returns>
        public async Task<HttpResponseMessage> DeleteAsync([FromODataUri] Guid key)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            await this.repository.DeleteHymnAsync(key);
            return this.Request.CreateResponse(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
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
    }
}