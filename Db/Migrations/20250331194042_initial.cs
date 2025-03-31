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
            migrationBuilder.CreateTable(
                name: "Emails",
                columns: table => new
                {
                    EmailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emails", x => x.EmailId);
                });

            migrationBuilder.CreateTable(
                name: "Phones",
                columns: table => new
                {
                    PhoneId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phones", x => x.PhoneId);
                });

            migrationBuilder.CreateTable(
                name: "Subscribers",
                columns: table => new
                {
                    SubscriberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MicrosoftGraphOwnerId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscribers", x => x.SubscriberId);
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MicrosoftGraphApiAppId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubscriberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.ApplicationId);
                    table.ForeignKey(
                        name: "FK_Applications_Subscribers_SubscriberId",
                        column: x => x.SubscriberId,
                        principalTable: "Subscribers",
                        principalColumn: "SubscriberId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Secrets",
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
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubscriberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Secrets", x => x.SecretId);
                    table.ForeignKey(
                        name: "FK_Secrets_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "ApplicationId");
                    table.ForeignKey(
                        name: "FK_Secrets_Subscribers_SubscriberId",
                        column: x => x.SubscriberId,
                        principalTable: "Subscribers",
                        principalColumn: "SubscriberId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiEndpoints",
                columns: table => new
                {
                    ApiEndpointId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BaseUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QueryParametersJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeadersJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HttpMethod = table.Column<int>(type: "int", nullable: false),
                    StrHttpMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BodyTemplate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecretId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiEndpoints", x => x.ApiEndpointId);
                    table.ForeignKey(
                        name: "FK_ApiEndpoints_Secrets_SecretId",
                        column: x => x.SecretId,
                        principalTable: "Secrets",
                        principalColumn: "SecretId");
                });

            migrationBuilder.CreateTable(
                name: "EmailSecret",
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
                        principalTable: "Emails",
                        principalColumn: "EmailId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmailSecret_Secrets_SecretsSecretId",
                        column: x => x.SecretsSecretId,
                        principalTable: "Secrets",
                        principalColumn: "SecretId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhoneSecret",
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
                        principalTable: "Phones",
                        principalColumn: "PhoneId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhoneSecret_Secrets_SecretsSecretId",
                        column: x => x.SecretsSecretId,
                        principalTable: "Secrets",
                        principalColumn: "SecretId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiEndpoints_SecretId",
                table: "ApiEndpoints",
                column: "SecretId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_SubscriberId",
                table: "Applications",
                column: "SubscriberId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailSecret_SecretsSecretId",
                table: "EmailSecret",
                column: "SecretsSecretId");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneSecret_SecretsSecretId",
                table: "PhoneSecret",
                column: "SecretsSecretId");

            migrationBuilder.CreateIndex(
                name: "IX_Secrets_ApplicationId",
                table: "Secrets",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Secrets_SubscriberId",
                table: "Secrets",
                column: "SubscriberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiEndpoints");

            migrationBuilder.DropTable(
                name: "EmailSecret");

            migrationBuilder.DropTable(
                name: "PhoneSecret");

            migrationBuilder.DropTable(
                name: "Emails");

            migrationBuilder.DropTable(
                name: "Phones");

            migrationBuilder.DropTable(
                name: "Secrets");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "Subscribers");
        }
    }
}
