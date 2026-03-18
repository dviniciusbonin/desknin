using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DeskNin.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
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