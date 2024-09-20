using System.ComponentModel.DataAnnotations;
namespace SchedulerWebApp.Server.Models.DTOs
{
    public class RegEventDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public TimeOnly Time { get; set; }
        [Required]
        public int Weekday { get; set; }
    }
}
