using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExGradoBack.Migrations
{
    /// <inheritdoc />
    public partial class Calificacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Detalle",
                table: "MarcaRepuesto");

            migrationBuilder.AddColumn<double>(
                name: "Calificacion",
                table: "MarcaRepuesto",
                type: "double",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Auth",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2025, 8, 13, 15, 44, 42, 763, DateTimeKind.Local).AddTicks(3324));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Calificacion",
                table: "MarcaRepuesto");

            migrationBuilder.AddColumn<string>(
                name: "Detalle",
                table: "MarcaRepuesto",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Auth",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2025, 8, 6, 12, 8, 55, 279, DateTimeKind.Local).AddTicks(7806));
        }
    }
}
