﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace itu_minitwit.Data;

public partial class MiniTwitDbContext : DbContext
{
    public MiniTwitDbContext()
    {
    }

    public MiniTwitDbContext(DbContextOptions<MiniTwitDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Follower> Followers { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source=minitwit.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("Sqlite:Autoincrement", true);
        
        modelBuilder.Entity<Follower>(entity =>
        {
            entity.ToTable("follower");

            entity.HasKey(e => new { e.WhoId, e.WhomId });

            entity.Property(e => e.WhoId).HasColumnName("who_id");
            entity.Property(e => e.WhomId).HasColumnName("whom_id");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("message");

            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.Flagged).HasColumnName("flagged");
            entity.Property(e => e.PubDate).HasColumnName("pub_date");
            entity.Property(e => e.Text)
                .HasColumnType("string")
                .HasColumnName("text");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("user");
                entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Email)
                .HasColumnType("string")
                .HasColumnName("email");
            entity.Property(e => e.PwHash)
                .HasColumnType("string")
                .HasColumnName("pw_hash");
            entity.Property(e => e.Username)
                .HasColumnType("string")
                .HasColumnName("username");
                
        });
    }
}
