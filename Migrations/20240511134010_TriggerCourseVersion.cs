﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvc.Migrations
{
    /// <inheritdoc />
    public partial class TriggerCourseVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE TRIGGER CoursesSetRowVersionOnInsert
                                   AFTER INSERT ON Courses
                                   BEGIN
                                   UPDATE Courses SET RowVersion = CURRENT_TIMESTAMP WHERE Id=NEW.Id;
                                   END;");
            migrationBuilder.Sql(@"CREATE TRIGGER CoursesSetRowVersionOnUpdate
                                   AFTER UPDATE ON Courses WHEN NEW.RowVersion <= OLD.RowVersion
                                   BEGIN
                                   UPDATE Courses SET RowVersion = CURRENT_TIMESTAMP WHERE Id=NEW.Id;
                                   END;");
            migrationBuilder.Sql("UPDATE Courses SET RowVersion = CURRENT_TIMESTAMP;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER CoursesSetRowVersionOnInsert;");
            migrationBuilder.Sql("DROP TRIGGER CoursesSetRowVersionOnUpdate;");
        }
    }
}
