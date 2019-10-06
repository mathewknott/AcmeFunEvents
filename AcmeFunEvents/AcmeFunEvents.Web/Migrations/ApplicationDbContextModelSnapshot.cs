﻿// <auto-generated />
using System;
using AcmeFunEvents.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AcmeFunEvents.Web.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AcmeFunEvents.Web.DTO.Activity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<int>("Code")
                        .HasColumnName("code");

                    b.Property<DateTime>("CreatedUtc")
                        .HasColumnName("created_utc");

                    b.Property<DateTime?>("Date")
                        .HasColumnName("date");

                    b.Property<DateTime>("ModifiedUtc")
                        .HasColumnName("modified_utc");

                    b.Property<string>("Name")
                        .HasColumnName("name");

                    b.Property<int>("Status")
                        .HasColumnName("status");

                    b.HasKey("Id")
                        .HasName("pk_acme_activity");

                    b.ToTable("acme_activity");
                });

            modelBuilder.Entity("AcmeFunEvents.Web.DTO.Registration", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<Guid?>("ActivityId")
                        .HasColumnName("activity_id");

                    b.Property<string>("Comments")
                        .HasColumnName("comments");

                    b.Property<DateTime>("CreatedUtc")
                        .HasColumnName("created_utc");

                    b.Property<DateTime>("ModifiedUtc")
                        .HasColumnName("modified_utc");

                    b.Property<int>("RegistrationNumber")
                        .HasColumnName("registration_number");

                    b.Property<int>("Status")
                        .HasColumnName("status");

                    b.Property<Guid?>("UserId")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_acme_registration");

                    b.HasIndex("ActivityId")
                        .HasName("ix_acme_registration_activity_id");

                    b.HasIndex("UserId")
                        .HasName("ix_acme_registration_user_id");

                    b.ToTable("acme_registration");
                });

            modelBuilder.Entity("AcmeFunEvents.Web.DTO.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedUtc")
                        .HasColumnName("created_utc");

                    b.Property<string>("EmailAddress")
                        .HasColumnName("email_address");

                    b.Property<string>("FirstName")
                        .HasColumnName("first_name");

                    b.Property<string>("LastName")
                        .HasColumnName("last_name");

                    b.Property<DateTime>("ModifiedUtc")
                        .HasColumnName("modified_utc");

                    b.Property<string>("PhoneNumber")
                        .HasColumnName("phone_number");

                    b.Property<int>("Status")
                        .HasColumnName("status");

                    b.HasKey("Id")
                        .HasName("pk_acme_user");

                    b.HasIndex("EmailAddress")
                        .IsUnique()
                        .HasName("ix_acme_user_email_address")
                        .HasFilter("[email_address] IS NOT NULL");

                    b.ToTable("acme_user");
                });

            modelBuilder.Entity("AcmeFunEvents.Web.DTO.Registration", b =>
                {
                    b.HasOne("AcmeFunEvents.Web.DTO.Activity", "Activity")
                        .WithMany()
                        .HasForeignKey("ActivityId")
                        .HasConstraintName("fk_acme_registration_acme_activity_activity_id");

                    b.HasOne("AcmeFunEvents.Web.DTO.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_acme_registration_acme_user_user_id");
                });
#pragma warning restore 612, 618
        }
    }
}
