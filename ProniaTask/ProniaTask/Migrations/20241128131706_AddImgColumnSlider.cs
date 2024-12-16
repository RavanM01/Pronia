using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProniaTask.Migrations
{
    /// <inheritdoc />
    public partial class AddImgColumnSlider : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImgUrl",
                table: "sliders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImgUrl",
                table: "sliders");
        }
    }
}
