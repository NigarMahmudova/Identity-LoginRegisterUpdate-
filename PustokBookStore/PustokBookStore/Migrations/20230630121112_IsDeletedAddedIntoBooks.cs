﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PustokBookStore.Migrations
{
    public partial class IsDeletedAddedIntoBooks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Books",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Books");
        }
    }
}
