using Microsoft.EntityFrameworkCore.Migrations;

namespace Cpm.Infrastructure.Data.Migrations
{
    public partial class MovedDescriptionFromFieldToFieldScore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Fields");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "FieldScores",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "FieldScores");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Fields",
                nullable: true);
        }
    }
}
