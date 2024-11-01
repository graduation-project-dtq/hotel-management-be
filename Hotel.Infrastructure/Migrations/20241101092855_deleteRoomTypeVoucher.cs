using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hotel.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class deleteRoomTypeVoucher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VoucherRoomTypeDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VoucherRoomTypeDetails",
                columns: table => new
                {
                    VoucherID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoomTypeDetailID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherRoomTypeDetails", x => new { x.VoucherID, x.RoomTypeDetailID });
                    table.ForeignKey(
                        name: "FK_VoucherRoomTypeDetails_RoomTypeDetails_RoomTypeDetailID",
                        column: x => x.RoomTypeDetailID,
                        principalTable: "RoomTypeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VoucherRoomTypeDetails_Vouchers_VoucherID",
                        column: x => x.VoucherID,
                        principalTable: "Vouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VoucherRoomTypeDetails_RoomTypeDetailID",
                table: "VoucherRoomTypeDetails",
                column: "RoomTypeDetailID");
        }
    }
}
