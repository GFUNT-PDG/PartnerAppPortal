using PartnerAppPortal.Core.Models.Assessments;

namespace PartnerAppPortal.Core.Interfaces.Services;

public interface IScoringEngine
{
    AssessmentResult CalculateScores(AssessmentSubmission submission);
}
