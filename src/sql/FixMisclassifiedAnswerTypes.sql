-- Correct AnswerType / options for mis-tagged questions
-- Run against: pacappdb01

-- Free-text (not Yes/No)
UPDATE dbo.AssessmentQuestions
SET AnswerType = N'Text',
    OptionsJson = NULL
WHERE QuestionText IN (
    N'What platform does Partner use for Invoicing?',
    N'Does Partner have a specific segment?',
    N'If Indirect, who is their Distributor?',
    N'If yes, what solution are they using?',
    N'If yes, how are they being tracked?'
)
OR QuestionText LIKE N'What platform does Partner use%';

-- Private / Resell choice (not Yes/No)
UPDATE dbo.AssessmentQuestions
SET AnswerType = N'Choice',
    OptionsJson = N'["Please Choose One","Private","Resell"]'
WHERE QuestionText LIKE N'%utilizing private or resell offers%'
   OR QuestionText LIKE N'%private or resell offers%';

-- Verify
SELECT QuestionId, AnswerType, OptionsJson, QuestionText
FROM dbo.AssessmentQuestions
WHERE QuestionText LIKE N'%Invoicing%'
   OR QuestionText LIKE N'%segment%'
   OR QuestionText LIKE N'%resell%'
   OR QuestionText LIKE N'%Distributor%'
ORDER BY QuestionText;
GO
