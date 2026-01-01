using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadrasahManagement.Migrations
{
    /// <inheritdoc />
    public partial class examexpenseup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "ExamIncomeExpenses",
                newName: "TypesOfExpense");

            migrationBuilder.CreateIndex(
                name: "IX_ExamIncomeExpenses_ExamId",
                table: "ExamIncomeExpenses",
                column: "ExamId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamIncomeExpenses_Examinations_ExamId",
                table: "ExamIncomeExpenses",
                column: "ExamId",
                principalTable: "Examinations",
                principalColumn: "ExamId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamIncomeExpenses_Examinations_ExamId",
                table: "ExamIncomeExpenses");

            migrationBuilder.DropIndex(
                name: "IX_ExamIncomeExpenses_ExamId",
                table: "ExamIncomeExpenses");

            migrationBuilder.RenameColumn(
                name: "TypesOfExpense",
                table: "ExamIncomeExpenses",
                newName: "Type");
        }
    }
}
