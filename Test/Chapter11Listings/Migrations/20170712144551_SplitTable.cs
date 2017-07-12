using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Test.Chapter11Listings.Migrations
{
    public partial class SplitTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Address = table.Column<string>(nullable: true),
                    CustFK = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_Customers_CustFK",
                        column: x => x.CustFK,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CustFK",
                table: "Addresses",
                column: "CustFK",
                unique: true);

            migrationBuilder.Sql(@"INSERT INTO [dbo].[Addresses] ([Address], [CustFK])
                SELECT Address, Id
                FROM [dbo].[CustomerAndAddresses]");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "CustomerAndAddresses");

            //Would need to handle any foreign key contraints that pointed to the CustomerAndAddresses before and after that table is renamed

            migrationBuilder.RenameTable(
                name: "CustomerAndAddresses",
                newName: "Customers");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new InvalidOperationException("I have not added the code to undo the change to the database!");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.CreateTable(
                name: "CustomerAndAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Address = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerAndAddresses", x => x.Id);
                });
        }
    }
}
