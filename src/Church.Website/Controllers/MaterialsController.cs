namespace Church.Website.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Description;

    using Church.Website.Models;

    public class MaterialsController : ApiController
    {
        private FrameworkEntities db = new FrameworkEntities();

        // GET api/Materials
        public IQueryable<Material> GetMaterials()
        {
            return db.Materials;
        }

        // GET api/Materials/5
        [ResponseType(typeof(Material))]
        public IHttpActionResult GetMaterial(Guid id)
        {
            var material = db.Materials.Find(id);
            if (material == null)
            {
                return NotFound();
            }

            return Ok(material);
        }

        // PUT api/Materials/5
        public IHttpActionResult PutMaterial(Guid id, Material material)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != material.Id)
            {
                return BadRequest();
            }

            db.Entry(material).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaterialExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST api/Materials
        [ResponseType(typeof(Material))]
        public IHttpActionResult PostMaterial(Material material)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Materials.Add(material);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (MaterialExists(material.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = material.Id }, material);
        }

        // DELETE api/Materials/5
        [ResponseType(typeof(Material))]
        public IHttpActionResult DeleteMaterial(Guid id)
        {
            var material = db.Materials.Find(id);
            if (material == null)
            {
                return NotFound();
            }

            db.Materials.Remove(material);
            db.SaveChanges();

            return Ok(material);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }

        private bool MaterialExists(Guid id)
        {
            return db.Materials.Count(e => e.Id == id) > 0;
        }
    }
}