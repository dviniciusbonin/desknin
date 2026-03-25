using System.Text;
using DeskNin.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DeskNin.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketComment> TicketComments { get; set; }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            if (!string.IsNullOrEmpty(tableName))
                entityType.SetTableName(ToSnakeCase(tableName));
        }

        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = "ROLE_ADMIN",
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
            new IdentityRole
            {
                Id = "ROLE_TECHNICAL",
                Name = "Technical",
                NormalizedName = "TECHNICAL"
            },
            new IdentityRole
            {
                Id = "ROLE_USER",
                Name = "User",
                NormalizedName = "USER"
            }
        );

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.Property(t => t.Title).HasMaxLength(200).IsRequired();
            entity.Property(t => t.Description).HasMaxLength(4000).IsRequired();
            entity.Property(t => t.CreatedAtUtc).IsRequired();
            entity.Property(t => t.UpdatedAtUtc).IsRequired();

            entity.HasOne(t => t.Author)
                .WithMany()
                .HasForeignKey(t => t.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.AssignedTechnician)
                .WithMany()
                .HasForeignKey(t => t.AssignedTechnicianId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(t => t.Status);
            entity.HasIndex(t => t.Priority);
            entity.HasIndex(t => t.AssignedTechnicianId);
            entity.HasIndex(t => t.CreatedAtUtc);
        });

        modelBuilder.Entity<TicketComment>(entity =>
        {
            entity.Property(c => c.Content).HasMaxLength(2000).IsRequired();
            entity.Property(c => c.CreatedAtUtc).IsRequired();

            entity.HasOne(c => c.Ticket)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.Author)
                .WithMany()
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(c => new { c.TicketId, c.CreatedAtUtc });
        });
    }

    /// <summary>
    /// Converts PascalCase to snake_case (e.g. AspNetUsers → asp_net_users).
    /// </summary>
    private static string ToSnakeCase(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        var result = new StringBuilder(name.Length);
        for (var i = 0; i < name.Length; i++)
        {
            var c = name[i];
            if (char.IsUpper(c))
            {
                if (i > 0)
                    result.Append('_');
                result.Append(char.ToLowerInvariant(c));
            }
            else
            {
                result.Append(c);
            }
        }
        return result.ToString();
    }
}