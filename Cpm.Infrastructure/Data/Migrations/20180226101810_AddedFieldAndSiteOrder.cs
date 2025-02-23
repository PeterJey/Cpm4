using Microsoft.EntityFrameworkCore.Migrations;

namespace Cpm.Infrastructure.Data.Migrations
{
    public partial class AddedFieldAndSiteOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Sites",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Fields",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Fields");
        }
    }
}
