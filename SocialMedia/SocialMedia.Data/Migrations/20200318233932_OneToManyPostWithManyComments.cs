using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialMedia.Data.Migrations
{
    public partial class OneToManyPostWithManyComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Posts_CommentedPostPostId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_CommentedPostPostId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "CommentedPostPostId",
                table: "Comments");

            migrationBuilder.AddColumn<int>(
                name: "CommentedPostId",
                table: "Comments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommentedPostId",
                table: "Comments",
                column: "CommentedPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Posts_CommentedPostId",
                table: "Comments",
                column: "CommentedPostId",
                principalTable: "Posts",
                principalColumn: "PostId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Posts_CommentedPostId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_CommentedPostId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "CommentedPostId",
                table: "Comments");

            migrationBuilder.AddColumn<int>(
                name: "CommentedPostPostId",
                table: "Comments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommentedPostPostId",
                table: "Comments",
                column: "CommentedPostPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Posts_CommentedPostPostId",
                table: "Comments",
                column: "CommentedPostPostId",
                principalTable: "Posts",
                principalColumn: "PostId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
