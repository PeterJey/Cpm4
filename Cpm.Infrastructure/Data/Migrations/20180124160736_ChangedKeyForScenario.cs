using Microsoft.EntityFrameworkCore.Migrations;

namespace Cpm.Infrastructure.Data.Migrations
{
    public partial class ChangedKeyForScenario : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Scenarios",
                table: "Scenarios");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Scenarios",
                table: "Scenarios",
                columns: new[] { "ScenarioId", "Version" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Scenarios",
                table: "Scenarios");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Scenarios",
                table: "Scenarios",
                column: "ScenarioId");
        }
    }
}
