using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Cpm.Infrastructure.Data.Migrations
{
    public partial class ChangingChillUnitsToIntAndRenaming : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "YieldRegisters");

            migrationBuilder.DropColumn(
                name: "ChillUnits",
                table: "FieldScores");

            migrationBuilder.AlterColumn<int>(
                name: "GrowingDegreeHours",
                table: "FieldScores",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(15,3)");

            migrationBuilder.AddColumn<int>(
                name: "ChillHours",
                table: "FieldScores",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Farms",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 255);

            migrationBuilder.CreateTable(
                name: "HarvestRegisters",
                columns: table => new
                {
                    FieldId = table.Column<string>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    FirstDay = table.Column<DateTime>(nullable: false),
                    SerializedValues = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HarvestRegisters", x => new { x.FieldId, x.Version });
                    table.ForeignKey(
                        name: "FK_HarvestRegisters_Fields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "Fields",
                        principalColumn: "FieldId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HarvestRegisters");

            migrationBuilder.DropColumn(
                name: "ChillHours",
                table: "FieldScores");

            migrationBuilder.AlterColumn<decimal>(
                name: "GrowingDegreeHours",
                table: "FieldScores",
                type: "DECIMAL(15,3)",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<decimal>(
                name: "ChillUnits",
                table: "FieldScores",
                type: "DECIMAL(15,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Farms",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateTable(
                name: "YieldRegisters",
                columns: table => new
                {
                    FieldId = table.Column<string>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    FirstDay = table.Column<DateTime>(nullable: false),
                    SerializedValues = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YieldRegisters", x => new { x.FieldId, x.Version });
                    table.ForeignKey(
                        name: "FK_YieldRegisters_Fields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "Fields",
                        principalColumn: "FieldId",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
