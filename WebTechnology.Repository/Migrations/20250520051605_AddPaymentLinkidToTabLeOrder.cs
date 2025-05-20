using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebTechnology.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentLinkidToTabLeOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentLinkId",
                table: "orders",
                type: "longtext",
                nullable: true,
                collation: "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentLinkId",
                table: "orders");
        }
    }
}
