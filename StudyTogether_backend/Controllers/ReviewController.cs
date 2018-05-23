using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using StudyTogether_backend.Filters;
using StudyTogether_backend.Models;
using System.Web.Http.Description;

namespace StudyTogether_backend.Controllers
{
    public class ReviewController : ApiController
    {
        private StudyTogetherEntities db = new StudyTogetherEntities();

        public IHttpActionResult GetReview(HttpRequestMessage message)
        {
            string reviewedUsername = message.Headers.GetValues("username").FirstOrDefault();
            int userId = Convert.ToInt32(db.User.Select(x => x.Username == reviewedUsername).FirstOrDefault());

            var reviews = db.Review.Select(x => x.ReviewedProfileId == userId);

            return Ok(reviews);
        }

        [JwtAuthentication]
        [ResponseType(typeof(Meeting))]
        public IHttpActionResult PostReview([FromBody] ReviewDTO reviewDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int reviewersId = JwtManager.getUserId(Request.Headers.Authorization.Parameter);

            Review review = new Review
            {
                Mark = reviewDTO.Mark,
                DateOfAssessment = DateTime.Now,
                Description = reviewDTO.Description,
                ReviewerProfileId = reviewersId,
                ReviewedProfileId = db.User.Where(x => x.Username == reviewDTO.ReviewedUsername).Select(x => x.UserId).FirstOrDefault()
            };

            db.Review.Add(review);
            db.SaveChanges();

            return Ok("You successfully added review!");
        }
    }
}
