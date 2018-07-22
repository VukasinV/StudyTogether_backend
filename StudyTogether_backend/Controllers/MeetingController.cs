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
        [JwtAuthentication]
        public IHttpActionResult GetMeeting()
        {
            List<MeetingDTO> allMeetings = new List<MeetingDTO>();

            var meetings = db.Meeting.ToList();

            foreach(var meeting in meetings)
            {
                allMeetings.Add(new MeetingDTO
                {
                    MeetingId = meeting.MeetingId,
                    Location = meeting.Location,
                    StartsAt = meeting.StartsAt,
                    Description = meeting.Description,
                    Capacity = meeting.Capacity,
                    CreatedBy = meeting.Participant.Where(x => x.Owner == true).Select(x => x.Profile.User.Fullname).FirstOrDefault(),
                    Participants = meeting.Participant.Where(x => x.Owner == false).Select(x => x.Profile.User.Fullname).ToArray(),
                });
            }

            return Ok(allMeetings);
        }

        // GET: api/Meeting/5
        [ResponseType(typeof(Meeting))]
        public IHttpActionResult GetMeeting(int id)
        {
            Meeting meeting = db.Meeting.Find(id);
            if (meeting == null)
            {
                return NotFound();
            }

            return Ok(meeting);
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
        public IHttpActionResult PostMeeting([FromBody]MeetingDTO meetingDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Meeting meeting = new Meeting
            {
                Description = meetingDTO.Description, 
                Capacity = meetingDTO.Capacity,
                Location = meetingDTO.Location,
                StartsAt = meetingDTO.StartsAt,
            };

            db.Meeting.Add(meeting);
            db.SaveChanges();

            int userId = JwtManager.getUserId(Request.Headers.Authorization.Parameter);

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
            int pID = db.Profile.Where(x => x.UserId == userId).Select(x => x.ProfileId).FirstOrDefault();
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