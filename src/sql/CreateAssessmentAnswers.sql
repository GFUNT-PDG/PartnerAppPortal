-- Per-question answers for Partner Assessment Wizard
-- Run against: pacappdb01
-- Safe to re-run (IF NOT EXISTS)

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
GO
