// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MaterialsController.cs" company="Church">
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
    /// Provides the materials controller.
    /// </summary>
    public class MaterialsController : ApiController
    {
        /// <summary>
        /// The repository.
        /// </summary>
        private readonly IRepository repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialsController"/> class.
        /// </summary>
        public MaterialsController()
            : this(
                Church.Models.EntityFramework.Repository.Create(
                    Utilities.FrameworkEntitiesConnectionString,
                    Utilities.Configuration.ChurchWebsiteLibrary))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialsController"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository from unit tests.
        /// </param>
        internal MaterialsController(IRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Gets a list of materials.
        /// </summary>
        /// <returns>The <see cref="IQueryable{IMaterial}"/> on materials.</returns>
        public IQueryable<IMaterial> Get()
        {
            return this.repository.GetMaterials();
        }

        /// <summary>
        /// Gets a material.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="HttpResponseMessage"/> with the  <see cref="IMaterial"/>.</returns>
        [ResponseType(typeof(IMaterial))]
        public async Task<HttpResponseMessage> GetAsync(Guid id)
        {
            var hymn = await this.repository.GetMaterials().SingleAsync(m => m.Id.Equals(id));

            return this.Request.CreateResponse(HttpStatusCode.OK, hymn);
        }

        /// <summary>
        /// Updates a material.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="material">The material.</param>
        /// <returns>The <see cref="HttpResponseMessage"/>.</returns>
        public async Task<HttpResponseMessage> PutAsync(Guid id, IMaterial material)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            if (id != material.Id)
            {
                var message = string.Format(
                    "The id '{0}' in the URL doesn't match the one '{1}' in the request body.",
                    id,
                    material.Id);
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
            }

            await this.repository.UpdateMaterialAsync(material);
            return this.Request.CreateResponse(HttpStatusCode.Accepted);
        }

        /// <summary>
        /// Creates a material.
        /// </summary>
        /// <param name="material">The material.</param>
        /// <returns>The <see cref="HttpResponseMessage"/> with the  <see cref="IMaterial"/>.</returns>
        [ResponseType(typeof(IMaterial))]
        public async Task<HttpResponseMessage> PostAsync(IMaterial material)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            await this.repository.AddMaterialAsync(material);
            return this.Request.CreateResponse(HttpStatusCode.Created, material);
        }

        /// <summary>
        /// Deletes a material.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="HttpResponseMessage"/>.</returns>
        public async Task<HttpResponseMessage> DeleteAsync(Guid id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            await this.repository.DeleteMaterialAsync(id);
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