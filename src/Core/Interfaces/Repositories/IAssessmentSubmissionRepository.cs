using PartnerAppPortal.Core.Models.Assessments;

namespace PartnerAppPortal.Core.Interfaces.Repositories;

public interface IAssessmentSubmissionRepository
{
    Task<Guid> SaveAssessmentAsync(AssessmentSubmission submission, AssessmentResult result);
}
