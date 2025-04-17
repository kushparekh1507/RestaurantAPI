using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedSuperAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "FullName", "IsFirstLogin", "Password", "RestaurantId", "RoleId", "Status", "UserType" },
                values: new object[] { 1, "kushparekh943@gmail.com", "Super Admin", false, "$2a$11$OZsGF7lj/fx2uKwVlDKUCu6yugoftyY0FaSbLD8gIrH0DHrzonZ5q", null, 1, 0, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1);
        }
    }
}
