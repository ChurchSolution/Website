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

    public class HymnsController : ApiController
    {
        private FrameworkEntities db = new FrameworkEntities();// DataProvider.Instance.FrameworkDatabase;

        // GET api/Hymns
        public IQueryable<Hymn> GetHymns()
        {
            return db.Hymns;
        }

        // GET api/Hymns/5
        [ResponseType(typeof(Hymn))]
        public IHttpActionResult GetHymn(Guid id)
        {
            var sermon = db.Hymns.Find(id);
            if (sermon == null)
            {
                return NotFound();
            }

            return Ok(sermon);
        }

        // PUT api/Sermons/5
        public IHttpActionResult PutHymn(Guid id, Sermon sermon)
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
                if (!HymnExists(id))
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

        // POST api/Hymns
        [ResponseType(typeof(Hymn))]
        public IHttpActionResult PostHymn(Sermon sermon)
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
                if (HymnExists(sermon.Id))
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

        // DELETE api/Hymns/5
        [ResponseType(typeof(Hymn))]
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

        private bool HymnExists(Guid id)
        {
            return db.Hymns.Count(e => e.Id == id) > 0;
        }
    }
}