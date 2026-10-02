using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBlogPostStatusAndScheduledAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledAt",
                schema: "public",
                table: "BlogPosts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "public",
                table: "BlogPosts",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Draft");

            migrationBuilder.Sql(
                "UPDATE \"public\".\"BlogPosts\" SET \"Status\" = 'Published' WHERE \"IsPublished\" = true;");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_Status_ScheduledAt",
                schema: "public",
                table: "BlogPosts",
                columns: new[] { "Status", "ScheduledAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_Status_ScheduledAt",
                schema: "public",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "ScheduledAt",
                schema: "public",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "public",
                table: "BlogPosts");
        }
    }
}
