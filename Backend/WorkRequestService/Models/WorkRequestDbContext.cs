using Microsoft.EntityFrameworkCore;

namespace WorkRequestService.Models;

public class WorkRequestDbContext : DbContext
{
    public WorkRequestDbContext(DbContextOptions<WorkRequestDbContext> options)
        : base(options)
    {
    }

    public DbSet<WorkRequest> WorkRequests => Set<WorkRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkRequest>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Title)
                .IsRequired();

            entity.Property(x => x.ClientName)
                .IsRequired();

            entity.Property(x => x.Description)
                .IsRequired();

            entity.Property(x => x.Priority)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.DueDate)
                .IsRequired();

            entity.Property(x => x.CreatedDate)
                .IsRequired();

            entity.Property(x => x.UpdatedDate)
                .IsRequired();
        });
    }
}
