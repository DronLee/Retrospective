using Microsoft.EntityFrameworkCore.Migrations;

namespace Retrospective.Migrations
{
    public partial class subject_password : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Subjects",
                maxLength: 344,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 20);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Subjects",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 344);
        }
    }
}
