using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mynewapp.Migrations
{
    /// <inheritdoc />
    public partial class Notification2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Addressed",
                table: "notifications",
                newName: "Sender");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Sender",
                table: "notifications",
                newName: "Addressed");
        }
    }
}
