using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    public partial class ghgkh : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Grades_GradeId",
                table: "Submissions");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Grades_GradeId",
                table: "Submissions",
                column: "GradeId",
                principalTable: "Grades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Grades_GradeId",
                table: "Submissions");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Grades_GradeId",
                table: "Submissions",
                column: "GradeId",
                principalTable: "Grades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
