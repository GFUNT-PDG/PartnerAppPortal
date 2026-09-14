namespace PartnerAppPortal.Portal.Services;

public class WizardState
{
    public Guid AssessmentId { get; set; }
    public string CustomerId { get; set; } = string.Empty;

    public Dictionary<string, Dictionary<string, string>> Answers { get; } = new();

    public void SetAnswer(string section, string questionId, string value)
    {
        if (!Answers.ContainsKey(section))
            Answers[section] = new Dictionary<string, string>();

        Answers[section][questionId] = value;
    }

    public string? GetAnswer(string section, string questionId)
    {
        return Answers.TryGetValue(section, out var sec)
            && sec.TryGetValue(questionId, out var val)
            ? val
            : null;
    }
}
