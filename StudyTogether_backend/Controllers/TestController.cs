using StudyTogether_backend.Filters;
using StudyTogether_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace StudyTogether_backend.Controllers
{
    public class TestController : ApiController
    {
        private StudyTogetherEntities db = new StudyTogetherEntities();


        [JwtAuthentication]
        public IHttpActionResult Get()
        {
            var userId = JwtManager.getUserId(Request.Headers.Authorization.Parameter);

            string username = db.User.Where(x => x.UserId == userId).Select(x => x.Username).Single();

            return Ok(username);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
