using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchedulerWebApp.Server.Models;
using SchedulerWebApp.Server.Models.DTOs;

namespace SchedulerWebApp.Server.Data
{
    public class PlannerContext : IdentityDbContext
    {
        public PlannerContext(DbContextOptions<PlannerContext> options) : base(options) 
        { 
        }

        public DbSet<RefreshToken>? RefreshTokens { get; set; }


        public DbSet<RepeatEvent>? RepeatEvents { get; set; }
        public DbSet<Cancellation>? Cancellations { get; set; }
        public DbSet<NonRegularEvent>? NonRegularEvents { get; set; }
        public DbSet<NonRegularEventReply>? NonRegularEventReplies { get; set; }
    }
}
