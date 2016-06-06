using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LubCycle.Api.Data.Migrations
{
    public partial class UpdateTravelDurationMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestinationUid",
                table: "TravelDurations");

            migrationBuilder.DropColumn(
                name: "StartUid",
                table: "TravelDurations");

            migrationBuilder.AddColumn<string>(
                name: "Station1Uid",
                table: "TravelDurations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Station2Uid",
                table: "TravelDurations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Station1Uid",
                table: "TravelDurations");

            migrationBuilder.DropColumn(
                name: "Station2Uid",
                table: "TravelDurations");

            migrationBuilder.AddColumn<string>(
                name: "DestinationUid",
                table: "TravelDurations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StartUid",
                table: "TravelDurations",
                nullable: true);
        }
    }
}
