﻿// <auto-generated />
using HelpDeskCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace HelpDeskCore.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.3-rtm-10026")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("HelpDeskCore.Data.Entities.AppSetting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.ToTable("AppSettings");
                });

            modelBuilder.Entity("HelpDeskCore.Data.Entities.AppUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<bool>("Disabled");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<long?>("FacebookId");

                    b.Property<string>("FirstName")
                        .HasMaxLength(255);

                    b.Property<string>("Greeting")
                        .HasMaxLength(500);

                    b.Property<string>("HostName")
                        .HasMaxLength(255);

                    b.Property<string>("IPAddress")
                        .HasMaxLength(50);

                    b.Property<bool>("IsAdministrator");

                    b.Property<bool>("IsManager");

                    b.Property<bool>("IsTech");

                    b.Property<string>("LastName")
                        .HasMaxLength(255);

                    b.Property<DateTime?>("LastSeen");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("Notes")
                        .HasMaxLength(4000);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("PictureUrl")
                        .HasMaxLength(255);

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("SendEmail");

                    b.Property<bool>("SendNewTicketTechEmail");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("HelpDeskCore.Data.Entities.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("ForSpecificUsers");

                    b.Property<bool>("ForTechsOnly");

                    b.Property<string>("FromAddress")
                        .HasMaxLength(255);

                    b.Property<bool>("FromAddressInReplyTo");

                    b.Property<string>("FromName")
                        .HasMaxLength(255);

                    b.Property<bool>("KbOnly");

                    b.Property<string>("Name")
                        .HasMaxLength(250);

                    b.Property<string>("Notes");

                    b.Property<int>("OrderByNumber");

                    b.Property<int?>("SectionId");

                    b.HasKey("Id");

                    b.HasIndex("SectionId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("HelpDeskCore.Data.Entities.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Body");

                    b.Property<DateTime>("CommentDate");

                    b.Property<string>("EmailHeaders");

                    b.Property<bool>("ForTechsOnly");

                    b.Property<bool>("IsSystem");

                    b.Property<int>("IssueId");

                    b.Property<string>("Recipients");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("IssueId");

                    b.HasIndex("UserId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("HelpDeskCore.Data.Entities.Company", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .HasMaxLength(255);

                    b.Property<string>("Notes")
                        .HasMaxLength(2000);

                    b.HasKey("Id");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("HelpDeskCore.Data.Entities.Department", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("HelpDeskCore.Data.Entities.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CompanyId");

                    b.Property<int?>("DepartmentId");

                    b.Property<string>("Gender")
                        .HasMaxLength(1);

                    b.Property<string>("Locale")
                        .HasMaxLength(10);

                    b.Property<string>("Location")
                        .HasMaxLength(1000);

                    b.Property<string>("PhoneNumberExtension")
                        .HasMaxLength(50);

                    b.Property<string>("Signature")
                        .HasMaxLength(2000);

                    b.Property<string>("Title")
                        .HasMaxLength(128);

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("UserId");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("HelpDeskCore.Data.Entities.FileAttachment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CommentId");

                    b.Property<string>("DropboxUrl")
                        .HasMaxLength(500);

                    b.Property<byte[]>("FileData");

                    b.Property<byte[]>("FileHash");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int>("FileSize");

                    b.Property<string>("GoogleDriveUrl")
                        .HasMaxLength(255);

                    b.Property<bool>("HiddenFromKB");

                    b.Property<bool>("HiddenFromTickets");

                    b.Property<int>("IssueId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("CommentId");

                    b.HasIndex("IssueId");

                    b.HasIndex("UserId");

                    b.ToTable("FileAttachments");
                });

            modelBuilder.Entity("HelpDeskCore.Data.Entities.FileDuplicate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CommentId");

                    b.Property<int>("FileId");

                    b.HasKey("Id");

                    b.ToTable("FileDuplicates");
                });

            modelBuilder.Entity("HelpDeskCore.Data.Entities.Issue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AssignedToUserId")
                        .HasMaxLength(128);

                    b.Property<string>("Body");

                    b.Property<int>("CategoryId");

                    b.Property<DateTime?>("DueDate");

                    b.Property<DateTime>("IssueDate");

                    b.Property<DateTime>("LastUpdated");

                    b.Property<short>("Priority");

                    b.Property<DateTime?>("ResolvedDate");

                    b.Property<DateTime?>("StartDate");

                    b.Property<int>("StatusId");

                    b.Property<string>("Subject")
                        .HasMaxLength(1000);

                    b.Property<int>("TimeSpentInSeconds");

                    b.Property<bool>("UpdatedByPerformer");

                    b.Property<bool>("UpdatedByUser");

                    b.Property<bool>("UpdatedForTechView");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("StatusId");

                    b.HasIndex("UserId");

                    b.ToTable("Issues");
                });

            modelBuilder.Entity("HelpDeskCore.Data.Entities.IssueSubscriber", b =>
                {
                    b.Property<int>("IssueId");

                    b.Property<string>("UserId")
                        .HasMaxLength(128);

                    b.HasKey("IssueId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("Subscribers");
                });

            modelBuilder.Entity("HelpDeskCore.Data.Entities.Section", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250);

                    b.Property<int>("OrderByNumber");

                    b.HasKey("Id");

                    b.ToTable("Sections");
                });

            modelBuilder.Entity("HelpDeskCore.Data.Entities.Status", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ButtonCaption")
                        .HasMaxLength(50);

                    b.Property<bool>("ForTechsOnly");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<bool>("StopTimeSpent");

                    b.HasKey("Id");

                    b.ToTable("Statuses");
                });

            modelBuilder.Entity("HelpDeskCore.Data.Entities.SysEventLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Date");

                    b.Property<string>("Description")
                        .HasMaxLength(500);

                    b.Property<string>("EventType")
                        .HasMaxLength(50);

                    b.Property<string>("MessageId")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<string>("ObjectState");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("SysEventLog");
                });

            modelBuilder.Entity("HelpDeskCore.Data.Entities.UserAvatar", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("ImageData");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserAvatars");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("HelpDeskCore.Data.Entities.Category", b =>
                {
                    b.HasOne("HelpDeskCore.Data.Entities.Section", "Section")
                        .WithMany("Categories")
                        .HasForeignKey("SectionId");
                });

            modelBuilder.Entity("HelpDeskCore.Data.Entities.Comment", b =>
                {
                    b.HasOne("HelpDeskCore.Data.Entities.Issue", "Issue")
                        .WithMany("Comments")
                        .HasForeignKey("IssueId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("HelpDeskCore.Data.Entities.AppUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("HelpDeskCore.Data.Entities.Employee", b =>
                {
                    b.HasOne("HelpDeskCore.Data.Entities.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");

                    b.HasOne("HelpDeskCore.Data.Entities.Department", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentId");

                    b.HasOne("HelpDeskCore.Data.Entities.AppUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("HelpDeskCore.Data.Entities.FileAttachment", b =>
                {
                    b.HasOne("HelpDeskCore.Data.Entities.Comment", "Comment")
                        .WithMany()
                        .HasForeignKey("CommentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("HelpDeskCore.Data.Entities.Issue", "Issue")
                        .WithMany()
                        .HasForeignKey("IssueId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("HelpDeskCore.Data.Entities.AppUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("HelpDeskCore.Data.Entities.Issue", b =>
                {
                    b.HasOne("HelpDeskCore.Data.Entities.Category", "Category")
                        .WithMany("Issues")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("HelpDeskCore.Data.Entities.Status", "Status")
                        .WithMany("Issues")
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("HelpDeskCore.Data.Entities.AppUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("HelpDeskCore.Data.Entities.IssueSubscriber", b =>
                {
                    b.HasOne("HelpDeskCore.Data.Entities.Issue", "Issue")
                        .WithMany("IssueSubscribers")
                        .HasForeignKey("IssueId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("HelpDeskCore.Data.Entities.AppUser", "User")
                        .WithMany("IssueSubscribers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HelpDeskCore.Data.Entities.SysEventLog", b =>
                {
                    b.HasOne("HelpDeskCore.Data.Entities.AppUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HelpDeskCore.Data.Entities.UserAvatar", b =>
                {
                    b.HasOne("HelpDeskCore.Data.Entities.AppUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("HelpDeskCore.Data.Entities.AppUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("HelpDeskCore.Data.Entities.AppUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("HelpDeskCore.Data.Entities.AppUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("HelpDeskCore.Data.Entities.AppUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
