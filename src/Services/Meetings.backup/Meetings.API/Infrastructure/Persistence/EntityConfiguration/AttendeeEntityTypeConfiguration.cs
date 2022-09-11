using Meetings.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Meetings.Infrastructure.Persistence.EntityConfiguration
{
    class AttendeeEntityTypeConfiguration
       : IEntityTypeConfiguration<Attendee>
    {
        public void Configure(EntityTypeBuilder<Attendee> builder)
        {
            builder.ToTable("Attendees");

            builder.Property<int>(ci => ci.Id)
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            builder.Property<int>(ci => ci.MeetingId)
                .IsRequired();

            builder.Property<int?>(ci => ci.MemberId)
                .IsRequired(false);

            builder.Property<int>(ci => ci.Order)
                .IsRequired()
                .HasDefaultValue(100);

            builder.Property<int>(ci => ci.RoleId)
                .IsRequired();

            builder.Property<bool>(ci => ci.Active)
                .IsRequired()
                .HasDefaultValue(true);

            builder
                .HasOne(ci => ci.Role)
                .WithMany(ci => ci.Attendees)
                .HasForeignKey(ci => ci.RoleId);

            builder.HasKey(ci => ci.Id);
        }
    }
}
