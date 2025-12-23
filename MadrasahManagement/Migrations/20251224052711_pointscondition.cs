using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadrasahManagement.Migrations
{
    /// <inheritdoc />
    public partial class pointscondition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Division",
                table: "PointConditions");

            migrationBuilder.DropColumn(
                name: "Grade",
                table: "PointConditions");

            migrationBuilder.DropColumn(
                name: "isRedColor",
                table: "PointConditions");

            migrationBuilder.RenameColumn(
                name: "ConditonMark",
                table: "PointConditions",
                newName: "HighestMark");

            migrationBuilder.CreateTable(
                name: "PointConditionDetails",
                columns: table => new
                {
                    PointConditionDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PointConditionId = table.Column<int>(type: "int", nullable: false),
                    FromMark = table.Column<int>(type: "int", nullable: false),
                    ToMark = table.Column<int>(type: "int", nullable: false),
                    Division = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSilverColor = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointConditionDetails", x => x.PointConditionDetailId);
                    table.ForeignKey(
                        name: "FK_PointConditionDetails_PointConditions_PointConditionId",
                        column: x => x.PointConditionId,
                        principalTable: "PointConditions",
                        principalColumn: "PointConditionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PointConditionDetails_PointConditionId",
                table: "PointConditionDetails",
                column: "PointConditionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PointConditionDetails");

            migrationBuilder.RenameColumn(
                name: "HighestMark",
                table: "PointConditions",
                newName: "ConditonMark");

            migrationBuilder.AddColumn<string>(
                name: "Division",
                table: "PointConditions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Grade",
                table: "PointConditions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isRedColor",
                table: "PointConditions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
