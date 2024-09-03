namespace SchedulerWebApp.Server.Models.DTOs
{
    public class DateDTO
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public DateOnly GetDate()
        {
            return new DateOnly(Year, Month, Day);
        }
    }
}
