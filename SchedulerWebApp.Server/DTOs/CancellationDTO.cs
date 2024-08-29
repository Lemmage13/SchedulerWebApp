using SchedulerWebApp.Server.Models;
namespace SchedulerWebApp.Server.DTOs
{
    public class CancellationDTO
    {
        public int RegEventId { get; set; }
        public DateDTO Date { get; set; }
    }
}
