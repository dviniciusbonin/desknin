using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace DeskNin.Data.Migrations
{
    /// <inheritdoc />
    public partial class EnsureDefaultAdmin : Migration
    {
        private static string EscapeSql(string value) => value?.Replace("'", "''") ?? "";

        private static IConfiguration Config => new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var email = Config["Seed:DefaultAdminEmail"] ?? "admin@desknin.local";
            var password = Config["Seed:DefaultAdminPassword"] ?? "Admin@123";
            var normalizedEmail = email.ToUpperInvariant();
            var adminId = Guid.NewGuid().ToString();
            var securityStamp = Guid.NewGuid().ToString();
            var concurrencyStamp = Guid.NewGuid().ToString();

            var hasher = new PasswordHasher<IdentityUser>();
            var passwordHash = EscapeSql(hasher.HashPassword(null!, password));

            migrationBuilder.Sql($@"
                INSERT INTO asp_net_users (
                    id, user_name, normalized_user_name, email, normalized_email,
                    email_confirmed, password_hash, security_stamp, concurrency_stamp,
                    phone_number, phone_number_confirmed, two_factor_enabled,
                    lockout_end, lockout_enabled, access_failed_count
                )
                SELECT '{adminId}', '{EscapeSql(email)}', '{EscapeSql(normalizedEmail)}', '{EscapeSql(email)}', '{EscapeSql(normalizedEmail)}',
                    true, '{passwordHash}', '{securityStamp}', '{concurrencyStamp}',
                    NULL, false, false, NULL, true, 0
                WHERE NOT EXISTS (SELECT 1 FROM asp_net_users WHERE normalized_email = '{EscapeSql(normalizedEmail)}');
            ");

            migrationBuilder.Sql($@"
                INSERT INTO asp_net_user_roles (user_id, role_id)
                SELECT u.id, 'ROLE_ADMIN'
                FROM asp_net_users u
                WHERE u.normalized_email = '{EscapeSql(normalizedEmail)}'
                AND NOT EXISTS (
                    SELECT 1 FROM asp_net_user_roles ur
                    WHERE ur.user_id = u.id AND ur.role_id = 'ROLE_ADMIN'
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var normalizedEmail = EscapeSql((Config["Seed:DefaultAdminEmail"] ?? "admin@desknin.local").ToUpperInvariant());
            migrationBuilder.Sql($@"
                DELETE FROM asp_net_user_roles
                WHERE user_id IN (SELECT id FROM asp_net_users WHERE normalized_email = '{normalizedEmail}');

                DELETE FROM asp_net_users WHERE normalized_email = '{normalizedEmail}';
            ");
        }
    }
}
