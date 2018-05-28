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
    public class ParticipantController : ApiController
    {
        private StudyTogetherEntities db = new StudyTogetherEntities();

        //[JwtAuthentication]
        //public IHttpActionResult GetParticipant(HttpRequestMessage message)
        //{
        //    int meetingId = Convert.ToInt32(message.Headers.GetValues("meetingId").FirstOrDefault());

        //    var participants = db.Participant.Where(x => x.MeetingId == meetingId).Select(x => new
        //    {
        //        x.Profile.User.Fullname
        //    });

        //    return Ok(participants);
        //}

        // GET: api/Participant/5
        // Send meetingId as parameter and check if participant is on this meeting
        [JwtAuthentication]
        [ResponseType(typeof(Participant))]
        public IHttpActionResult GetParticipant(int id)
        {
            int userId = JwtManager.getUserId(Request.Headers.Authorization.Parameter);
            int profileId = db.Profile.Where(x => x.UserId == userId).Select(x => x.ProfileId).FirstOrDefault();

            Participant participant = db.Participant.Find(profileId, id);

            if (participant == null)
            {
                return NotFound();
            }

            return Ok();
        }

        // PUT: api/Participant/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutParticipant(int id, Participant participant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != participant.ProfileId)
            {
                return BadRequest();
            }

            db.Entry(participant).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParticipantExists(id))
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

        // POST: api/Participant
        [JwtAuthentication]
        public IHttpActionResult PostParticipant(HttpRequestMessage message)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int meetingId = Convert.ToInt32(message.Headers.GetValues("MeetingId").FirstOrDefault());

            int userId = JwtManager.getUserId(Request.Headers.Authorization.Parameter);
            int profileId = db.Profile.Where(x => x.UserId == userId).Select(x => x.ProfileId).FirstOrDefault();

            if (db.Participant.Any(x => x.ProfileId == profileId && x.MeetingId == meetingId))
                return BadRequest("User is already on that meeting!");

            db.Participant.Add(new Participant
            {
                ProfileId = profileId,
                MeetingId = meetingId
            });

            db.SaveChanges();

            return Ok("Successfully joined meeting");
        }

        [JwtAuthentication]
        public IHttpActionResult DeleteParticipant(HttpRequestMessage message)
        {
            int meetingId = Convert.ToInt32(message.Headers.GetValues("MeetingId").FirstOrDefault());
            int userId = JwtManager.getUserId(Request.Headers.Authorization.Parameter);
            int id = db.Participant.Where(x => x.Profile.UserId == userId).Select(x => x.ProfileId).FirstOrDefault();

            Participant participant = db.Participant.Find(id, meetingId);
            if (participant == null)
            {
                return NotFound();
            }

            db.Participant.Remove(participant);
            db.SaveChanges();

            return Ok("Successfuly deleted!");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ParticipantExists(int id)
        {
            return db.Participant.Count(e => e.ProfileId == id) > 0;
        }
    }
}