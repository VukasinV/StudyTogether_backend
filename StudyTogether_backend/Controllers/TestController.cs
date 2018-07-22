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
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Net.Http.Headers;

namespace StudyTogether_backend.Controllers
{
    public class TestController : ApiController
    {
        private StudyTogetherEntities db = new StudyTogetherEntities();
        public List<Korisnik> Korisnici = Korisnik.Napuni();

        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage Get()
        {
            MemoryStream ms = new MemoryStream(db.Profile.Where(x => x.ProfileId == 1).Select(x => x.Picture).FirstOrDefault());
            Image returnImage = Image.FromStream(ms);
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(ms.ToArray());
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            return result;
        }

        // POST api/values
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Upload()
        {
            try
            {

                if (!Request.Content.IsMimeMultipartContent())
                {
                    return this.StatusCode(HttpStatusCode.UnsupportedMediaType);
                }

                var filesProvider = await Request.Content.ReadAsMultipartAsync();
                var fileContents = filesProvider.Contents.FirstOrDefault();
                if (fileContents == null)
                {
                    return this.BadRequest("Missing file");
                }

                byte[] payload = await fileContents.ReadAsByteArrayAsync();
                Profile profile = db.Profile.Find(1);
                profile.Picture = payload;
                db.Profile.Attach(profile);
                db.Entry(profile).Property(x => x.Picture).IsModified = true;
                db.SaveChanges();
                // TODO: do something with the payload.
                // note that this method is reading the uploaded file in memory
                // which might not be optimal for large files. If you just want to
                // save the file to disk or stream it to another system over HTTP
                // you should work directly with the fileContents.ReadAsStreamAsync() stream

                return Ok(new
                {
                    Result = "file uploaded successfully",
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            finally
            {

            }
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

        public static List<Korisnik> Napuni()
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
