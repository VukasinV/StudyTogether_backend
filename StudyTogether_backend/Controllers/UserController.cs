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
using StudyTogether_backend.Code;
using StudyTogether_backend.Models;
using StudyTogether_backend.Filters;
using System.Configuration;

namespace StudyTogether_backend.Controllers
{
    public class UserController : ApiController
    {
        private StudyTogetherEntities db = new StudyTogetherEntities();


        [JwtAuthentication]
        public IHttpActionResult GetUser()
        {
            int userId = JwtManager.GetUserId(Request.Headers.Authorization.Parameter);

            var profile = db.Profile.Where(x => x.UserId == userId).Select(x => new
            {
                x.ProfileId,
                x.User.Fullname,
                x.Description,
            });

            return Ok(profile);
        }

        // PUT: api/User/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUser(int id, User user)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //if (id != user.UserId)
            //{
            //    return BadRequest();
            //}

            //db.Entry(user).State = EntityState.Modified;

            //try
            //{
            //    db.SaveChanges();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!UserExists(id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            //return StatusCode(HttpStatusCode.NoContent);

            throw new NotImplementedException();
        }

        [AllowAnonymous]
        public IHttpActionResult PostUser(User user)
        {
            string hashAlgoritm = ConfigurationManager.AppSettings["HashAlgoritm"];

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (db.User.Any(x => x.Username == user.Username))
                return Conflict();

            byte[] salt = HashManager.Instance.GenerateSalt(16);

            // assumed that default algorithm is sha256 so it doesn't throw exception
            string passwordHash = Convert.ToBase64String(HashManager.Instance.HashPassword(user.Password, salt));

            switch (hashAlgoritm)
            {
                case "sha256": passwordHash = Convert.ToBase64String(HashManager.Instance.HashPassword(user.Password, salt));
                    break;
                case "sha512": passwordHash = Convert.ToBase64String(HashManager512.Instance.HashPassword(user.Password, salt));
                    break;
            }

            user.Salt = Convert.ToBase64String(salt);
            user.Password = passwordHash;
            user.HashAlgorithm = hashAlgoritm;
            user.EmailConfirmed = false;
            user.DateCreated = DateTime.Now;
            user.DateModified = null;
            user.TwoFaEnabled = false;
            user.RoleId = 2;
            

            db.User.Add(user);
            db.SaveChanges();

            CreateProfile(user.UserId);

            return Ok("User successfuly created!");
        }

        // DELETE: api/User/5
        [AllowAnonymous]
        [ResponseType(typeof(User))]
        public IHttpActionResult DeleteUser(int id)
        {
            User user = db.User.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            db.User.Remove(user);
            db.SaveChanges();

            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(int id)
        {
            return db.User.Count(e => e.UserId == id) > 0;
        }

        private void CreateProfile(int userId)
        {
            db.Profile.Add(new Profile
            {
                UserId = userId
            });

            db.SaveChanges();
        }
    }
}