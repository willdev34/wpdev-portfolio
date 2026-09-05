using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExplicitPublicSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.RenameTable(
                name: "TimelineEvents",
                newName: "TimelineEvents",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Projects",
                newName: "Projects",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "NowSections",
                newName: "NowSections",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "GalleryImages",
                newName: "GalleryImages",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "ContactMessages",
                newName: "ContactMessages",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "BlogPosts",
                newName: "BlogPosts",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                newName: "AspNetUserTokens",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "AspNetUsers",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                newName: "AspNetUserRoles",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                newName: "AspNetUserLogins",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                newName: "AspNetUserClaims",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                newName: "AspNetRoles",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                newName: "AspNetRoleClaims",
                newSchema: "public");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "TimelineEvents",
                schema: "public",
                newName: "TimelineEvents");

            migrationBuilder.RenameTable(
                name: "Projects",
                schema: "public",
                newName: "Projects");

            migrationBuilder.RenameTable(
                name: "NowSections",
                schema: "public",
                newName: "NowSections");

            migrationBuilder.RenameTable(
                name: "GalleryImages",
                schema: "public",
                newName: "GalleryImages");

            migrationBuilder.RenameTable(
                name: "ContactMessages",
                schema: "public",
                newName: "ContactMessages");

            migrationBuilder.RenameTable(
                name: "BlogPosts",
                schema: "public",
                newName: "BlogPosts");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                schema: "public",
                newName: "AspNetUserTokens");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                schema: "public",
                newName: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                schema: "public",
                newName: "AspNetUserRoles");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                schema: "public",
                newName: "AspNetUserLogins");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                schema: "public",
                newName: "AspNetUserClaims");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                schema: "public",
                newName: "AspNetRoles");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                schema: "public",
                newName: "AspNetRoleClaims");
        }
    }
}
