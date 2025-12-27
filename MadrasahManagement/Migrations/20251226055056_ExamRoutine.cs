using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadrasahManagement.Migrations
{
    /// <inheritdoc />
    public partial class ExamRoutine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "ExamRoutines",
                newName: "ExamStartTime");

            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "PointConditions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "ExamRoutines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EducationYear",
                table: "ExamRoutines",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExamDay",
                table: "ExamRoutines",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExamEndTime",
                table: "ExamRoutines",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RoomNumber",
                table: "ExamRoutines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "ExamRoutines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "ExamFeeCollections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PointConditions_SubjectId",
                table: "PointConditions",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamRoutines_ClassId",
                table: "ExamRoutines",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamRoutines_ExamId",
                table: "ExamRoutines",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamRoutines_SubjectId",
                table: "ExamRoutines",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamFeeCollections_SubjectId",
                table: "ExamFeeCollections",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamFeeCollections_Subjects_SubjectId",
                table: "ExamFeeCollections",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "SubjectId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamRoutines_Classes_ClassId",
                table: "ExamRoutines",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "ClassId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamRoutines_Examinations_ExamId",
                table: "ExamRoutines",
                column: "ExamId",
                principalTable: "Examinations",
                principalColumn: "ExamId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamRoutines_Subjects_SubjectId",
                table: "ExamRoutines",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "SubjectId",
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
                name: "FK_ExamFeeCollections_Subjects_SubjectId",
                table: "ExamFeeCollections");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamRoutines_Classes_ClassId",
                table: "ExamRoutines");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamRoutines_Examinations_ExamId",
                table: "ExamRoutines");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamRoutines_Subjects_SubjectId",
                table: "ExamRoutines");

            migrationBuilder.DropForeignKey(
                name: "FK_PointConditions_Subjects_SubjectId",
                table: "PointConditions");

            migrationBuilder.DropIndex(
                name: "IX_PointConditions_SubjectId",
                table: "PointConditions");

            migrationBuilder.DropIndex(
                name: "IX_ExamRoutines_ClassId",
                table: "ExamRoutines");

            migrationBuilder.DropIndex(
                name: "IX_ExamRoutines_ExamId",
                table: "ExamRoutines");

            migrationBuilder.DropIndex(
                name: "IX_ExamRoutines_SubjectId",
                table: "ExamRoutines");

            migrationBuilder.DropIndex(
                name: "IX_ExamFeeCollections_SubjectId",
                table: "ExamFeeCollections");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "PointConditions");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "ExamRoutines");

            migrationBuilder.DropColumn(
                name: "EducationYear",
                table: "ExamRoutines");

            migrationBuilder.DropColumn(
                name: "ExamDay",
                table: "ExamRoutines");

            migrationBuilder.DropColumn(
                name: "ExamEndTime",
                table: "ExamRoutines");

            migrationBuilder.DropColumn(
                name: "RoomNumber",
                table: "ExamRoutines");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "ExamRoutines");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "ExamFeeCollections");

            migrationBuilder.RenameColumn(
                name: "ExamStartTime",
                table: "ExamRoutines",
                newName: "Subject");
        }
    }
}
