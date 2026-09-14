public class AssessmentQuestionScoreDto
{
    public string Section { get; set; } = string.Empty;
    public string? Subsection { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int ScoreMax { get; set; }
    public int? ScoreValue { get; set; }
}
