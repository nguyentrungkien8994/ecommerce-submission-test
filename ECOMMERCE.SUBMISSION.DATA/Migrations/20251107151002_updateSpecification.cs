using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECOMMERCE.SUBMISSION.DATA.Migrations
{
    /// <inheritdoc />
    public partial class updateSpecification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "account_id",
                table: "Specification",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Specification_account_id",
                table: "Specification",
                column: "account_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Specification_Account_account_id",
                table: "Specification",
                column: "account_id",
                principalTable: "Account",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Specification_Account_account_id",
                table: "Specification");

            migrationBuilder.DropIndex(
                name: "IX_Specification_account_id",
                table: "Specification");

            migrationBuilder.DropColumn(
                name: "account_id",
                table: "Specification");
        }
    }
}
