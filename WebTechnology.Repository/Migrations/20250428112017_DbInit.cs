using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebTechnology.Repository.Migrations
{
    /// <inheritdoc />
    public partial class DbInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key constraints first
            migrationBuilder.DropForeignKey(
                name: "FK_order_details_orders",
                table: "order_details");

            migrationBuilder.DropForeignKey(
                name: "FK_order_details_products",
                table: "order_details");

            // Then drop the unique index
            migrationBuilder.DropIndex(
                name: "unique_order_product",
                table: "order_details");

            // Recreate foreign key constraints
            migrationBuilder.AddForeignKey(
                name: "FK_order_details_orders",
                table: "order_details",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "orderid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_order_details_products",
                table: "order_details",
                column: "product_id",
                principalTable: "products",
                principalColumn: "productid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_order_details_order_id",
                table: "order_details");

            migrationBuilder.CreateIndex(
                name: "unique_order_product",
                table: "order_details",
                columns: new[] { "order_id", "product_id" },
                unique: true);
        }
    }
}
