using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PontPesee.API.Migrations.Reports
{
    public partial class _20260110_AddReportJobs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "report_jobs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    FilterJson = table.Column<string>(type: "longtext", nullable: true),
                    Status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    FilePath = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    Error = table.Column<string>(type: "longtext", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    OwnerUserId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_report_jobs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_report_jobs_Status",
                table: "report_jobs",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_report_jobs_CreatedAt",
                table: "report_jobs",
                column: "CreatedAt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "report_jobs");
        }
    }
}
