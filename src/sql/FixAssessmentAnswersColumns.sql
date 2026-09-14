-- Fix AssessmentAnswers columns if the table was created without Response / PointsEarned
-- Run against: pacappdb01
-- Safe to re-run

IF OBJECT_ID(N'dbo.AssessmentAnswers', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AssessmentAnswers
    (
        Id             uniqueidentifier NOT NULL CONSTRAINT PK_AssessmentAnswers PRIMARY KEY,
        AssessmentId   uniqueidentifier NOT NULL,
        QuestionId     nvarchar(100)    NOT NULL,
        Response       nvarchar(max)    NULL,
        PointsEarned   int              NOT NULL CONSTRAINT DF_AssessmentAnswers_Points DEFAULT (0)
    );

    CREATE UNIQUE INDEX UX_AssessmentAnswers_Assessment_Question
        ON dbo.AssessmentAnswers(AssessmentId, QuestionId);

    CREATE INDEX IX_AssessmentAnswers_AssessmentId
        ON dbo.AssessmentAnswers(AssessmentId);
END
ELSE
BEGIN
    IF COL_LENGTH(N'dbo.AssessmentAnswers', N'Response') IS NULL
    BEGIN
        ALTER TABLE dbo.AssessmentAnswers ADD Response nvarchar(max) NULL;
    END

    IF COL_LENGTH(N'dbo.AssessmentAnswers', N'PointsEarned') IS NULL
    BEGIN
        ALTER TABLE dbo.AssessmentAnswers ADD PointsEarned int NOT NULL
            CONSTRAINT DF_AssessmentAnswers_Points DEFAULT (0);
    END

    IF COL_LENGTH(N'dbo.AssessmentAnswers', N'QuestionId') IS NULL
    BEGIN
        ALTER TABLE dbo.AssessmentAnswers ADD QuestionId nvarchar(100) NOT NULL
            CONSTRAINT DF_AssessmentAnswers_QuestionId DEFAULT (N'');
    END

    IF COL_LENGTH(N'dbo.AssessmentAnswers', N'AssessmentId') IS NULL
    BEGIN
        ALTER TABLE dbo.AssessmentAnswers ADD AssessmentId uniqueidentifier NOT NULL
            CONSTRAINT DF_AssessmentAnswers_AssessmentId DEFAULT ('00000000-0000-0000-0000-000000000000');
    END

    IF NOT EXISTS (
        SELECT 1 FROM sys.indexes
        WHERE name = N'UX_AssessmentAnswers_Assessment_Question'
          AND object_id = OBJECT_ID(N'dbo.AssessmentAnswers'))
    BEGIN
        CREATE UNIQUE INDEX UX_AssessmentAnswers_Assessment_Question
            ON dbo.AssessmentAnswers(AssessmentId, QuestionId);
    END
END
GO

-- Verify
SELECT c.name AS ColumnName, t.name AS TypeName
FROM sys.columns c
JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID(N'dbo.AssessmentAnswers')
ORDER BY c.column_id;
GO
