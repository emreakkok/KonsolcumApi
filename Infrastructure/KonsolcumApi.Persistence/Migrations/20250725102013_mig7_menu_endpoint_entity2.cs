using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KonsolcumApi.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig7_menu_endpoint_entity2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppRoleEndpoint_Endpoints_EndpointId",
                table: "AppRoleEndpoint");

            migrationBuilder.RenameColumn(
                name: "EndpointId",
                table: "AppRoleEndpoint",
                newName: "EndpointsId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppRoleEndpoint_Endpoints_EndpointsId",
                table: "AppRoleEndpoint",
                column: "EndpointsId",
                principalTable: "Endpoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppRoleEndpoint_Endpoints_EndpointsId",
                table: "AppRoleEndpoint");

            migrationBuilder.RenameColumn(
                name: "EndpointsId",
                table: "AppRoleEndpoint",
                newName: "EndpointId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppRoleEndpoint_Endpoints_EndpointId",
                table: "AppRoleEndpoint",
                column: "EndpointId",
                principalTable: "Endpoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
