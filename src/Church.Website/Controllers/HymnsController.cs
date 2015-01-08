namespace Church.Website.Controllers
{
    using Church.Website.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Description;

    public class HymnsController : ApiController
    {
        private FrameworkEntities entities;

        public HymnsController()
            : this(new FrameworkEntities())
        {
        }

        internal HymnsController(FrameworkEntities entities)
        {
            this.entities = entities;
        }

        // GET api/Hymns
        public IQueryable<Hymn> GetHymns()
        {
            return this.entities.Hymns;
        }

        // GET api/Hymns/5
        [ResponseType(typeof(Hymn))]
        public HttpResponseMessage GetHymn(string id, string name, string source)
        {
            Guid sermonId;
            var sermon = Guid.TryParse(id, out sermonId) ? this.entities.Hymns.Find(id) :
                this.entities.Hymns.FirstOrDefault(
                s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                    && s.Source.Equals(source, StringComparison.OrdinalIgnoreCase));
            if (sermon == null)
            {
                return this.Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return this.Request.CreateResponse(HttpStatusCode.OK, sermon);
        }

        // PUT api/Sermons/5
        public IHttpActionResult PutHymn(Guid id, Sermon sermon)
        {
            if (!this.ModelState.IsValid)
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
                if (!this.HymnExists(id))
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

        // POST api/Hymns
        [ResponseType(typeof(Hymn))]
        public IHttpActionResult PostHymn(Sermon sermon)
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
                if (this.HymnExists(sermon.Id))
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

        // DELETE api/Hymns/5
        [ResponseType(typeof(Hymn))]
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

        private bool HymnExists(Guid id)
        {
            return this.entities.Hymns.Count(e => e.Id == id) > 0;
        }
    }
}