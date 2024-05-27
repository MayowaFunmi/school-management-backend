using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementApi.Migrations
{
    /// <inheritdoc />
    public partial class RegisterModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SchoolId",
                table: "SchoolSessions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ClassAttendances",
                columns: table => new
                {
                    AttendanceId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<string>(type: "text", nullable: false),
                    ClassArmId = table.Column<string>(type: "text", nullable: false),
                    DayAndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MorningAttendance = table.Column<string>(type: "text", nullable: false),
                    AfternoonAttendance = table.Column<string>(type: "text", nullable: false),
                    IsMarked = table.Column<bool>(type: "boolean", nullable: false),
                    StudentId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassAttendances", x => x.AttendanceId);
                    table.ForeignKey(
                        name: "FK_ClassAttendances_Students_StudentId1",
                        column: x => x.StudentId1,
                        principalTable: "Students",
                        principalColumn: "StudentId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassAttendances_StudentId1",
                table: "ClassAttendances",
                column: "StudentId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassAttendances");

            migrationBuilder.DropColumn(
                name: "SchoolId",
                table: "SchoolSessions");
        }
    }
}
