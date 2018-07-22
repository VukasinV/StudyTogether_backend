using StudyTogether_backend.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using StudyTogether_backend.Models;
using StudyTogether_backend.Code;
using System.Data.Entity;

namespace StudyTogether_backend.Controllers
{
    public class AccountController : ApiController
    {
        private StudyTogetherEntities db = new StudyTogetherEntities();

        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult PostAccount(HttpRequestMessage message)
        {
            if (!CheckHeaders(message, out string responseMessage))
                return BadRequest(responseMessage);

            var username = message.Headers.GetValues("username").FirstOrDefault();
            var password = message.Headers.GetValues("password").FirstOrDefault();

            if (VerifyUser(username, password))
            {

                bool sendEmailConformation = db.User.Where(x => x.Username == username).Select(x => x.TwoFaEnabled).FirstOrDefault();

                if(sendEmailConformation)
                {
                    int sid = JwtManager.generateConfirmationSid();
                    int userId = db.User.Where(x => x.Username == username).Select(x => x.UserId).FirstOrDefault();
                    string authToken = JwtManager.GenerateAuthToken(sid);

                    try
                    {
                        if (!ModelState.IsValid)
                        {
                            return BadRequest(ModelState);
                        }

                        User user = db.User.Find(userId);
                        user.TokenAuth = authToken;

                        db.User.Attach(user);
                        db.Entry(user).Property(x => x.TokenAuth).IsModified = true;
                        db.SaveChanges();

                    } catch(Exception exception)
                    {
                        throw exception;
                    }

                    string email = db.User.Where(x => x.UserId == userId).Select(x => x.Email).FirstOrDefault();
                    EmailManager.SendMail(email, $"Your conformation code is: {sid}", "Confirm your identity");
                    string token = JwtManager.GenerateToken(userId, role: "notUser");
                    return Created("Created",token);

                } else
                {
                    int userId = db.User.Where(x => x.Username == username).Select(x => x.UserId).FirstOrDefault();
                    string token = JwtManager.GenerateToken(userId);
                    return Ok(token);
                }
            }

            return Unauthorized();
        }

        private bool VerifyUser(string username, string password)
        {
            if (!db.User.Any(x => x.Username.Equals(username)))
                return false;

            var salt = Convert.FromBase64String(db.User.Where(x => x.Username == username).Select(x => x.Salt).Single());

            var hashAlgorithm = db.User.Where(x => x.Username == username).Select(x => x.HashAlgorithm).Single();

            // assumed that default algorithm is sha256 so it doesn't throw exception
            var HPassword = Convert.ToBase64String(HashManager.Instance.HashPassword(password, salt));

            switch (hashAlgorithm)
            {
                case "sha256": HPassword = Convert.ToBase64String(HashManager.Instance.HashPassword(password, salt));
                    break;
                case "sha512": HPassword = Convert.ToBase64String(HashManager512.Instance.HashPassword(password, salt));
                    break;
            }


            if (HPassword == db.User.Where(x => x.Username == username).Select(x => x.PasswordHash).Single())
                return true;

            return false;
        }

        private bool CheckHeaders (HttpRequestMessage message, out string responseMessage)
        {
            if (!message.Headers.Contains("username") || !message.Headers.Contains("password"))
            {
                responseMessage = "Missing right headers!";
                return false;
            }

            var username = message.Headers.GetValues("username").FirstOrDefault();
            var password = message.Headers.GetValues("password").FirstOrDefault();

            if (username == null || password == null)
            {
                responseMessage = "Value of headers is empty!";
                return false;
            }

            responseMessage = "Headers are ok";
            return true;
        }
    }
}
