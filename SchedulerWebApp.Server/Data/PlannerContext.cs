using Microsoft.EntityFrameworkCore;
using SchedulerWebApp.Server.Models;

namespace SchedulerWebApp.Server.Data
{
    public class PlannerContext : DbContext
    {
        public PlannerContext(DbContextOptions<PlannerContext> options) : base(options) 
        { 
        }

        public DbSet<RepeatEvent>? RepeatEvents { get; set; }
        public DbSet<Cancellation>? Cancellations { get; set; }
        public DbSet<NonRegularEvent>? NonRegularEvents { get; set; }
        public DbSet<NonRegularEventReply>? NonRegularEventReplies { get; set; }
    }
}
