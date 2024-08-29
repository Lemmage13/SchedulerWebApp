using SchedulerWebApp.Server.Models;

namespace SchedulerWebApp.Server.Data
{
    public static class TestDataSeeder
    {
        public static void SeedRegEvents(this PlannerContext context)
        {
            if (!context.RepeatEvents.Any())
            {
                List<RepeatEvent> regEvents = [new RepeatEvent { DayOfWeek = 1, Name = "D&D", Time = new TimeOnly(19, 0)}];
                context.RepeatEvents.AddRange(regEvents);
                context.SaveChanges();
            }
            if (!context.Cancellations.Any())
            {
                List<Cancellation> cancellations = [new Cancellation { Date = new DateOnly(2024, 8, 26), RegEventId = context.RepeatEvents.Single(e => e.DayOfWeek == 1).Id }];
                context.Cancellations.AddRange(cancellations);
                context.SaveChanges();
            }
            if (!context.NonRegularEvents.Any()) 
            {
                List<NonRegularEvent> nrEvents = [new NonRegularEvent { Date = new DateOnly(2024, 8, 30), Name = "D&D", Time = new TimeOnly(19, 0), RegEventId = context.RepeatEvents.Single(e => e.Name == "D&D").Id }];
                context.NonRegularEvents.AddRange(nrEvents);
                context.SaveChanges();
            }
            if (!context.NonRegularEventReplies.Any())
            {
                List<NonRegularEventReply> nrEReplies = new List<NonRegularEventReply>();
                foreach (NonRegularEvent nrE in context.NonRegularEvents.OrderBy(e => e.Id)){
                    nrEReplies.Add(new NonRegularEventReply { NonRegularEventId = nrE.Id });
                }
                context.AddRange(nrEReplies);
                context.SaveChanges();
            }
        }
    }
}
