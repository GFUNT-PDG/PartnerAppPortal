namespace PartnerAppPortal.Core.Models.Assessments
{
    public class AssessmentQuestion
    {
        // SQL: uniqueidentifier (NOT NULL)
        public Guid Id { get; set; }

        // SQL: bit (NOT NULL)
        public bool IsActive { get; set; }

        // SQL: nvarchar(100) (NOT NULL)
        public required string QuestionId { get; set; }

        // SQL: nvarchar(max) (NOT NULL)
        public required string QuestionText { get; set; }

        // SQL: nvarchar(200) (NOT NULL)
        public required string Section { get; set; }

        // ⭐ Additional NOT NULL columns in SQL — MUST exist in EF

        // SQL: nvarchar(200) (NOT NULL)
        public required string Category { get; set; }

        // SQL: nvarchar(100) (NOT NULL)
        public required string AnswerType { get; set; }

        // SQL: int (NOT NULL)
        public int DisplayOrder { get; set; }

        // SQL: int (NOT NULL)
        public int Score { get; set; }

        // ⭐ Nullable columns — safe to mark nullable in EF

        // SQL: nvarchar(200) (NULL)
        public string? Subsection { get; set; }

        // SQL: nvarchar(max) (NULL)
        public string? OptionsJson { get; set; }
       // public string ResponseType { get; set; } = string.Empty;

        //public int MaxScore { get; set; }

        //public string? Notes { get; set; }

        public string? Response { get; set; }
public string? ResponseType { get; set; }

    }
}
