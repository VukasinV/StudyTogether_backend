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
    public class ParticipantController : ApiController
    {
        private StudyTogetherEntities db = new StudyTogetherEntities();

        // GET: api/Participant
        public IHttpActionResult GetParticipant(HttpRequestMessage message)
        {
            int meetingId = Convert.ToInt32(message.Headers.GetValues("meetingId").FirstOrDefault());

            var participants = db.Participant.Where(x => x.MeetingId == meetingId).Select(x => new
            {
                x.Profile.User.Fullname
            });

            return Ok(participants);
        }

        //// GET: api/Participant/5
        //[ResponseType(typeof(Participant))]
        //public IHttpActionResult GetParticipant(int id)
        //{
        //    Participant participant = db.Participant.Find(id);
        //    if (participant == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(participant);
        //}

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
        [ResponseType(typeof(Participant))]
        public IHttpActionResult PostParticipant([FromBody]ParticipantDTO participantDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int userId = db.User.Where(x => x.Username == participantDTO.Username).Select(x => x.UserId).Single();

            db.Participant.Add(new Participant
            {
                ProfileId = db.Profile.Where(x => x.UserId == userId).Select(x => x.ProfileId).FirstOrDefault(),
                MeetingId = participantDTO.MeetingId
            });

            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }

            return Ok("Successfully joined meeting");
        }

        // DELETE: api/Participant/5
        [ResponseType(typeof(Participant))]
        public IHttpActionResult DeleteParticipant(int id)
        {
            Participant participant = db.Participant.Find(id);
            if (participant == null)
            {
                return NotFound();
            }

            db.Participant.Remove(participant);
            db.SaveChanges();

            return Ok(participant);
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