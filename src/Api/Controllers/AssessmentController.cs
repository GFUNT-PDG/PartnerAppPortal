using Microsoft.AspNetCore.Mvc;
using PartnerAppPortal.Core.Interfaces.Repositories;
using PartnerAppPortal.Core.Models.Assessments;

namespace PartnerAppPortal.Api.Controllers
{
    [ApiController]
    [Route("api/assessments")]
    public class AssessmentController : ControllerBase
    {
        private readonly IAssessmentRepository _repo;

        public AssessmentController(IAssessmentRepository repo)
        {
            _repo = repo;
        }

        // ------------------------------------------------------------
        // EXISTING ENDPOINTS (UNCHANGED)
        // ------------------------------------------------------------

        // GET: api/assessments/questions
        [HttpGet("questions")]
        public async Task<IActionResult> GetQuestions()
        {
            try
            {
                var questions = await _repo.GetQuestionsAsync();
                return Ok(questions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"GetQuestions failed: {ex.Message}");
            }
        }

        // GET: api/assessments/questions/section/{section}
        [HttpGet("questions/section/{section}")]
        public async Task<IActionResult> GetQuestionsBySection(string section)
        {
            try
            {
                var questions = await _repo.GetQuestionsBySectionAsync(section);

                if (questions == null || !questions.Any())
                    return Ok(Array.Empty<AssessmentQuestion>());

                return Ok(questions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"GetQuestionsBySection failed: {ex.Message}");
            }
        }

        // GET: api/assessments/questions/category/{category}
        [HttpGet("questions/category/{category}")]
        public async Task<IActionResult> GetQuestionsByCategory(string category)
        {
            try
            {
                var questions = await _repo.GetQuestionsByCategoryAsync(category);
                return Ok(questions ?? Array.Empty<AssessmentQuestion>());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"GetQuestionsByCategory failed: {ex.Message}");
            }
        }

        // ------------------------------------------------------------
        // NEW WIZARD ENDPOINTS (ADD ONLY — NO BREAKING CHANGES)
        // ------------------------------------------------------------

        // GET: api/assessments/{id}/partner-info
        [HttpGet("{id:guid}/partner-info")]
        public async Task<IActionResult> GetPartnerInfo(Guid id)
        {
            try
            {
                var info = await _repo.GetPartnerInfoAsync(id);
                return Ok(info ?? new PartnerInfoModel { AssessmentId = id });
            }
            catch (Exception ex)
            {
                var detail = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"GetPartnerInfo failed: {detail}");
            }
        }

        // PUT: api/assessments/{id}/partner-info
        [HttpPut("{id:guid}/partner-info")]
        public async Task<IActionResult> SavePartnerInfo(Guid id, PartnerInfoModel model)
        {
            try
            {
                await _repo.SavePartnerInfoAsync(id, model);
                return NoContent();
            }
            catch (Exception ex)
            {
                var detail = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"SavePartnerInfo failed: {detail}");
            }
        }

        // GET: api/assessments/{id}/csp
        [HttpGet("{id:guid}/csp")]
        public async Task<IActionResult> GetCsp(Guid id)
        {
            try
            {
                var data = await _repo.GetCustomersCspAsync(id);
                return Ok(data ?? new CustomersCspModel { AssessmentId = id });
            }
            catch (Exception ex)
            {
                var detail = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"GetCsp failed: {detail}");
            }
        }

        // PUT: api/assessments/{id}/csp
        [HttpPut("{id:guid}/csp")]
        public async Task<IActionResult> SaveCsp(Guid id, CustomersCspModel model)
        {
            try
            {
                await _repo.SaveCustomersCspAsync(id, model);
                return NoContent();
            }
            catch (Exception ex)
            {
                var detail = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"SaveCsp failed: {detail}");
            }
        }

        // GET: api/assessments/{id}/co-sell
        [HttpGet("{id:guid}/co-sell")]
        public async Task<IActionResult> GetCoSell(Guid id)
        {
            try
            {
                var data = await _repo.GetCoSellReferralsAsync(id);
                return Ok(data ?? new CoSellReferralsModel { AssessmentId = id });
            }
            catch (Exception ex)
            {
                var detail = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"GetCoSell failed: {detail}");
            }
        }

        // PUT: api/assessments/{id}/co-sell
        [HttpPut("{id:guid}/co-sell")]
        public async Task<IActionResult> SaveCoSell(Guid id, CoSellReferralsModel model)
        {
            try
            {
                await _repo.SaveCoSellReferralsAsync(id, model);
                return NoContent();
            }
            catch (Exception ex)
            {
                var detail = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"SaveCoSell failed: {detail}");
            }
        }

        // GET: api/assessments/{id}/marketplace
        [HttpGet("{id:guid}/marketplace")]
        public async Task<IActionResult> GetMarketplace(Guid id)
        {
            try
            {
                var data = await _repo.GetMarketplaceUsageAsync(id);
                return Ok(data ?? new MarketPlaceUsageModel { AssessmentId = id });
            }
            catch (Exception ex)
            {
                var detail = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"GetMarketplace failed: {detail}");
            }
        }

        // PUT: api/assessments/{id}/marketplace
        [HttpPut("{id:guid}/marketplace")]
        public async Task<IActionResult> SaveMarketplace(Guid id, MarketPlaceUsageModel model)
        {
            try
            {
                await _repo.SaveMarketplaceUsageAsync(id, model);
                return NoContent();
            }
            catch (Exception ex)
            {
                var detail = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"SaveMarketplace failed: {detail}");
            }
        }

        // GET: api/assessments/{id}/incentives
        [HttpGet("{id:guid}/incentives")]
        public async Task<IActionResult> GetIncentives(Guid id)
        {
            try
            {
                var data = await _repo.GetIncentivesUsageAsync(id);
                return Ok(data ?? new IncentivesUsageModel { AssessmentId = id });
            }
            catch (Exception ex)
            {
                var detail = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"GetIncentives failed: {detail}");
            }
        }

        // PUT: api/assessments/{id}/incentives
        [HttpPut("{id:guid}/incentives")]
        public async Task<IActionResult> SaveIncentives(Guid id, IncentivesUsageModel model)
        {
            try
            {
                await _repo.SaveIncentivesUsageAsync(id, model);
                return NoContent();
            }
            catch (Exception ex)
            {
                var detail = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"SaveIncentives failed: {detail}");
            }
        }

        // GET: api/assessments/{id}/growth
        [HttpGet("{id:guid}/growth")]
        public async Task<IActionResult> GetGrowth(Guid id)
        {
            try
            {
                var data = await _repo.GetGrowthMetricsAsync(id);
                return Ok(data ?? new GrowthMetricsModel { AssessmentId = id });
            }
            catch (Exception ex)
            {
                var detail = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"GetGrowth failed: {detail}");
            }
        }

        // PUT: api/assessments/{id}/growth
        [HttpPut("{id:guid}/growth")]
        public async Task<IActionResult> SaveGrowth(Guid id, GrowthMetricsModel model)
        {
            try
            {
                await _repo.SaveGrowthMetricsAsync(id, model);
                return NoContent();
            }
            catch (Exception ex)
            {
                var detail = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"SaveGrowth failed: {detail}");
            }
        }

        // GET: api/assessments/{id}/summary
        [HttpGet("{id:guid}/summary")]
        public async Task<IActionResult> GetSummary(Guid id)
        {
            try
            {
                var summary = await _repo.GetAssessmentSummaryAsync(id);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                var detail = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"GetSummary failed: {detail}");
            }
        }

        // POST: api/assessments/{id}/submit
        [HttpPost("{id:guid}/submit")]
        public async Task<IActionResult> Submit(Guid id)
        {
            try
            {
                var result = await _repo.SubmitAssessmentAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var detail = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"Submit failed: {detail}");
            }
        }

        // GET: api/assessments/{id}/scores
        [HttpGet("{id:guid}/scores")]
        public async Task<IActionResult> GetScores(Guid id)
        {
            try
            {
                var answers = await _repo.GetAssessmentAnswersAsync(id);
                return Ok(answers);
            }
            catch (Exception ex)
            {
                var detail = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"GetScores failed: {detail}");
            }
        }

        // POST: api/assessments/{id}/scores
        [HttpPost("{id:guid}/scores")]
        public async Task<IActionResult> SaveScores(Guid id, [FromBody] List<AssessmentQuestion> questions)
        {
            try
            {
                await _repo.SaveAssessmentAnswersAsync(id, questions ?? new List<AssessmentQuestion>());
                return NoContent();
            }
            catch (Exception ex)
            {
                var detail = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"SaveScores failed: {detail}");
            }
        }

        // GET: api/assessments/{id}/final
        [HttpGet("{id:guid}/final")]
        public async Task<IActionResult> GetFinal(Guid id)
        {
            try
            {
                var result = await _repo.GetFinalResultAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"GetFinal failed: {ex.Message}");
            }
        }

        // GET: api/assessments/customer/{customerId}
        [HttpGet("customer/{customerId:guid}")]
        public async Task<IActionResult> GetByCustomer(Guid customerId)
        {
            try
            {
                var list = await _repo.GetAssessmentsByCustomerAsync(customerId);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"GetByCustomer failed: {ex.Message}");
            }
        }

        // GET: api/assessments/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var assessment = await _repo.GetByIdAsync(id);
                return assessment is null ? NotFound() : Ok(assessment);
            }
            catch (Exception ex)
            {
                var detail = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"GetById failed: {detail}");
            }
        }

        // POST: api/assessments
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AssessmentRequest request)
        {
            try
            {
                var id = await _repo.CreateAssessmentAsync(request);
                return Ok(new { id });
            }
            catch (Exception ex)
            {
                var detail = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"CreateAssessment failed: {detail}");
            }
        }

        // DELETE: api/assessments/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var deleted = await _repo.DeleteAssessmentAsync(id);
                return deleted ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                var detail = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"DeleteAssessment failed: {detail}");
            }
        }
    }
}
