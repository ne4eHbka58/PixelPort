using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PixelPort.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddProductsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Manufacturers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ManufacturerName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufacturers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: false),
                    ManufacturerID = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Products_Manufacturers_ManufacturerID",
                        column: x => x.ManufacturerID,
                        principalTable: "Manufacturers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductCharacteristic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CharacteristicName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CharacteristicValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCharacteristic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductCharacteristic_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CategoryName" },
                values: new object[] { 1, "Телефоны" });

            migrationBuilder.InsertData(
                table: "Manufacturers",
                columns: new[] { "Id", "ManufacturerName" },
                values: new object[,]
                {
                    { 1, "Apple" },
                    { 2, "Samsung" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryID", "CreatedDate", "ManufacturerID", "Price", "ProductName", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 40000, "Iphone 14 128 Гб Чёрный ", new DateTime(2025, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 1, new DateTime(2025, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 20000, "Sumsung Galaxy A26 128 Гб Зелёный", new DateTime(2025, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "ProductCharacteristic",
                columns: new[] { "Id", "CharacteristicName", "CharacteristicValue", "ProductId" },
                values: new object[,]
                {
                    { 1, "Объём встроенной памяти", "128 Гб", 1 },
                    { 2, "Объём оперативной памяти", "6 Гб", 1 },
                    { 3, "Цвет", "Чёрный", 1 },
                    { 4, "Количество основных камер", "2", 1 },
                    { 5, "Диагональ экрана", "6.1", 1 },
                    { 6, "Объём встроенной памяти", "128 Гб", 2 },
                    { 7, "Объём оперативной памяти", "6 Гб", 2 },
                    { 8, "Цвет", "Зелёный", 2 },
                    { 9, "Количество основных камер", "3", 2 },
                    { 10, "Диагональ экрана", "6.7", 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductCharacteristic_ProductId",
                table: "ProductCharacteristic",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryID",
                table: "Products",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ManufacturerID",
                table: "Products",
                column: "ManufacturerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductCharacteristic");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Manufacturers");
        }
    }
}
