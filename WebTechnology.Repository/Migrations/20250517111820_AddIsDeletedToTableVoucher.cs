using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebTechnology.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedToTableVoucher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "vouchers",
                type: "tinyint(1)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "vouchers");
        }
    }
}
