namespace SchedulerWebApp.Server.Models
{
    public class NonRegularEventReply
    {
        public int Id { get; set; }
        public int NonRegularEventId { get; set; }
        public bool? Accepted { get; set; }
    }
}
