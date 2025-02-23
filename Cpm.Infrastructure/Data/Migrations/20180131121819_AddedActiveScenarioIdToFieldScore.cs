using Microsoft.EntityFrameworkCore.Migrations;

namespace Cpm.Infrastructure.Data.Migrations
{
    public partial class AddedActiveScenarioIdToFieldScore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActiveScenarioId",
                table: "FieldScores",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveScenarioId",
                table: "FieldScores");
        }
    }
}
