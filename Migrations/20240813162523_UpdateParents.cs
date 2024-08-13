using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateParents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Parents_Schools_StudentSchoolId",
                table: "Parents");

            migrationBuilder.DropIndex(
                name: "IX_Parents_StudentSchoolId",
                table: "Parents");

            migrationBuilder.DropColumn(
                name: "StudentSchoolId",
                table: "Parents");

            migrationBuilder.AddColumn<string>(
                name: "SchoolUniqueId",
                table: "Parents",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SchoolUniqueId",
                table: "Parents");

            migrationBuilder.AddColumn<Guid>(
                name: "StudentSchoolId",
                table: "Parents",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Parents_StudentSchoolId",
                table: "Parents",
                column: "StudentSchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_Parents_Schools_StudentSchoolId",
                table: "Parents",
                column: "StudentSchoolId",
                principalTable: "Schools",
                principalColumn: "SchoolId");
        }
    }
}
