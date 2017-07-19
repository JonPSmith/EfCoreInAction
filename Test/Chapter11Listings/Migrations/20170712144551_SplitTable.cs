using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Test.Chapter11Listings.Migrations
{
    public partial class SplitTable : Migration 
    {
        protected override void Up
            (MigrationBuilder migrationBuilder) //#A
        {
            //Would need to handle any foreign key contraints that pointed to the CustomerAndAddresses before and after that table is renamed

            migrationBuilder.RenameTable(     //#B
                name: "CustomerAndAddresses",
                newName: "Customers");

            migrationBuilder.CreateTable( //#C
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

            migrationBuilder.Sql( //#D
                @"INSERT INTO [dbo].[Addresses] ([Address], [CustFK])
                SELECT Address, Id
                FROM [dbo].[Customers]");

            migrationBuilder.DropColumn( //#E
                name: "Address",
                table: "Customers");
        }
        /************************************************************
        #A I change the code produced by EF Core's add-migration command to produce the changes I want
        #B EF Core would drop the CustomerAndAddresses table and create a new Customers table, but to save data I rename the CustomerAndAddresses table to the Customers table
        #C EF Core adds the new Addresses table
        #D I now copy the Address part of the renamed CustomerAndAddresses table to the Addresses table
        #E Fianlly I drop the Address column from the renamed CustomerAndAddresses so that it now acts like Customers table that EF Core expects
         * **********************************************************/

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
