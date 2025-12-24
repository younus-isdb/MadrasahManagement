using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadrasahManagement.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PointConditions_Subjects_SubjectId",
                table: "PointConditions");

            migrationBuilder.DropIndex(
                name: "IX_PointConditions_SubjectId",
                table: "PointConditions");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "PointConditions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "PointConditions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PointConditions_SubjectId",
                table: "PointConditions",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_PointConditions_Subjects_SubjectId",
                table: "PointConditions",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "SubjectId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
