using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SWP391_ITMMS_Api.Migrations
{
    /// <inheritdoc />
    public partial class FixShadowPropertiesRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_TreatmentPlans_TreatmentPlanId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Appointments_AppointmentId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Customers_CustomerId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Doctors_DoctorId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentPlans_Customers_CustomerId",
                table: "TreatmentPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentPlans_Doctors_DoctorId",
                table: "TreatmentPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentPlans_TreatmentServices_TreatmentServiceId",
                table: "TreatmentPlans");

            migrationBuilder.DropTable(
                name: "Prescriptions");

            migrationBuilder.DropTable(
                name: "TestResults");

            migrationBuilder.DropTable(
                name: "TreatmentHistories");

            migrationBuilder.DropTable(
                name: "UserFeedbacks");

            migrationBuilder.DropTable(
                name: "MedicalRecords");

            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_TreatmentServices_ServiceCode",
                table: "TreatmentServices");

            migrationBuilder.DropIndex(
                name: "IX_TreatmentPlans_TreatmentServiceId",
                table: "TreatmentPlans");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_AppointmentId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_LicenseNumber",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_TreatmentPlanId",
                table: "Appointments");

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TreatmentServices",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TreatmentServices",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TreatmentServices",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TreatmentServices",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BasePrice",
                table: "TreatmentServices");

            migrationBuilder.DropColumn(
                name: "Procedures",
                table: "TreatmentServices");

            migrationBuilder.DropColumn(
                name: "Requirements",
                table: "TreatmentServices");

            migrationBuilder.DropColumn(
                name: "ServiceCode",
                table: "TreatmentServices");

            migrationBuilder.DropColumn(
                name: "SuccessRate",
                table: "TreatmentServices");

            migrationBuilder.DropColumn(
                name: "CurrentPhase",
                table: "TreatmentPlans");

            migrationBuilder.DropColumn(
                name: "NextPhaseDate",
                table: "TreatmentPlans");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "TreatmentPlans");

            migrationBuilder.DropColumn(
                name: "PaidAmount",
                table: "TreatmentPlans");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "TreatmentPlans");

            migrationBuilder.DropColumn(
                name: "PhaseDescription",
                table: "TreatmentPlans");

            migrationBuilder.DropColumn(
                name: "ProgressNotes",
                table: "TreatmentPlans");

            migrationBuilder.DropColumn(
                name: "TreatmentServiceId",
                table: "TreatmentPlans");

            migrationBuilder.DropColumn(
                name: "TreatmentType",
                table: "TreatmentPlans");

            migrationBuilder.DropColumn(
                name: "TimeSlot",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "TreatmentPlanId",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Users",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "ServiceName",
                table: "TreatmentServices",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "DurationDays",
                table: "TreatmentServices",
                newName: "Duration");

            migrationBuilder.RenameColumn(
                name: "NextVisitDate",
                table: "TreatmentPlans",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "DoctorId",
                table: "TreatmentPlans",
                newName: "ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_TreatmentPlans_DoctorId",
                table: "TreatmentPlans",
                newName: "IX_TreatmentPlans_ServiceId");

            migrationBuilder.RenameColumn(
                name: "DateOfBirth",
                table: "Customers",
                newName: "UpdatedAt");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "TreatmentServices",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "TreatmentServices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalCost",
                table: "TreatmentPlans",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(12,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "TreatmentPlans",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TreatmentPlans",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "TreatmentPlans",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Feedbacks",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Feedbacks",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Feedbacks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Education",
                table: "Doctors",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ConsultationFee",
                table: "Doctors",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Doctors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Doctors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Doctors",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MedicalHistory",
                table: "Customers",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Customers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "BlogPosts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "FeaturedImage",
                table: "BlogPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishedAt",
                table: "BlogPosts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "BlogPosts",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "BlogPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Appointments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Appointments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Appointments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Appointments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentServices_CategoryId",
                table: "TreatmentServices",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_AppointmentId",
                table: "Feedbacks",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_UserId1",
                table: "Doctors",
                column: "UserId1",
                unique: true,
                filter: "[UserId1] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_UserId1",
                table: "Customers",
                column: "UserId1",
                unique: true,
                filter: "[UserId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Users_UserId1",
                table: "Customers",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Users_UserId1",
                table: "Doctors",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Appointments_AppointmentId",
                table: "Feedbacks",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Customers_CustomerId",
                table: "Feedbacks",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Doctors_DoctorId",
                table: "Feedbacks",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentPlans_Customers_CustomerId",
                table: "TreatmentPlans",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentPlans_TreatmentServices_ServiceId",
                table: "TreatmentPlans",
                column: "ServiceId",
                principalTable: "TreatmentServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentServices_Categories_CategoryId",
                table: "TreatmentServices",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Users_UserId1",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Users_UserId1",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Appointments_AppointmentId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Customers_CustomerId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Doctors_DoctorId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentPlans_Customers_CustomerId",
                table: "TreatmentPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentPlans_TreatmentServices_ServiceId",
                table: "TreatmentPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentServices_Categories_CategoryId",
                table: "TreatmentServices");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_TreatmentServices_CategoryId",
                table: "TreatmentServices");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_AppointmentId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_UserId1",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "IX_Customers_UserId1",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "TreatmentServices");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "TreatmentServices");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "TreatmentPlans");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "FeaturedImage",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "PublishedAt",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Users",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "TreatmentServices",
                newName: "ServiceName");

            migrationBuilder.RenameColumn(
                name: "Duration",
                table: "TreatmentServices",
                newName: "DurationDays");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "TreatmentPlans",
                newName: "NextVisitDate");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "TreatmentPlans",
                newName: "DoctorId");

            migrationBuilder.RenameIndex(
                name: "IX_TreatmentPlans_ServiceId",
                table: "TreatmentPlans",
                newName: "IX_TreatmentPlans_DoctorId");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Customers",
                newName: "DateOfBirth");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "BasePrice",
                table: "TreatmentServices",
                type: "decimal(15,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Procedures",
                table: "TreatmentServices",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Requirements",
                table: "TreatmentServices",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ServiceCode",
                table: "TreatmentServices",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "SuccessRate",
                table: "TreatmentServices",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalCost",
                table: "TreatmentPlans",
                type: "decimal(12,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "TreatmentPlans",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TreatmentPlans",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddColumn<int>(
                name: "CurrentPhase",
                table: "TreatmentPlans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextPhaseDate",
                table: "TreatmentPlans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "TreatmentPlans",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "PaidAmount",
                table: "TreatmentPlans",
                type: "decimal(12,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "TreatmentPlans",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhaseDescription",
                table: "TreatmentPlans",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProgressNotes",
                table: "TreatmentPlans",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TreatmentServiceId",
                table: "TreatmentPlans",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TreatmentType",
                table: "TreatmentPlans",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Feedbacks",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Education",
                table: "Doctors",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ConsultationFee",
                table: "Doctors",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "MedicalHistory",
                table: "Customers",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "BlogPosts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Appointments",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Appointments",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TimeSlot",
                table: "Appointments",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TreatmentPlanId",
                table: "Appointments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MedicalRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    Diagnosis = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Prescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RecordDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Symptoms = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Treatment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalRecords_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicalRecords_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicalRecords_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    NormalRange = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Results = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TestName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TestType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestResults_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestResults_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TreatmentHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserFeedbacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFeedbacks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prescriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicalRecordId = table.Column<int>(type: "int", nullable: false),
                    Dosage = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Duration = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Frequency = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MedicineName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prescriptions_MedicalRecords_MedicalRecordId",
                        column: x => x.MedicalRecordId,
                        principalTable: "MedicalRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TreatmentServices",
                columns: new[] { "Id", "BasePrice", "CreatedAt", "Description", "DurationDays", "IsActive", "Procedures", "Requirements", "ServiceCode", "ServiceName", "SuccessRate", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 85000000m, new DateTime(2025, 6, 26, 16, 39, 19, 294, DateTimeKind.Local).AddTicks(2892), "Kỹ thuật hỗ trợ sinh sản hiện đại, tỷ lệ thành công cao. Quy trình bao gồm kích thích buồng trung, lấy trứng, thụ tinh ngoài cơ thể và chuyển phôi.", 30, true, "Khám sàng lọc → Kích thích buồng trung → Lấy trứng → Thụ tinh → Nuôi cấy phôi → Chuyển phôi", "Khám tổng quát, xét nghiệm hormone, siêu âm, tinh dịch đồ", "IVF001", "Thụ tinh trong ống nghiệm (IVF)", 68.5f, null },
                    { 2, 15000000m, new DateTime(2025, 6, 26, 16, 39, 19, 294, DateTimeKind.Local).AddTicks(2898), "Kỹ thuật đưa tinh trùng đã được xử lý vào buồng tử cung vào thời điểm rụng trứng.", 14, true, "Khám sàng lọc → Theo dõi rụng trứng → Xử lý tinh trùng → Bơm tinh trùng vào tử cung", "Vòi trứng thông thoáng, tinh trùng đạt chất lượng tối thiểu", "IUI001", "Thụ tinh nhân tạo (IUI)", 35.2f, null },
                    { 3, 95000000m, new DateTime(2025, 6, 26, 16, 39, 19, 294, DateTimeKind.Local).AddTicks(2902), "Kỹ thuật tiêm tinh trùng vào bào tương trứng, áp dụng cho các trường hợp nam giới có chất lượng tinh trùng kém.", 35, true, "Quy trình IVF kết hợp với kỹ thuật ICSI", "Tinh trùng số lượng ít hoặc chất lượng kém", "ICSI001", "IVF với ICSI", 72.3f, null },
                    { 4, 5000000m, new DateTime(2025, 6, 26, 16, 39, 19, 294, DateTimeKind.Local).AddTicks(2904), "Điều trị bằng thuốc cho các trường hợp rối loạn nội tiết, PCOS, rối loạn tinh trùng.", 90, true, "Khám và chẩn đoán → Điều trị nội khoa → Theo dõi đáp ứng", "Khám tổng quát, xét nghiệm chuyên sâu", "MED001", "Điều trị nội khoa hiếm muộn", 45.7f, null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "CreatedAt", "DateOfBirth", "Email", "FullName", "IsActive", "Password", "Phone", "Role", "UpdatedAt", "Username" },
                values: new object[,]
                {
                    { 1, "System", new DateTime(2025, 6, 26, 16, 39, 19, 21, DateTimeKind.Local).AddTicks(3095), null, "admin@itmms.com", "System Administrator", true, "$2a$11$Lc9N67wxc/UGhnXDUPKfLOYJKcQzpQmgwi6Z5320twuH0S.D5cNhG", "0123456789", "Admin", null, "admin" },
                    { 2, "Hà Nội", new DateTime(2025, 6, 26, 16, 39, 19, 157, DateTimeKind.Local).AddTicks(5148), null, "doctor1@itmms.com", "Dr. Nguyễn Văn A", true, "$2a$11$3bfxWeMutoGVhSU93GJLPeTkto7rKhxkyNqJRAtJWvvkIiz9XhT/S", "0987654321", "Doctor", null, "doctor1" },
                    { 4, "Hà Nội", new DateTime(2025, 6, 26, 16, 39, 19, 294, DateTimeKind.Local).AddTicks(1751), null, "manager@itmms.com", "Nguyễn Thị B", true, "$2a$11$n.KesFq1LrW2EYX0aAYK1elhcvXLLXYPc3f0hd/50fnEKN1Y8wM8.", "0123456790", "Manager", null, "manager1" }
                });

            migrationBuilder.InsertData(
                table: "Doctors",
                columns: new[] { "Id", "ConsultationFee", "Description", "Education", "ExperienceYears", "IsAvailable", "LicenseNumber", "Specialization", "UserId" },
                values: new object[] { 1, 500000m, "Chuyên gia điều trị hiếm muộn với 10 năm kinh nghiệm", "Bác sĩ chuyên khoa II - Đại học Y Hà Nội", 10, true, "BS001234", "Sản phụ khoa - Hiếm muộn", 2 });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentServices_ServiceCode",
                table: "TreatmentServices",
                column: "ServiceCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentPlans_TreatmentServiceId",
                table: "TreatmentPlans",
                column: "TreatmentServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_AppointmentId",
                table: "Feedbacks",
                column: "AppointmentId",
                unique: true,
                filter: "[AppointmentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_LicenseNumber",
                table: "Doctors",
                column: "LicenseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_TreatmentPlanId",
                table: "Appointments",
                column: "TreatmentPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_AppointmentId",
                table: "MedicalRecords",
                column: "AppointmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_CustomerId",
                table: "MedicalRecords",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_DoctorId",
                table: "MedicalRecords",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_MedicalRecordId",
                table: "Prescriptions",
                column: "MedicalRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_CustomerId",
                table: "TestResults",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_DoctorId",
                table: "TestResults",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_TreatmentPlans_TreatmentPlanId",
                table: "Appointments",
                column: "TreatmentPlanId",
                principalTable: "TreatmentPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Appointments_AppointmentId",
                table: "Feedbacks",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Customers_CustomerId",
                table: "Feedbacks",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Doctors_DoctorId",
                table: "Feedbacks",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentPlans_Customers_CustomerId",
                table: "TreatmentPlans",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentPlans_Doctors_DoctorId",
                table: "TreatmentPlans",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentPlans_TreatmentServices_TreatmentServiceId",
                table: "TreatmentPlans",
                column: "TreatmentServiceId",
                principalTable: "TreatmentServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
