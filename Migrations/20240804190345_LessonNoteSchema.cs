using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using SchoolManagementApi.DTOs;

#nullable disable

namespace SchoolManagementApi.Migrations
{
    /// <inheritdoc />
    public partial class LessonNoteSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<string>>(
                name: "CurrentClasses",
                table: "TeachingStaffs",
                type: "text[]",
                nullable: false);

            migrationBuilder.CreateTable(
                name: "LessonNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeacherId = table.Column<string>(type: "text", nullable: false),
                    WeekNumber = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SubjectId = table.Column<string>(type: "text", nullable: false),
                    SubjectId1 = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassArmId = table.Column<string>(type: "text", nullable: false),
                    ClassArmId1 = table.Column<Guid>(type: "uuid", nullable: false),
                    Topic = table.Column<string>(type: "text", nullable: false),
                    SubTopic = table.Column<string>(type: "text", nullable: false),
                    ReferenceBook = table.Column<string>(type: "text", nullable: false),
                    InstructionalAid = table.Column<string>(type: "text", nullable: false),
                    IsTemplate = table.Column<bool>(type: "boolean", nullable: false),
                    CustomFields = table.Column<List<CustomField>>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonNotes_AspNetUsers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LessonNotes_ClassArms_ClassArmId1",
                        column: x => x.ClassArmId1,
                        principalTable: "ClassArms",
                        principalColumn: "ClassArmId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LessonNotes_Subjects_SubjectId1",
                        column: x => x.SubjectId1,
                        principalTable: "Subjects",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LessonNoteTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateName = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonNoteTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LessonPeriods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LessonNotesId = table.Column<string>(type: "text", nullable: false),
                    SubTopic = table.Column<string>(type: "text", nullable: false),
                    BehaviouralObjective = table.Column<string>(type: "text", nullable: false),
                    PreviousKnowledge = table.Column<string>(type: "text", nullable: false),
                    Presentations = table.Column<string>(type: "text", nullable: false),
                    Evaluations = table.Column<string>(type: "text", nullable: false),
                    Conclusion = table.Column<string>(type: "text", nullable: false),
                    Assignment = table.Column<string>(type: "text", nullable: false),
                    LessonNotesId1 = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonPeriods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonPeriods_LessonNotes_LessonNotesId1",
                        column: x => x.LessonNotesId1,
                        principalTable: "LessonNotes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LessonPeriodTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateId = table.Column<string>(type: "text", nullable: false),
                    TemplateNoteId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonPeriodTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonPeriodTemplates_LessonNoteTemplates_TemplateNoteId",
                        column: x => x.TemplateNoteId,
                        principalTable: "LessonNoteTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomField",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    LessonNoteTemplateId = table.Column<Guid>(type: "uuid", nullable: true),
                    LessonPeriodId = table.Column<Guid>(type: "uuid", nullable: true),
                    LessonPeriodTemplateId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomField", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomField_LessonNoteTemplates_LessonNoteTemplateId",
                        column: x => x.LessonNoteTemplateId,
                        principalTable: "LessonNoteTemplates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomField_LessonPeriodTemplates_LessonPeriodTemplateId",
                        column: x => x.LessonPeriodTemplateId,
                        principalTable: "LessonPeriodTemplates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomField_LessonPeriods_LessonPeriodId",
                        column: x => x.LessonPeriodId,
                        principalTable: "LessonPeriods",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomField_LessonNoteTemplateId",
                table: "CustomField",
                column: "LessonNoteTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomField_LessonPeriodId",
                table: "CustomField",
                column: "LessonPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomField_LessonPeriodTemplateId",
                table: "CustomField",
                column: "LessonPeriodTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonNotes_ClassArmId1",
                table: "LessonNotes",
                column: "ClassArmId1");

            migrationBuilder.CreateIndex(
                name: "IX_LessonNotes_SubjectId1",
                table: "LessonNotes",
                column: "SubjectId1");

            migrationBuilder.CreateIndex(
                name: "IX_LessonNotes_TeacherId",
                table: "LessonNotes",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonPeriods_LessonNotesId1",
                table: "LessonPeriods",
                column: "LessonNotesId1");

            migrationBuilder.CreateIndex(
                name: "IX_LessonPeriodTemplates_TemplateNoteId",
                table: "LessonPeriodTemplates",
                column: "TemplateNoteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomField");

            migrationBuilder.DropTable(
                name: "LessonPeriodTemplates");

            migrationBuilder.DropTable(
                name: "LessonPeriods");

            migrationBuilder.DropTable(
                name: "LessonNoteTemplates");

            migrationBuilder.DropTable(
                name: "LessonNotes");

            migrationBuilder.DropColumn(
                name: "CurrentClasses",
                table: "TeachingStaffs");
        }
    }
}
