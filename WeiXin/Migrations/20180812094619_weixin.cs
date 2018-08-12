using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeiXin.Migrations
{
    public partial class weixin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Map",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ManTelphone = table.Column<string>(nullable: true),
                    WomenTelphone = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Map", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UserInfo",
                columns: table => new
                {
                    AddTime = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Sex = table.Column<string>(nullable: true),
                    Telphone = table.Column<string>(nullable: false),
                    QQ = table.Column<string>(nullable: true),
                    BirthDay = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Grade = table.Column<string>(nullable: true),
                    WX = table.Column<string>(nullable: true),
                    College = table.Column<string>(nullable: true),
                    Height = table.Column<string>(nullable: true),
                    Introduce = table.Column<string>(nullable: true),
                    Expect = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfo", x => x.Telphone);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Map");

            migrationBuilder.DropTable(
                name: "UserInfo");
        }
    }
}
