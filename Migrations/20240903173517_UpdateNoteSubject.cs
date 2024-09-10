using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNoteSubject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LessonNotes_Subjects_SubjectId1",
                table: "LessonNotes");

            migrationBuilder.DropIndex(
                name: "IX_LessonNotes_SubjectId1",
                table: "LessonNotes");

            migrationBuilder.DropColumn(
                name: "SubjectId1",
                table: "LessonNotes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SubjectId1",
                table: "LessonNotes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_LessonNotes_SubjectId1",
                table: "LessonNotes",
                column: "SubjectId1");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonNotes_Subjects_SubjectId1",
                table: "LessonNotes",
                column: "SubjectId1",
                principalTable: "Subjects",
                principalColumn: "SubjectId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
