using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DeskNin.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedIdentityRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "asp_net_roles",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[,]
                {
                    { "ROLE_ADMIN", "ef0dbc30-e5af-4c43-be90-7d563ad1672e", "Admin", "ADMIN" },
                    { "ROLE_TECHNICAL", "37adad9f-9d91-45ca-b9ad-e2f5561fd184", "Technical", "TECHNICAL" },
                    { "ROLE_USER", "7cd19f1f-da5a-44a2-8984-8d210066bb76", "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "asp_net_roles",
                keyColumn: "id",
                keyValue: "ROLE_ADMIN");

            migrationBuilder.DeleteData(
                table: "asp_net_roles",
                keyColumn: "id",
                keyValue: "ROLE_TECHNICAL");

            migrationBuilder.DeleteData(
                table: "asp_net_roles",
                keyColumn: "id",
                keyValue: "ROLE_USER");
        }
    }
}
