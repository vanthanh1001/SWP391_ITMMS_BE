using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWP391_ITMMS_Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAvatarToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "TreatmentHistories",
                newName: "Treatment");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "TreatmentHistories",
                newName: "TreatmentDate");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ServiceName",
                table: "TreatmentServices",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "ServiceCode",
                table: "TreatmentServices",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "Diagnosis",
                table: "TreatmentHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "TreatmentHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Prescription",
                table: "TreatmentHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "TreatmentServices",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 17, 13, 5, 17, 354, DateTimeKind.Local).AddTicks(3438));

            migrationBuilder.UpdateData(
                table: "TreatmentServices",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 17, 13, 5, 17, 354, DateTimeKind.Local).AddTicks(3442));

            migrationBuilder.UpdateData(
                table: "TreatmentServices",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 17, 13, 5, 17, 354, DateTimeKind.Local).AddTicks(3444));

            migrationBuilder.UpdateData(
                table: "TreatmentServices",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 17, 13, 5, 17, 354, DateTimeKind.Local).AddTicks(3450));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Avatar", "CreatedAt", "Password" },
                values: new object[] { null, new DateTime(2025, 7, 17, 13, 5, 17, 55, DateTimeKind.Local).AddTicks(3637), "$2a$11$mDPXXXVx89jCMpGtmpBtKeab2nbXMYfUOL8KQ0YusHx.0SFXJb17u" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Avatar", "CreatedAt", "Password" },
                values: new object[] { null, new DateTime(2025, 7, 17, 13, 5, 17, 204, DateTimeKind.Local).AddTicks(5463), "$2a$11$obdoOcLlbJ4cXmFb8u4xc.Bw.raj9jgj2ObloS1F86j3tkpfg2/uW" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Avatar", "CreatedAt", "Password" },
                values: new object[] { null, new DateTime(2025, 7, 17, 13, 5, 17, 354, DateTimeKind.Local).AddTicks(3072), "$2a$11$3FFPiVImW2IGted2bNHJhe4gkbQZmPLzqT3KAl3LzMBvsNA7CWT.q" });

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentHistories_UserId",
                table: "TreatmentHistories",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentHistories_Users_UserId",
                table: "TreatmentHistories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentHistories_Users_UserId",
                table: "TreatmentHistories");

            migrationBuilder.DropIndex(
                name: "IX_TreatmentHistories_UserId",
                table: "TreatmentHistories");

            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Diagnosis",
                table: "TreatmentHistories");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "TreatmentHistories");

            migrationBuilder.DropColumn(
                name: "Prescription",
                table: "TreatmentHistories");

            migrationBuilder.RenameColumn(
                name: "TreatmentDate",
                table: "TreatmentHistories",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "Treatment",
                table: "TreatmentHistories",
                newName: "Description");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ServiceName",
                table: "TreatmentServices",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ServiceCode",
                table: "TreatmentServices",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

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
