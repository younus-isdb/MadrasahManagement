using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadrasahManagement.Migrations
{
    /// <inheritdoc />
    public partial class SaifUllah : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamFeeCollections_Subjects_SubjectId",
                table: "ExamFeeCollections");

            migrationBuilder.AlterColumn<int>(
                name: "SubjectId",
                table: "ExamFeeCollections",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamFeeCollections_Subjects_SubjectId",
                table: "ExamFeeCollections",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "SubjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamFeeCollections_Subjects_SubjectId",
                table: "ExamFeeCollections");

            migrationBuilder.AlterColumn<int>(
                name: "SubjectId",
                table: "ExamFeeCollections",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamFeeCollections_Subjects_SubjectId",
                table: "ExamFeeCollections",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "SubjectId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
