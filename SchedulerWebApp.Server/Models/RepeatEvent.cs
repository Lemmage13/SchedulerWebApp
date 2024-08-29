namespace SchedulerWebApp.Server.Models
{
    public class RepeatEvent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TimeOnly Time { get; set; }
        public int DayOfWeek { get; set; } // 0 = Sun; 1 = Mon, etc....
    }
}
