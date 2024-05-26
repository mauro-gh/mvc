using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvc.Migrations
{
    /// <inheritdoc />
    public partial class RelazioneCorsoUtente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.AddColumn<string>(
            //     name: "AuthorId",
            //     table: "Courses",
            //     type: "TEXT",
            //     nullable: false,
            //     defaultValue: "");

            // migrationBuilder.CreateIndex(
            //     name: "IX_Courses_AuthorId",
            //     table: "Courses",
            //     column: "AuthorId");

            // migrationBuilder.AddForeignKey(
            //     name: "FK_Courses_AspNetUsers_AuthorId",
            //     table: "Courses",
            //     column: "AuthorId",
            //     principalTable: "AspNetUsers",
            //     principalColumn: "Id",
            //     onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql(@"
PRAGMA foreign_keys = 0;

CREATE TABLE sqlitestudio_temp_table AS SELECT *
                                          FROM Courses;

DROP TABLE Courses;

CREATE TABLE Courses (
    Id                    INTEGER  NOT NULL
                                   CONSTRAINT PK_Courses PRIMARY KEY AUTOINCREMENT,
    Title                 TEXT     NOT NULL,
    Description           TEXT,
    LogoPath              TEXT,
    Author                TEXT     NOT NULL,
    Email                 TEXT,
    Rating                REAL     NOT NULL,
    FullPrice_Amount      NUMERIC  NOT NULL,
    FullPrice_Currency    TEXT     NOT NULL,
    CurrentPrice_Amount   NUMERIC  NOT NULL,
    CurrentPrice_Currency TEXT     NOT NULL,
    RowVersion            DATETIME,
    Status                TEXT     NOT NULL
                                   DEFAULT 'Draft',
    AuthorId              TEXT     REFERENCES AspNetUsers (Id) ON DELETE CASCADE
);

INSERT INTO Courses (
                        Id,
                        Title,
                        Description,
                        LogoPath,
                        Author,
                        Email,
                        Rating,
                        FullPrice_Amount,
                        FullPrice_Currency,
                        CurrentPrice_Amount,
                        CurrentPrice_Currency,
                        RowVersion,
                        Status
                    )
                    SELECT Id,
                           Title,
                           Description,
                           LogoPath,
                           Author,
                           Email,
                           Rating,
                           FullPrice_Amount,
                           FullPrice_Currency,
                           CurrentPrice_Amount,
                           CurrentPrice_Currency,
                           RowVersion,
                           Status
                      FROM sqlitestudio_temp_table;

DROP TABLE sqlitestudio_temp_table;

CREATE UNIQUE INDEX IX_Courses_Title ON Courses (
    'Title'
);

CREATE TRIGGER CoursesSetRowVersionOnInsert
         AFTER INSERT
            ON Courses
BEGIN
    UPDATE Courses
       SET RowVersion = CURRENT_TIMESTAMP
     WHERE Id = NEW.Id;
END;

CREATE TRIGGER CoursesSetRowVersionOnUpdate
         AFTER UPDATE
            ON Courses
          WHEN NEW.RowVersion <= OLD.RowVersion
BEGIN
    UPDATE Courses
       SET RowVersion = CURRENT_TIMESTAMP
     WHERE Id = NEW.Id;
END;

PRAGMA foreign_keys = 1;
",suppressTransaction: true);
            

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_AspNetUsers_AuthorId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_AuthorId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Courses");
        }
    }
}
