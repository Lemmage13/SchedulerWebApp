using SchedulerWebApp.Server.Models;
namespace SchedulerWebApp.Server.Models.DTOs
{
    public class CancellationDTO
    {
        public int RegEventId { get; set; }
        public DateDTO Date { get; set; }
    }
}
