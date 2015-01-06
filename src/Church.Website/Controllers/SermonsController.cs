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
        private FrameworkEntities entities;

        public SermonsController()
            : this(new FrameworkEntities())
        {
        }

        internal SermonsController(FrameworkEntities entities)
        {
            this.entities = entities;
        }

        // GET api/Sermons
        public IQueryable<Sermon> GetSermons()
        {
            return this.entities.Sermons;
        }

        // GET api/Sermons/5
        [ResponseType(typeof(Sermon))]
        public HttpResponseMessage GetSermon(string id, string speaker, DateTime date, string title)
        {
            Guid sermonId;
            var sermon = Guid.TryParse(id, out sermonId) ? this.entities.Sermons.Find(id) :
                this.entities.Sermons.FirstOrDefault(
                s => s.Speaker.Equals(speaker, StringComparison.OrdinalIgnoreCase)
                    && s.Date == date
                    && s.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
            if (sermon == null)
            {
                return this.Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return this.Request.CreateResponse(HttpStatusCode.OK, sermon);
        }

        // PUT api/Sermons/5
        public IHttpActionResult PutSermon(Guid id, Sermon sermon)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(ModelState);
            }

            if (id != sermon.Id)
            {
                return this.BadRequest();
            }

            this.entities.Entry(sermon).State = EntityState.Modified;

            try
            {
                this.entities.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SermonExists(id))
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

        // POST api/Sermons
        [ResponseType(typeof(Sermon))]
        public IHttpActionResult PostSermon(Sermon sermon)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(ModelState);
            }

            this.entities.Sermons.Add(sermon);

            try
            {
                this.entities.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (this.SermonExists(sermon.Id))
                {
                    return this.Conflict();
                }
                else
                {
                    throw;
                }
            }

            return this.CreatedAtRoute("DefaultApi", new { id = sermon.Id }, sermon);
        }

        // DELETE api/Sermons/5
        [ResponseType(typeof(Sermon))]
        public IHttpActionResult DeleteSermon(Guid id)
        {
            var sermon = this.entities.Sermons.Find(id);
            if (sermon == null)
            {
                return this.NotFound();
            }

            this.entities.Sermons.Remove(sermon);
            this.entities.SaveChanges();

            return this.Ok(sermon);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.entities.Dispose();
            }

            base.Dispose(disposing);
        }

        private bool SermonExists(Guid id)
        {
            return this.entities.Sermons.Count(e => e.Id == id) > 0;
        }
    }
}