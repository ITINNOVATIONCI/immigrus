using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace immigrus.Migrations
{
    public partial class fourth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId", table: "AspNetRoleClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId", table: "AspNetUserClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId", table: "AspNetUserLogins");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_IdentityRole_RoleId", table: "AspNetUserRoles");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_ApplicationUser_UserId", table: "AspNetUserRoles");
            migrationBuilder.DropTable("Enfants");
            migrationBuilder.DropTable("Clients");
            migrationBuilder.AddColumn<string>(
                name: "AdrPos",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "AutresDip",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "ClientsId",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreation",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            migrationBuilder.AddColumn<DateTime>(
                name: "DateNais",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            migrationBuilder.AddColumn<string>(
                name: "Diplome",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "Etat",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "LieuNais",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "NbEnfts",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "Nom",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "ParainIdf",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "PaysEl",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "PaysNais",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "PaysRes",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "Photo",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "Prenoms",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "Sexe",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "Statut",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "StatutMarital",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "Tel1",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "Tel2",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_IdentityRole_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_ApplicationUser_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId", table: "AspNetRoleClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId", table: "AspNetUserClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId", table: "AspNetUserLogins");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_IdentityRole_RoleId", table: "AspNetUserRoles");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_ApplicationUser_UserId", table: "AspNetUserRoles");
            migrationBuilder.DropColumn(name: "AdrPos", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "AutresDip", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "ClientsId", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "DateCreation", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "DateNais", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "Diplome", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "Etat", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "LieuNais", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "NbEnfts", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "Nom", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "ParainIdf", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "Password", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "PaysEl", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "PaysNais", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "PaysRes", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "Photo", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "Prenoms", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "Sexe", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "Statut", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "StatutMarital", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "Tel1", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "Tel2", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "ZipCode", table: "AspNetUsers");
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    ClientsId = table.Column<string>(nullable: false),
                    AdrPos = table.Column<string>(nullable: true),
                    AutresDip = table.Column<string>(nullable: true),
                    ConfimationNumber = table.Column<string>(nullable: true),
                    DateCreation = table.Column<DateTime>(nullable: false),
                    DateNais = table.Column<DateTime>(nullable: false),
                    Diplome = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Etat = table.Column<string>(nullable: true),
                    LieuNais = table.Column<string>(nullable: true),
                    NbEnfts = table.Column<string>(nullable: true),
                    Nom = table.Column<string>(nullable: true),
                    ParainIdf = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    PaysEl = table.Column<string>(nullable: true),
                    PaysNais = table.Column<string>(nullable: true),
                    PaysRes = table.Column<string>(nullable: true),
                    Photo = table.Column<string>(nullable: true),
                    Prenoms = table.Column<string>(nullable: true),
                    Resultat = table.Column<string>(nullable: true),
                    Sexe = table.Column<string>(nullable: true),
                    Statut = table.Column<string>(nullable: true),
                    StatutMarital = table.Column<string>(nullable: true),
                    Tel1 = table.Column<string>(nullable: true),
                    Tel2 = table.Column<string>(nullable: true),
                    ZipCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.ClientsId);
                });
            migrationBuilder.CreateTable(
                name: "Enfants",
                columns: table => new
                {
                    EnfantsId = table.Column<string>(nullable: false),
                    ClientsId = table.Column<string>(nullable: true),
                    DateNais = table.Column<DateTime>(nullable: false),
                    Nom = table.Column<string>(nullable: true),
                    Prenoms = table.Column<string>(nullable: true),
                    sexe = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enfants", x => x.EnfantsId);
                    table.ForeignKey(
                        name: "FK_Enfants_Clients_ClientsId",
                        column: x => x.ClientsId,
                        principalTable: "Clients",
                        principalColumn: "ClientsId",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_IdentityRole_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_ApplicationUser_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
