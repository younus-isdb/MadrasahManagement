using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadrasahManagement.Migrations
{
    /// <inheritdoc />
    public partial class examfeemcrud : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamFeeCollections_Classes_ClassId",
                table: "ExamFeeCollections");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamFeeCollections_ExamFees_ExamFeeId",
                table: "ExamFeeCollections");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamFeeCollections_Examinations_ExamId",
                table: "ExamFeeCollections");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamFees_Examinations_ExamId",
                table: "ExamFees");

            migrationBuilder.DropIndex(
                name: "IX_ExamFees_ExamId",
                table: "ExamFees");

            migrationBuilder.DropIndex(
                name: "IX_ExamFeeCollections_ExamId",
                table: "ExamFeeCollections");

            migrationBuilder.DropColumn(
                name: "EducationYear",
                table: "ExamFeeCollections");

            migrationBuilder.DropColumn(
                name: "ExamFee",
                table: "ExamFeeCollections");

            migrationBuilder.DropColumn(
                name: "ExamId",
                table: "ExamFeeCollections");

            migrationBuilder.AlterColumn<string>(
                name: "EducationYear",
                table: "ExamFees",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AddColumn<int>(
                name: "ExaminationExamId",
                table: "ExamFees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "TotalSubject",
                table: "ExamFeeCollections",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "ExamFeeId",
                table: "ExamFeeCollections",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ClassId",
                table: "ExamFeeCollections",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<decimal>(
                name: "ExamFeeAmount",
                table: "ExamFeeCollections",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_ExamFees_ExaminationExamId",
                table: "ExamFees",
                column: "ExaminationExamId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamFeeCollections_Classes_ClassId",
                table: "ExamFeeCollections",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamFeeCollections_ExamFees_ExamFeeId",
                table: "ExamFeeCollections",
                column: "ExamFeeId",
                principalTable: "ExamFees",
                principalColumn: "ExamFeeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamFees_Examinations_ExaminationExamId",
                table: "ExamFees",
                column: "ExaminationExamId",
                principalTable: "Examinations",
                principalColumn: "ExamId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamFeeCollections_Classes_ClassId",
                table: "ExamFeeCollections");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamFeeCollections_ExamFees_ExamFeeId",
                table: "ExamFeeCollections");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamFees_Examinations_ExaminationExamId",
                table: "ExamFees");

            migrationBuilder.DropIndex(
                name: "IX_ExamFees_ExaminationExamId",
                table: "ExamFees");

            migrationBuilder.DropColumn(
                name: "ExaminationExamId",
                table: "ExamFees");

            migrationBuilder.DropColumn(
                name: "ExamFeeAmount",
                table: "ExamFeeCollections");

            migrationBuilder.AlterColumn<string>(
                name: "EducationYear",
                table: "ExamFees",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "TotalSubject",
                table: "ExamFeeCollections",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ExamFeeId",
                table: "ExamFeeCollections",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ClassId",
                table: "ExamFeeCollections",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EducationYear",
                table: "ExamFeeCollections",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ExamFee",
                table: "ExamFeeCollections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExamId",
                table: "ExamFeeCollections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ExamFees_ExamId",
                table: "ExamFees",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamFeeCollections_ExamId",
                table: "ExamFeeCollections",
                column: "ExamId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamFeeCollections_Classes_ClassId",
                table: "ExamFeeCollections",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "ClassId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamFeeCollections_ExamFees_ExamFeeId",
                table: "ExamFeeCollections",
                column: "ExamFeeId",
                principalTable: "ExamFees",
                principalColumn: "ExamFeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamFeeCollections_Examinations_ExamId",
                table: "ExamFeeCollections",
                column: "ExamId",
                principalTable: "Examinations",
                principalColumn: "ExamId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamFees_Examinations_ExamId",
                table: "ExamFees",
                column: "ExamId",
                principalTable: "Examinations",
                principalColumn: "ExamId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
