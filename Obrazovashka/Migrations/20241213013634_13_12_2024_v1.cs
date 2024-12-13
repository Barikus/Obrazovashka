using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Obrazovashka.Migrations
{
    public partial class _13_12_2024_v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Certificates");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Certificates",
                newName: "EnrollmentId");

            migrationBuilder.AddColumn<DateTime>(
                name: "EnrollmentDate",
                table: "Enrollments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "FeedbackId",
                table: "Enrollments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "Courses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string[]>(
                name: "Tags",
                table: "Courses",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnrollmentDate",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "FeedbackId",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "EnrollmentId",
                table: "Certificates",
                newName: "UserId");

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "Certificates",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
