using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderAndOrderItemsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuCategories_Restaurant_RestaurantId",
                table: "MenuCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuItem_MenuCategories_MenuCategoryId",
                table: "MenuItem");

            migrationBuilder.DropForeignKey(
                name: "FK_Tables_Restaurant_RestaurantId",
                table: "Tables");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Restaurant_RestaurantId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalAmount = table.Column<double>(type: "float", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TableId = table.Column<int>(type: "int", nullable: false),
                    CustomerUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Order_Tables_TableId",
                        column: x => x.TableId,
                        principalTable: "Tables",
                        principalColumn: "TableId");
                    table.ForeignKey(
                        name: "FK_Order_Users_CustomerUserId",
                        column: x => x.CustomerUserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "OrderItem",
                columns: table => new
                {
                    OrderItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    TotalPrice = table.Column<double>(type: "float", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItem", x => x.OrderItemId);
                    table.ForeignKey(
                        name: "FK_OrderItem_MenuItem_ItemId",
                        column: x => x.ItemId,
                        principalTable: "MenuItem",
                        principalColumn: "MenuItemId");
                    table.ForeignKey(
                        name: "FK_OrderItem_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "OrderId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_CustomerUserId",
                table: "Order",
                column: "CustomerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_TableId",
                table: "Order",
                column: "TableId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_ItemId",
                table: "OrderItem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_OrderId",
                table: "OrderItem",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCategories_Restaurant_RestaurantId",
                table: "MenuCategories",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "RestaurantId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItem_MenuCategories_MenuCategoryId",
                table: "MenuItem",
                column: "MenuCategoryId",
                principalTable: "MenuCategories",
                principalColumn: "MenuCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tables_Restaurant_RestaurantId",
                table: "Tables",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "RestaurantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Restaurant_RestaurantId",
                table: "Users",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "RestaurantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "RoleId");
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

            migrationBuilder.DropForeignKey(
                name: "FK_Tables_Restaurant_RestaurantId",
                table: "Tables");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Restaurant_RestaurantId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "OrderItem");

            migrationBuilder.DropTable(
                name: "Order");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Tables_Restaurant_RestaurantId",
                table: "Tables",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "RestaurantId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Restaurant_RestaurantId",
                table: "Users",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "RestaurantId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
