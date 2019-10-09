using Absences.Models;
using Members.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Members.DataAccess.Persistence.EntityConfiguration
{
    class AbsenceEntityTypeConfiguration
       : IEntityTypeConfiguration<Absence>
    {
        public void Configure(EntityTypeBuilder<Absence> builder)
        {
            builder.ToTable("Absences");

            builder.Property<int>(ci => ci.Id)
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            builder.Property<bool>(ci => ci.Deleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property<DateTime>(ci => ci.StartAt)
                .IsRequired();

            builder.Property<DateTime>(ci => ci.EndAt)
                .IsRequired();

            builder.HasKey(ci => ci.Id);

            builder.HasOne(ci => ci.Member).WithMany(ci => ci.Absences);
        }
    }
}
