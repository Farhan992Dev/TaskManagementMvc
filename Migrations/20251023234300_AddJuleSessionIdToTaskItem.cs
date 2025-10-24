using Microsoft.EntityFrameworkCore.Migrations;

namespace TaskManagementMvc.Migrations
{
    public partial class AddJuleSessionIdToTaskItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JuleSessionId",
                table: "Tasks",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JuleSessionId",
                table: "Tasks");
        }
    }
}
