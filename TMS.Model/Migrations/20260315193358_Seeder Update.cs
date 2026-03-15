using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TMS.Model.Migrations
{
    /// <inheritdoc />
    public partial class SeederUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$a70d5ImpgyJvTfKnBBqCCOOZRbe.l0eFku4fO8nUwDoaiuypMElnu");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$plq.dLqoWMAx7hzoIvK0G.uCKp5tUOc/vTCzjajjLIC0Grd2VmM8W");
        }
    }
}
