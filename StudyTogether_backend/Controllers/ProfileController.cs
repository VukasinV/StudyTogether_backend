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
using StudyTogether_backend.Models;
using StudyTogether_backend.Filters;

namespace StudyTogether_backend.Controllers
{
    public class ProfileController : ApiController
    {
        private StudyTogetherEntities db = new StudyTogetherEntities();

        // GET: api/Profile
        [JwtAuthentication]
        public IHttpActionResult GetProfiles()
        {
            var profiles = db.Profile.Select(x => new
            {
                x.ProfileId,
                x.User.Fullname,
                x.Description
            });

            return Ok(profiles);
        }

        // GET: api/Profile?fullname=milica
        [JwtAuthentication]
        public IHttpActionResult GetProfile(string fullname)
        {
            var profiles = db.Profile.Where(x => x.User.Fullname.StartsWith(fullname)).Select(x => new
            {
                x.ProfileId,
                x.User.Fullname,
                x.Description
            });

            return Ok(profiles);
        }

        // GET: api/Profile/5
        [JwtAuthentication]
        public IHttpActionResult GetProfile(int id)
        { 
            if (!db.Profile.Any(x => x.ProfileId == id))
            {
                return NotFound();
            }

            var profile = db.Profile.Where(x => x.ProfileId == id).Select(x => new
            {
                x.User.Fullname,
                x.Description,
                x.User.Role.RoleName
            }).FirstOrDefault();

            return Ok(profile);
        }

        [JwtAuthentication]
        [Route("api/myprofile")]
        public IHttpActionResult GetProfile()
        {
            int userid = JwtManager.GetUserId(Request.Headers.Authorization.Parameter);
            int id = db.Profile.Where(x => x.UserId == userid).Select(x => x.ProfileId).FirstOrDefault();

            if (!db.Profile.Any(x => x.ProfileId == id))
            {
                return NotFound();
            }

            var profile = db.Profile.Where(x => x.ProfileId == id).Select(x => new
            {
                x.User.Username,
                x.User.Fullname,
                x.Description,
                x.User.Role.RoleName
            }).FirstOrDefault();

            return Ok(profile);
        }

        // PUT: api/Profile/5
        [ResponseType(typeof(void))] 
        public IHttpActionResult PutProfile(int id, Profile profile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != profile.ProfileId)
            {
                return BadRequest();
            }

            db.Entry(profile).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfileExists(id))
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

        // POST: api/Profile
        [ResponseType(typeof(Profile))]
        public IHttpActionResult PostProfile(Profile profile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Profile.Add(profile);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = profile.ProfileId }, profile);
        }

        // DELETE: api/Profile/5
        [ResponseType(typeof(Profile))]
        public IHttpActionResult DeleteProfile(int id)
        {
            Profile profile = db.Profile.Find(id);
            if (profile == null)
            {
                return NotFound();
            }

            db.Profile.Remove(profile);
            db.SaveChanges();

            return Ok(profile);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProfileExists(int id)
        {
            return db.Profile.Count(e => e.ProfileId == id) > 0;
        }
    }
}