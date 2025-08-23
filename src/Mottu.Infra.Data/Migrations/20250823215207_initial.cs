using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mottu.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Motorcycles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Brand = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    LicensePlate = table.Column<string>(type: "character varying(7)", unicode: false, maxLength: 7, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Motorcycles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Outbox",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    OccuredOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ProcessedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Outbox", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rentals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MotorcycleId = table.Column<int>(type: "integer", nullable: false),
                    CourierId = table.Column<int>(type: "integer", nullable: false),
                    Plan = table.Column<int>(type: "integer", nullable: false),
                    DailyPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DailyPriceCurrency = table.Column<string>(type: "character varying(3)", unicode: false, maxLength: 3, nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    TotalPriceCurrency = table.Column<string>(type: "character varying(3)", unicode: false, maxLength: 3, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ForecastEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rentals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Cnpj = table.Column<string>(type: "character varying(14)", unicode: false, maxLength: 14, nullable: true),
                    CnhNumber = table.Column<string>(type: "character varying(11)", unicode: false, maxLength: 11, nullable: true),
                    CnhCategory = table.Column<int>(type: "integer", nullable: true),
                    CnhType = table.Column<int>(type: "integer", nullable: true),
                    CnhImageUri = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Motorcycles",
                columns: new[] { "Id", "CreationTime", "IsDeleted", "LastModificationTime", "Brand", "LicensePlate", "Year" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, "Honda", "ABC1234", 2020 },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, null, "Yamaha", "XYZ9A23", 2024 }
                });

            migrationBuilder.InsertData(
                table: "Rentals",
                columns: new[] { "Id", "CourierId", "CreatedAtUtc", "EndDate", "ForecastEndDate", "MotorcycleId", "Plan", "StartDate", "Status", "DailyPriceCurrency", "DailyPrice", "TotalPriceCurrency", "TotalPrice" },
                values: new object[] { 1, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2025, 1, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 7, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "BRL", 30.00m, "BRL", 150.00m });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BirthDate", "CnhImageUri", "CnhType", "CreatedAtUtc", "Role", "UpdatedAtUtc", "Name" },
                values: new object[] { 1, new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, null, "Admin User" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BirthDate", "CnhImageUri", "CnhType", "CreatedAtUtc", "Role", "UpdatedAtUtc", "CnhCategory", "CnhNumber", "Cnpj", "Name" },
                values: new object[,]
                {
                    { 2, new DateTime(1995, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "courier1-cnh.png", 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, 0, "12345678901", "12345678000195", "Courier One" },
                    { 3, new DateTime(1998, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "courier2-cnh.png", 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, 2, "98765432100", "98765432000109", "Courier Two" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Motorcycles_CreationTime",
                table: "Motorcycles",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_Motorcycles_LicensePlate",
                table: "Motorcycles",
                column: "LicensePlate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Outbox_OccuredOn",
                table: "Outbox",
                column: "OccuredOn");

            migrationBuilder.CreateIndex(
                name: "IX_Outbox_ProcessedOn",
                table: "Outbox",
                column: "ProcessedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_CourierId",
                table: "Rentals",
                column: "CourierId");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_MotorcycleId",
                table: "Rentals",
                column: "MotorcycleId");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_Status",
                table: "Rentals",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Users_BirthDate",
                table: "Users",
                column: "BirthDate");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CnhNumber",
                table: "Users",
                column: "CnhNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Cnpj",
                table: "Users",
                column: "Cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Name",
                table: "Users",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Role",
                table: "Users",
                column: "Role");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Motorcycles");

            migrationBuilder.DropTable(
                name: "Outbox");

            migrationBuilder.DropTable(
                name: "Rentals");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
