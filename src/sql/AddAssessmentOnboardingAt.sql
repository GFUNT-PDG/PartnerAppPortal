-- Onboarding date is stored once. Revisions update SubmittedAt only.
-- Run this on db-pap-prod01 before deploying the API that reads OnboardingAt.

IF COL_LENGTH('dbo.Assessments', 'OnboardingAt') IS NULL
BEGIN
    ALTER TABLE dbo.Assessments
        ADD OnboardingAt datetimeoffset NULL;
END
GO

-- Existing submitted rows keep the date they have now as the onboarding date.
UPDATE dbo.Assessments
SET OnboardingAt = SubmittedAt
WHERE OnboardingAt IS NULL
  AND SubmittedAt > '1753-01-02T00:00:00+00:00';
GO
