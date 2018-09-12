using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Retrospective.Migrations
{
    public partial class record : Migration
    {
        private const string tableName = "Records";
        private const string foreignKeyName = "FK_Records_Subject_SubjectId";
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: tableName,
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SubjectId = table.Column<int>(nullable: false),
                    Author = table.Column<string>(maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    RecordType = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Records", x => x.Id);
                });
            migrationBuilder.AddForeignKey(foreignKeyName, 
                tableName, "SubjectId", "Subjects", principalColumn: "Id", 
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(foreignKeyName, tableName);
            migrationBuilder.DropTable(
                name: tableName);
        }
    }
}
