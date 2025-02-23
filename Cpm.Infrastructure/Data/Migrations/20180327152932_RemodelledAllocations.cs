using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Cpm.Infrastructure.Data.Migrations
{
    public partial class RemodelledAllocations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyAllocations");

            migrationBuilder.CreateTable(
                name: "Allocations",
                columns: table => new
                {
                    FieldId = table.Column<string>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    ProductType = table.Column<string>(nullable: false),
                    PerTray = table.Column<int>(nullable: false),
                    PerPunnet = table.Column<int>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    Weight = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Allocations", x => new { x.FieldId, x.Date, x.ProductType, x.PerTray, x.PerPunnet, x.Version });
                    table.ForeignKey(
                        name: "FK_Allocations_Fields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "Fields",
                        principalColumn: "FieldId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Allocations");

            migrationBuilder.CreateTable(
                name: "DailyAllocations",
                columns: table => new
                {
                    SiteId = table.Column<string>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    Serialized = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyAllocations", x => new { x.SiteId, x.Date, x.Version });
                    table.ForeignKey(
                        name: "FK_DailyAllocations_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "SiteId",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
