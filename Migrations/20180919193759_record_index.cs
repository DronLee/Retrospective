using Microsoft.EntityFrameworkCore.Migrations;

namespace Retrospective.Migrations
{
    public partial class record_index : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Records_SubjectId_CreatedOn",
                table: "Records",
                columns: new[] { "SubjectId", "CreatedOn" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Records_SubjectId_CreatedOn",
                table: "Records");
        }
    }
}
