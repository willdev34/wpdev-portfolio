using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Data;

public class PortfolioDbContext : DbContext
{
    public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options)
        : base(options)
    {
    }

    // DbSets - Tabelas do banco
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    public DbSet<TimelineEvent> TimelineEvents => Set<TimelineEvent>();
    public DbSet<GalleryImage> GalleryImages => Set<GalleryImage>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<NowSection> NowSections => Set<NowSection>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurações das entidades
        ConfigureProject(modelBuilder);
        ConfigureBlogPost(modelBuilder);
        ConfigureTimelineEvent(modelBuilder);
        ConfigureGalleryImage(modelBuilder);
        ConfigureContactMessage(modelBuilder);
        ConfigureNowSection(modelBuilder);
    }

    private void ConfigureProject(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.ShortDescription).HasMaxLength(500);
            entity.Property(e => e.ImageUrl).IsRequired().HasMaxLength(500);
            entity.Property(e => e.DemoUrl).HasMaxLength(500);
            entity.Property(e => e.RepositoryUrl).HasMaxLength(500);
            entity.Property(e => e.FlavorText).HasMaxLength(300);
            
            // Converter lista para JSON
            entity.Property(e => e.Technologies)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                );

            entity.HasIndex(e => e.IsFeatured);
            entity.HasIndex(e => e.Year);
        });
    }

    private void ConfigureBlogPost(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BlogPost>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Excerpt).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.FeaturedImageUrl).HasMaxLength(500);
            
            // Converter lista para JSON
            entity.Property(e => e.Tags)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                );

            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasIndex(e => e.IsPublished);
            entity.HasIndex(e => e.PublishedAt);
        });
    }

    private void ConfigureTimelineEvent(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TimelineEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.IconUrl).HasMaxLength(500);
            entity.Property(e => e.LinkUrl).HasMaxLength(500);
            entity.Property(e => e.LinkText).HasMaxLength(100);

            entity.HasIndex(e => e.Date);
            entity.HasIndex(e => e.Order);
        });
    }

    private void ConfigureGalleryImage(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GalleryImage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ImageUrl).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ThumbnailUrl).HasMaxLength(500);
            entity.Property(e => e.AltText).IsRequired().HasMaxLength(200);
            
            // Converter lista para JSON
            entity.Property(e => e.Tags)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                );

            entity.HasIndex(e => e.Order);
        });
    }

    private void ConfigureContactMessage(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ContactMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Subject).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            entity.Property(e => e.ResponseMessage).HasMaxLength(2000);

            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
        });
    }

    private void ConfigureNowSection(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NowSection>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.CurrentProject).HasMaxLength(200);
            entity.Property(e => e.CurrentProjectUrl).HasMaxLength(500);
            
            // Converter listas para JSON
            entity.Property(e => e.CurrentlyLearning)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                );
            
            entity.Property(e => e.CurrentGoals)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                );

            entity.HasIndex(e => e.IsActive);
        });
    }
}