using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beam.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePostLikeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddPrimaryKey(
                name: "PK_PostLikes",
                table: "PostLikes",
                columns: new[] { "UserId", "PostId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PostLikes",
                table: "PostLikes");
        }
    }
}
