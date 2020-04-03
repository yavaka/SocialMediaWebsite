using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialMedia.Data.Migrations
{
    public partial class TagFriends : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TagFriends",
                columns: table => new
                {
                    TaggerId = table.Column<string>(nullable: false),
                    TaggedId = table.Column<string>(nullable: false),
                    PostId = table.Column<int>(nullable: true),
                    CommentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagFriends", x => new { x.TaggerId, x.TaggedId });
                    table.ForeignKey(
                        name: "TagFreindsToComment_FK",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "TagFriendsToPost_FK",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "TagFriendsToTagged_FK",
                        column: x => x.TaggedId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "TagFriendsToTagger_FK",
                        column: x => x.TaggerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TagFriends_CommentId",
                table: "TagFriends",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_TagFriends_PostId",
                table: "TagFriends",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_TagFriends_TaggedId",
                table: "TagFriends",
                column: "TaggedId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TagFriends");
        }
    }
}
