using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassAttendances_Students_StudentId1",
                table: "ClassAttendances");

            migrationBuilder.DropForeignKey(
                name: "FK_Schools_AspNetUsers_AdminId",
                table: "Schools");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentsCARecords_SchoolSessions_SchoolSessionId1",
                table: "StudentsCARecords");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentsScores_Students_StudentId1",
                table: "StudentsScores");

            migrationBuilder.AlterColumn<string>(
                name: "State",
                table: "Zones",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<List<string>>(
                name: "LocalGovtAreas",
                table: "Zones",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(List<string>),
                oldType: "text[]",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "StudentsCARecordTestId",
                table: "StudentsScores",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "StudentId1",
                table: "StudentsScores",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "SchoolSessionId1",
                table: "StudentsCARecords",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "SchoolSessionId",
                table: "SchoolTerms",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AdminId",
                table: "Schools",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "StudentId1",
                table: "ClassAttendances",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassAttendances_Students_StudentId1",
                table: "ClassAttendances",
                column: "StudentId1",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Schools_AspNetUsers_AdminId",
                table: "Schools",
                column: "AdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentsCARecords_SchoolSessions_SchoolSessionId1",
                table: "StudentsCARecords",
                column: "SchoolSessionId1",
                principalTable: "SchoolSessions",
                principalColumn: "SchoolSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentsScores_Students_StudentId1",
                table: "StudentsScores",
                column: "StudentId1",
                principalTable: "Students",
                principalColumn: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassAttendances_Students_StudentId1",
                table: "ClassAttendances");

            migrationBuilder.DropForeignKey(
                name: "FK_Schools_AspNetUsers_AdminId",
                table: "Schools");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentsCARecords_SchoolSessions_SchoolSessionId1",
                table: "StudentsCARecords");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentsScores_Students_StudentId1",
                table: "StudentsScores");

            migrationBuilder.AlterColumn<string>(
                name: "State",
                table: "Zones",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<List<string>>(
                name: "LocalGovtAreas",
                table: "Zones",
                type: "text[]",
                nullable: true,
                oldClrType: typeof(List<string>),
                oldType: "text[]");

            migrationBuilder.AlterColumn<Guid>(
                name: "StudentsCARecordTestId",
                table: "StudentsScores",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "StudentId1",
                table: "StudentsScores",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SchoolSessionId1",
                table: "StudentsCARecords",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SchoolSessionId",
                table: "SchoolTerms",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "AdminId",
                table: "Schools",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "StudentId1",
                table: "ClassAttendances",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassAttendances_Students_StudentId1",
                table: "ClassAttendances",
                column: "StudentId1",
                principalTable: "Students",
                principalColumn: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schools_AspNetUsers_AdminId",
                table: "Schools",
                column: "AdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentsCARecords_SchoolSessions_SchoolSessionId1",
                table: "StudentsCARecords",
                column: "SchoolSessionId1",
                principalTable: "SchoolSessions",
                principalColumn: "SchoolSessionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentsScores_Students_StudentId1",
                table: "StudentsScores",
                column: "StudentId1",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
