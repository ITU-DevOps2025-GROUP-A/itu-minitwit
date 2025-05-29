using Api.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.DataAccess;

public partial class MinitwitDbContext : DbContext
{
    public MinitwitDbContext()
    {
    }

    public MinitwitDbContext(DbContextOptions<MinitwitDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Follower> Followers { get; set; }

    public virtual DbSet<LatestProcessedSimAction> LatestProcessedSimActions { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LatestProcessedSimAction>(entity => { entity.HasKey(e => e.Id); });

        modelBuilder.Entity<Follower>(entity =>
        {
            entity.HasKey(e => new { e.WhoId, e.WhomId });

            entity.ToTable("follower");

            entity.Property(e => e.WhoId).HasColumnName("who_id");
            entity.Property(e => e.WhomId).HasColumnName("whom_id");
            
            entity.HasIndex(e => new { e.WhoId, e.WhomId })
                  .IsUnique();
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("message");

            entity
                .HasOne(m => m.Author)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.AuthorId);

            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.Flagged).HasColumnName("flagged");
            entity.Property(e => e.PubDate).HasColumnName("pub_date");
            entity.Property(e => e.Text)
                .HasColumnType("text")
                .HasColumnName("text");
            
            //index for filtering messages from author
            entity.HasIndex(e => new { e.AuthorId, e.Flagged, e.PubDate })
                  .IsDescending(false, false, true);
            //index for get all messages
            entity.HasIndex(e => new { e.Flagged, e.PubDate })
                  .IsDescending(false, true);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("user");

            entity.HasIndex(e => e.Username, "IX_user_username").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Email)
                .HasColumnType("text")
                .HasColumnName("email");
            entity.Property(e => e.PwHash)
                .HasColumnType("text")
                .HasColumnName("pw_hash");
            entity.Property(e => e.Username)
                .HasColumnType("text")
                .HasColumnName("username");
        });
    }
}