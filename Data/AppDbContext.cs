using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class AppDbContext : IdentityDbContext<User, Role, int>
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
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Name).HasDefaultValue("Новый");
            
            entity.ToTable("statuses");
        });
        
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(50);
            entity.Property(c => c.Description).HasMaxLength(200);
            entity.ToTable("categories");
        });
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Name).IsRequired().HasMaxLength(50);
            
            entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
            entity.HasIndex(u => u.Email).IsUnique();
            
            entity.Property(u => u.CreatedAt).HasDefaultValueSql("NOW()");
            entity.Property(u => u.LastLogin).IsRequired(false);
            
            entity.ToTable("users");
        });
        
        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(f => f.Id);

            entity.Property(f => f.UserId).IsRequired();
            entity.Property(f => f.CategoryId).IsRequired();
            entity.Property(f => f.StatusId).IsRequired();

            entity.Property(f => f.Message).IsRequired();
            entity.Property(f => f.CreatedAt).HasDefaultValueSql("NOW()");
            
            entity.HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.ToTable("feedbacks"); 
        });

        modelBuilder.Entity<Response>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.Property(r => r.FeedbackId).IsRequired();
            entity.Property(r => r.UserId).IsRequired();  
            
            entity.Property(r => r.Message).IsRequired();
            entity.Property(r => r.CreatedAt).HasDefaultValueSql("NOW()");

            entity.HasOne(r => r.Feedback)
                .WithMany(f => f.Responses)
                .HasForeignKey(r => r.FeedbackId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.ToTable("responses");
        });

        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.HasKey(a => a.Id);
            
            entity.Property(a => a.FilePath).IsRequired().HasMaxLength(255);
            entity.Property(a => a.FileType).IsRequired().HasMaxLength(100);
            entity.Property(a => a.CreatedAt).HasDefaultValueSql("NOW()");
            
            entity.HasOne(a => a.Feedback)
                .WithMany(f => f.Attachments)
                .HasForeignKey(a => a.FeedbackId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.ToTable("attachments");
        });
    }
}