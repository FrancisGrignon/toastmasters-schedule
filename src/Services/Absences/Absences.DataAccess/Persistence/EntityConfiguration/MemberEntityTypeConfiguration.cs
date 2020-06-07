using Absences.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Absences.DataAccess.Persistence.EntityConfiguration
{
    class MemberEntityTypeConfiguration
       : IEntityTypeConfiguration<Absence>
    {
        public void Configure(EntityTypeBuilder<Absence> builder)
        {
            builder.ToTable("Absences");

            builder.Property<int>(ci => ci.Id)
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

             builder.Property<string>(ci => ci.Member)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property<int>(ci => ci.MemberId)
                .IsRequired();

            builder.Property<DateTime>(ci => ci.StartAt)
                .IsRequired();

            builder.Property<DateTime>(ci => ci.EndAt)
                .IsRequired();

            builder.Property<DateTime>(ci => ci.CreatedAt)
                .IsRequired();

            builder.Property<DateTime>(ci => ci.UpdatedAt)
                .IsRequired();

            builder.Property<bool>(ci => ci.Deleted)
                 .IsRequired()
                 .HasDefaultValue(false);

            builder.HasKey(ci => ci.Id);
        }
    }
}
