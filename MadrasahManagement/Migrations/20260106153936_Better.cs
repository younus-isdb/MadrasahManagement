using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadrasahManagement.Migrations
{
    /// <inheritdoc />
    public partial class Better : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Timetables_Classes_ClassId1",
                table: "Timetables");

            migrationBuilder.DropForeignKey(
                name: "FK_Timetables_Sections_SectionId1",
                table: "Timetables");

            migrationBuilder.DropForeignKey(
                name: "FK_Timetables_Teachers_TeacherId1",
                table: "Timetables");

            migrationBuilder.DropIndex(
                name: "IX_Timetables_ClassId1",
                table: "Timetables");

            migrationBuilder.DropIndex(
                name: "IX_Timetables_SectionId1",
                table: "Timetables");

            migrationBuilder.DropIndex(
                name: "IX_Timetables_TeacherId1",
                table: "Timetables");

            migrationBuilder.DropColumn(
                name: "ClassId1",
                table: "Timetables");

            migrationBuilder.DropColumn(
                name: "SectionId1",
                table: "Timetables");

            migrationBuilder.DropColumn(
                name: "TeacherId1",
                table: "Timetables");

            migrationBuilder.AlterColumn<int>(
                name: "TeacherId",
                table: "Timetables",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "SubjectId",
                table: "Timetables",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "PeriodName",
                table: "Timetables",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DayName",
                table: "Timetables",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AcademicYear",
                table: "Timetables",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Timetables_AcademicYear_DepartmentId_ClassId_SectionId_DayName_PeriodName",
                table: "Timetables",
                columns: new[] { "AcademicYear", "DepartmentId", "ClassId", "SectionId", "DayName", "PeriodName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Timetables_AcademicYear_DepartmentId_ClassId_SectionId_DayName_PeriodName",
                table: "Timetables");

            migrationBuilder.AlterColumn<int>(
                name: "TeacherId",
                table: "Timetables",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SubjectId",
                table: "Timetables",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PeriodName",
                table: "Timetables",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "DayName",
                table: "Timetables",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "AcademicYear",
                table: "Timetables",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<int>(
                name: "ClassId1",
                table: "Timetables",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SectionId1",
                table: "Timetables",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TeacherId1",
                table: "Timetables",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Timetables_ClassId1",
                table: "Timetables",
                column: "ClassId1");

            migrationBuilder.CreateIndex(
                name: "IX_Timetables_SectionId1",
                table: "Timetables",
                column: "SectionId1");

            migrationBuilder.CreateIndex(
                name: "IX_Timetables_TeacherId1",
                table: "Timetables",
                column: "TeacherId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Timetables_Classes_ClassId1",
                table: "Timetables",
                column: "ClassId1",
                principalTable: "Classes",
                principalColumn: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Timetables_Sections_SectionId1",
                table: "Timetables",
                column: "SectionId1",
                principalTable: "Sections",
                principalColumn: "SectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Timetables_Teachers_TeacherId1",
                table: "Timetables",
                column: "TeacherId1",
                principalTable: "Teachers",
                principalColumn: "TeacherId");
        }
    }
}
