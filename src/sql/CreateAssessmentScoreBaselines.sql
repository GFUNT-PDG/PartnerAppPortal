-- Snapshot of first-submitted scores so revisions can show Initial → Latest deltas
-- Run against: pacappdb01

IF OBJECT_ID(N'dbo.AssessmentScoreBaselines', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AssessmentScoreBaselines
    (
        AssessmentId      uniqueidentifier NOT NULL CONSTRAINT PK_AssessmentScoreBaselines PRIMARY KEY,
        CapturedAt        datetimeoffset   NOT NULL,
        OverallScore      decimal(9,2)     NOT NULL CONSTRAINT DF_AssessmentScoreBaselines_Overall DEFAULT (0),
        SectionScoresJson nvarchar(max)    NULL
    );
END
GO
