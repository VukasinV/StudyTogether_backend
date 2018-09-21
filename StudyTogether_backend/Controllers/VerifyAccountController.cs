using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using StudyTogether_backend.Filters;
using StudyTogether_backend.Models;

namespace StudyTogether_backend.Controllers
{
    public class VerifyAccountController : ApiController
    {
        private StudyTogetherEntities db = new StudyTogetherEntities();

        [JwtAuthentication]
        public IHttpActionResult Post(HttpRequestMessage message)
        {
            int sid = Convert.ToInt32(message.Headers.GetValues("conformationCode").FirstOrDefault());
            int userId = JwtManager.GetUserId(Request.Headers.Authorization.Parameter);
            string authToken = db.User.Where(x => x.UserId == userId).Select(x => x.TokenAuth).FirstOrDefault();
            int dbConformationSid = JwtManager.GetUserConformationCode(authToken);

            if (sid == dbConformationSid)
            {
                string token = JwtManager.GenerateToken(userId);
                return Ok(token);
            }
            return Unauthorized();
        }

        // PUT: api/VerifyAccount/5
        public void Put(int id, [FromBody]string value)
        {
            throw new NotImplementedException();
        }

        // DELETE: api/VerifyAccount/5
        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
