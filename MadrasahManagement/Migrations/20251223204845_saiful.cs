using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadrasahManagement.Migrations
{
    /// <inheritdoc />
    public partial class saiful : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ObtainedMarks",
                table: "PointConditions",
                newName: "SubjectId");

            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "PointConditions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ConditonMark",
                table: "PointConditions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Division",
                table: "PointConditions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EducationYear",
                table: "PointConditions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ExamId",
                table: "PointConditions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PassMarks",
                table: "PointConditions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "isRedColor",
                table: "PointConditions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_PointConditions_ClassId",
                table: "PointConditions",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_PointConditions_ExamId",
                table: "PointConditions",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_PointConditions_SubjectId",
                table: "PointConditions",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_PointConditions_Classes_ClassId",
                table: "PointConditions",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "ClassId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PointConditions_Examinations_ExamId",
                table: "PointConditions",
                column: "ExamId",
                principalTable: "Examinations",
                principalColumn: "ExamId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PointConditions_Subjects_SubjectId",
                table: "PointConditions",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "SubjectId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PointConditions_Classes_ClassId",
                table: "PointConditions");

            migrationBuilder.DropForeignKey(
                name: "FK_PointConditions_Examinations_ExamId",
                table: "PointConditions");

            migrationBuilder.DropForeignKey(
                name: "FK_PointConditions_Subjects_SubjectId",
                table: "PointConditions");

            migrationBuilder.DropIndex(
                name: "IX_PointConditions_ClassId",
                table: "PointConditions");

            migrationBuilder.DropIndex(
                name: "IX_PointConditions_ExamId",
                table: "PointConditions");

            migrationBuilder.DropIndex(
                name: "IX_PointConditions_SubjectId",
                table: "PointConditions");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "PointConditions");

            migrationBuilder.DropColumn(
                name: "ConditonMark",
                table: "PointConditions");

            migrationBuilder.DropColumn(
                name: "Division",
                table: "PointConditions");

            migrationBuilder.DropColumn(
                name: "EducationYear",
                table: "PointConditions");

            migrationBuilder.DropColumn(
                name: "ExamId",
                table: "PointConditions");

            migrationBuilder.DropColumn(
                name: "PassMarks",
                table: "PointConditions");

            migrationBuilder.DropColumn(
                name: "isRedColor",
                table: "PointConditions");

            migrationBuilder.RenameColumn(
                name: "SubjectId",
                table: "PointConditions",
                newName: "ObtainedMarks");
        }
    }
}
