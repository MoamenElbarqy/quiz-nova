using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizNova.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFeedbackAndGradedAtToManuallyGradedAnswers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "QuizAttempts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Feedback",
                table: "ManuallyGradedAnswers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "GradedAt",
                table: "ManuallyGradedAnswers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Admins_Users_Id",
                table: "Admins",
                column: "Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admins_Users_Id",
                table: "Admins");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "QuizAttempts");

            migrationBuilder.DropColumn(
                name: "Feedback",
                table: "ManuallyGradedAnswers");

            migrationBuilder.DropColumn(
                name: "GradedAt",
                table: "ManuallyGradedAnswers");
        }
    }
}
