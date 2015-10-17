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
    using System.Web.Http.Description;
    using System.Web.OData;

    using Church.Models;

    /// <summary>
    /// Provides the materials controller.
    /// </summary>
    public class MaterialsController : AbstractODataController
    {
        /// <summary>
        /// Gets a list of materials.
        /// </summary>
        /// <returns>The <see cref="IQueryable{IMaterial}"/> on materials.</returns>
        [EnableQuery]
        public IQueryable<Material> Get()
        {
            return this.Repository.GetMaterials();
        }

        /// <summary>
        /// Gets a material.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="HttpResponseMessage"/> with the <see cref="Material"/>.</returns>
        public async Task<Material> GetAsync([FromODataUri] Guid key)
        {
            return await this.Repository.GetMaterials().SingleOrDefaultAsync(m => m.Id.Equals(key));
        }

        /// <summary>
        /// Creates a material.
        /// </summary>
        /// <param name="material">The material.</param>
        /// <returns>The <see cref="HttpResponseMessage"/> with the <see cref="Material"/>.</returns>
        [ResponseType(typeof(Material))]
        public async Task<HttpResponseMessage> PostAsync(Material material)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            await this.Repository.AddMaterialAsync(material);
            return this.Request.CreateResponse(HttpStatusCode.Created, material);
        }

        /// <summary>
        /// Patches a material.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="material">The material.</param>
        /// <returns>The <see cref="HttpResponseMessage"/> with the <see cref="Material"/>.</returns>
        [ResponseType(typeof(Material))]
        public async Task<HttpResponseMessage> PatchAsync([FromODataUri] Guid key, Delta<Material> material)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            var materialToBeUpdated = await this.Repository.GetMaterials().SingleAsync(s => s.Id.Equals(key));
            material.Patch(materialToBeUpdated);
            await this.Repository.UpdateMaterialAsync(materialToBeUpdated);
            return this.Request.CreateResponse(HttpStatusCode.Accepted, materialToBeUpdated);
        }

        /// <summary>
        /// Updates a material.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="material">The material.</param>
        /// <returns>The <see cref="HttpResponseMessage"/> with the <see cref="Material"/>.</returns>
        [ResponseType(typeof(Material))]
        public async Task<HttpResponseMessage> PutAsync([FromODataUri] Guid key, Material material)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            if (key != material.Id)
            {
                var message = string.Format(
                    "The key '{0}' in the URL doesn't match the one '{1}' in the request body.",
                    key,
                    material.Id);
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
            }

            await this.Repository.UpdateMaterialAsync(material);
            return this.Request.CreateResponse(HttpStatusCode.Accepted, material);
        }

        /// <summary>
        /// Deletes a material.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="HttpResponseMessage"/>.</returns>
        public async Task<HttpResponseMessage> DeleteAsync([FromODataUri] Guid key)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
            }

            await this.Repository.DeleteMaterialAsync(key);
            return this.Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}