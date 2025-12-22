using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadrasahManagement.Migrations
{
    /// <inheritdoc />
    public partial class Math : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_IssuedBooks_BookId",
                table: "IssuedBooks");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "ReturnDate",
                table: "IssuedBooks",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "IssueDate",
                table: "IssuedBooks",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.CreateIndex(
                name: "IX_IssuedBooks_BookId_IssuedTo",
                table: "IssuedBooks",
                columns: new[] { "BookId", "IssuedTo" },
                unique: true,
                filter: "[ReturnDate] IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_IssuedBooks_BookId_IssuedTo",
                table: "IssuedBooks");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "ReturnDate",
                table: "IssuedBooks",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "IssueDate",
                table: "IssuedBooks",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.CreateIndex(
                name: "IX_IssuedBooks_BookId",
                table: "IssuedBooks",
                column: "BookId");
        }
    }
}
