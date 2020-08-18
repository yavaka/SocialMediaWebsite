using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialMedia.Data.Migrations
{
    public partial class TagFriendEntitiesInCommentsAndPosts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TagFriends");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Posts",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Comments",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "TagFriendsInComments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaggerId = table.Column<string>(nullable: true),
                    TaggedId = table.Column<string>(nullable: true),
                    CommentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagFriendsInComments", x => x.Id);
                    table.ForeignKey(
                        name: "TagFreindToComment_FK",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "TagFriendToTaggedInComment_FK",
                        column: x => x.TaggedId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "TagFriendToTaggerInComment_FK",
                        column: x => x.TaggerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TagFriendsInPosts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaggerId = table.Column<string>(nullable: true),
                    TaggedId = table.Column<string>(nullable: true),
                    PostId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("TagFriendsInPosts_PK", x => x.Id);
                    table.ForeignKey(
                        name: "TagFriendsToPost_FK",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "TagFriendsToTaggedInPost_FK",
                        column: x => x.TaggedId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "TagFriendsToTaggerInPost_FK",
                        column: x => x.TaggerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TagFriendsInComments_CommentId",
                table: "TagFriendsInComments",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_TagFriendsInComments_TaggedId",
                table: "TagFriendsInComments",
                column: "TaggedId");

            migrationBuilder.CreateIndex(
                name: "IX_TagFriendsInComments_TaggerId",
                table: "TagFriendsInComments",
                column: "TaggerId");

            migrationBuilder.CreateIndex(
                name: "IX_TagFriendsInPosts_PostId",
                table: "TagFriendsInPosts",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_TagFriendsInPosts_TaggedId",
                table: "TagFriendsInPosts",
                column: "TaggedId");

            migrationBuilder.CreateIndex(
                name: "IX_TagFriendsInPosts_TaggerId",
                table: "TagFriendsInPosts",
                column: "TaggerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TagFriendsInComments");

            migrationBuilder.DropTable(
                name: "TagFriendsInPosts");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "TagFriends",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommentId = table.Column<int>(type: "int", nullable: true),
                    PostId = table.Column<int>(type: "int", nullable: true),
                    TaggedId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TaggerId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("TagFriends_PK", x => x.Id);
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

            migrationBuilder.CreateIndex(
                name: "IX_TagFriends_TaggerId",
                table: "TagFriends",
                column: "TaggerId");
        }
    }
}
