// Repositories/AppDbContext.cs

using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DbSets для всех сущностей
    public DbSet<User> Users => Set<User>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Feedback> Feedbacks => Set<Feedback>();
    public DbSet<FeedbackCategory> FeedbackCategories => Set<FeedbackCategory>();
    public DbSet<Response> Responses => Set<Response>();
    public DbSet<Attachment> Attachments => Set<Attachment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Настройка User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);
            entity.HasIndex(u => u.Email)
                .IsUnique();
            entity.Property(u => u.CreatedAt)
                .HasDefaultValueSql("NOW()");
        });

        // Настройка Admin
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.UserName)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(a => a.Email)
                .IsRequired()
                .HasMaxLength(100);
            entity.HasIndex(a => a.Email)
                .IsUnique();
            entity.Property(a => a.CreatedAt)
                .HasDefaultValueSql("NOW()");
        });

        // Настройка Category
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.CategoryName)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(c => c.Description)
                .HasMaxLength(200);
        });

        // Настройка Feedback
        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(f => f.Id);
            entity.Property(f => f.FeedbackType)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(f => f.Message)
                .IsRequired();
            entity.Property(f => f.Status)
                .HasDefaultValue("Новый")
                .HasMaxLength(20);
            entity.Property(f => f.CreatedAt)
                .HasDefaultValueSql("NOW()");
            
            entity.HasOne(f => f.User)
                .WithMany(u => u.Feedbacks)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Настройка FeedbackCategory (связующая таблица)
        modelBuilder.Entity<FeedbackCategory>(entity =>
        {
            entity.HasKey(fc => fc.Id);
            
            entity.HasOne(fc => fc.Feedback)
                .WithMany(f => f.FeedbackCategories)
                .HasForeignKey(fc => fc.FeedbackId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(fc => fc.Category)
                .WithMany(c => c.FeedbackCategories)
                .HasForeignKey(fc => fc.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Настройка Response
        modelBuilder.Entity<Response>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.ResponseMessage)
                .IsRequired();
            entity.Property(r => r.CreatedAt)
                .HasDefaultValueSql("NOW()");
            
            entity.HasOne(r => r.Feedback)
                .WithMany(f => f.Responses)
                .HasForeignKey(r => r.FeedbackId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(r => r.AdminUser)
                .WithMany(a => a.Responses)
                .HasForeignKey(r => r.AdminUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Настройка Attachment
        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.FilePath)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(a => a.FileType)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(a => a.CreatedAt)
                .HasDefaultValueSql("NOW()");
            
            entity.HasOne(a => a.Feedback)
                .WithMany(f => f.Attachments)
                .HasForeignKey(a => a.FeedbackId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}