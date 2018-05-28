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
        public IHttpActionResult GetUser(HttpRequestMessage message)
        {
            int userId = JwtManager.getUserId(Request.Headers.Authorization.Parameter);

            return Ok(userId);
        }

        // PUT: api/User/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUser(int id, User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.UserId)
            {
                return BadRequest();
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        [AllowAnonymous]
        [ResponseType(typeof(User))]
        public IHttpActionResult PostUser(UserDTO userDTO)
        {
            string hashAlgoritm = ConfigurationManager.AppSettings["HashAlgoritm"];

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (db.User.Any(x => x.Username == userDTO.Username))
                return Conflict();

            byte[] salt = HashManager.Instance.GenerateSalt(16);

            // assumed that default algorithm is sha256 so it doesn't throw exception
            string passwordHash = Convert.ToBase64String(HashManager.Instance.HashPassword(userDTO.Password, salt));

            switch (hashAlgoritm)
            {
                case "sha256": passwordHash = Convert.ToBase64String(HashManager.Instance.HashPassword(userDTO.Password, salt));
                    break;
                case "sha512": passwordHash = Convert.ToBase64String(HashManager512.Instance.HashPassword(userDTO.Password, salt));
                    break;
            }

            User user = new User
            {
                Username = userDTO.Username,
                Salt = Convert.ToBase64String(salt),
                PasswordHash = passwordHash,
                Fullname = userDTO.Fullname,
                Email = userDTO.Email,
                EmailConfirmed = false,
                DateCreated = DateTime.Now,
                DateModified = null,
                TwoFaEnabled = false,
                RoleId = 2
            };

            db.User.Add(user);

            db.SaveChanges();

            return Ok("User sueccessfuly created!");
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
    }
}