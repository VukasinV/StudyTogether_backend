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
using StudyTogether_backend.Models;

namespace StudyTogether_backend.Controllers
{
    public class MeetingController : ApiController
    {
        private StudyTogetherEntities db = new StudyTogetherEntities();

        // GET: api/Meeting
        public IHttpActionResult GetMeeting()
        {
            var meetings = db.Meeting.Select(x => new
            {
                x.Location,
                x.StartsAt,
                x.Description,
                x.Capacity,
            });

            return Ok(meetings);
        }

        //// GET: api/Meeting/5
        //[ResponseType(typeof(Meeting))]
        //public IHttpActionResult GetMeeting(int id)
        //{
        //    Meeting meeting = db.Meeting.Find(id);
        //    if (meeting == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(meeting);
        //}

        // PUT: api/Meeting/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutMeeting(int id, Meeting meeting)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != meeting.MeetingId)
            {
                return BadRequest();
            }

            db.Entry(meeting).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MeetingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Meeting
        [ResponseType(typeof(Meeting))]
        public IHttpActionResult PostMeeting([FromBody]MeetingDTO meetingDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Meeting meeting = new Meeting
            //{
            //    Description = meetingDTO.Description,
            //    Capacity = meetingDTO.Capacity,
            //    Location = meetingDTO.Location,
            //    StartsAt = meetingDTO.StartsAt,
            //};

            //db.Meeting.Add(meeting);
            //db.SaveChanges();
            CreateMeeting(meetingDTO, true);

            int userId = db.User.Where(x => x.Username == meetingDTO.CreatedBy).Select(x => x.UserId).Single();

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

        public void CreateMeeting(MeetingDTO meetingDTO, bool a)
        {
            Meeting meeting = new Meeting
            {
                Description = meetingDTO.Description,
                Capacity = meetingDTO.Capacity,
                Location = meetingDTO.Location,
                StartsAt = meetingDTO.StartsAt,
            };

            db.Meeting.Add(meeting);
            db.SaveChanges();
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