using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AnimalMarketplace.Migrations
{
    /// <inheritdoc />
    public partial class initial_migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "varchar(50)", nullable: false),
                    surname = table.Column<string>(type: "varchar(50)", nullable: false),
                    email = table.Column<string>(type: "varchar(100)", nullable: false),
                    register_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_DATE"),
                    phone = table.Column<string>(type: "varchar(20)", nullable: false),
                    gender = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    is_admin = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    password_hash = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sellers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    business_name = table.Column<string>(type: "varchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sellers", x => x.id);
                    table.ForeignKey(
                        name: "FK_sellers_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "adverts",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_DATE"),
                    state = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false, defaultValue: "Active"),
                    price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    description = table.Column<string>(type: "varchar(500)", nullable: false),
                    seller_id = table.Column<int>(type: "integer", nullable: false),
                    city = table.Column<string>(type: "varchar(50)", nullable: false),
                    district = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adverts", x => x.id);
                    table.CheckConstraint("ck_advert_price_valid", "price >= 0 ");
                    table.ForeignKey(
                        name: "FK_adverts_sellers_seller_id",
                        column: x => x.seller_id,
                        principalTable: "sellers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "advert_images",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    url = table.Column<string>(type: "varchar(500)", nullable: false),
                    public_id = table.Column<string>(type: "varchar(200)", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    uploaded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    advert_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_advert_images", x => x.id);
                    table.ForeignKey(
                        name: "FK_advert_images_adverts_advert_id",
                        column: x => x.advert_id,
                        principalTable: "adverts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "animals",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    gender = table.Column<string>(type: "varchar(6)", nullable: false),
                    age = table.Column<int>(type: "integer", nullable: false),
                    kind = table.Column<string>(type: "varchar(10)", nullable: false),
                    advert_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_animals", x => x.id);
                    table.CheckConstraint("ck_age_valid", "age between 0 and 100");
                    table.ForeignKey(
                        name: "FK_animals_adverts_advert_id",
                        column: x => x.advert_id,
                        principalTable: "adverts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "favorites",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    advert_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_favorites", x => new { x.user_id, x.advert_id });
                    table.ForeignKey(
                        name: "FK_favorites_adverts_advert_id",
                        column: x => x.advert_id,
                        principalTable: "adverts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_favorites_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_advert_images_advert_id",
                table: "advert_images",
                column: "advert_id");

            migrationBuilder.CreateIndex(
                name: "IX_adverts_seller_id",
                table: "adverts",
                column: "seller_id");

            migrationBuilder.CreateIndex(
                name: "IX_animals_advert_id",
                table: "animals",
                column: "advert_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_favorites_advert_id",
                table: "favorites",
                column: "advert_id");

            migrationBuilder.CreateIndex(
                name: "IX_sellers_user_id",
                table: "sellers",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "advert_images");

            migrationBuilder.DropTable(
                name: "animals");

            migrationBuilder.DropTable(
                name: "favorites");

            migrationBuilder.DropTable(
                name: "adverts");

            migrationBuilder.DropTable(
                name: "sellers");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
