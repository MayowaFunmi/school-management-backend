using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementApi.Migrations
{
    /// <inheritdoc />
    public partial class addScores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SchoolSessions",
                columns: table => new
                {
                    SchoolSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    SessionStarts = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SessionEnds = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolSessions", x => x.SchoolSessionId);
                });

            migrationBuilder.CreateTable(
                name: "SchoolTerms",
                columns: table => new
                {
                    SchoolTermId = table.Column<Guid>(type: "uuid", nullable: false),
                    SchoolId = table.Column<string>(type: "text", nullable: false),
                    SchoolSessionId = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TermStarts = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TermEnds = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SchoolSessionId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolTerms", x => x.SchoolTermId);
                    table.ForeignKey(
                        name: "FK_SchoolTerms_SchoolSessions_SchoolSessionId1",
                        column: x => x.SchoolSessionId1,
                        principalTable: "SchoolSessions",
                        principalColumn: "SchoolSessionId");
                });

            migrationBuilder.CreateTable(
                name: "StudentsCARecords",
                columns: table => new
                {
                    TestId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassId = table.Column<string>(type: "text", nullable: false),
                    SubjectId = table.Column<string>(type: "text", nullable: false),
                    SchoolSessionId = table.Column<string>(type: "text", nullable: false),
                    SchoolSessionId1 = table.Column<Guid>(type: "uuid", nullable: false),
                    Term = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentsCARecords", x => x.TestId);
                    table.ForeignKey(
                        name: "FK_StudentsCARecords_SchoolSessions_SchoolSessionId1",
                        column: x => x.SchoolSessionId1,
                        principalTable: "SchoolSessions",
                        principalColumn: "SchoolSessionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentsScores",
                columns: table => new
                {
                    StudentScoresId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentsCARecordId = table.Column<string>(type: "text", nullable: false),
                    StudentId = table.Column<string>(type: "text", nullable: false),
                    CATest1 = table.Column<int>(type: "integer", nullable: false),
                    CATest2 = table.Column<int>(type: "integer", nullable: false),
                    CATest3 = table.Column<int>(type: "integer", nullable: false),
                    Exam = table.Column<int>(type: "integer", nullable: false),
                    StudentId1 = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentsCARecordTestId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentsScores", x => x.StudentScoresId);
                    table.ForeignKey(
                        name: "FK_StudentsScores_StudentsCARecords_StudentsCARecordTestId",
                        column: x => x.StudentsCARecordTestId,
                        principalTable: "StudentsCARecords",
                        principalColumn: "TestId");
                    table.ForeignKey(
                        name: "FK_StudentsScores_Students_StudentId1",
                        column: x => x.StudentId1,
                        principalTable: "Students",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SchoolTerms_SchoolSessionId1",
                table: "SchoolTerms",
                column: "SchoolSessionId1");

            migrationBuilder.CreateIndex(
                name: "IX_StudentsCARecords_SchoolSessionId1",
                table: "StudentsCARecords",
                column: "SchoolSessionId1");

            migrationBuilder.CreateIndex(
                name: "IX_StudentsScores_StudentId1",
                table: "StudentsScores",
                column: "StudentId1");

            migrationBuilder.CreateIndex(
                name: "IX_StudentsScores_StudentsCARecordTestId",
                table: "StudentsScores",
                column: "StudentsCARecordTestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SchoolTerms");

            migrationBuilder.DropTable(
                name: "StudentsScores");

            migrationBuilder.DropTable(
                name: "StudentsCARecords");

            migrationBuilder.DropTable(
                name: "SchoolSessions");
        }
    }
}
