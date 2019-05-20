using Members.DataAccess.Persistence.EntityConfiguration;
using Members.Models;
using Microsoft.EntityFrameworkCore;

namespace Members.DataAccess
{
    public class MemberContext : DbContext
    {
        public MemberContext()
        {
            // Empty
        }

        public MemberContext(DbContextOptions<MemberContext> options) : base(options)
        {
            // Empty
        }

        public DbSet<Member> Members { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if (!optionsBuilder.IsConfigured)
            {
                var connection = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Members;Integrated Security=True";

                optionsBuilder.UseSqlServer(connection);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new MemberEntityTypeConfiguration());
        }
    }
}