using Members.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Members.DataAccess.Persistence.EntityConfiguration
{
    class RoleEntityTypeConfiguration
       : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Role");

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

            builder.Property<string>(ci => ci.Note);

            builder.HasKey(ci => ci.Id);
        }
    }
}
