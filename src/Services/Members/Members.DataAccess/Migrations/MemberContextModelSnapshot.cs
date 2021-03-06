﻿// <auto-generated />
using System;
using Members.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Members.DataAccess.Migrations
{
    [DbContext(typeof(MemberContext))]
    partial class MemberContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Members.Models.Absence", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Deleted")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(false);

                    b.Property<DateTime>("EndAt");

                    b.Property<int>("MemberId");

                    b.Property<DateTime>("StartAt");

                    b.HasKey("Id");

                    b.HasIndex("MemberId");

                    b.ToTable("Absences");
                });

            modelBuilder.Entity("Members.Models.Member", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(true);

                    b.Property<string>("Alias")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<bool>("Deleted")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(false);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("Email2")
                        .HasMaxLength(255);

                    b.Property<string>("Email3")
                        .HasMaxLength(255);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("Note")
                        .HasMaxLength(2048);

                    b.Property<bool>("Notify");

                    b.Property<bool>("Notify2");

                    b.Property<bool>("Notify3");

                    b.Property<string>("Rank");

                    b.Property<int>("ToastmastersId");

                    b.HasKey("Id");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("Members.Models.Absence", b =>
                {
                    b.HasOne("Members.Models.Member", "Member")
                        .WithMany("Absences")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
