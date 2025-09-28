using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Db.Migrations
{
    /// <inheritdoc />
    public partial class initialmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "suprusr");

            migrationBuilder.EnsureSchema(
                name: "adm");

            migrationBuilder.EnsureSchema(
                name: "usr");

            migrationBuilder.CreateTable(
                name: "Subscribers",
                schema: "adm",
                columns: table => new
                {
                    SubscriberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubscriberIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Seeded = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscribers", x => x.SubscriberId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "suprusr",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
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
                    SubscriberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiEndpoints", x => x.ApiEndpointId);
                    table.ForeignKey(
                        name: "FK_ApiEndpoints_Subscribers_SubscriberId",
                        column: x => x.SubscriberId,
                        principalSchema: "adm",
                        principalTable: "Subscribers",
                        principalColumn: "SubscriberId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                schema: "adm",
                columns: table => new
                {
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalApplicationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExternalProvider = table.Column<int>(type: "int", nullable: false),
                    StrExternalProvider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Seeded = table.Column<bool>(type: "bit", nullable: false),
                    ContactByEmail = table.Column<bool>(type: "bit", nullable: false),
                    ContactBySMS = table.Column<bool>(type: "bit", nullable: false),
                    ContactByApiEndpoint = table.Column<bool>(type: "bit", nullable: false),
                    SubscriberId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.ApplicationId);
                    table.ForeignKey(
                        name: "FK_Applications_Subscribers_SubscriberId",
                        column: x => x.SubscriberId,
                        principalSchema: "adm",
                        principalTable: "Subscribers",
                        principalColumn: "SubscriberId");
                });

            migrationBuilder.CreateTable(
                name: "Emails",
                schema: "suprusr",
                columns: table => new
                {
                    EmailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    Seeded = table.Column<bool>(type: "bit", nullable: false),
                    SubscriberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emails", x => x.EmailId);
                    table.ForeignKey(
                        name: "FK_Emails_Subscribers_SubscriberId",
                        column: x => x.SubscriberId,
                        principalSchema: "adm",
                        principalTable: "Subscribers",
                        principalColumn: "SubscriberId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Phones",
                schema: "suprusr",
                columns: table => new
                {
                    PhoneId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Seeded = table.Column<bool>(type: "bit", nullable: false),
                    SubscriberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phones", x => x.PhoneId);
                    table.ForeignKey(
                        name: "FK_Phones_Subscribers_SubscriberId",
                        column: x => x.SubscriberId,
                        principalSchema: "adm",
                        principalTable: "Subscribers",
                        principalColumn: "SubscriberId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscriberUsers",
                schema: "suprusr",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubscriberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriberUsers", x => new { x.UserId, x.SubscriberId });
                    table.ForeignKey(
                        name: "FK_SubscriberUsers_Subscribers_SubscriberId",
                        column: x => x.SubscriberId,
                        principalSchema: "adm",
                        principalTable: "Subscribers",
                        principalColumn: "SubscriberId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscriberUsers_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "suprusr",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Secrets",
                schema: "usr",
                columns: table => new
                {
                    SecretId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalSecretId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastTimeNotified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Seeded = table.Column<bool>(type: "bit", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Secrets", x => x.SecretId);
                    table.ForeignKey(
                        name: "FK_Secrets_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "adm",
                        principalTable: "Applications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailAplications",
                schema: "suprusr",
                columns: table => new
                {
                    EmailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailAplications", x => new { x.EmailId, x.ApplicationId });
                    table.ForeignKey(
                        name: "FK_EmailAplications_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "adm",
                        principalTable: "Applications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmailAplications_Emails_EmailId",
                        column: x => x.EmailId,
                        principalSchema: "suprusr",
                        principalTable: "Emails",
                        principalColumn: "EmailId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhoneApplications",
                schema: "suprusr",
                columns: table => new
                {
                    PhoneId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneApplications", x => new { x.PhoneId, x.ApplicationId });
                    table.ForeignKey(
                        name: "FK_PhoneApplications_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "adm",
                        principalTable: "Applications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhoneApplications_Phones_PhoneId",
                        column: x => x.PhoneId,
                        principalSchema: "suprusr",
                        principalTable: "Phones",
                        principalColumn: "PhoneId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiEndpoints_SubscriberId",
                schema: "suprusr",
                table: "ApiEndpoints",
                column: "SubscriberId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ExternalApplicationId",
                schema: "adm",
                table: "Applications",
                column: "ExternalApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_SubscriberId",
                schema: "adm",
                table: "Applications",
                column: "SubscriberId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailAplications_ApplicationId",
                schema: "suprusr",
                table: "EmailAplications",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Emails_SubscriberId",
                schema: "suprusr",
                table: "Emails",
                column: "SubscriberId");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneApplications_ApplicationId",
                schema: "suprusr",
                table: "PhoneApplications",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Phones_SubscriberId",
                schema: "suprusr",
                table: "Phones",
                column: "SubscriberId");

            migrationBuilder.CreateIndex(
                name: "IX_Secrets_ApplicationId",
                schema: "usr",
                table: "Secrets",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriberUsers_SubscriberId",
                schema: "suprusr",
                table: "SubscriberUsers",
                column: "SubscriberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiEndpoints",
                schema: "suprusr");

            migrationBuilder.DropTable(
                name: "EmailAplications",
                schema: "suprusr");

            migrationBuilder.DropTable(
                name: "PhoneApplications",
                schema: "suprusr");

            migrationBuilder.DropTable(
                name: "Secrets",
                schema: "usr");

            migrationBuilder.DropTable(
                name: "SubscriberUsers",
                schema: "suprusr");

            migrationBuilder.DropTable(
                name: "Emails",
                schema: "suprusr");

            migrationBuilder.DropTable(
                name: "Phones",
                schema: "suprusr");

            migrationBuilder.DropTable(
                name: "Applications",
                schema: "adm");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "suprusr");

            migrationBuilder.DropTable(
                name: "Subscribers",
                schema: "adm");
        }
    }
}
