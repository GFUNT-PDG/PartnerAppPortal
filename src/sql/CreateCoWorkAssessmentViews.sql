-- CoWork read access: views over submitted assessment results
-- Run against: pacappdb01 (or the current App Development database)
-- Safe to re-run (CREATE OR ALTER VIEW)
--
-- Submitted rule: SubmittedAt > '1753-01-01' (app sentinel for in-progress)
-- Do NOT grant CoWork access to sqladmin / API login.

------------------------------------------------------------
-- 1) Header: one row per submitted assessment
------------------------------------------------------------
CREATE OR ALTER VIEW dbo.vw_CoWork_AssessmentHeaders
AS
SELECT
    a.Id                    AS AssessmentId,
    TRY_CAST(a.CustomerId AS uniqueidentifier) AS AccountId,
    c.Name                  AS AccountName,
    c.Email                 AS AccountEmail,
    a.SubmittedAt           AS SubmittedAtUtc,
    CAST(a.SubmittedAt AS date) AS SubmittedDate,
    s.TotalScore            AS OverallScore,
    b.OverallScore          AS BaselineOverallScore,
    b.CapturedAt            AS BaselineCapturedAtUtc,
    b.SectionScoresJson     AS BaselineSectionScoresJson,
    CASE
        WHEN b.OverallScore IS NULL THEN NULL
        ELSE CAST(s.TotalScore AS decimal(9,2)) - CAST(b.OverallScore AS decimal(9,2))
    END                     AS OverallScoreDelta
FROM dbo.Assessments a
LEFT JOIN dbo.CrmAccounts c
    ON c.AccountId = TRY_CAST(a.CustomerId AS uniqueidentifier)
LEFT JOIN dbo.AssessmentScores s
    ON s.AssessmentId = a.Id
LEFT JOIN dbo.AssessmentScoreBaselines b
    ON b.AssessmentId = a.Id
WHERE a.SubmittedAt > '1753-01-01T00:00:00+00:00';
GO

------------------------------------------------------------
-- 2) Answers: question-level results for submitted assessments
------------------------------------------------------------
CREATE OR ALTER VIEW dbo.vw_CoWork_AssessmentAnswers
AS
SELECT
    a.Id                    AS AssessmentId,
    TRY_CAST(a.CustomerId AS uniqueidentifier) AS AccountId,
    c.Name                  AS AccountName,
    a.SubmittedAt           AS SubmittedAtUtc,
    ans.QuestionId,
    q.Category,
    q.Section,
    q.Subsection,
    q.QuestionText,
    q.DisplayOrder,
    ans.Response,
    ans.PointsEarned,
    q.Score                 AS QuestionScoreMax
FROM dbo.Assessments a
INNER JOIN dbo.AssessmentAnswers ans
    ON ans.AssessmentId = a.Id
LEFT JOIN dbo.AssessmentQuestions q
    ON q.QuestionId = ans.QuestionId
   AND q.IsActive = 1
LEFT JOIN dbo.CrmAccounts c
    ON c.AccountId = TRY_CAST(a.CustomerId AS uniqueidentifier)
WHERE a.SubmittedAt > '1753-01-01T00:00:00+00:00';
GO

------------------------------------------------------------
-- 3) Latest submitted assessment per account (handy for agents)
------------------------------------------------------------
CREATE OR ALTER VIEW dbo.vw_CoWork_LatestAssessmentByAccount
AS
WITH ranked AS
(
    SELECT
        h.*,
        ROW_NUMBER() OVER (
            PARTITION BY h.AccountId
            ORDER BY h.SubmittedAtUtc DESC
        ) AS rn
    FROM dbo.vw_CoWork_AssessmentHeaders h
    WHERE h.AccountId IS NOT NULL
)
SELECT
    AssessmentId,
    AccountId,
    AccountName,
    AccountEmail,
    SubmittedAtUtc,
    SubmittedDate,
    OverallScore,
    BaselineOverallScore,
    BaselineCapturedAtUtc,
    BaselineSectionScoresJson,
    OverallScoreDelta
FROM ranked
WHERE rn = 1;
GO

------------------------------------------------------------
-- Smoke checks (optional)
------------------------------------------------------------
-- SELECT TOP 20 * FROM dbo.vw_CoWork_AssessmentHeaders ORDER BY SubmittedAtUtc DESC;
-- SELECT TOP 50 * FROM dbo.vw_CoWork_AssessmentAnswers ORDER BY SubmittedAtUtc DESC, DisplayOrder;
-- SELECT TOP 20 * FROM dbo.vw_CoWork_LatestAssessmentByAccount ORDER BY AccountName;
GO
