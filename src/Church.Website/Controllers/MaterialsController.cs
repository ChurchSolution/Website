namespace Church.Website.Controllers
{
    using Church.Website.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;

    public class MaterialsController : ApiController
    {
        private FrameworkEntities entities;

        public MaterialsController()
            : this(new FrameworkEntities())
        {
        }

        internal MaterialsController(FrameworkEntities entities)
        {
            this.entities = entities;
        }

        // GET api/Materials
        [HttpGet]
        public Task<IQueryable<Material>> GetMaterials()
        {
            return Task.FromResult<IQueryable<Material>>(this.entities.Materials);
        }

        // GET api/Materials/5
        [HttpGet]
        [ResponseType(typeof(Material))]
        public async Task<HttpResponseMessage> GetMaterialAsync(Guid id)
        {
            var material = await this.entities.Materials.FindAsync(id);
            if (material == null)
            {
                return this.Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return this.Request.CreateResponse(HttpStatusCode.OK, material);
        }

        // PUT api/Materials/5
        public IHttpActionResult PutMaterial(Guid id, Material material)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(ModelState);
            }

            if (id != material.Id)
            {
                return this.BadRequest();
            }

            this.entities.Entry(material).State = EntityState.Modified;

            try
            {
                this.entities.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!this.MaterialExists(id))
                {
                    return this.NotFound();
                }
                else
                {
                    throw;
                }
            }

            return this.StatusCode(HttpStatusCode.NoContent);
        }

        // POST api/Materials
        [ResponseType(typeof(Material))]
        public IHttpActionResult PostMaterial(Material material)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(ModelState);
            }

            this.entities.Materials.Add(material);

            try
            {
                this.entities.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (this.MaterialExists(material.Id))
                {
                    return this.Conflict();
                }
                else
                {
                    throw;
                }
            }

            return this.CreatedAtRoute("DefaultApi", new { id = material.Id }, material);
        }

        // DELETE api/Materials/5
        [ResponseType(typeof(Material))]
        public IHttpActionResult DeleteMaterial(Guid id)
        {
            var material = this.entities.Materials.Find(id);
            if (material == null)
            {
                return this.NotFound();
            }

            this.entities.Materials.Remove(material);
            this.entities.SaveChanges();

            return this.Ok(material);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.entities.Dispose();
            }

            base.Dispose(disposing);
        }

        private bool MaterialExists(Guid id)
        {
            return this.entities.Materials.Count(e => e.Id == id) > 0;
        }
    }
}