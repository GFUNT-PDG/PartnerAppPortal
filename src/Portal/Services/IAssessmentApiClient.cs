using PartnerAppPortal.Core.Models.Assessments;
using PartnerAppPortal.Core.Models.Crm;

namespace PartnerAppPortal.Portal.Services
{
    public interface IAssessmentApiClient
    {
        Task<List<AssessmentModel>> GetAssessmentsByCustomerAsync(Guid customerId);
        Task<IReadOnlyList<AssessmentQuestion>> GetQuestionsAsync();
        Task<IReadOnlyList<AssessmentQuestion>> GetQuestionsBySectionAsync(string section);
        Task<IReadOnlyList<AssessmentQuestion>> GetQuestionsByCategoryAsync(string category);

        Task<AssessmentEntity> GetAssessmentByIdAsync(Guid id);

        Task<Customer> GetCustomerByNameAsync(string name);

        Task<PartnerInfo?> GetPartnerInfoAsync(Guid id);
        Task SavePartnerInfoAsync(Guid id, PartnerInfo model);

        Task<List<AssessmentAnswerDto>> GetAnswersByAssessmentAsync(Guid id);
        Task SaveScoresAsync(Guid id, IEnumerable<AssessmentQuestion> questions);

        Task<CustomersCsp?> GetCustomersCspAsync(Guid id);
        Task SaveCustomersCspAsync(Guid id, CustomersCsp model);

        Task<CoSellReferrals?> GetCoSellReferralsAsync(Guid id);
        Task SaveCoSellReferralsAsync(Guid id, CoSellReferrals model);

        Task<MarketPlaceUsage?> GetMarketplaceUsageAsync(Guid id);
        Task SaveMarketplaceUsageAsync(Guid id, MarketPlaceUsage model);

        Task<IncentivesUsage?> GetIncentivesUsageAsync(Guid id);
        Task SaveIncentivesUsageAsync(Guid id, IncentivesUsage model);

        Task<GrowthMetrics?> GetGrowthMetricsAsync(Guid id);
        Task SaveGrowthMetricsAsync(Guid id, GrowthMetrics model);

        Task CreateAssessmentAsync(AssessmentRequest request);

        Task DeleteAssessmentAsync(Guid id);

        Task<Dictionary<string, List<AssessmentReviewItem>>> GetAssessmentSummaryAsync(Guid id);

        Task<SubmitResultModel> SubmitAssessmentAsync(Guid id);

        Task<AssessmentFinalResult> GetFinalResultAsync(Guid id);

        // ⭐ REQUIRED FOR CUSTOMER DROPDOWN
        Task<List<Customer>> GetCustomersAsync();

        Task<CrmSyncStartResult> StartCrmSyncAsync();

        /// <summary>Match login email domain to CrmAccounts.Email domain.</summary>
        Task<Customer?> FindCustomerByEmailAsync(string email);
    }
}
