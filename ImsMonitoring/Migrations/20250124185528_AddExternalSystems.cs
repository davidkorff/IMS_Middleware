using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ImsMonitoring.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalSystems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExternalSystems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalSystems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImsInstanceConnections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ImsInstanceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalSystemId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConnectionString = table.Column<string>(type: "text", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: true),
                    ApiKey = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastSuccessfulConnection = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImsInstanceConnections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImsInstanceConnections_ExternalSystems_ExternalSystemId",
                        column: x => x.ExternalSystemId,
                        principalTable: "ExternalSystems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ImsInstanceConnections_ImsInstances_ImsInstanceId",
                        column: x => x.ImsInstanceId,
                        principalTable: "ImsInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ExternalSystems",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "Version" },
                values: new object[,]
                {
                    { new Guid("2afca612-ef87-4f13-8016-87a7d8dba551"), new DateTime(2025, 1, 24, 18, 55, 27, 842, DateTimeKind.Utc).AddTicks(7831), "AIM Invoicing Platform v1", true, "AIM", "1.0" },
                    { new Guid("8cd5368a-b51e-4c92-8aba-ab7a53fa57b1"), new DateTime(2025, 1, 24, 18, 55, 27, 842, DateTimeKind.Utc).AddTicks(7837), "AIM Invoicing Platform v2", true, "AIM", "2.0" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImsInstanceConnections_ExternalSystemId",
                table: "ImsInstanceConnections",
                column: "ExternalSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_ImsInstanceConnections_ImsInstanceId",
                table: "ImsInstanceConnections",
                column: "ImsInstanceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImsInstanceConnections");

            migrationBuilder.DropTable(
                name: "ExternalSystems");
        }
    }
}
