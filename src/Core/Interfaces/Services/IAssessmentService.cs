using PartnerAppPortal.Core.Models.Assessments;

namespace PartnerAppPortal.Core.Interfaces.Services;

public interface IAssessmentService
{
    Task<Guid> SubmitAsync(AssessmentSubmission submission);
}
