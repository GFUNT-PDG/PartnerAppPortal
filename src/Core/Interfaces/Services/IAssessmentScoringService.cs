using PartnerAppPortal.Core.Models.Assessments;

namespace PartnerAppPortal.Core.Interfaces.Services
{
    public interface IAssessmentScoringService
    {
        Task<PartnerScoreResult> ScoreAssessmentAsync(Guid assessmentId, CancellationToken cancellationToken = default);
    }
}
