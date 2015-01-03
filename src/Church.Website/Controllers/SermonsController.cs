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

    public class SermonsController : ApiController
    {
        private FrameworkEntities db = new FrameworkEntities();

        // GET api/Sermons
        public IQueryable<Sermon> GetSermons()
        {
            return db.Sermons;
        }

        // GET api/Sermons/5
        [ResponseType(typeof(Sermon))]
        public IHttpActionResult GetSermon(Guid id)
        {
            var sermon = db.Sermons.Find(id);
            if (sermon == null)
            {
                return NotFound();
            }

            return Ok(sermon);
        }

        // PUT api/Sermons/5
        public IHttpActionResult PutSermon(Guid id, Sermon sermon)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != sermon.Id)
            {
                return BadRequest();
            }

            db.Entry(sermon).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SermonExists(id))
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

        // POST api/Sermons
        [ResponseType(typeof(Sermon))]
        public IHttpActionResult PostSermon(Sermon sermon)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Sermons.Add(sermon);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (SermonExists(sermon.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = sermon.Id }, sermon);
        }

        // DELETE api/Sermons/5
        [ResponseType(typeof(Sermon))]
        public IHttpActionResult DeleteSermon(Guid id)
        {
            var sermon = db.Sermons.Find(id);
            if (sermon == null)
            {
                return NotFound();
            }

            db.Sermons.Remove(sermon);
            db.SaveChanges();

            return Ok(sermon);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }

        private bool SermonExists(Guid id)
        {
            return db.Sermons.Count(e => e.Id == id) > 0;
        }
    }
}