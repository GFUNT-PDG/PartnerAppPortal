-- Rebuild AssessmentQuestions from Partner Assessment Workbook (Excel)
-- Safe: deletes existing question bank rows, then inserts canonical set
-- Run against: pacappdb01
SET NOCOUNT ON;
BEGIN TRANSACTION;
DELETE FROM dbo.AssessmentQuestions;

INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-001', 'Solutions_Specializations', N'Is Partner enrolled in any Solutions Designations?', 'Solutions_Specializations', 'Partner Membership Enrollment', 'YesNo', 1, 1, 0, '["Please Choose One","Yes","No","N/A"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-002', 'Solutions_Specializations', N'Is Partner enrolled in any Advanced Specializations?', 'Solutions_Specializations', 'Partner Membership Enrollment', 'YesNo', 2, 1, 0, '["Please Choose One","Yes","No","N/A"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-003', 'Solutions_Specializations', N'Does Partner have the Business Applications Designation?', 'Solutions_Specializations', 'Solutions Designations', 'YesNo', 3, 1, 0, '["Please Choose One","Yes","No","N/A"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-004', 'Solutions_Specializations', N'What is the current Business Applications Designation Score?', 'Solutions_Specializations', 'Solutions Designations', 'Number', 4, 1, 0, NULL);
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-005', 'Solutions_Specializations', N'Is Partner going to target the Business Applications Designation?', 'Solutions_Specializations', 'Solutions Designations', 'YesNo', 5, 1, 0, '["Please Choose One","Yes","No","N/A"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-006', 'Solutions_Specializations', N'Does Partner have the Data & AI Designation?', 'Solutions_Specializations', 'Solutions Designations', 'YesNo', 6, 1, 0, '["Please Choose One","Yes","No","N/A"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-007', 'Solutions_Specializations', N'What is the current Data & AI Designation Score?', 'Solutions_Specializations', 'Solutions Designations', 'Number', 7, 1, 0, NULL);
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-008', 'Solutions_Specializations', N'Is Partner going to target the Data & AI Designation?', 'Solutions_Specializations', 'Solutions Designations', 'YesNo', 8, 1, 0, '["Please Choose One","Yes","No","N/A"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-009', 'Solutions_Specializations', N'Does Partner have the Digital & App Innovation Designation?', 'Solutions_Specializations', 'Solutions Designations', 'YesNo', 9, 1, 0, '["Please Choose One","Yes","No","N/A"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-010', 'Solutions_Specializations', N'What is the current Digital & App Innovation Designation Score?', 'Solutions_Specializations', 'Solutions Designations', 'Number', 10, 1, 0, NULL);
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-011', 'Solutions_Specializations', N'Is Partner going to target the Digital & App innovation Designation?', 'Solutions_Specializations', 'Solutions Designations', 'YesNo', 11, 1, 0, '["Please Choose One","Yes","No","N/A"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-012', 'Solutions_Specializations', N'Does Partner have the Infrastructure Designation?', 'Solutions_Specializations', 'Solutions Designations', 'YesNo', 12, 1, 0, '["Please Choose One","Yes","No","N/A"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-013', 'Solutions_Specializations', N'What is the current Infrastructure Designation Score?', 'Solutions_Specializations', 'Solutions Designations', 'Number', 13, 1, 0, NULL);
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-014', 'Solutions_Specializations', N'Is Partner going to target the Infrastructure Designation?', 'Solutions_Specializations', 'Solutions Designations', 'YesNo', 14, 1, 0, '["Please Choose One","Yes","No","N/A"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-015', 'Solutions_Specializations', N'Does Partner have the Modern Work Designation?', 'Solutions_Specializations', 'Solutions Designations', 'YesNo', 15, 1, 0, '["Please Choose One","Yes","No","N/A"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-016', 'Solutions_Specializations', N'What is the current Modern Work Designation Score?', 'Solutions_Specializations', 'Solutions Designations', 'Number', 16, 1, 0, NULL);
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-017', 'Solutions_Specializations', N'Is Partner going to target the Modern Work Designation?', 'Solutions_Specializations', 'Solutions Designations', 'YesNo', 17, 1, 0, '["Please Choose One","Yes","No","N/A"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-018', 'Solutions_Specializations', N'Does Partner have the Security Designation?', 'Solutions_Specializations', 'Solutions Designations', 'YesNo', 18, 1, 0, '["Please Choose One","Yes","No","N/A"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-019', 'Solutions_Specializations', N'What is the current Security Designation Score?', 'Solutions_Specializations', 'Solutions Designations', 'Number', 19, 1, 0, NULL);
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-020', 'Solutions_Specializations', N'Is Partner going to target the Security Designation?', 'Solutions_Specializations', 'Solutions Designations', 'YesNo', 20, 1, 0, '["Please Choose One","Yes","No","N/A"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-021', 'Solutions_Specializations', N'Does Partner have the AI Platform on Microsoft Azure Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 21, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-022', 'Solutions_Specializations', N'Does Partner have the Analytics on Microsoft Azure Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 22, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-023', 'Solutions_Specializations', N'Does Partner have the Agentic DevOps with Microsoft Azure and GitHub Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 23, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-024', 'Solutions_Specializations', N'Does Partner have the Hybrid Cloud Infrastructure with Microsoft Azure Stack HCI Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 24, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-025', 'Solutions_Specializations', N'Does Partner have the Networking Services in Microsoft Azure Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 25, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-026', 'Solutions_Specializations', N'Does Partner have the Microsoft Azure VMware Solution Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 26, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-027', 'Solutions_Specializations', N'Does Partner have the AI Apps on Microsoft Azure Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 27, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-028', 'Solutions_Specializations', N'Does Partner have the Data Warehouse Migration to Microsoft Azure Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 28, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-029', 'Solutions_Specializations', N'Does Partner have the Infra and Database Migration to Microsoft Azure Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 29, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-030', 'Solutions_Specializations', N'Does Partner have the Kubernetes on Microsoft Azure Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 30, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-031', 'Solutions_Specializations', N'Does Partner have the Migrate Enterprise Apps to Microsoft Azure Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 31, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-032', 'Solutions_Specializations', N'Does Partner have the SAP on Microsoft Azure Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 32, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-033', 'Solutions_Specializations', N'Does Partner have the Microsoft Azure Virtual Desktop Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 33, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-034', 'Solutions_Specializations', N'Does Partner have the Finance Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 34, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-035', 'Solutions_Specializations', N'Does Partner have the Microsoft Low Code Application Development Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 35, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-036', 'Solutions_Specializations', N'Does Partner have the Intelligent Automation Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 36, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-037', 'Solutions_Specializations', N'Does Partner have the Business Intelligence Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 37, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-038', 'Solutions_Specializations', N'Does Partner have the Sales Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 38, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-039', 'Solutions_Specializations', N'Does Partner have the Service Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 39, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-040', 'Solutions_Specializations', N'Does Partner have the Small and Midsize Business Management Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 40, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-041', 'Solutions_Specializations', N'Does Partner have the Supply Chain Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 41, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-042', 'Solutions_Specializations', N'Does Partner have the Adoption and Change Management Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 42, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-043', 'Solutions_Specializations', N'Does Partner have the Calling for Microsoft Teams Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 43, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-044', 'Solutions_Specializations', N'Does Partner have the Copilot Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 44, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-045', 'Solutions_Specializations', N'Does Partner have the Custom Teams Solutions Advanced Specialization Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 45, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-046', 'Solutions_Specializations', N'Does Partner have the Meetings and Meeting Rooms for Microsoft Teams Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 46, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-047', 'Solutions_Specializations', N'Does Partner have the Modernize End Points Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 47, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-048', 'Solutions_Specializations', N'Does Partner have the Teamwork Deployment Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 48, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-049', 'Solutions_Specializations', N'Does Partner have the Cloud Security Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 49, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-050', 'Solutions_Specializations', N'Does Partner have the Identity and Access Management Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 50, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-051', 'Solutions_Specializations', N'Does Partner have the Data Security Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 51, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'SOL-052', 'Solutions_Specializations', N'Does Partner have the Threat Protection Advanced Specialization?', 'Solutions_Specializations', 'Advanced Specializations', 'YesNo', 52, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'CSP-001', 'CSP', N'Is this partner a Direct or Indirect CSP?', 'CSP', 'Program Setup Details', 'Choice', 1, 1, 0, '["Please Choose One","Direct CSP","Indirect CSP","Not a CSP"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'CSP-002', 'CSP', N'If Indirect, who is their Distributor?', 'CSP', 'Program Setup Details', 'Text', 2, 1, 0, NULL);
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'CSP-003', 'CSP', N'Does Partner have a specific segment?', 'CSP', 'Program Setup Details', 'YesNo', 3, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'CSP-004', 'CSP', N'How long has the Partner been a CSP?', 'CSP', 'Program Setup Details', 'Number', 4, 1, 0, NULL);
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'CSP-005', 'CSP', N'What is Partner''s total 12 month CSP revenue?', 'CSP', 'Program Setup Details', 'Number', 5, 1, 0, NULL);
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'CSP-006', 'CSP', N'Does Partner have a dedicated licensing specialist or team?', 'CSP', 'Team Structure', 'YesNo', 6, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'CSP-007', 'CSP', N'Self-Service Portal for licensing changes?', 'CSP', 'Customer Available Options', 'YesNo', 7, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'CSP-008', 'CSP', N'Automated Invoicing?', 'CSP', 'Customer Available Options', 'YesNo', 8, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'CSP-009', 'CSP', N'What platform does Partner use for Invoicing?', 'CSP', 'Customer Available Options', 'Text', 9, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'CSP-010', 'CSP', N'Online Payment option?', 'CSP', 'Customer Available Options', 'YesNo', 10, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'CSP-011', 'CSP', N'Automatic Payments?', 'CSP', 'Customer Available Options', 'YesNo', 11, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'CSP-012', 'CSP', N'Does Partner offer internal support?', 'CSP', 'Customer Available Options', 'YesNo', 12, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'CSP-013', 'CSP', N'Is support capability integrated with the self-service portal?', 'CSP', 'Customer Available Options', 'YesNo', 13, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'CSP-014', 'CSP', N'Does Parter have a Microsoft Support Agreement?', 'CSP', 'Customer Support Details', 'Choice', 14, 1, 0, '["Please Choose One","Premier Support","Advanced Support","None"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'CSP-015', 'CSP', N'Does Partner offer any custom services on top of licensing?', 'CSP', 'Customer Support Details', 'YesNo', 15, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'CSP-016', 'CSP', N'How many customers does Partner have under CSP?', 'CSP', 'Sales & Growth Details', 'Number', 16, 1, 0, NULL);
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'CSP-017', 'CSP', N'Net new customers in the past 3 months', 'CSP', 'Sales & Growth Details', 'Number', 17, 1, 0, NULL);
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'CSP-018', 'CSP', N'Net new customers in the past 6 months', 'CSP', 'Sales & Growth Details', 'Number', 18, 1, 0, NULL);
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'CSP-019', 'CSP', N'Net new customers in the past 12 months', 'CSP', 'Sales & Growth Details', 'Number', 19, 1, 0, NULL);
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'CSP-020', 'CSP', N'Net new customers in the past 24 months', 'CSP', 'Sales & Growth Details', 'Number', 20, 1, 0, NULL);
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'CSP-021', 'CSP', N'What is the average Commitment length?', 'CSP', 'Sales & Growth Details', 'Choice', 21, 1, 0, '["Please Choose One","Monthly Commit","Yearly Commit","3-year Commit"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'CSP-022', 'CSP', N'What is the average CSP Customer tenure?', 'CSP', 'Sales & Growth Details', 'Choice', 22, 1, 0, '["Please Choose One","1 year or less","2 Years","3 or More Years"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'COSELL-001', 'CoSell', N'Is Partner currently submitting Co-Sell Referrals to Microsoft?', 'CoSell', 'Referral Process Overview', 'YesNo', 1, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'COSELL-002', 'CoSell', N'What type of referrals is Partner submitting?', 'CoSell', 'Referral Process Overview', 'Choice', 2, 1, 0, '["Please Choose One","IP Co-Sell","Service Co-Sell","None","Both"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'COSELL-003', 'CoSell', N'Does Partner have an automated method of submitted referrals?', 'CoSell', 'Referral Process Overview', 'YesNo', 3, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'COSELL-004', 'CoSell', N'If yes, what solution are they using?', 'CoSell', 'Referral Process Overview', 'Text', 4, 1, 0, NULL);
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'COSELL-005', 'CoSell', N'Is partner resubmitting expired referrals?', 'CoSell', 'Referral Process Overview', 'YesNo', 5, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'COSELL-006', 'CoSell', N'Does Partner have a responsible person for submitting referrals?', 'CoSell', 'Team Structure', 'YesNo', 6, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'COSELL-007', 'CoSell', N'Is this process documented as a standard SOP?', 'CoSell', 'Team Structure', 'YesNo', 7, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'COSELL-008', 'CoSell', N'Is Partner receiving inbound leads from Microsoft?', 'CoSell', 'Inbound Leads', 'YesNo', 8, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'COSELL-009', 'CoSell', N'If yes, How Many in the past 12 months?', 'CoSell', 'Inbound Leads', 'Number', 9, 1, 0, NULL);
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'COSELL-010', 'CoSell', N'Does Partner have a responsible person for accepting leads?', 'CoSell', 'Inbound Leads', 'YesNo', 10, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'COSELL-011', 'CoSell', N'Is this process documented as a standard SOP?', 'CoSell', 'Inbound Leads', 'YesNo', 11, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'COSELL-012', 'CoSell', N'Does Partner have an automated method for syncing leads to CRM?', 'CoSell', 'Inbound Leads', 'YesNo', 12, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'COSELL-013', 'CoSell', N'If yes, what solution are they using?', 'CoSell', 'Inbound Leads', 'Text', 13, 1, 0, NULL);
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'COSELL-014', 'CoSell', N'Is Partner doing P2P business?', 'CoSell', 'P2P Referrals', 'YesNo', 14, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'COSELL-015', 'CoSell', N'If yes, is Partner sharing P2P deals in Partner center?', 'CoSell', 'P2P Referrals', 'YesNo', 15, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'COSELL-016', 'CoSell', N'If yes, are inbound deals being accepted in Partner Center?', 'CoSell', 'P2P Referrals', 'YesNo', 16, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'COSELL-017', 'CoSell', N'Does Partner have a responsible person for accepting P2P referrals?', 'CoSell', 'P2P Referrals', 'YesNo', 17, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'COSELL-018', 'CoSell', N'Is this process documented as a standard SOP?', 'CoSell', 'P2P Referrals', 'YesNo', 18, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'COSELL-019', 'CoSell', N'How Many Referrals has partner submitted?', 'CoSell', 'P2P Referrals', 'Number', 19, 1, 0, NULL);
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'COSELL-020', 'CoSell', N'New Referrals in the past 3 months', 'CoSell', 'P2P Referrals', 'Number', 20, 1, 0, NULL);
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'COSELL-021', 'CoSell', N'New Referrals in the past 6 months', 'CoSell', 'P2P Referrals', 'Number', 21, 1, 0, NULL);
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'COSELL-022', 'CoSell', N'New Referrals in the past 12 months', 'CoSell', 'P2P Referrals', 'Number', 22, 1, 0, NULL);
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'COSELL-023', 'CoSell', N'New Referrals in the past 24 months', 'CoSell', 'P2P Referrals', 'Number', 23, 1, 0, NULL);
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'COSELL-024', 'CoSell', N'How many Referrals has Microsoft accepted?', 'CoSell', 'P2P Referrals', 'Number', 24, 1, 0, NULL);
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'COSELL-025', 'CoSell', N'Is Partner tracking deals submitted versus not submitted?', 'CoSell', 'P2P Referrals', 'YesNo', 25, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'COSELL-026', 'CoSell', N'If yes, how are they being tracked?', 'CoSell', 'P2P Referrals', 'Text', 26, 1, 0, NULL);
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'MKT-001', 'MarketPlace', N'Does Partner have a Publisher ID?', 'MarketPlace', 'Marketplace Setup', 'YesNo', 1, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'MKT-002', 'MarketPlace', N'Is Partner working on publishing new offers in the marketplace?', 'MarketPlace', 'Marketplace Setup', 'YesNo', 2, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'MKT-003', 'MarketPlace', N'Does Partner have someone dedicated to building and updating offers?', 'MarketPlace', 'Team Structure', 'YesNo', 3, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'MKT-004', 'MarketPlace', N'Does partner have any co-branded offers?', 'MarketPlace', 'P2P Offerings', 'YesNo', 4, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'MKT-005', 'MarketPlace', N'How Many Marketplace Offers does Partner have?', 'MarketPlace', 'Marketplace Offers', 'Number', 5, 1, 0, NULL);
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'MKT-006', 'MarketPlace', N'How many offers have been created in the past 3 months?', 'MarketPlace', 'Marketplace Offers', 'Number', 6, 1, 0, NULL);
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'MKT-007', 'MarketPlace', N'How many offers have been created in the past 6 months?', 'MarketPlace', 'Marketplace Offers', 'Number', 7, 1, 0, NULL);
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'MKT-008', 'MarketPlace', N'How many offers have been created in the past 12 months', 'MarketPlace', 'Marketplace Offers', 'Number', 8, 1, 0, NULL);
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'MKT-009', 'MarketPlace', N'Does partner have any Co-Sell ready offers?', 'MarketPlace', 'Marketplace Offers', 'YesNo', 9, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'MKT-010', 'MarketPlace', N'Does partner hae any Transactable offers?', 'MarketPlace', 'Marketplace Offers', 'YesNo', 10, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'MKT-011', 'MarketPlace', N'Is partner utilizing private or resell offers?', 'MarketPlace', 'Marketplace Offers', 'YesNo', 11, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'MKT-012', 'MarketPlace', N'Does Partner review their offers regularly for fit?', 'MarketPlace', 'Marketplace Offers', 'YesNo', 12, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'INC-001', 'Incentives', N'Has Partner activated any Benefits', 'Incentives', 'Referral Process Overview', 'YesNo', 1, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'INC-002', 'Incentives', N'Has Partner enrolled in MCI Programs', 'Incentives', 'Referral Process Overview', 'YesNo', 2, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'INC-003', 'Incentives', N'Is Partner using MCI Programs', 'Incentives', 'Referral Process Overview', 'YesNo', 3, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'INC-004', 'Incentives', N'Is Partner using Coop Funds', 'Incentives', 'Referral Process Overview', 'YesNo', 4, 1, 0, '["Please Choose One","Yes","No"]');
INSERT INTO dbo.AssessmentQuestions
  (Id, QuestionId, Category, QuestionText, Section, Subsection, AnswerType, DisplayOrder, IsActive, Score, OptionsJson)
VALUES
  (NEWID(), 'INC-005', 'Incentives', N'Is Partner a registered supplier?', 'Incentives', 'Referral Process Overview', 'YesNo', 5, 1, 0, '["Please Choose One","Yes","No"]');
COMMIT;
SELECT Category, AnswerType, COUNT(*) AS Cnt FROM dbo.AssessmentQuestions GROUP BY Category, AnswerType ORDER BY Category, AnswerType;
SELECT COUNT(*) AS TotalQuestions FROM dbo.AssessmentQuestions;


-- Post-fix: free-text questions mis-tagged YesNo in Excel data validation
UPDATE dbo.AssessmentQuestions
SET AnswerType = N'Text', OptionsJson = NULL
WHERE QuestionText IN (
    N'What platform does Partner use for Invoicing?',
    N'Does Partner have a specific segment?'
);
