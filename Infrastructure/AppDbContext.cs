using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class AppDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Status> Statuses => Set<Status>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Feedback> Feedbacks => Set<Feedback>();
    public DbSet<Response> Responses => Set<Response>();
    public DbSet<Attachment> Attachments => Set<Attachment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Status>(entity =>
        {
            entity.ToTable("statuses");
            
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Id).HasColumnName("id");
            
            entity.Property(s => s.Name).HasDefaultValue("Новый").HasColumnName("name");
        });
        
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("categories");
            
            entity.HasKey(c => c.Id);
            entity.Property(с => с.Id).HasColumnName("id");

            entity.Property(c => c.Name).IsRequired().HasMaxLength(50).HasColumnName("name");
            entity.Property(c => c.Description).HasMaxLength(200).HasColumnName("description");
        });
        
        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.ToTable("feedbacks"); 
            
            entity.HasKey(f => f.Id);
            entity.Property(f => f.Id).HasColumnName("id");

            entity.Property(f => f.UserId).IsRequired().HasColumnName("user_id");
            entity.Property(f => f.CategoryId).IsRequired().HasColumnName("category_id");
            entity.Property(f => f.StatusId).IsRequired().HasColumnName("status_id");

            entity.Property(f => f.Message).IsRequired().HasColumnName("message");
            entity.Property(f => f.CreatedAt).HasDefaultValueSql("NOW()").HasColumnName("created_at");
            
            entity.HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Response>(entity =>
        {
            entity.ToTable("responses");
            
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Id).HasColumnName("id");

            entity.Property(r => r.FeedbackId).IsRequired().HasColumnName("feedback_id");
            entity.Property(r => r.UserId).IsRequired().HasColumnName("user_id");  
            
            entity.Property(r => r.Message).IsRequired().HasColumnName("message");
            entity.Property(r => r.CreatedAt).HasDefaultValueSql("NOW()").HasColumnName("created_at");

            entity.HasOne(r => r.Feedback)
                .WithMany(f => f.Responses)
                .HasForeignKey(r => r.FeedbackId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.ToTable("attachments");

            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).HasColumnName("id");

            entity.Property(a => a.FeedbackId).HasColumnName("feedback_id");

            entity.Property(a => a.FilePath).IsRequired().HasMaxLength(255).HasColumnName("file_path");
            entity.Property(a => a.FileType).IsRequired().HasMaxLength(100).HasColumnName("file_type");
            entity.Property(a => a.CreatedAt).HasDefaultValueSql("NOW()").HasColumnName("created_at");

            entity.HasOne(a => a.Feedback)
                .WithMany(f => f.Attachments)
                .HasForeignKey(a => a.FeedbackId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}