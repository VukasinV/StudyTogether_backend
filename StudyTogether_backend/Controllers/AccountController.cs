using StudyTogether_backend.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using StudyTogether_backend.Models;
using StudyTogether_backend.Code;

namespace StudyTogether_backend.Controllers
{
    public class AccountController : ApiController
    {
        private StudyTogetherEntities db = new StudyTogetherEntities();

        [AllowAnonymous]
        public IHttpActionResult PostAccount(HttpRequestMessage message)
        {
            // Safer way for sending this may be via Basic authentication with username and password, consider changing

            if (!message.Headers.Contains("username") || !message.Headers.Contains("password"))
                return BadRequest("Missing right headers!");

            var username = message.Headers.GetValues("username").FirstOrDefault();
            var password = message.Headers.GetValues("password").FirstOrDefault();

            if (username == null || password == null)
                return BadRequest("Value of headers is empty!");

            if (VerifyUser(username, password))
            {
                int userId = db.User.Where(x => x.Username == username).Select(x => x.UserId).FirstOrDefault();

                string token = JwtManager.GenerateToken(userId);

                return Ok(token);
            }

            return Unauthorized();
        }

        private bool VerifyUser(string username, string password)
        {
            if (!db.User.Any(x => x.Username.Equals(username)))
                return false;

            var salt = Convert.FromBase64String(db.User.Where(x => x.Username == username).Select(x => x.Salt).Single());

            var HPassword = Convert.ToBase64String(HashManager.Instance.HashPassword(password, salt));

            if (HPassword == db.User.Where(x => x.Username == username).Select(x => x.PasswordHash).Single())
                return true;

            return false;
        }
    }
}
