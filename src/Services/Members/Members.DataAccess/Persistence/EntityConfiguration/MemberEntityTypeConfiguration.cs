using Members.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Members.DataAccess.Persistence.EntityConfiguration
{
    class MemberEntityTypeConfiguration
       : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.ToTable("Members");

            builder.Property<int>(ci => ci.Id)
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            builder.Property<bool>(ci => ci.Active)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property<string>(ci => ci.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property<string>(ci => ci.Note)
                .HasMaxLength(2048);

            builder.Property<string>(ci => ci.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasKey(ci => ci.Id);
        }
    }
}
