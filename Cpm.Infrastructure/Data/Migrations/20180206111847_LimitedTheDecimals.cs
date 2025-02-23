using Microsoft.EntityFrameworkCore.Migrations;

namespace Cpm.Infrastructure.Data.Migrations
{
    public partial class LimitedTheDecimals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "GrowingDegreeHours",
                table: "FieldScores",
                type: "DECIMAL(15,3)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "ChillUnits",
                table: "FieldScores",
                type: "DECIMAL(15,3)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "AreaInHectares",
                table: "Fields",
                type: "DECIMAL(15,3)",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "GrowingDegreeHours",
                table: "FieldScores",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(15,3)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ChillUnits",
                table: "FieldScores",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(15,3)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AreaInHectares",
                table: "Fields",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(15,3)");
        }
    }
}
