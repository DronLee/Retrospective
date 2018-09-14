using Microsoft.EntityFrameworkCore.Migrations;

namespace Retrospective.Migrations
{
    public partial class subject_name_key : Migration
    {
        private const string tableName = "Subjects";
        private const string keyName = "UK_Subject_Name";
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(keyName, tableName, "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(keyName, tableName);
        }
    }
}
