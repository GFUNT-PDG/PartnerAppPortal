using PartnerAppPortal.Core.Interfaces.Repositories;
using PartnerAppPortal.Core.Interfaces.Services;
using PartnerAppPortal.Core.Models.Assessments;

namespace PartnerAppPortal.Application.Services;

public class AssessmentService : IAssessmentService
{
    private readonly IAssessmentRepository _assessmentRepo;
    private readonly IAssessmentSubmissionRepository _submissionRepo;
    private readonly IScoringEngine _scoring;

    public AssessmentService(
        IAssessmentRepository assessmentRepo,
        IAssessmentSubmissionRepository submissionRepo,
        IScoringEngine scoring)
    {
        _assessmentRepo = assessmentRepo;
        _submissionRepo = submissionRepo;
        _scoring = scoring;
    }

    public async Task<Guid> SubmitAsync(AssessmentSubmission submission)
    {
        // 1. Calculate result using the correct interface method
        var result = _scoring.CalculateScores(submission);

        // 2. Save submission + result using the correct interface signature
        var id = await _submissionRepo.SaveAssessmentAsync(submission, result);

        return id;
    }
}
