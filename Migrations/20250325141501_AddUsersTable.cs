using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuCategory_Restaurant_RestaurantId",
                table: "MenuCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuItem_MenuCategory_MenuCategoryId",
                table: "MenuItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MenuCategory",
                table: "MenuCategory");

            migrationBuilder.RenameTable(
                name: "MenuCategory",
                newName: "MenuCategories");

            migrationBuilder.RenameIndex(
                name: "IX_MenuCategory_RestaurantId",
                table: "MenuCategories",
                newName: "IX_MenuCategories_RestaurantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MenuCategories",
                table: "MenuCategories",
                column: "MenuCategoryId");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    RestaurantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Restaurant_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurant",
                        principalColumn: "RestaurantId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_RestaurantId",
                table: "Users",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCategories_Restaurant_RestaurantId",
                table: "MenuCategories",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "RestaurantId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItem_MenuCategories_MenuCategoryId",
                table: "MenuItem",
                column: "MenuCategoryId",
                principalTable: "MenuCategories",
                principalColumn: "MenuCategoryId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuCategories_Restaurant_RestaurantId",
                table: "MenuCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuItem_MenuCategories_MenuCategoryId",
                table: "MenuItem");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MenuCategories",
                table: "MenuCategories");

            migrationBuilder.RenameTable(
                name: "MenuCategories",
                newName: "MenuCategory");

            migrationBuilder.RenameIndex(
                name: "IX_MenuCategories_RestaurantId",
                table: "MenuCategory",
                newName: "IX_MenuCategory_RestaurantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MenuCategory",
                table: "MenuCategory",
                column: "MenuCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCategory_Restaurant_RestaurantId",
                table: "MenuCategory",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "RestaurantId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItem_MenuCategory_MenuCategoryId",
                table: "MenuItem",
                column: "MenuCategoryId",
                principalTable: "MenuCategory",
                principalColumn: "MenuCategoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
