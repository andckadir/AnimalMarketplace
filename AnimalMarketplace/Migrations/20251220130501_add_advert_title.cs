using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnimalMarketplace.Migrations
{
    /// <inheritdoc />
    public partial class add_advert_title : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "title",
                table: "adverts",
                type: "varchar(50)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "title",
                table: "adverts");
        }
    }
}
