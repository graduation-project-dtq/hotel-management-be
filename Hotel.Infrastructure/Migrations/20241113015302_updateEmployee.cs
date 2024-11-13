using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hotel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Accounts_AccountID",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_AccountID",
                table: "Employee");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "HireDate",
                table: "Employee",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DateOfBirth",
                table: "Employee",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "AccountID",
                table: "Employee",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_AccountID",
                table: "Employee",
                column: "AccountID",
                unique: true,
                filter: "[AccountID] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Accounts_AccountID",
                table: "Employee",
                column: "AccountID",
                principalTable: "Accounts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Accounts_AccountID",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_AccountID",
                table: "Employee");

            migrationBuilder.AlterColumn<DateTime>(
                name: "HireDate",
                table: "Employee",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "Employee",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<string>(
                name: "AccountID",
                table: "Employee",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_AccountID",
                table: "Employee",
                column: "AccountID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Accounts_AccountID",
                table: "Employee",
                column: "AccountID",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
