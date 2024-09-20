using Microsoft.AspNetCore.Mvc;
using SchedulerWebApp.Server.Models;
using System.ComponentModel.DataAnnotations;
using SchedulerWebApp.Server.Data;
using SchedulerWebApp.Server.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SchedulerWebApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EventsController : ControllerBase
    {
        private PlannerContext _plannerContext;
        private CancellationOrganiser _cancellationOrganiser;
        public EventsController(PlannerContext plannerContext)
        {
            _plannerContext = plannerContext;
            _cancellationOrganiser = new CancellationOrganiser(plannerContext);
        }
        #region RegEvents
        [HttpGet]
        [Route("RegEvents")]
        [ProducesResponseType(200, Type = typeof(RepeatEvent[]))]
        public IActionResult GetRepeatEvents()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(_plannerContext.RepeatEvents.OrderBy(e => e.Id));
        }
        [HttpPost]
        [Route("RegEvents")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> AddRegEvent([FromBody] RegEventDTO newEvent)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            Debug.WriteLine(newEvent.Name);
            Debug.WriteLine(newEvent.Weekday);
            Debug.WriteLine(newEvent.Time);

            RepeatEvent rE = new RepeatEvent() { Name = newEvent.Name, Time = newEvent.Time, DayOfWeek = newEvent.Weekday };
            await _plannerContext.RepeatEvents.AddAsync(rE);
            await _plannerContext.SaveChangesAsync();

            return CreatedAtAction(nameof(rE), rE);
        }
        [HttpDelete]
        [Route("RegEvents/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRegEvent(int id)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            RepeatEvent? rE = await _plannerContext.RepeatEvents.FirstOrDefaultAsync(e => e.Id == id);
            if (rE == null) { return NotFound(); }

            _plannerContext.RepeatEvents.Remove(rE);
            _plannerContext.SaveChangesAsync();

            return Ok();
        }
        #endregion RegEvents
        #region NREvents
        [HttpGet]
        [Route("NREvents")]
        [ProducesResponseType(200, Type=typeof(NonRegularEvent[]))]
        public IActionResult GetNREvents()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(_plannerContext.NonRegularEvents.OrderBy(e => e.Id));
        }

        [HttpGet]
        [Route("NREvents/date/{date}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetNREventsByDate(string date) // YYYY-MM-DD
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            DateOnly formattedDate = DateOnly.Parse(date);

            return Ok(_plannerContext.Cancellations.Where(e => e.Date == formattedDate));
        }
        #region NREventReplies
        [HttpGet]
        [Route("NREvents/Replies")]
        [ProducesResponseType<NonRegularEventReply>(StatusCodes.Status200OK)]
        public IActionResult GetNREventReplies()
        {
            return Ok(_plannerContext.NonRegularEventReplies.OrderBy(e => e.Id));
        }

        [HttpPut]
        [Route("NREvents/Replies/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateNREventReply(int id, NonRegularEventReply nreReply)
        {
            if(!ModelState.IsValid) { return BadRequest(ModelState); }
            if (id != nreReply.Id) { return BadRequest("Mismatched Ids in URI and body"); }

            var replyToUpdate = _plannerContext.NonRegularEventReplies.FirstOrDefault(e => e.Id == id);
            if (replyToUpdate != null) {
                replyToUpdate.Accepted = nreReply.Accepted;
                _plannerContext.SaveChanges();
                return NoContent();
            }
            return NotFound();
        }

        #endregion NREventReplies

        #endregion NREvents
        #region Cancellations
        [HttpGet]
        [Route("Cancellations")]
        [ProducesResponseType<Cancellation[]>(StatusCodes.Status200OK)]
        public IActionResult GetCancellations()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }
            return Ok(_plannerContext.Cancellations.OrderBy(c => c.Id));
        }
        [HttpGet]
        [Route("Cancellations/{id}")]
        [ProducesResponseType<Cancellation>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetCancellation(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(_plannerContext.Cancellations.Where(c => c.Id == id));
        }
        [HttpGet]
        [Route("Cancellations/RegEvent/{regEventId}")]
        [ProducesResponseType<Cancellation>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetCancellationForEvent(int regEventId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Cancellation? cancellation = _plannerContext.Cancellations.FirstOrDefault(c => c.RegEventId == regEventId);
            if(cancellation != null)
            {
                return Ok(cancellation);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpDelete]
        [Route("Cancellations/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteCancellation(int id) 
        {
            Cancellation? cancellation = _plannerContext.Cancellations.SingleOrDefault(c => c.Id == id);
            if (cancellation != null)
            {
                _plannerContext.Cancellations.Remove(cancellation);
                _plannerContext.SaveChanges();
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("Cancellations")]
        [ProducesResponseType<Cancellation>(StatusCodes.Status201Created)]
        public IActionResult AddCancellation(CancellationDTO cDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //creates and adds cancellation to DB from DTO
            Cancellation c = new Cancellation { Date = cDTO.Date.GetDate(), RegEventId = cDTO.RegEventId };
            _plannerContext.Cancellations.Add(c);
            _plannerContext.SaveChanges();

            _cancellationOrganiser.HandleCancellation(c);

            //handle addition of NREvent reschedules
            return CreatedAtAction(nameof(GetCancellation), new { Id = c.Id }, c);
        }
        #endregion Cancellations
    }
}
