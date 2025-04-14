using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Db.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "suprusr");

            migrationBuilder.EnsureSchema(
                name: "usr");

            migrationBuilder.CreateTable(
                name: "Emails",
                schema: "suprusr",
                columns: table => new
                {
                    EmailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    Seeded = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emails", x => x.EmailId);
                });

            migrationBuilder.CreateTable(
                name: "Phones",
                schema: "suprusr",
                columns: table => new
                {
                    PhoneId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Seeded = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phones", x => x.PhoneId);
                });

            migrationBuilder.CreateTable(
                name: "Subscribers",
                schema: "suprusr",
                columns: table => new
                {
                    SubscriberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MicrosoftGraphOwnerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Seeded = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscribers", x => x.SubscriberId);
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                schema: "suprusr",
                columns: table => new
                {
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MicrosoftGraphApiAppId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Seeded = table.Column<bool>(type: "bit", nullable: false),
                    SubscriberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.ApplicationId);
                    table.ForeignKey(
                        name: "FK_Applications_Subscribers_SubscriberId",
                        column: x => x.SubscriberId,
                        principalSchema: "suprusr",
                        principalTable: "Subscribers",
                        principalColumn: "SubscriberId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Secrets",
                schema: "usr",
                columns: table => new
                {
                    SecretId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MicrosoftGraphApiKeyId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EndDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastTimeNotified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ContactByEmail = table.Column<bool>(type: "bit", nullable: false),
                    ContactBySMS = table.Column<bool>(type: "bit", nullable: false),
                    ContactByApiEndpoint = table.Column<bool>(type: "bit", nullable: false),
                    Seeded = table.Column<bool>(type: "bit", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubscriberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Secrets", x => x.SecretId);
                    table.ForeignKey(
                        name: "FK_Secrets_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "suprusr",
                        principalTable: "Applications",
                        principalColumn: "ApplicationId");
                    table.ForeignKey(
                        name: "FK_Secrets_Subscribers_SubscriberId",
                        column: x => x.SubscriberId,
                        principalSchema: "suprusr",
                        principalTable: "Subscribers",
                        principalColumn: "SubscriberId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiEndpoints",
                schema: "suprusr",
                columns: table => new
                {
                    ApiEndpointId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BaseUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QueryParametersJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeadersJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HttpMethod = table.Column<int>(type: "int", nullable: false),
                    StrHttpMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BodyTemplate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Seeded = table.Column<bool>(type: "bit", nullable: false),
                    SecretId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiEndpoints", x => x.ApiEndpointId);
                    table.ForeignKey(
                        name: "FK_ApiEndpoints_Secrets_SecretId",
                        column: x => x.SecretId,
                        principalSchema: "usr",
                        principalTable: "Secrets",
                        principalColumn: "SecretId");
                });

            migrationBuilder.CreateTable(
                name: "EmailSecret",
                schema: "suprusr",
                columns: table => new
                {
                    EmailsEmailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SecretsSecretId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailSecret", x => new { x.EmailsEmailId, x.SecretsSecretId });
                    table.ForeignKey(
                        name: "FK_EmailSecret_Emails_EmailsEmailId",
                        column: x => x.EmailsEmailId,
                        principalSchema: "suprusr",
                        principalTable: "Emails",
                        principalColumn: "EmailId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmailSecret_Secrets_SecretsSecretId",
                        column: x => x.SecretsSecretId,
                        principalSchema: "usr",
                        principalTable: "Secrets",
                        principalColumn: "SecretId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhoneSecret",
                schema: "suprusr",
                columns: table => new
                {
                    PhonesPhoneId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SecretsSecretId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneSecret", x => new { x.PhonesPhoneId, x.SecretsSecretId });
                    table.ForeignKey(
                        name: "FK_PhoneSecret_Phones_PhonesPhoneId",
                        column: x => x.PhonesPhoneId,
                        principalSchema: "suprusr",
                        principalTable: "Phones",
                        principalColumn: "PhoneId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhoneSecret_Secrets_SecretsSecretId",
                        column: x => x.SecretsSecretId,
                        principalSchema: "usr",
                        principalTable: "Secrets",
                        principalColumn: "SecretId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiEndpoints_SecretId",
                schema: "suprusr",
                table: "ApiEndpoints",
                column: "SecretId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_SubscriberId",
                schema: "suprusr",
                table: "Applications",
                column: "SubscriberId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailSecret_SecretsSecretId",
                schema: "suprusr",
                table: "EmailSecret",
                column: "SecretsSecretId");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneSecret_SecretsSecretId",
                schema: "suprusr",
                table: "PhoneSecret",
                column: "SecretsSecretId");

            migrationBuilder.CreateIndex(
                name: "IX_Secrets_ApplicationId",
                schema: "usr",
                table: "Secrets",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Secrets_SubscriberId",
                schema: "usr",
                table: "Secrets",
                column: "SubscriberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiEndpoints",
                schema: "suprusr");

            migrationBuilder.DropTable(
                name: "EmailSecret",
                schema: "suprusr");

            migrationBuilder.DropTable(
                name: "PhoneSecret",
                schema: "suprusr");

            migrationBuilder.DropTable(
                name: "Emails",
                schema: "suprusr");

            migrationBuilder.DropTable(
                name: "Phones",
                schema: "suprusr");

            migrationBuilder.DropTable(
                name: "Secrets",
                schema: "usr");

            migrationBuilder.DropTable(
                name: "Applications",
                schema: "suprusr");

            migrationBuilder.DropTable(
                name: "Subscribers",
                schema: "suprusr");
        }
    }
}
