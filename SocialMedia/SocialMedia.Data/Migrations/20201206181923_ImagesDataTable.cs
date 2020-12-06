using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialMedia.Data.Migrations
{
    public partial class ImagesDataTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImagesData",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OriginalFileName = table.Column<string>(nullable: true),
                    OriginalType = table.Column<string>(nullable: true),
                    OriginalContent = table.Column<byte[]>(nullable: true),
                    FullscreenContent = table.Column<byte[]>(nullable: true),
                    ThumbnailContent = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImagesData", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImagesData");
        }
    }
}
