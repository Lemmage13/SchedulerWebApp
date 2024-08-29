using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SchedulerWebApp.Server.Models
{
    public class NonRegularEvent
    {
        public int Id { get; set; }
        public int? RegEventId { get; set; }
        public string Name { get; set; }
        public TimeOnly Time { get; set; }
        public DateOnly Date { get; set; }
        public int? CancellationId { get; set; }
    }
}