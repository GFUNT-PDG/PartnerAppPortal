using Microsoft.AspNetCore.Mvc;
using PartnerAppPortal.Core.Interfaces.Services;
using PartnerAppPortal.Core.Models.Assessments;

namespace PartnerAppPortal.Api.Endpoints.Assessments
{
    [ApiController]
    [Route("api/assessments")]
    public class AssessmentScoringController : ControllerBase
    {
        private readonly IAssessmentScoringService _scoringService;

        public AssessmentScoringController(IAssessmentScoringService scoringService)
        {
            _scoringService = scoringService;
        }

        [HttpPost("{assessmentId:guid}/score")]
        public async Task<ActionResult<PartnerScoreResult>> Score(Guid assessmentId, CancellationToken cancellationToken)
        {
            var result = await _scoringService.ScoreAssessmentAsync(assessmentId, cancellationToken);
            return Ok(result);
        }
    }
}
