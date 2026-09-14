using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PartnerAppPortal.Core.Interfaces.Repositories;
using PartnerAppPortal.Core.Models.Assessments;
using PartnerAppPortal.Core.Interfaces.Services;

namespace PartnerAppPortal.Core.Interfaces.Services

{
    public class AssessmentScoringService : IAssessmentScoringService
    {
        private readonly IAssessmentRepository _assessmentRepository;

        public AssessmentScoringService(IAssessmentRepository assessmentRepository)
        {
            _assessmentRepository = assessmentRepository;
        }

        public async Task<PartnerScoreResult> ScoreAssessmentAsync(
            Guid assessmentId,
            CancellationToken cancellationToken = default)
        {
            var assessment = await _assessmentRepository
                .GetAssessmentWithResponsesAsync(assessmentId);

            if (assessment == null)
                throw new InvalidOperationException($"Assessment {assessmentId} not found.");

            var solutions = ScoreCategory(assessment, "Solutions_Specializations");
            var customers = ScoreCategory(assessment, "Customers_CSP");
            var coSell = ScoreCategory(assessment, "CoSell_Referrals");
            var marketplace = ScoreCategory(assessment, "Marketplace_Usage");
            var incentives = ScoreCategory(assessment, "Incentives_Usage");

            var overall = Math.Round(new[]
            {
                solutions.Percentage,
                customers.Percentage,
                coSell.Percentage,
                marketplace.Percentage,
                incentives.Percentage
            }.Average(), 2);

            return new PartnerScoreResult
            {
                AssessmentId = assessmentId,
                SolutionsAndSpecializations = solutions,
                CustomersAndCsp = customers,
                CoSellAndReferrals = coSell,
                MarketplaceUsage = marketplace,
                IncentivesUsage = incentives,
                OverallPartnerScore = (decimal)overall

            };
        }

        private CategoryScoreResult ScoreCategory(AssessmentEntity assessment, string section)
        {
            var responses = assessment.Responses?
                .Where(r => r.Section == section)
                .ToList();

            if (responses == null || responses.Count == 0)
            {
                return new CategoryScoreResult
                {
                    CategoryName = section.Replace("_", " "),
                    TotalPointsEarned = 0,
                    PotentialPoints = 0,
                    Percentage = 0
                };
            }

            var totalPoints = responses.Sum(r => r.Score ?? 0);
            var potentialPoints = responses.Sum(r => r.MaxScore);
            var percentage = potentialPoints == 0
                ? 0
                : Math.Round((double)totalPoints / potentialPoints * 100, 2);

            return new CategoryScoreResult
            {
                CategoryName = section.Replace("_", " "),
                TotalPointsEarned = totalPoints,
                PotentialPoints = potentialPoints,
                Percentage = percentage
            };
        }
    }
}
