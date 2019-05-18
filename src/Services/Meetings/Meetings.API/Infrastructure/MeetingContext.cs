using Meetings.Infrastructure.Persistence.EntityConfiguration;
using Meetings.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Meetings.Infrastructure
{
    public class MeetingContext : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;

        public MeetingContext()
        {
            // Empty
        }

        public MeetingContext(DbContextOptions<MeetingContext> options, ILoggerFactory loggerFactory) : base(options)
        {
            _loggerFactory = loggerFactory;
        }

        //public MeetingContext(ILoggerFactory loggerFactory)
        //{
        //    _loggerFactory = loggerFactory;
        //}

        public DbSet<Attendee> Attendees { get; set; }

        public DbSet<Meeting> Meetings { get; set; }

        public DbSet<Role> Roles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if (!optionsBuilder.IsConfigured)
        //    {
                var connection = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Meetings;Integrated Security=True";

            optionsBuilder.UseSqlServer(connection);

            optionsBuilder.UseLoggerFactory(_loggerFactory);

            //if (null != _loggerFactory)
            //{
            //    optionsBuilder.UseLoggerFactory(_loggerFactory);
            //}
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