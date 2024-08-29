namespace SchedulerWebApp.Server.Models
{
    public class Cancellation
    {
        public int Id { get; set; }
        public int RegEventId { get; set; }
        public DateOnly Date { get; set; }
    }
}
