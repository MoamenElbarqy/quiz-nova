using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizNova.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseChatRooms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseChatRooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    InstructorId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseChatRooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CourseChatRoomMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    SenderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReplyOnId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Content = table.Column<JsonDocument>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseChatRoomMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseChatRoomMessages_CourseChatRooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "CourseChatRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseChatRoomMessages_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CourseChatRoomStudents",
                columns: table => new
                {
                    CourseChatRoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseChatRoomStudents", x => new { x.CourseChatRoomId, x.StudentsId });
                    table.ForeignKey(
                        name: "FK_CourseChatRoomStudents_CourseChatRooms_CourseChatRoomId",
                        column: x => x.CourseChatRoomId,
                        principalTable: "CourseChatRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseChatRoomStudents_Users_StudentsId",
                        column: x => x.StudentsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageReactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReactorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Emoji = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageReactions_CourseChatRoomMessages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "CourseChatRoomMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageReactions_Users_ReactorId",
                        column: x => x.ReactorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseChatRoomMessages_RoomId",
                table: "CourseChatRoomMessages",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseChatRoomMessages_SenderId",
                table: "CourseChatRoomMessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseChatRooms_CourseId",
                table: "CourseChatRooms",
                column: "CourseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseChatRoomStudents_StudentsId",
                table: "CourseChatRoomStudents",
                column: "StudentsId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReactions_MessageId_ReactorId",
                table: "MessageReactions",
                columns: new[] { "MessageId", "ReactorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageReactions_ReactorId",
                table: "MessageReactions",
                column: "ReactorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseChatRoomStudents");

            migrationBuilder.DropTable(
                name: "MessageReactions");

            migrationBuilder.DropTable(
                name: "CourseChatRoomMessages");

            migrationBuilder.DropTable(
                name: "CourseChatRooms");
        }
    }
}
