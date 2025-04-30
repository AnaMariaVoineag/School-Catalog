using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseAndGrades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseStudent_Courses_EnrolledCoursesCourseId",
                table: "CourseStudent");

            migrationBuilder.RenameColumn(
                name: "EnrolledCoursesCourseId",
                table: "CourseStudent",
                newName: "EnrolledCoursesId");

            migrationBuilder.RenameColumn(
                name: "CourseName",
                table: "Courses",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "Courses",
                newName: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseStudent_Courses_EnrolledCoursesId",
                table: "CourseStudent",
                column: "EnrolledCoursesId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseStudent_Courses_EnrolledCoursesId",
                table: "CourseStudent");

            migrationBuilder.RenameColumn(
                name: "EnrolledCoursesId",
                table: "CourseStudent",
                newName: "EnrolledCoursesCourseId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Courses",
                newName: "CourseName");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Courses",
                newName: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseStudent_Courses_EnrolledCoursesCourseId",
                table: "CourseStudent",
                column: "EnrolledCoursesCourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
