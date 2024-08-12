using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SchoolTerms_Name",
                table: "SchoolTerms");

            migrationBuilder.DropIndex(
                name: "IX_SchoolSessions_Name",
                table: "SchoolSessions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SchoolTerms_Name",
                table: "SchoolTerms",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchoolSessions_Name",
                table: "SchoolSessions",
                column: "Name",
                unique: true);
        }
    }
}
