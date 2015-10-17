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
    using System.Web.Http.Description;
    using System.Web.OData;

    using Church.Models;
    using Church.Website.Models;

    /// <summary>
    /// Provides the sermons controller.
    /// </summary>
    public class SermonsController : AbstractODataController
    {
        /// <summary>
        /// Gets a list of hymns.
        /// </summary>
        /// <returns>The <see cref="IQueryable{ISermon}"/> on sermons.</returns>
        [EnableQuery]
        public IQueryable<Sermon> Get()
        {
            return this.Repository.GetSermons();
        }

        /// <summary>
        /// Gets a sermon.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="HttpResponseMessage"/> with the  <see cref="Sermon"/>.</returns>
        [EnableQuery]
        public async Task<Sermon> GetAsync([FromODataUri] Guid key)
        {
            var sermon = await this.Repository.GetSermons().SingleAsync(s => s.Id.Equals(key));
            return sermon;
        }

        /// <summary>
        /// Creates a sermon.
        /// </summary>
        /// <param name="sermon">The sermon.</param>
        /// <returns>The <see cref="HttpResponseMessage"/> with the <see cref="Sermon"/>.</returns>
        [ResponseType(typeof(Sermon))]
        public async Task<HttpResponseMessage> PostAsync(Sermon sermon)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            await this.Repository.AddSermonAsync(sermon);
            return this.Request.CreateResponse(HttpStatusCode.Created, sermon);
        }

        /// <summary>
        /// Patches a sermon.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="sermon">The sermon.</param>
        /// <returns>The <see cref="HttpResponseMessage"/> with the <see cref="Sermon"/>.</returns>
        [ResponseType(typeof(Sermon))]
        public async Task<HttpResponseMessage> PatchAsync([FromODataUri] Guid key, Delta<Sermon> sermon)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            var sermonToBeUpdated = await this.Repository.GetSermons().SingleAsync(s => s.Id.Equals(key));
            sermon.Patch(sermonToBeUpdated);
            await this.Repository.UpdateSermonAsync(sermonToBeUpdated);
            return this.Request.CreateResponse(HttpStatusCode.Accepted, sermonToBeUpdated);
        }

        /// <summary>
        /// Updates a sermon.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="sermon">The sermon.</param>
        /// <returns>The <see cref="HttpResponseMessage"/> with the <see cref="Sermon"/>.</returns>
        [ResponseType(typeof(Sermon))]
        public async Task<HttpResponseMessage> PutAsync([FromODataUri] Guid key, Sermon sermon)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            if (key != sermon.Id)
            {
                var message = string.Format(
                    "The key '{0}' in the URL doesn't match the one '{1}' in the request body.",
                    key,
                    sermon.Id);
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
            }

            await this.Repository.UpdateSermonAsync(sermon);
            return this.Request.CreateResponse(HttpStatusCode.Accepted, sermon);
        }

        /// <summary>
        /// Deletes a sermon.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="HttpResponseMessage"/>.</returns>
        public async Task<HttpResponseMessage> DeleteAsync([FromODataUri] Guid key)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            await this.Repository.DeleteSermonAsync(key);
            return this.Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}