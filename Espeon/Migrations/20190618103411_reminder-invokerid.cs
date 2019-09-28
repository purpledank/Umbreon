﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Espeon.Migrations
{
    public partial class reminderinvokerid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int[]>(
                name: "ResponsePacks",
                table: "Users",
                nullable: true,
                defaultValue: new[] { 0 },
                oldClrType: typeof(int[]),
                oldNullable: true,
                oldDefaultValue: new[] { 0 });

            migrationBuilder.AddColumn<decimal>(
                name: "InvokeId",
                table: "Reminders",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvokeId",
                table: "Reminders");

            migrationBuilder.AlterColumn<int[]>(
                name: "ResponsePacks",
                table: "Users",
                nullable: true,
                defaultValue: new[] { 0 },
                oldClrType: typeof(int[]),
                oldNullable: true,
                oldDefaultValue: new[] { 0 });
        }
    }
}
