using Microsoft.EntityFrameworkCore.Migrations;

namespace Cpm.Infrastructure.Data.Migrations
{
    public partial class ImprovedPermissionModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsViewer",
                table: "SiteUserPermissions",
                newName: "CanView");

            migrationBuilder.RenameColumn(
                name: "IsManager",
                table: "SiteUserPermissions",
                newName: "CanUpdateDiary");

            migrationBuilder.RenameColumn(
                name: "IsDataEntry",
                table: "SiteUserPermissions",
                newName: "CanPlan");

            migrationBuilder.RenameColumn(
                name: "IsAdmin",
                table: "SiteUserPermissions",
                newName: "CanManagePermissions");

            migrationBuilder.AddColumn<bool>(
                name: "CanAllocate",
                table: "SiteUserPermissions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanForecast",
                table: "SiteUserPermissions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanLogActualData",
                table: "SiteUserPermissions",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanAllocate",
                table: "SiteUserPermissions");

            migrationBuilder.DropColumn(
                name: "CanForecast",
                table: "SiteUserPermissions");

            migrationBuilder.DropColumn(
                name: "CanLogActualData",
                table: "SiteUserPermissions");

            migrationBuilder.RenameColumn(
                name: "CanView",
                table: "SiteUserPermissions",
                newName: "IsViewer");

            migrationBuilder.RenameColumn(
                name: "CanUpdateDiary",
                table: "SiteUserPermissions",
                newName: "IsManager");

            migrationBuilder.RenameColumn(
                name: "CanPlan",
                table: "SiteUserPermissions",
                newName: "IsDataEntry");

            migrationBuilder.RenameColumn(
                name: "CanManagePermissions",
                table: "SiteUserPermissions",
                newName: "IsAdmin");
        }
    }
}
