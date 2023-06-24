using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InvitationS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_invitation_invitation_credentials_InviterCredentialsId",
                table: "invitation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_invitation",
                table: "invitation");

            migrationBuilder.RenameTable(
                name: "invitation",
                newName: "invitations");

            migrationBuilder.RenameIndex(
                name: "IX_invitation_InviterCredentialsId",
                table: "invitations",
                newName: "IX_invitations_InviterCredentialsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_invitations",
                table: "invitations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_invitations_invitation_credentials_InviterCredentialsId",
                table: "invitations",
                column: "InviterCredentialsId",
                principalTable: "invitation_credentials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_invitations_invitation_credentials_InviterCredentialsId",
                table: "invitations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_invitations",
                table: "invitations");

            migrationBuilder.RenameTable(
                name: "invitations",
                newName: "invitation");

            migrationBuilder.RenameIndex(
                name: "IX_invitations_InviterCredentialsId",
                table: "invitation",
                newName: "IX_invitation_InviterCredentialsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_invitation",
                table: "invitation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_invitation_invitation_credentials_InviterCredentialsId",
                table: "invitation",
                column: "InviterCredentialsId",
                principalTable: "invitation_credentials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
