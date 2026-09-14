using PartnerAppPortal.Core.Models.Assessments;

namespace PartnerAppPortal.Core.Interfaces.Repositories
{
    public interface IAssessmentRepository
    {
        // ------------------------------------------------------------
        // EXISTING METHODS (UNCHANGED)
        // ------------------------------------------------------------

        // Returns all active questions (wizard + scoring)
        Task<IReadOnlyList<AssessmentQuestion>> GetQuestionsAsync();

        // Returns active questions filtered by section
        Task<IReadOnlyList<AssessmentQuestion>> GetQuestionsBySectionAsync(string section);

        // Returns active questions filtered by category (wizard step key)
        Task<IReadOnlyList<AssessmentQuestion>> GetQuestionsByCategoryAsync(string category);


        // ------------------------------------------------------------
        // NEW WIZARD METHODS (ADD ONLY — NO BREAKING CHANGES)
        // ------------------------------------------------------------

        // Partner Info
        Task<PartnerInfoModel?> GetPartnerInfoAsync(Guid assessmentId);
        Task SavePartnerInfoAsync(Guid assessmentId, PartnerInfoModel model);

        // Customers & CSP
        Task<CustomersCspModel?> GetCustomersCspAsync(Guid assessmentId);
        Task SaveCustomersCspAsync(Guid assessmentId, CustomersCspModel model);

        // Co-Sell & Referrals
        Task<CoSellReferralsModel?> GetCoSellReferralsAsync(Guid assessmentId);
        Task SaveCoSellReferralsAsync(Guid assessmentId, CoSellReferralsModel model);

        // Marketplace Usage
        Task<MarketPlaceUsageModel?> GetMarketplaceUsageAsync(Guid assessmentId);
        Task SaveMarketplaceUsageAsync(Guid assessmentId, MarketPlaceUsageModel model);

        // Incentives Usage
        Task<IncentivesUsageModel?> GetIncentivesUsageAsync(Guid assessmentId);
        Task SaveIncentivesUsageAsync(Guid assessmentId, IncentivesUsageModel model);

        // Growth Metrics
        Task<GrowthMetricsModel?> GetGrowthMetricsAsync(Guid assessmentId);
        Task SaveGrowthMetricsAsync(Guid assessmentId, GrowthMetricsModel model);

        // Summary (Review & Submit)
        Task<Dictionary<string, List<AssessmentReviewItem>>> GetAssessmentSummaryAsync(Guid assessmentId);
        Task<AssessmentEntity?> GetAssessmentWithResponsesAsync(Guid assessmentId);


        // Submit (final scoring + timestamp)
        Task<SubmitResultModel> SubmitAssessmentAsync(Guid assessmentId);

        // Final result (WizardComplete)
        Task<AssessmentFinalResult> GetFinalResultAsync(Guid assessmentId);

        // Create / load assessments
        Task<Guid> CreateAssessmentAsync(AssessmentRequest request);
        Task<AssessmentEntity?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<AssessmentModel>> GetAssessmentsByCustomerAsync(Guid customerId);
        Task<bool> DeleteAssessmentAsync(Guid assessmentId);

        Task SaveAssessmentAnswersAsync(Guid assessmentId, IEnumerable<AssessmentQuestion> questions);
        Task<IReadOnlyList<AssessmentAnswerDto>> GetAssessmentAnswersAsync(Guid assessmentId);
    }
}
