using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using StudyTogether_backend.Models;

namespace StudyTogether_backend.Controllers
{
    public class UserController : ApiController
    {
        private StudyTogetherEntities db = new StudyTogetherEntities();

        // Get list of all users or login one user
        // GET: api/User/5
        [ResponseType(typeof(User))]
        public IHttpActionResult GetUser(HttpRequestMessage message)
        {

            if (message.Headers.Contains("conformationNeeded") && message.Headers.GetValues("conformationNeeded").FirstOrDefault() == "false")
            {
                var users = db.User.Select(x => new {
                    x.Username
                });

                return Ok(users);
            }

            if (!message.Headers.Contains("username") || !message.Headers.Contains("password"))
                return BadRequest();


            var username = message.Headers.GetValues("username").FirstOrDefault();  
            var password = message.Headers.GetValues("password").FirstOrDefault();

            if (username == null || password == null)
                return BadRequest();


            if (!db.User.Any(x => x.Username == username))
            {
                return NotFound();
            }

            var salt = Convert.FromBase64String(db.User.Where(x => x.Username == username).Select(x => x.Salt).Single());

            var HPassword = Convert.ToBase64String(HashPassword(password, salt));
            
            if (HPassword == db.User.Where(x => x.Username == username).Select(x => x.PasswordHash).Single())
                return Ok("Logged In");

            return Unauthorized();
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

        // Create new user
        // POST: api/User
        [ResponseType(typeof(User))]
        public IHttpActionResult PostUser(UserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (db.User.Any(x => x.Username == userDTO.Username))
                return Conflict();

            byte[] salt = GenerateSalt(16);


            User user = new User
            {
                Username = userDTO.Username,
                Salt = Convert.ToBase64String(salt),
                PasswordHash = Convert.ToBase64String(HashPassword(userDTO.Password, salt)),
                Fullname = userDTO.Fullname,
                Email = userDTO.Email,
                EmailConfirmed = false,
                DateCreated = DateTime.Now,
                DateModified = null,
                RoleId = 2
            };

            db.User.Add(user);

            db.SaveChanges();

            //return CreatedAtRoute("DefaultApi", new { id = user.UserId }, user);
            return Ok("User sueccessfuly created!");
        }

        // DELETE: api/User/5
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

        private byte[] GenerateSalt(int size)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] salt = new byte[size];
            rng.GetBytes(salt);

            return salt;
        }

        private byte[] HashPassword(string password, byte[] salt)
        {
            HashAlgorithm algorithm = new SHA256Managed();
            byte[] pwd = Encoding.UTF8.GetBytes(password);

            byte[] PwdSalt = new byte[pwd.Length + salt.Length];

            for (int i = 0; i < pwd.Length; i++)
            {
                PwdSalt[i] = pwd[i];
            }

            for (int i = 0; i < salt.Length; i++)
            {
                PwdSalt[pwd.Length + i] = salt[i];
            }

            return algorithm.ComputeHash(PwdSalt);
        }
    }
}