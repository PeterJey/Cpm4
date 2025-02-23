using Microsoft.EntityFrameworkCore.Migrations;

namespace Cpm.Infrastructure.Data.Migrations
{
    public partial class AddedAreaUnitToApplicationUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AreaUnit",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AreaUnit",
                table: "AspNetUsers");
        }
    }
}
