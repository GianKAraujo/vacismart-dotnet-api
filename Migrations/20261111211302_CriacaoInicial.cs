using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaciSmart.Api.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VS_UNIDADE_SAUDE",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    MUNICIPIO = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: false),
                    UF = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VS_UNIDADE_SAUDE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "VS_TRANSPORTE",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    CODIGO = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    STATUS = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    CRIADO_EM = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    RECEBIDO_EM = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    UNIDADE_DESTINO_ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VS_TRANSPORTE", x => x.ID);
                    table.ForeignKey(
                        name: "FK_VS_TRANSPORTE_UNIDADE",
                        column: x => x.UNIDADE_DESTINO_ID,
                        principalTable: "VS_UNIDADE_SAUDE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VS_TRANSPORTE_UNIDADE",
                table: "VS_TRANSPORTE",
                column: "UNIDADE_DESTINO_ID");

            migrationBuilder.CreateIndex(
                name: "UK_VS_TRANSPORTE_CODIGO",
                table: "VS_TRANSPORTE",
                column: "CODIGO",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VS_TRANSPORTE");

            migrationBuilder.DropTable(
                name: "VS_UNIDADE_SAUDE");
        }
    }
}
