using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PataFacil.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaSenhaTutor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SenhaHash",
                table: "Tutores",
                type: "longtext",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SenhaHash",
                table: "Tutores");
        }
    }
}
