using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaciSmart.Api.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaObservacaoTransporte : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OBSERVACAO",
                table: "VS_TRANSPORTE",
                type: "NVARCHAR2(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OBSERVACAO",
                table: "VS_TRANSPORTE");
        }
    }
}
