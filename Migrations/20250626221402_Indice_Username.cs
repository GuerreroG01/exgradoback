using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExGradoBack.Migrations
{
    /// <inheritdoc />
    public partial class Indice_Username : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Auth_Username",
                table: "Auth",
                column: "Username");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Auth_Username",
                table: "Auth");
        }
    }
}
