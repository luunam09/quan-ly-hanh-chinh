using Microsoft.EntityFrameworkCore;
using StudentAdminPortal.Models;

namespace StudentAdminPortal.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Procedure> Procedures => Set<Procedure>();
    public DbSet<AdministrativeRequest> Requests => Set<AdministrativeRequest>();
    public DbSet<RequestAttachment> Attachments => Set<RequestAttachment>();
    public DbSet<RequestLog> RequestLogs => Set<RequestLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Username).IsUnique();
        });

        modelBuilder.Entity<AdministrativeRequest>(entity =>
        {
            entity.HasIndex(r => r.RequestCode).IsUnique();

            entity.HasOne(r => r.Student)
                  .WithMany(u => u.Requests)
                  .HasForeignKey(r => r.StudentId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.Procedure)
                  .WithMany(p => p.Requests)
                  .HasForeignKey(r => r.ProcedureId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RequestAttachment>(entity =>
        {
            entity.HasOne(a => a.Request)
                  .WithMany(r => r.Attachments)
                  .HasForeignKey(a => a.RequestId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RequestLog>(entity =>
        {
            entity.HasOne(l => l.Request)
                  .WithMany(r => r.Logs)
                  .HasForeignKey(l => l.RequestId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
