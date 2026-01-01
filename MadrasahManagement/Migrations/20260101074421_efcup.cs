using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadrasahManagement.Migrations
{
    /// <inheritdoc />
    public partial class efcup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamFees_Examinations_ExaminationExamId",
                table: "ExamFees");

            migrationBuilder.DropIndex(
                name: "IX_ExamFees_ExaminationExamId",
                table: "ExamFees");

            migrationBuilder.DropColumn(
                name: "ExaminationExamId",
                table: "ExamFees");

            migrationBuilder.CreateIndex(
                name: "IX_ExamFees_ExamId",
                table: "ExamFees",
                column: "ExamId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamFees_Examinations_ExamId",
                table: "ExamFees",
                column: "ExamId",
                principalTable: "Examinations",
                principalColumn: "ExamId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamFees_Examinations_ExamId",
                table: "ExamFees");

            migrationBuilder.DropIndex(
                name: "IX_ExamFees_ExamId",
                table: "ExamFees");

            migrationBuilder.AddColumn<int>(
                name: "ExaminationExamId",
                table: "ExamFees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ExamFees_ExaminationExamId",
                table: "ExamFees",
                column: "ExaminationExamId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamFees_Examinations_ExaminationExamId",
                table: "ExamFees",
                column: "ExaminationExamId",
                principalTable: "Examinations",
                principalColumn: "ExamId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
