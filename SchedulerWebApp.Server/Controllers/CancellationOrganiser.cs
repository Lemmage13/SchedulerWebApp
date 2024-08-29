using SchedulerWebApp.Server.Data;
using SchedulerWebApp.Server.Models;

namespace SchedulerWebApp.Server.Controllers
{
    public class CancellationOrganiser
    {
        private PlannerContext _plannerContext;
        DateOnly currentDay = DateOnly.FromDateTime(DateTime.Now);
        public CancellationOrganiser(PlannerContext pC)
        {
            _plannerContext = pC;
        }
        public void HandleCancellation(Cancellation cancellation)
        {
            RepeatEvent repeatEvent = _plannerContext.RepeatEvents.FirstOrDefault(e => e.Id == cancellation.RegEventId);
            if (repeatEvent == null)
            {
                throw new ArgumentNullException(nameof(repeatEvent));
            }
            //Add non regular events to db based on availability, current day etc
            DateOnly rEDate = cancellation.Date;
            List<DateOnly> dates = DaysForNREvents(rEDate);
            List<NonRegularEvent> newEvents = new List<NonRegularEvent>();
            foreach (DateOnly date in dates)
            {
                newEvents.Add(new NonRegularEvent { CancellationId = cancellation.Id, Date = date, Name = repeatEvent.Name, RegEventId = repeatEvent.Id, Time = repeatEvent.Time });
            }

            _plannerContext.NonRegularEvents.AddRange(newEvents);
            _plannerContext.SaveChanges();

            //Add reply objects corresponding to these events
            List<NonRegularEventReply> replies = new List<NonRegularEventReply>();
            foreach (NonRegularEvent e in newEvents)
            {
                replies.Add(new NonRegularEventReply { Accepted = null, NonRegularEventId = e.Id });
            }

            _plannerContext.NonRegularEventReplies.AddRange(replies);
            _plannerContext.SaveChanges();
        }
        private List<DateOnly> DaysForNREvents(DateOnly rEDate, int daysbefore = 2, int daysafter = 2)
        {
            List<DateOnly> dates = new List<DateOnly>();
            DateOnly day = rEDate.AddDays(-1);
            while (daysbefore > 0 && day > currentDay)
            {
                dates.Add(day);
                daysbefore -= 1;
                day = day.AddDays(-1);
            }
            daysafter += daysbefore;
            day = rEDate.AddDays(1);
            while (daysafter > 0)
            {
                dates.Add(day);
                daysafter -= 1;
                day = day.AddDays(1);
            }
            return dates;
        }
    }
}
