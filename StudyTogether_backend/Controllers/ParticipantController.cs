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

        // Send meetingId as parameter and check if participant is on this meeting
        // GET: api/Participant/meetingId
        [JwtAuthentication]
        public IHttpActionResult GetParticipant(int id)
        {
            int userId = JwtManager.getUserId(Request.Headers.Authorization.Parameter);

            int profileId = db.Profile.Where(x => x.UserId == userId)
                                      .Select(x => x.ProfileId)
                                      .FirstOrDefault();

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
        public IHttpActionResult PostParticipant(Participant participant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int userId = JwtManager.getUserId(Request.Headers.Authorization.Parameter);
            int profileId = db.Profile.Where(x => x.UserId == userId)
                                      .Select(x => x.ProfileId)
                                      .FirstOrDefault();

            if (db.Participant.Any(x => x.ProfileId == profileId && x.MeetingId == participant.MeetingId))
                return BadRequest("User is already on that meeting!");

            participant.ProfileId = profileId;

            db.Participant.Add(participant);
            db.SaveChanges();

            return Ok();
        }

        // DELETE: api/Participant/meetingId
        [JwtAuthentication]
        public IHttpActionResult DeleteParticipant(int id)
        {
            int userId = JwtManager.getUserId(Request.Headers.Authorization.Parameter);
            int profileId = db.Profile.Where(x => x.UserId == userId).Select(x => x.ProfileId).Single();

            Participant participant = db.Participant.Find(profileId, id);
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