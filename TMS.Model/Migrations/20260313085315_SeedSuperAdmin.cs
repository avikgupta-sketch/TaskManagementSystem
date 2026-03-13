using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TMS.Model.Migrations
{
    /// <inheritdoc />
    public partial class SeedSuperAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "IsDeleted", "PasswordHash", "Role", "Username" },
                values: new object[] { 999999, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "gaurav@123", false, "$2a$11$Itq6CVnSGTQUia673H3D/uskekL97BqtnUgzqHahBqZ2cv66N9Uy.", "SuperAdmin", "Gaurav" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
