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
    using System.Web.Http;
    using System.Web.Http.Description;

    using Church.Models;
    using Church.Website.Models;

    /// <summary>
    /// Provides the hymns controller.
    /// </summary>
    public class HymnsController : ApiController
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
        public IQueryable<IHymn> Get()
        {
            return this.repository.GetHymns();
        }

        /// <summary>
        /// Gets a hymn.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="HttpResponseMessage"/> with the  <see cref="IHymn"/>.</returns>
        [ResponseType(typeof(IHymn))]
        public async Task<HttpResponseMessage> GetAsync(Guid id)
        {
            var hymn = await this.repository.GetHymns().SingleAsync(h => h.Id.Equals(id));

            return this.Request.CreateResponse(HttpStatusCode.OK, hymn);
        }

        /// <summary>
        /// Updates a hymn.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="hymn">The hymn.</param>
        /// <returns>The <see cref="HttpResponseMessage"/>.</returns>
        public async Task<HttpResponseMessage> PutAsync(Guid id, IHymn hymn)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            if (id != hymn.Id)
            {
                var message = string.Format(
                    "The id '{0}' in the URL doesn't match the one '{1}' in the request body.",
                    id,
                    hymn.Id);
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
            }

            await this.repository.UpdateHymnsAsync(hymn);
            return this.Request.CreateResponse(HttpStatusCode.Accepted);
        }

        /// <summary>
        /// Creates a hymn.
        /// </summary>
        /// <param name="hymn">The hymn.</param>
        /// <returns>The <see cref="HttpResponseMessage"/> with the  <see cref="IHymn"/>.</returns>
        [ResponseType(typeof(IHymn))]
        public async Task<HttpResponseMessage> PostAsync(IHymn hymn)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            await this.repository.AddHymnAsync(hymn);
            return this.Request.CreateResponse(HttpStatusCode.Created, hymn);
        }

        /// <summary>
        /// Deletes a hymn.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="HttpResponseMessage"/>.</returns>
        public async Task<HttpResponseMessage> DeleteAsync(Guid id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            await this.repository.DeleteHymnsAsync(id);
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