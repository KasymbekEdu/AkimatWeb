using AkimatWeb.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AkimatWeb.Infrastructure;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<News> News => Set<News>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<Application> Applications => Set<Application>();
    public DbSet<ServiceCategory> ServiceCategories => Set<ServiceCategory>();
    public DbSet<ServiceItem> ServiceItems => Set<ServiceItem>();
    public DbSet<NewsImage> NewsImages { get; set; }
    public DbSet<MapObject> MapObjects => Set<MapObject>();
    public DbSet<Poll> Polls => Set<Poll>();
    public DbSet<PollOption> PollOptions => Set<PollOption>();
    public DbSet<PollVote> PollVotes => Set<PollVote>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Application>()
            .HasIndex(a => a.TrackingNumber)
            .IsUnique();

        builder.Entity<Application>()
            .Property(a => a.Status)
            .HasConversion<string>();

        builder.Entity<Announcement>()
            .Property(a => a.Type)
            .HasConversion<string>();

        builder.Entity<ServiceItem>()
            .HasOne(s => s.Category)
            .WithMany(c => c.Services)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<NewsImage>()
            .HasOne(x => x.News)
            .WithMany(x => x.Images)
            .HasForeignKey(x => x.NewsId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<MapObject>()
    .Property(m => m.Type).HasConversion<string>();
        builder.Entity<MapObject>()
            .Property(m => m.Status).HasConversion<string>();

        builder.Entity<Poll>()
            .Property(p => p.Type).HasConversion<string>();

        builder.Entity<PollOption>()
            .HasOne(o => o.Poll)
            .WithMany(p => p.Options)
            .HasForeignKey(o => o.PollId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PollVote>()
            .HasOne(v => v.Poll)
            .WithMany(p => p.Votes)
            .HasForeignKey(v => v.PollId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PollVote>()
            .HasOne(v => v.Option)
            .WithMany(o => o.Votes)
            .HasForeignKey(v => v.OptionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
