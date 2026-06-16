using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NowSection_ReplaceProjectWithProjectLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentProject",
                table: "NowSections");

            migrationBuilder.DropColumn(
                name: "CurrentProjectUrl",
                table: "NowSections");

            migrationBuilder.AddColumn<string>(
                name: "CurrentProjects",
                table: "NowSections",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentProjects",
                table: "NowSections");

            migrationBuilder.AddColumn<string>(
                name: "CurrentProject",
                table: "NowSections",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentProjectUrl",
                table: "NowSections",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
