using System.Net.Http.Json;
using PartnerAppPortal.Core.Models.Assessments;
using PartnerAppPortal.Core.Models.Crm;

namespace PartnerAppPortal.Portal.Services
{
    public class AssessmentApiClient : IAssessmentApiClient
    {
        private readonly HttpClient _http;

        public AssessmentApiClient(HttpClient http)
        {
            _http = http;
        }

        private static async Task EnsureSuccessAsync(HttpResponseMessage response, string action)
        {
            if (response.IsSuccessStatusCode)
                return;

            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"{action} failed ({(int)response.StatusCode}): {body}");
        }

        private async Task<T?> GetJsonOrDefaultAsync<T>(string url)
        {
            var response = await _http.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return default;

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return default;

            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content) || content == "null")
                return default;

            return System.Text.Json.JsonSerializer.Deserialize<T>(content,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<List<Customer>> GetCustomersAsync()
        {
            var result = await _http.GetFromJsonAsync<List<Customer>>("api/crm/customers");
            return result ?? new List<Customer>();
        }

        public async Task<Customer?> FindCustomerByEmailAsync(string email)
        {
            var encoded = Uri.EscapeDataString(email);
            var response = await _http.GetAsync($"api/crm/customers/by-email?email={encoded}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            await EnsureSuccessAsync(response, "Find customer by email");
            return await response.Content.ReadFromJsonAsync<Customer>();
        }

        public async Task<List<AssessmentModel>> GetAssessmentsByCustomerAsync(Guid customerId)
        {
            return await GetJsonOrDefaultAsync<List<AssessmentModel>>(
                       $"api/assessments/customer/{customerId}")
                   ?? new List<AssessmentModel>();
        }

        public async Task<IReadOnlyList<AssessmentQuestion>> GetQuestionsAsync()
        {
            return await GetJsonOrDefaultAsync<List<AssessmentQuestion>>("api/assessments/questions")
                   ?? new List<AssessmentQuestion>();
        }

        public async Task<IReadOnlyList<AssessmentQuestion>> GetQuestionsBySectionAsync(string section)
        {
            var encoded = Uri.EscapeDataString(section);
            return await GetJsonOrDefaultAsync<List<AssessmentQuestion>>(
                       $"api/assessments/questions/section/{encoded}")
                   ?? new List<AssessmentQuestion>();
        }

        public async Task<IReadOnlyList<AssessmentQuestion>> GetQuestionsByCategoryAsync(string category)
        {
            var encoded = Uri.EscapeDataString(category);
            return await GetJsonOrDefaultAsync<List<AssessmentQuestion>>(
                       $"api/assessments/questions/category/{encoded}")
                   ?? new List<AssessmentQuestion>();
        }

        public async Task<AssessmentEntity> GetAssessmentByIdAsync(Guid id)
        {
            var response = await _http.GetAsync($"api/assessments/{id}");
            await EnsureSuccessAsync(response, "Get assessment");
            return await response.Content.ReadFromJsonAsync<AssessmentEntity>()
                   ?? new AssessmentEntity();
        }

        public async Task<Customer> GetCustomerByNameAsync(string nameOrId)
        {
            var customers = await GetCustomersAsync();

            if (Guid.TryParse(nameOrId, out var id))
            {
                var byId = customers.FirstOrDefault(c => c.Id == id);
                if (byId != null)
                    return byId;
            }

            return customers.FirstOrDefault(c =>
                       string.Equals(c.Name, nameOrId, StringComparison.OrdinalIgnoreCase))
                   ?? new Customer { Name = nameOrId, AccountId = nameOrId };
        }

        public Task<PartnerInfo?> GetPartnerInfoAsync(Guid id) =>
            GetJsonOrDefaultAsync<PartnerInfo>($"api/assessments/{id}/partner-info");

        public async Task SavePartnerInfoAsync(Guid id, PartnerInfo model)
        {
            var response = await _http.PutAsJsonAsync($"api/assessments/{id}/partner-info", model);
            await EnsureSuccessAsync(response, "Save partner info");
        }

        public async Task<List<AssessmentAnswerDto>> GetAnswersByAssessmentAsync(Guid id) =>
            await GetJsonOrDefaultAsync<List<AssessmentAnswerDto>>($"api/assessments/{id}/scores")
            ?? new List<AssessmentAnswerDto>();

        public async Task SaveScoresAsync(Guid id, IEnumerable<AssessmentQuestion> questions)
        {
            var response = await _http.PostAsJsonAsync($"api/assessments/{id}/scores", questions);
            await EnsureSuccessAsync(response, "Save answers");
        }

        public Task<CustomersCsp?> GetCustomersCspAsync(Guid id) =>
            GetJsonOrDefaultAsync<CustomersCsp>($"api/assessments/{id}/csp");

        public async Task SaveCustomersCspAsync(Guid id, CustomersCsp model)
        {
            var response = await _http.PutAsJsonAsync($"api/assessments/{id}/csp", model);
            await EnsureSuccessAsync(response, "Save CSP");
        }

        public Task<CoSellReferrals?> GetCoSellReferralsAsync(Guid id) =>
            GetJsonOrDefaultAsync<CoSellReferrals>($"api/assessments/{id}/co-sell");

        public async Task SaveCoSellReferralsAsync(Guid id, CoSellReferrals model)
        {
            var response = await _http.PutAsJsonAsync($"api/assessments/{id}/co-sell", model);
            await EnsureSuccessAsync(response, "Save co-sell");
        }

        public Task<MarketPlaceUsage?> GetMarketplaceUsageAsync(Guid id) =>
            GetJsonOrDefaultAsync<MarketPlaceUsage>($"api/assessments/{id}/marketplace");

        public async Task SaveMarketplaceUsageAsync(Guid id, MarketPlaceUsage model)
        {
            var response = await _http.PutAsJsonAsync($"api/assessments/{id}/marketplace", model);
            await EnsureSuccessAsync(response, "Save marketplace");
        }

        public Task<IncentivesUsage?> GetIncentivesUsageAsync(Guid id) =>
            GetJsonOrDefaultAsync<IncentivesUsage>($"api/assessments/{id}/incentives");

        public async Task SaveIncentivesUsageAsync(Guid id, IncentivesUsage model)
        {
            var response = await _http.PutAsJsonAsync($"api/assessments/{id}/incentives", model);
            await EnsureSuccessAsync(response, "Save incentives");
        }

        public Task<GrowthMetrics?> GetGrowthMetricsAsync(Guid id) =>
            GetJsonOrDefaultAsync<GrowthMetrics>($"api/assessments/{id}/growth");

        public async Task SaveGrowthMetricsAsync(Guid id, GrowthMetrics model)
        {
            var response = await _http.PutAsJsonAsync($"api/assessments/{id}/growth", model);
            await EnsureSuccessAsync(response, "Save growth");
        }

        public async Task CreateAssessmentAsync(AssessmentRequest request)
        {
            var response = await _http.PostAsJsonAsync("api/assessments", request);
            await EnsureSuccessAsync(response, "Create assessment");
        }

        public async Task DeleteAssessmentAsync(Guid id)
        {
            var response = await _http.DeleteAsync($"api/assessments/{id}");
            await EnsureSuccessAsync(response, "Delete assessment");
        }

        public async Task<Dictionary<string, List<AssessmentReviewItem>>> GetAssessmentSummaryAsync(Guid id)
        {
            var response = await _http.GetAsync($"api/assessments/{id}/summary");
            await EnsureSuccessAsync(response, "Get assessment summary");
            return await response.Content.ReadFromJsonAsync<Dictionary<string, List<AssessmentReviewItem>>>()
                   ?? new Dictionary<string, List<AssessmentReviewItem>>();
        }

        public async Task<SubmitResultModel> SubmitAssessmentAsync(Guid id)
        {
            var response = await _http.PostAsJsonAsync($"api/assessments/{id}/submit", new { });
            await EnsureSuccessAsync(response, "Submit assessment");
            return await response.Content.ReadFromJsonAsync<SubmitResultModel>()
                   ?? new SubmitResultModel();
        }

        public async Task<AssessmentFinalResult> GetFinalResultAsync(Guid id)
        {
            var response = await _http.GetAsync($"api/assessments/{id}/final");
            await EnsureSuccessAsync(response, "Get final result");
            return await response.Content.ReadFromJsonAsync<AssessmentFinalResult>()
                   ?? new AssessmentFinalResult();
        }
    }
}
