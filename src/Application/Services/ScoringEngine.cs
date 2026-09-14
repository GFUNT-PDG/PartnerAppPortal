using PartnerAppPortal.Core.Interfaces.Services;
using PartnerAppPortal.Core.Models.Assessments;

namespace PartnerAppPortal.Application.Services;

public class ScoringEngine : IScoringEngine
{
    public AssessmentResult CalculateScores(AssessmentSubmission submission)
    {
        var sectionScores = new List<AssessmentScore>();
        int total = 0;

        foreach (var section in submission.Answers)
        {
            // Simple scoring: each answered question = 1 point
            var score = section.Value.Count;

            sectionScores.Add(new AssessmentScore
            {
                Section = section.Key,
                Score = score
            });

            total += score;
        }

        return new AssessmentResult
        {
            TotalScore = total,
            SectionScores = sectionScores
        };
    }
}
