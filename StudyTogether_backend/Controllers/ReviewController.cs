using StudyTogether_backend.Filters;
using StudyTogether_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace StudyTogether_backend.Controllers
{
    public class ReviewController : ApiController
    {
        private StudyTogetherEntities db = new StudyTogetherEntities();
        
        // GET: api/Review
        [HttpGet]
        [JwtAuthentication]
        public IHttpActionResult Get()
        {
            int userId = JwtManager.GetUserId(Request.Headers.Authorization.Parameter);

            int profileId = db.Profile.Where(x => x.UserId == userId).Select(x => x.ProfileId).FirstOrDefault();

            var result = db.Review.Where(x => x.ReviewedProfileId == profileId);

            return Ok(result);
        }

        // GET: api/Review/5
        public IHttpActionResult Get (int id)
        {
            var result = db.Review.Where(x => x.ReviewedProfileId == id).Select(x => new
            {
                x.Mark,
                x.Description
            });

            return Ok(result);
        }

        // POST: api/Review
        public IHttpActionResult Post([FromBody]Review review)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int reviewer = JwtManager.GetUserId(Request.Headers.Authorization.Parameter);
            review.ReviewerProfileId = reviewer;
            review.DateOfAssessment = DateTime.Now;

            db.Review.Add(review);
            db.SaveChanges();

            return Ok();
        }

        // PUT: api/Review/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Review/5
        public void Delete(int id)
        {
        }
    }
}
