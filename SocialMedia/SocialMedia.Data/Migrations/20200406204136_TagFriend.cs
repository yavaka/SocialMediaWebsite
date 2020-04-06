using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialMedia.Data.Migrations
{
    public partial class TagFriend : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TagFriends",
                table: "TagFriends");

            migrationBuilder.AlterColumn<string>(
                name: "TaggedId",
                table: "TagFriends",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "TaggerId",
                table: "TagFriends",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "TagFriends",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "TagFriends_PK",
                table: "TagFriends",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TagFriends_TaggerId",
                table: "TagFriends",
                column: "TaggerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "TagFriends_PK",
                table: "TagFriends");

            migrationBuilder.DropIndex(
                name: "IX_TagFriends_TaggerId",
                table: "TagFriends");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TagFriends");

            migrationBuilder.AlterColumn<string>(
                name: "TaggerId",
                table: "TagFriends",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TaggedId",
                table: "TagFriends",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TagFriends",
                table: "TagFriends",
                columns: new[] { "TaggerId", "TaggedId" });
        }
    }
}
