using Microsoft.EntityFrameworkCore.Migrations;

namespace TestTaskServer.Migrations
{
    public partial class RenameField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Word",
                table: "Words");

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "Words",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "Words");

            migrationBuilder.AddColumn<string>(
                name: "Word",
                table: "Words",
                type: "text",
                nullable: true);
        }
    }
}
