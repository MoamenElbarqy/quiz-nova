using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizNova.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingSelectedChoiceAndStudentChoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseChatRoomStudents_Users_StudentsId",
                table: "CourseChatRoomStudents");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "CourseChatRooms");

            migrationBuilder.AddColumn<bool>(
                name: "StudentChoice",
                table: "TfAnswers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "SelectedChoiceId",
                table: "McqAnswers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddForeignKey(
                name: "FK_CourseChatRoomStudents_Students_StudentsId",
                table: "CourseChatRoomStudents",
                column: "StudentsId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseChatRoomStudents_Students_StudentsId",
                table: "CourseChatRoomStudents");

            migrationBuilder.DropColumn(
                name: "StudentChoice",
                table: "TfAnswers");

            migrationBuilder.DropColumn(
                name: "SelectedChoiceId",
                table: "McqAnswers");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "CourseChatRooms",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseChatRoomStudents_Users_StudentsId",
                table: "CourseChatRoomStudents",
                column: "StudentsId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
