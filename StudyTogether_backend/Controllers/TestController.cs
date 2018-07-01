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
        public List<Korisnik> Korisnici = Korisnik.Napuni();

        [AllowAnonymous]
        public IHttpActionResult Get() 
        {
            var res = db.User.Select(x => new
            {
                TotalUsers = db.User.Count(),
                TotalAdmins = db.User.Count(y => y.RoleId == 1),
                MVPUser = "Milica Andric"
            }).First();

            return Ok(res);
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

    public class Korisnik
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static List <Korisnik> Napuni ()
        {
            List<Korisnik> test = new List<Korisnik>();
            test.Add(new Korisnik
            {
                Id = 1,
                Name = "Milan"
            });

            test.Add(new Korisnik
            {
                Id = 2,
                Name = "Jovan"
            });

            test.Add(new Korisnik
            {
                Id = 3,
                Name = "Zoran"
            });

            test.Add(new Korisnik
            {
                Id = 4,
                Name = "Milica"
            });

            test.Add(new Korisnik
            {
                Id = 5,
                Name = "Lana"
            });

            test.Add(new Korisnik
            {
                Id = 6,
                Name = "Marko"
            });

            test.Add(new Korisnik
            {
                Id = 7,
                Name = "Milenko"
            });

            return test;
        }
    }
}
