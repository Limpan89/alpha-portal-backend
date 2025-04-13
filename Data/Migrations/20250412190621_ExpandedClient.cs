using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class ExpandedClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CityName",
                table: "UserAddresses");

            migrationBuilder.AlterColumn<int>(
                name: "PostalCode",
                table: "UserAddresses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PostalAddresses",
                columns: table => new
                {
                    PostalCode = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CityName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostalAddresses", x => x.PostalCode);
                });

            migrationBuilder.CreateTable(
                name: "ClientBillings",
                columns: table => new
                {
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BillingAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillingReference = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PostalCode = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientBillings", x => x.ClientId);
                    table.ForeignKey(
                        name: "FK_ClientBillings_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientBillings_PostalAddresses_PostalCode",
                        column: x => x.PostalCode,
                        principalTable: "PostalAddresses",
                        principalColumn: "PostalCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_PostalCode",
                table: "UserAddresses",
                column: "PostalCode");

            migrationBuilder.CreateIndex(
                name: "IX_ClientBillings_BillingReference",
                table: "ClientBillings",
                column: "BillingReference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientBillings_PostalCode",
                table: "ClientBillings",
                column: "PostalCode");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAddresses_PostalAddresses_PostalCode",
                table: "UserAddresses",
                column: "PostalCode",
                principalTable: "PostalAddresses",
                principalColumn: "PostalCode",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAddresses_PostalAddresses_PostalCode",
                table: "UserAddresses");

            migrationBuilder.DropTable(
                name: "ClientBillings");

            migrationBuilder.DropTable(
                name: "PostalAddresses");

            migrationBuilder.DropIndex(
                name: "IX_UserAddresses_PostalCode",
                table: "UserAddresses");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Clients");

            migrationBuilder.AlterColumn<string>(
                name: "PostalCode",
                table: "UserAddresses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "CityName",
                table: "UserAddresses",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
