using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SWP391_ITMMS_Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTreatmentServiceAndEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropColumn(
                name: "Fee",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Appointments");

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

            migrationBuilder.AddColumn<DateTime>(
                name: "NextVisitDate",
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

            migrationBuilder.CreateTable(
                name: "TreatmentServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ServiceCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    Procedures = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Requirements = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DurationDays = table.Column<int>(type: "int", nullable: false),
                    SuccessRate = table.Column<float>(type: "real", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentServices", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TreatmentServices",
                columns: new[] { "Id", "BasePrice", "CreatedAt", "Description", "DurationDays", "IsActive", "Procedures", "Requirements", "ServiceCode", "ServiceName", "SuccessRate", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 85000000m, new DateTime(2025, 6, 23, 15, 59, 14, 926, DateTimeKind.Local).AddTicks(3051), "Kỹ thuật hỗ trợ sinh sản hiện đại, tỷ lệ thành công cao. Quy trình bao gồm kích thích buồng trung, lấy trứng, thụ tinh ngoài cơ thể và chuyển phôi.", 30, true, "Khám sàng lọc → Kích thích buồng trung → Lấy trứng → Thụ tinh → Nuôi cấy phôi → Chuyển phôi", "Khám tổng quát, xét nghiệm hormone, siêu âm, tinh dịch đồ", "IVF001", "Thụ tinh trong ống nghiệm (IVF)", 68.5f, null },
                    { 2, 15000000m, new DateTime(2025, 6, 23, 15, 59, 14, 926, DateTimeKind.Local).AddTicks(3064), "Kỹ thuật đưa tinh trùng đã được xử lý vào buồng tử cung vào thời điểm rụng trứng.", 14, true, "Khám sàng lọc → Theo dõi rụng trứng → Xử lý tinh trùng → Bơm tinh trùng vào tử cung", "Vòi trứng thông thoáng, tinh trùng đạt chất lượng tối thiểu", "IUI001", "Thụ tinh nhân tạo (IUI)", 35.2f, null },
                    { 3, 95000000m, new DateTime(2025, 6, 23, 15, 59, 14, 926, DateTimeKind.Local).AddTicks(3071), "Kỹ thuật tiêm tinh trùng vào bào tương trứng, áp dụng cho các trường hợp nam giới có chất lượng tinh trùng kém.", 35, true, "Quy trình IVF kết hợp với kỹ thuật ICSI", "Tinh trùng số lượng ít hoặc chất lượng kém", "ICSI001", "IVF với ICSI", 72.3f, null },
                    { 4, 5000000m, new DateTime(2025, 6, 23, 15, 59, 14, 926, DateTimeKind.Local).AddTicks(3532), "Điều trị bằng thuốc cho các trường hợp rối loạn nội tiết, PCOS, rối loạn tinh trùng.", 90, true, "Khám và chẩn đoán → Điều trị nội khoa → Theo dõi đáp ứng", "Khám tổng quát, xét nghiệm chuyên sâu", "MED001", "Điều trị nội khoa hiếm muộn", 45.7f, null }
                });

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

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "CreatedAt", "Email", "FullName", "IsActive", "Password", "Phone", "Role", "UpdatedAt", "Username" },
                values: new object[] { 4, "Hà Nội", new DateTime(2025, 6, 23, 15, 59, 14, 926, DateTimeKind.Local).AddTicks(1700), "manager@itmms.com", "Nguyễn Thị B", true, "$2a$11$Iiy6964RJbcyzlKJs9y67uELPqkQvcAQL3MiBBC0x4BGnxH/zsTjK", "0123456790", "Manager", null, "manager1" });

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentPlans_TreatmentServiceId",
                table: "TreatmentPlans",
                column: "TreatmentServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentServices_ServiceCode",
                table: "TreatmentServices",
                column: "ServiceCode",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentPlans_TreatmentServices_TreatmentServiceId",
                table: "TreatmentPlans",
                column: "TreatmentServiceId",
                principalTable: "TreatmentServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentPlans_TreatmentServices_TreatmentServiceId",
                table: "TreatmentPlans");

            migrationBuilder.DropTable(
                name: "TreatmentServices");

            migrationBuilder.DropIndex(
                name: "IX_TreatmentPlans_TreatmentServiceId",
                table: "TreatmentPlans");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "CurrentPhase",
                table: "TreatmentPlans");

            migrationBuilder.DropColumn(
                name: "NextPhaseDate",
                table: "TreatmentPlans");

            migrationBuilder.DropColumn(
                name: "NextVisitDate",
                table: "TreatmentPlans");

            migrationBuilder.DropColumn(
                name: "Notes",
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

            migrationBuilder.AddColumn<decimal>(
                name: "Fee",
                table: "Appointments",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "Appointments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Appointments",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2025, 6, 23, 13, 5, 35, 834, DateTimeKind.Local).AddTicks(8252), "$2a$11$zJ7s9rFI291a4d5ShnU7/OBQV/MWeBtDqiKQ4zlIsCNj8p2.iW/hm" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2025, 6, 23, 13, 5, 36, 12, DateTimeKind.Local).AddTicks(3718), "$2a$11$Jyfjv6E867viArLFqMKu9.eo1NEt0Lu8lWyp40fVoHDK/eMSoCkDu" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_AppointmentId",
                table: "Payments",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CustomerId",
                table: "Payments",
                column: "CustomerId");
        }
    }
}
