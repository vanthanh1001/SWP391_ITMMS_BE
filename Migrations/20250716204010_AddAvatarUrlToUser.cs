using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWP391_ITMMS_Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAvatarUrlToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "TreatmentServices",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 17, 3, 40, 9, 54, DateTimeKind.Local).AddTicks(4934));

            migrationBuilder.UpdateData(
                table: "TreatmentServices",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 17, 3, 40, 9, 54, DateTimeKind.Local).AddTicks(4941));

            migrationBuilder.UpdateData(
                table: "TreatmentServices",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 17, 3, 40, 9, 54, DateTimeKind.Local).AddTicks(4944));

            migrationBuilder.UpdateData(
                table: "TreatmentServices",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 17, 3, 40, 9, 54, DateTimeKind.Local).AddTicks(4953));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AvatarUrl", "CreatedAt", "Password" },
                values: new object[] { null, new DateTime(2025, 7, 17, 3, 40, 8, 512, DateTimeKind.Local).AddTicks(4395), "$2a$11$0xTs3RscrMVePfFalyRaNeT.FZXu9JRweLONJzFMlPgQMSgNZoZ3y" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AvatarUrl", "CreatedAt", "Password" },
                values: new object[] { null, new DateTime(2025, 7, 17, 3, 40, 8, 767, DateTimeKind.Local).AddTicks(568), "$2a$11$4AXa6zu78PfhwWfspxZ/nOF2tMYfNfDwR03HIRoVZ5vC5/GGrvqcy" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "AvatarUrl", "CreatedAt", "Password" },
                values: new object[] { null, new DateTime(2025, 7, 17, 3, 40, 9, 54, DateTimeKind.Local).AddTicks(3962), "$2a$11$XJr6ye/HvmfMU0WMOk5qc.oXDG9X6LME1POouNnah5l4FzOTQnL6m" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "TreatmentServices",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 23, 15, 59, 14, 926, DateTimeKind.Local).AddTicks(3051));

            migrationBuilder.UpdateData(
                table: "TreatmentServices",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 23, 15, 59, 14, 926, DateTimeKind.Local).AddTicks(3064));

            migrationBuilder.UpdateData(
                table: "TreatmentServices",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 23, 15, 59, 14, 926, DateTimeKind.Local).AddTicks(3071));

            migrationBuilder.UpdateData(
                table: "TreatmentServices",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 23, 15, 59, 14, 926, DateTimeKind.Local).AddTicks(3532));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2025, 6, 23, 15, 59, 14, 452, DateTimeKind.Local).AddTicks(410), "$2a$11$vy9OQYSg31Ab9sXsF6QsT.HnkIvNUXOxPWcFsdtA3xJopIvvYYN6y" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2025, 6, 23, 15, 59, 14, 683, DateTimeKind.Local).AddTicks(6347), "$2a$11$fJ.tdjzckU3UyV.7GgGQRuZrHpQKEPuRPcNJqdSpVVVtzwJR1xmb6" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2025, 6, 23, 15, 59, 14, 926, DateTimeKind.Local).AddTicks(1700), "$2a$11$Iiy6964RJbcyzlKJs9y67uELPqkQvcAQL3MiBBC0x4BGnxH/zsTjK" });
        }
    }
}
