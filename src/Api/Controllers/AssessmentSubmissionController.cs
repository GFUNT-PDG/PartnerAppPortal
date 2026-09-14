using Microsoft.AspNetCore.Mvc;
using PartnerAppPortal.Core.Interfaces.Services;
using PartnerAppPortal.Core.Models.Assessments;

namespace PartnerAppPortal.Api.Controllers
{
    [ApiController]
    [Route("api/assessment-submission")]
    public class AssessmentSubmissionController : ControllerBase
    {
        private readonly IAssessmentService _service;

        public AssessmentSubmissionController(IAssessmentService service)
        {
            _service = service;
        }

        /// <summary>
        /// Submits an assessment, calculates scores, and persists results.
        /// Returns the real AssessmentId created in the database.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Submit([FromBody] AssessmentSubmission submission)
        {
            try
            {
                if (submission == null)
                    return BadRequest("Submission payload is required.");

                // Service handles:
                // - scoring
                // - saving assessment
                // - saving scores
                // - returning AssessmentId
                var id = await _service.SubmitAsync(submission);

                return Ok(new { id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Assessment submission failed: {ex.Message}");
            }
        }
    }
}
