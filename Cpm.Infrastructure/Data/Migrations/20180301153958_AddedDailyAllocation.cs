using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Cpm.Infrastructure.Data.Migrations
{
    public partial class AddedDailyAllocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyAllocations");
        }
    }
}
