using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECOMMERCE.SUBMISSION.DATA.Migrations
{
    /// <inheritdoc />
    public partial class addStatusOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "Order",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "Order");
        }
    }
}
