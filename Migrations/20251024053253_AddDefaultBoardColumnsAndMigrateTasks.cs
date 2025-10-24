using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagementMvc.Migrations
{
    public partial class AddDefaultBoardColumnsAndMigrateTasks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert default board columns for each project
            migrationBuilder.Sql(@"
                INSERT INTO `BoardColumns` (`Name`, `Order`, `IsCompletedColumn`, `ProjectId`)
                SELECT 'Backlog', 1, 0, `Id` FROM `Projects`;
            ");

            migrationBuilder.Sql(@"
                INSERT INTO `BoardColumns` (`Name`, `Order`, `IsCompletedColumn`, `ProjectId`)
                SELECT 'To Do', 2, 0, `Id` FROM `Projects`;
            ");

            migrationBuilder.Sql(@"
                INSERT INTO `BoardColumns` (`Name`, `Order`, `IsCompletedColumn`, `ProjectId`)
                SELECT 'In Progress', 3, 0, `Id` FROM `Projects`;
            ");

            migrationBuilder.Sql(@"
                INSERT INTO `BoardColumns` (`Name`, `Order`, `IsCompletedColumn`, `ProjectId`)
                SELECT 'Done', 4, 1, `Id` FROM `Projects`;
            ");

            // Migrate existing tasks
            migrationBuilder.Sql(@"
                UPDATE `Tasks`
                SET `BoardColumnId` = (
                    SELECT `Id` FROM `BoardColumns`
                    WHERE `ProjectId` = `Tasks`.`ProjectId` AND `Name` = CASE
                        WHEN `Status` = 0 THEN 'To Do'
                        WHEN `Status` = 1 THEN 'In Progress'
                        WHEN `Status` = 2 THEN 'Done'
                        ELSE 'Backlog'
                    END
                    LIMIT 1
                )
                WHERE `ProjectId` IS NOT NULL;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM BoardColumns WHERE Name IN ('Backlog', 'To Do', 'In Progress', 'Done');");
        }
    }
}
