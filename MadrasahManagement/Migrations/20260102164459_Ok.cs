using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadrasahManagement.Migrations
{
    /// <inheritdoc />
    public partial class Ok : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "FeeTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FeeTypes_DepartmentId",
                table: "FeeTypes",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_FeeTypes_Departments_DepartmentId",
                table: "FeeTypes",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeeTypes_Departments_DepartmentId",
                table: "FeeTypes");

            migrationBuilder.DropIndex(
                name: "IX_FeeTypes_DepartmentId",
                table: "FeeTypes");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "FeeTypes");
        }
    }
}
