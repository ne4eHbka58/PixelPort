using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PixelPort.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedProductCharacteristicsDbSetToContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductCharacteristic_Products_ProductId",
                table: "ProductCharacteristic");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductCharacteristic",
                table: "ProductCharacteristic");

            migrationBuilder.RenameTable(
                name: "ProductCharacteristic",
                newName: "ProductCharacteristics");

            migrationBuilder.RenameIndex(
                name: "IX_ProductCharacteristic_ProductId",
                table: "ProductCharacteristics",
                newName: "IX_ProductCharacteristics_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductCharacteristics",
                table: "ProductCharacteristics",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCharacteristics_Products_ProductId",
                table: "ProductCharacteristics",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductCharacteristics_Products_ProductId",
                table: "ProductCharacteristics");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductCharacteristics",
                table: "ProductCharacteristics");

            migrationBuilder.RenameTable(
                name: "ProductCharacteristics",
                newName: "ProductCharacteristic");

            migrationBuilder.RenameIndex(
                name: "IX_ProductCharacteristics_ProductId",
                table: "ProductCharacteristic",
                newName: "IX_ProductCharacteristic_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductCharacteristic",
                table: "ProductCharacteristic",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCharacteristic_Products_ProductId",
                table: "ProductCharacteristic",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
