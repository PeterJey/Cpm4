using Microsoft.EntityFrameworkCore.Migrations;

namespace Cpm.Infrastructure.Data.Migrations
{
    public partial class ChangedToSerializedFieldBudget : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BudgetPerHectare",
                table: "FieldScores");

            migrationBuilder.DropColumn(
                name: "BudgetPerPlant",
                table: "FieldScores");

            migrationBuilder.AddColumn<string>(
                name: "SerializedBudget",
                table: "FieldScores",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SerializedBudget",
                table: "FieldScores");

            migrationBuilder.AddColumn<decimal>(
                name: "BudgetPerHectare",
                table: "FieldScores",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BudgetPerPlant",
                table: "FieldScores",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
