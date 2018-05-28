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
using StudyTogether_backend.Code;

namespace StudyTogether_backend.Controllers
{
    public class TestController : ApiController
    {
        private StudyTogetherEntities db = new StudyTogetherEntities();


        [JwtAuthentication]
        public IHttpActionResult Get()
        {

            EmailManager.SendMail("test", "test", "test");

            return Ok("Mail sent");

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
