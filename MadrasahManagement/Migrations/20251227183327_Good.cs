using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadrasahManagement.Migrations
{
    /// <inheritdoc />
    public partial class Good : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SalaryId",
                table: "Expenses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_SalaryId",
                table: "Expenses",
                column: "SalaryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Salaries_SalaryId",
                table: "Expenses",
                column: "SalaryId",
                principalTable: "Salaries",
                principalColumn: "SalaryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Salaries_SalaryId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_SalaryId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "SalaryId",
                table: "Expenses");
        }
    }
}
