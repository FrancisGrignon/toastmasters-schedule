using Meetings.Infrastructure.Persistence.EntityConfiguration;
using Meetings.Models;
using Microsoft.EntityFrameworkCore;

namespace Meetings.Infrastructure
{
    public class MeetingContext : DbContext
    {
        public MeetingContext()
        {
            // Empty
        }

        public MeetingContext(DbContextOptions<MeetingContext> options) : base(options)
        {
            // Empty
        }

        public DbSet<Attendee> Attendees { get; set; }

        public DbSet<Meeting> Meetings { get; set; }

        public DbSet<Role> Roles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if (!optionsBuilder.IsConfigured)
        //    {
                var connection = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Meetings;Integrated Security=True";

                optionsBuilder.UseSqlServer(connection);
        //    }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new RoleEntityTypeConfiguration());
            builder.ApplyConfiguration(new MeetingEntityTypeConfiguration());
            builder.ApplyConfiguration(new AttendeeEntityTypeConfiguration());
        }
    }
}