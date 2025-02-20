﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using itu_minitwit.Data;

#nullable disable

namespace itu_minitwit.Migrations
{
    [DbContext(typeof(MiniTwitDbContext))]
    [Migration("20250215084305_AddedLatestProcessedSimAction")]
    partial class AddedLatestProcessedSimAction
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Sqlite:Autoincrement", true);

            modelBuilder.Entity("itu_minitwit.Data.Follower", b =>
                {
                    b.Property<int>("WhoId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("who_id");

                    b.Property<int>("WhomId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("whom_id");

                    b.HasKey("WhoId", "WhomId");

                    b.ToTable("follower", (string)null);
                });

            modelBuilder.Entity("itu_minitwit.Data.LatestProcessedSimAction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("LatestProcessedSimActions");
                });

            modelBuilder.Entity("itu_minitwit.Data.Message", b =>
                {
                    b.Property<int>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("message_id");

                    b.Property<int>("AuthorId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("author_id");

                    b.Property<int?>("Flagged")
                        .HasColumnType("INTEGER")
                        .HasColumnName("flagged");

                    b.Property<int?>("PubDate")
                        .HasColumnType("INTEGER")
                        .HasColumnName("pub_date");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("string")
                        .HasColumnName("text");

                    b.HasKey("MessageId");

                    b.ToTable("message", (string)null);
                });

            modelBuilder.Entity("itu_minitwit.Data.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("user_id");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("string")
                        .HasColumnName("email");

                    b.Property<string>("PwHash")
                        .IsRequired()
                        .HasColumnType("string")
                        .HasColumnName("pw_hash");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("string")
                        .HasColumnName("username");

                    b.HasKey("UserId");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("user", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
