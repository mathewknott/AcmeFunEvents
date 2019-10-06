using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AcmeFunEvents.Web.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "acme_activity",
                columns: table => new
                {
                    created_utc = table.Column<DateTime>(nullable: false),
                    modified_utc = table.Column<DateTime>(nullable: false),
                    status = table.Column<int>(nullable: false),
                    id = table.Column<Guid>(nullable: false),
                    code = table.Column<int>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    date = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_acme_activity", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "acme_user",
                columns: table => new
                {
                    created_utc = table.Column<DateTime>(nullable: false),
                    modified_utc = table.Column<DateTime>(nullable: false),
                    status = table.Column<int>(nullable: false),
                    id = table.Column<Guid>(nullable: false),
                    first_name = table.Column<string>(nullable: true),
                    last_name = table.Column<string>(nullable: true),
                    phone_number = table.Column<string>(nullable: true),
                    email_address = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_acme_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "acme_registration",
                columns: table => new
                {
                    created_utc = table.Column<DateTime>(nullable: false),
                    modified_utc = table.Column<DateTime>(nullable: false),
                    status = table.Column<int>(nullable: false),
                    id = table.Column<Guid>(nullable: false),
                    registration_number = table.Column<int>(nullable: false),
                    comments = table.Column<string>(nullable: true),
                    user_id = table.Column<Guid>(nullable: true),
                    activity_id = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_acme_registration", x => x.id);
                    table.ForeignKey(
                        name: "fk_acme_registration_acme_activity_activity_id",
                        column: x => x.activity_id,
                        principalTable: "acme_activity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_acme_registration_acme_user_user_id",
                        column: x => x.user_id,
                        principalTable: "acme_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_acme_activity_code",
                table: "acme_activity",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_acme_registration_activity_id",
                table: "acme_registration",
                column: "activity_id");

            migrationBuilder.CreateIndex(
                name: "ix_acme_registration_registration_number",
                table: "acme_registration",
                column: "registration_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_acme_registration_user_id",
                table: "acme_registration",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_acme_user_email_address",
                table: "acme_user",
                column: "email_address",
                unique: true,
                filter: "[email_address] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "acme_registration");

            migrationBuilder.DropTable(
                name: "acme_activity");

            migrationBuilder.DropTable(
                name: "acme_user");
        }
    }
}
