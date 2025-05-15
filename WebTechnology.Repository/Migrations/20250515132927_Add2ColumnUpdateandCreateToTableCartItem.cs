using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebTechnology.Repository.Migrations
{
    /// <inheritdoc />
    public partial class Add2ColumnUpdateandCreateToTableCartItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "cart_items",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "cart_items",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "cart_items");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "cart_items");
        }
    }
}
