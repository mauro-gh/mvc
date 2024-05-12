using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvc.Migrations
{
    /// <inheritdoc />
    public partial class NuovaColonnaCourseStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Courses",
                type: "TEXT",
                nullable: false,
                defaultValue: nameof(Models.Enums.CourseStatus.Draft)); //nameof, restituisce il valore corretto dell'enum e non il suo id
            
            migrationBuilder.Sql($"UPDATE Courses SET Status = '{nameof(Models.Enums.CourseStatus.Published)}'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Courses");
        }
    }
}
