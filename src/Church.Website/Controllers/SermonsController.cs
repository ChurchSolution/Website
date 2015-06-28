// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SermonsController.cs" company="Church">
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
    /// Provides the sermons controller.
    /// </summary>
    public class SermonsController : ApiController
    {
        /// <summary>
        /// The repository.
        /// </summary>
        private readonly IRepository repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SermonsController"/> class.
        /// </summary>
        public SermonsController()
            : this(
                Church.Models.EntityFramework.Repository.Create(
                    Utilities.FrameworkEntitiesConnectionString,
                    Utilities.Configuration.ChurchWebsiteLibrary))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SermonsController"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository from unit tests.
        /// </param>
        internal SermonsController(IRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Gets a list of hymns.
        /// </summary>
        /// <returns>The <see cref="IQueryable{ISermon}"/> on sermons.</returns>
        public IQueryable<ISermon> Get()
        {
            return this.repository.GetSermons();
        }

        /// <summary>
        /// Gets a sermon.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="HttpResponseMessage"/> with the  <see cref="ISermon"/>.</returns>
        [ResponseType(typeof(ISermon))]
        public async Task<HttpResponseMessage> GetAsync(Guid id)
        {
            var sermon = await this.repository.GetSermons().SingleAsync(s => s.Id.Equals(id));

            return this.Request.CreateResponse(HttpStatusCode.OK, sermon);
        }

        /// <summary>
        /// Updates a sermon.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="sermon">The sermon.</param>
        /// <returns>The <see cref="HttpResponseMessage"/>.</returns>
        public async Task<HttpResponseMessage> PutAsync(Guid id, ISermon sermon)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            if (id != sermon.Id)
            {
                var message = string.Format(
                    "The id '{0}' in the URL doesn't match the one '{1}' in the request body.",
                    id,
                    sermon.Id);
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
            }

            await this.repository.UpdateSermonAsync(sermon);
            return this.Request.CreateResponse(HttpStatusCode.Accepted);
        }

        /// <summary>
        /// Creates a sermon.
        /// </summary>
        /// <param name="sermon">The sermon.</param>
        /// <returns>The <see cref="HttpResponseMessage"/> with the  <see cref="ISermon"/>.</returns>
        [ResponseType(typeof(ISermon))]
        public async Task<HttpResponseMessage> PostAsync(ISermon sermon)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            await this.repository.AddSermonAsync(sermon);
            return this.Request.CreateResponse(HttpStatusCode.Created, sermon);
        }

        /// <summary>
        /// Deletes a sermon.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="HttpResponseMessage"/>.</returns>
        public async Task<HttpResponseMessage> DeleteAsync(Guid id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            await this.repository.DeleteSermonAsync(id);
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