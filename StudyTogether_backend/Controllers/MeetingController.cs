using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using StudyTogether_backend.Filters;
using StudyTogether_backend.Models;

namespace StudyTogether_backend.Controllers
{
    public class MeetingController : ApiController
    {
        private StudyTogetherEntities db = new StudyTogetherEntities();

        // GET: api/Meeting
        [HttpGet]
        [JwtAuthentication]
        public IHttpActionResult GetMeeting()
        {
            var meetings = db.Meeting.Where(x => x.StartsAt > DateTime.Now).Select(x => new
            {
                x.MeetingId,
                x.Location,
                x.StartsAt,
                x.Description,
                x.Capacity,
                Owner = x.Participant.Where(y => y.Owner == true)
                                        .Select(y => y.Profile.User.Fullname)
                                        .FirstOrDefault(),
                OwnerId = x.Participant.Where(y => y.Owner == true)
                                        .Select(y => y.ProfileId)
                                        .FirstOrDefault(),
                Participants = x.Participant.Where(y => y.Owner == false)
                                            .Select(y => y.Profile.User.Fullname)
                                            .ToList(),
            });

            return Ok(meetings);
        }

        [HttpGet]
        [JwtAuthentication]
        public IHttpActionResult GetMeetingsByUser(int id)
        {
            var meetings = db.Participant.Where(x => x.ProfileId == id && x.Owner).Select(x => new
            {
                x.MeetingId,
                x.Meeting.Location,
                x.Meeting.StartsAt,
                x.Meeting.Description,
                x.Meeting.Capacity,
                Participants = db.Participant.Where(y => y.MeetingId == x.MeetingId && !x.Owner).ToList()
            });

            return Ok(meetings);
        }


        [HttpGet]
        [JwtAuthentication]
        [Route("api/mymeetings")]
        public IHttpActionResult GetMyMeetings()
        {
            int id = JwtManager.GetUserId(Request.Headers.Authorization.Parameter);
            var meetings = db.Participant.Where(x => x.Profile.UserId == id && x.Owner).Select(x => new
            {
                x.MeetingId,
                x.Meeting.Location,
                x.Meeting.StartsAt,
                x.Meeting.Description,
                x.Meeting.Capacity,
                Participants = db.Participant.Where(y => y.MeetingId == x.MeetingId && !x.Owner).ToList()
            });

            return Ok(meetings);
        }

        // PUT: api/Meeting/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutMeeting(int id, Meeting meeting)
        {
            throw new NotImplementedException();
        }

        // POST: api/Meeting
        [JwtAuthentication]
        [ResponseType(typeof(Meeting))]
        public IHttpActionResult PostMeeting([FromBody]Meeting meeting)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Meeting.Add(meeting);
            db.SaveChanges();

            int userId = JwtManager.GetUserId(Request.Headers.Authorization.Parameter);

            CreateOwner(userId);

            return Ok("Meeting successfully created");
        }

        // DELETE: api/Meeting/5
        [ResponseType(typeof(Meeting))]
        public IHttpActionResult DeleteMeeting(int id)
        {
            Meeting meeting = db.Meeting.Find(id);
            if (meeting == null)
            {
                return NotFound();
            }

            db.Meeting.Remove(meeting);
            db.SaveChanges();

            return Ok(meeting);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MeetingExists(int id)
        {
            return db.Meeting.Count(e => e.MeetingId == id) > 0;
        }

        public void CreateOwner(int userId)
        {
            int pID = db.Profile.Where(x => x.UserId == userId)
                                .Select(x => x.ProfileId)
                                .FirstOrDefault();

            db.Participant.Add(new Participant
            {
                Owner = true,
                ProfileId = pID,
                MeetingId = db.Meeting.Max(x => x.MeetingId)
            });

            db.SaveChanges();
        }
    }
}