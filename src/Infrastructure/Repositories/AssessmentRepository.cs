using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using PartnerAppPortal.Core.Interfaces.Repositories;
using PartnerAppPortal.Core.Models.Assessments;
using PartnerAppPortal.Core.Services.Assessments;
using PartnerAppPortal.Infrastructure.Data;
using PartnerAppPortal.Infrastructure;


namespace PartnerAppPortal.Infrastructure.Repositories
{
    public class AssessmentRepository : IAssessmentRepository
    {
        private readonly PortalDbContext _db;

        public AssessmentRepository(PortalDbContext db)
        {
            _db = db;
        }

        // ------------------------------------------------------------
        // EXISTING METHODS (UNCHANGED)
        // ------------------------------------------------------------

        /// <summary>
        /// Returns ALL active questions, ordered by DisplayOrder.
        /// Fully aligned with SQL schema and EF model.
        /// </summary>
        public async Task<IReadOnlyList<AssessmentQuestion>> GetQuestionsAsync()
        {
            return await _db.AssessmentQuestions
                .AsNoTracking()
                .Where(q => q.IsActive)
                .OrderBy(q => q.DisplayOrder)
                .Select(q => new AssessmentQuestion
                {
                    Id = q.Id,
                    IsActive = q.IsActive,
                    QuestionId = q.QuestionId,
                    QuestionText = q.QuestionText,
                    Section = q.Section,
                    Category = q.Category,
                    AnswerType = q.AnswerType,
                    DisplayOrder = q.DisplayOrder,
                    Score = q.Score,
                    Subsection = q.Subsection,
                    OptionsJson = q.OptionsJson
                })
                .ToListAsync();
        }

        public async Task<IReadOnlyList<AssessmentQuestion>> GetQuestionsBySectionAsync(string section)
        {
            return await _db.AssessmentQuestions
                .AsNoTracking()
                .Where(q => q.IsActive && q.Section == section)
                .OrderBy(q => q.DisplayOrder)
                .Select(q => new AssessmentQuestion
                {
                    Id = q.Id,
                    IsActive = q.IsActive,
                    QuestionId = q.QuestionId,
                    QuestionText = q.QuestionText,
                    Section = q.Section,
                    Category = q.Category,
                    AnswerType = q.AnswerType,
                    DisplayOrder = q.DisplayOrder,
                    Score = q.Score,
                    Subsection = q.Subsection,
                    OptionsJson = q.OptionsJson
                })
                .ToListAsync();
        }

        public async Task<IReadOnlyList<AssessmentQuestion>> GetQuestionsByCategoryAsync(string category)
        {
            return await _db.AssessmentQuestions
                .AsNoTracking()
                .Where(q => q.IsActive && q.Category == category)
                .OrderBy(q => q.DisplayOrder)
                .Select(q => new AssessmentQuestion
                {
                    Id = q.Id,
                    IsActive = q.IsActive,
                    QuestionId = q.QuestionId,
                    QuestionText = q.QuestionText,
                    Section = q.Section,
                    Category = q.Category,
                    AnswerType = q.AnswerType,
                    DisplayOrder = q.DisplayOrder,
                    Score = q.Score,
                    Subsection = q.Subsection,
                    OptionsJson = q.OptionsJson
                })
                .ToListAsync();
        }


        // ------------------------------------------------------------
        // NEW WIZARD METHODS (ADD ONLY â€” NO BREAKING CHANGES)
        // ------------------------------------------------------------

        // -------------------------
        // Partner Info
        // -------------------------
        public async Task<PartnerInfoModel?> GetPartnerInfoAsync(Guid assessmentId)
        {
            try
            {
                return await _db.PartnerInfos
                    .AsNoTracking()
                    .Where(x => x.AssessmentId == assessmentId)
                    .Select(x => new PartnerInfoModel
                    {
                        AssessmentId = x.AssessmentId,
                        PartnerName = x.PartnerName,
                        MpnId = x.MpnId,
                        Country = x.Country,
                        Segment = x.Segment
                    })
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex) when (SqlExceptionHints.IsMissingTable(ex))
            {
                // Table not created yet â€” allow wizard Step 2 to show an empty form
                return null;
            }
        }

        public async Task SavePartnerInfoAsync(Guid assessmentId, PartnerInfoModel model)
        {
            try
            {
                var entity = await _db.PartnerInfos
                    .FirstOrDefaultAsync(x => x.AssessmentId == assessmentId);

                if (entity == null)
                {
                    entity = new PartnerInfo
                    {
                        Id = Guid.NewGuid(),
                        AssessmentId = assessmentId
                    };
                    _db.PartnerInfos.Add(entity);
                }

                entity.PartnerName = model.PartnerName;
                entity.MpnId = model.MpnId;
                entity.Country = model.Country;
                entity.Segment = model.Segment;

                await _db.SaveChangesAsync();
            }
            catch (Exception ex) when (SqlExceptionHints.IsMissingTable(ex))
            {
                throw new InvalidOperationException(
                    "Table dbo.PartnerInfo does not exist. Run sql/CreateWizardTables.sql against pacappdb01.",
                    ex);
            }
        }


        // -------------------------
        // Customers & CSP
        // -------------------------
        public async Task<CustomersCspModel?> GetCustomersCspAsync(Guid assessmentId)
        {
            try
            {
                return await _db.CustomersCsp
                    .AsNoTracking()
                    .Where(x => x.AssessmentId == assessmentId)
                    .Select(x => new CustomersCspModel
                    {
                        AssessmentId = x.AssessmentId,
                        CspTier = x.CspTier,
                        ActiveSubscriptions = x.ActiveSubscriptions,
                        AzureConsumption = x.AzureConsumption,
                        MarketplaceUsage = x.MarketplaceUsage,
                        IncentivesUsage = x.IncentivesUsage
                    })
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex) when (SqlExceptionHints.IsMissingTable(ex))
            {
                return null;
            }
        }

        public async Task SaveCustomersCspAsync(Guid assessmentId, CustomersCspModel model)
        {
            try
            {
                var entity = await _db.CustomersCsp
                    .FirstOrDefaultAsync(x => x.AssessmentId == assessmentId);

                if (entity == null)
                {
                    entity = new CustomersCsp
                    {
                        Id = Guid.NewGuid(),
                        AssessmentId = assessmentId
                    };
                    _db.CustomersCsp.Add(entity);
                }

                entity.CspTier = model.CspTier;
                entity.ActiveSubscriptions = model.ActiveSubscriptions;
                entity.AzureConsumption = model.AzureConsumption;
                entity.MarketplaceUsage = model.MarketplaceUsage;
                entity.IncentivesUsage = model.IncentivesUsage;

                await _db.SaveChangesAsync();
            }
            catch (Exception ex) when (SqlExceptionHints.IsMissingTable(ex))
            {
                throw new InvalidOperationException(
                    "Table dbo.CustomersCsp does not exist. Run sql/CreateWizardTables.sql against pacappdb01.",
                    ex);
            }
        }


        // -------------------------
        // Co-Sell & Referrals
        // -------------------------
        public async Task<CoSellReferralsModel?> GetCoSellReferralsAsync(Guid assessmentId)
        {
            try
            {
                return await _db.CoSellReferrals
                    .AsNoTracking()
                    .Where(x => x.AssessmentId == assessmentId)
                    .Select(x => new CoSellReferralsModel
                    {
                        AssessmentId = x.AssessmentId,
                        CoSellOpportunities = x.CoSellOpportunities,
                        ReferralsSubmitted = x.ReferralsSubmitted,
                        ReferralsWon = x.ReferralsWon,
                        PipelineValue = x.PipelineValue
                    })
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex) when (SqlExceptionHints.IsMissingTable(ex))
            {
                return null;
            }
        }

        public async Task SaveCoSellReferralsAsync(Guid assessmentId, CoSellReferralsModel model)
        {
            try
            {
                var entity = await _db.CoSellReferrals
                    .FirstOrDefaultAsync(x => x.AssessmentId == assessmentId);

                if (entity == null)
                {
                    entity = new CoSellReferrals
                    {
                        Id = Guid.NewGuid(),
                        AssessmentId = assessmentId
                    };
                    _db.CoSellReferrals.Add(entity);
                }

                entity.CoSellOpportunities = model.CoSellOpportunities;
                entity.ReferralsSubmitted = model.ReferralsSubmitted;
                entity.ReferralsWon = model.ReferralsWon;
                entity.PipelineValue = model.PipelineValue;

                await _db.SaveChangesAsync();
            }
            catch (Exception ex) when (SqlExceptionHints.IsMissingTable(ex))
            {
                throw new InvalidOperationException(
                    "Table dbo.CoSellReferrals does not exist. Run sql/CreateWizardTables.sql against pacappdb01.",
                    ex);
            }
        }


        // -------------------------
        // Marketplace Usage
        // -------------------------
        public async Task<MarketPlaceUsageModel?> GetMarketplaceUsageAsync(Guid assessmentId)
        {
            try
            {
                return await _db.MarketplaceUsage
                    .AsNoTracking()
                    .Where(x => x.AssessmentId == assessmentId)
                    .Select(x => new MarketPlaceUsageModel
                    {
                        AssessmentId = x.AssessmentId,
                        Offers = x.Offers,
                        Transactions = x.Transactions,
                        Revenue = x.Revenue,
                        Growth = x.Growth
                    })
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex) when (SqlExceptionHints.IsMissingTable(ex))
            {
                return null;
            }
        }

        public async Task SaveMarketplaceUsageAsync(Guid assessmentId, MarketPlaceUsageModel model)
        {
            try
            {
                var entity = await _db.MarketplaceUsage
                    .FirstOrDefaultAsync(x => x.AssessmentId == assessmentId);

                if (entity == null)
                {
                    entity = new MarketPlaceUsage
                    {
                        Id = Guid.NewGuid(),
                        AssessmentId = assessmentId
                    };
                    _db.MarketplaceUsage.Add(entity);
                }

                entity.Offers = model.Offers;
                entity.Transactions = model.Transactions;
                entity.Revenue = model.Revenue;
                entity.Growth = model.Growth;

                await _db.SaveChangesAsync();
            }
            catch (Exception ex) when (SqlExceptionHints.IsMissingTable(ex))
            {
                throw new InvalidOperationException(
                    "Table dbo.MarketplaceUsage does not exist. Run sql/CreateWizardTables.sql against pacappdb01.",
                    ex);
            }
        }


        // -------------------------
        // Incentives Usage
        // -------------------------
        public async Task<IncentivesUsageModel?> GetIncentivesUsageAsync(Guid assessmentId)
        {
            try
            {
                return await _db.IncentivesUsage
                    .AsNoTracking()
                    .Where(x => x.AssessmentId == assessmentId)
                    .Select(x => new IncentivesUsageModel
                    {
                        AssessmentId = x.AssessmentId,
                        Earned = x.Earned,
                        Redeemed = x.Redeemed,
                        Balance = x.Balance,
                        Growth = x.Growth
                    })
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex) when (SqlExceptionHints.IsMissingTable(ex))
            {
                return null;
            }
        }

        public async Task SaveIncentivesUsageAsync(Guid assessmentId, IncentivesUsageModel model)
        {
            try
            {
                var entity = await _db.IncentivesUsage
                    .FirstOrDefaultAsync(x => x.AssessmentId == assessmentId);

                if (entity == null)
                {
                    entity = new IncentivesUsage
                    {
                        Id = Guid.NewGuid(),
                        AssessmentId = assessmentId
                    };
                    _db.IncentivesUsage.Add(entity);
                }

                entity.Earned = model.Earned;
                entity.Redeemed = model.Redeemed;
                entity.Balance = model.Balance;
                entity.Growth = model.Growth;

                await _db.SaveChangesAsync();
            }
            catch (Exception ex) when (SqlExceptionHints.IsMissingTable(ex))
            {
                throw new InvalidOperationException(
                    "Table dbo.IncentivesUsage does not exist. Run sql/CreateWizardTables.sql against pacappdb01.",
                    ex);
            }
        }


        // -------------------------
        // Growth Metrics
        // -------------------------
        public async Task<GrowthMetricsModel?> GetGrowthMetricsAsync(Guid assessmentId)
        {
            try
            {
                return await _db.GrowthMetrics
                    .AsNoTracking()
                    .Where(x => x.AssessmentId == assessmentId)
                    .Select(x => new GrowthMetricsModel
                    {
                        AssessmentId = x.AssessmentId,
                        YearOverYear = x.YearOverYear,
                        CustomerAdds = x.CustomerAdds,
                        RevenueGrowth = x.RevenueGrowth,
                        PipelineGrowth = x.PipelineGrowth
                    })
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex) when (SqlExceptionHints.IsMissingTable(ex))
            {
                return null;
            }
        }

        public async Task SaveGrowthMetricsAsync(Guid assessmentId, GrowthMetricsModel model)
        {
            try
            {
                var entity = await _db.GrowthMetrics
                    .FirstOrDefaultAsync(x => x.AssessmentId == assessmentId);

                if (entity == null)
                {
                    entity = new GrowthMetrics
                    {
                        Id = Guid.NewGuid(),
                        AssessmentId = assessmentId
                    };
                    _db.GrowthMetrics.Add(entity);
                }

                entity.YearOverYear = model.YearOverYear;
                entity.CustomerAdds = model.CustomerAdds;
                entity.RevenueGrowth = model.RevenueGrowth;
                entity.PipelineGrowth = model.PipelineGrowth;

                await _db.SaveChangesAsync();
            }
            catch (Exception ex) when (SqlExceptionHints.IsMissingTable(ex))
            {
                throw new InvalidOperationException(
                    "Table dbo.GrowthMetrics does not exist. Run sql/CreateWizardTables.sql against pacappdb01.",
                    ex);
            }
        }


        // -------------------------
        // Summary (Review & Submit)
        // -------------------------
        public async Task<Dictionary<string, List<AssessmentReviewItem>>> GetAssessmentSummaryAsync(Guid assessmentId)
        {
            var dict = new Dictionary<string, List<AssessmentReviewItem>>();
            var answers = await GetAssessmentAnswersAsync(assessmentId);
            var answerMap = answers.ToDictionary(a => a.QuestionId, a => a, StringComparer.OrdinalIgnoreCase);

            try
            {
                var questions = await _db.AssessmentQuestions
                    .AsNoTracking()
                    .Where(q => q.IsActive)
                    .OrderBy(q => q.Category)
                    .ThenBy(q => q.DisplayOrder)
                    .Select(q => new
                    {
                        q.QuestionId,
                        q.Category,
                        q.Section,
                        q.Subsection,
                        q.QuestionText
                    })
                    .ToListAsync();

                foreach (var q in questions)
                {
                    var key = string.IsNullOrWhiteSpace(q.Category) ? q.Section : q.Category;
                    if (!dict.TryGetValue(key, out var sectionList))
                    {
                        sectionList = new List<AssessmentReviewItem>();
                        dict[key] = sectionList;
                    }

                    answerMap.TryGetValue(q.QuestionId, out var ans);
                    sectionList.Add(new AssessmentReviewItem
                    {
                        Section = key,
                        Subsection = q.Subsection,
                        QuestionText = q.QuestionText,
                        Response = ans?.Response,
                        Score = ans?.PointsEarned
                    });
                }
            }
            catch (Exception ex) when (SqlExceptionHints.IsMissingTable(ex))
            {
            }

            var partner = await GetPartnerInfoAsync(assessmentId);
            if (partner != null)
            {
                void AddField(string section, string label, string? value)
                {
                    if (string.IsNullOrWhiteSpace(value))
                        return;
                    if (!dict.TryGetValue(section, out var list))
                    {
                        list = new List<AssessmentReviewItem>();
                        dict[section] = list;
                    }
                    list.Add(new AssessmentReviewItem
                    {
                        Section = section,
                        QuestionText = label,
                        Response = value
                    });
                }

                AddField("Partner_Info", "Partner Name", partner.PartnerName);
                AddField("Partner_Info", "MPN Id", partner.MpnId);
                AddField("Partner_Info", "Country", partner.Country);
                AddField("Partner_Info", "Segment", partner.Segment);
            }

            if (dict.Count == 0)
            {
                dict["Summary"] = new List<AssessmentReviewItem>
                {
                    new()
                    {
                        Section = "Summary",
                        QuestionText = "Status",
                        Response = "No review items yet — answer wizard questions and click Next to save."
                    }
                };
            }

            return dict;
        }


        // -------------------------
        // Submit (final scoring)
        // -------------------------
        public async Task<SubmitResultModel> SubmitAssessmentAsync(Guid assessmentId)
        {
            var questions = await GetQuestionsAsync();
            var existing = await GetAssessmentAnswersAsync(assessmentId);
            if (existing.Count > 0)
            {
                var byId = existing.ToDictionary(a => a.QuestionId, StringComparer.OrdinalIgnoreCase);
                var toScore = questions
                    .Where(q => byId.ContainsKey(q.QuestionId))
                    .Select(q =>
                    {
                        q.Response = byId[q.QuestionId].Response;
                        return q;
                    })
                    .ToList();

                await SaveAssessmentAnswersAsync(assessmentId, toScore);
            }

            var answers = await GetAssessmentAnswersAsync(assessmentId);
            var sectionScores = await BuildSectionScoresAsync(assessmentId);
            var totalScore = sectionScores.Count == 0
                ? answers.Sum(a => a.PointsEarned)
                : (decimal)Math.Round(sectionScores.Average(s => s.Percentage), 1);

            decimal? priorOverall = null;
            try
            {
                priorOverall = await _db.AssessmentScores
                    .AsNoTracking()
                    .Where(x => x.AssessmentId == assessmentId)
                    .Select(x => (decimal?)x.TotalScore)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex) when (SqlExceptionHints.IsMissingTable(ex))
            {
            }

            try
            {
                await _db.Database.ExecuteSqlInterpolatedAsync(
                    $"DELETE FROM dbo.AssessmentScores WHERE AssessmentId = {assessmentId}");

                var scoreId = Guid.NewGuid();
                await _db.Database.ExecuteSqlInterpolatedAsync($@"
INSERT INTO dbo.AssessmentScores (Id, AssessmentId, TotalScore)
VALUES ({scoreId}, {assessmentId}, {totalScore})");
            }
            catch (Exception ex) when (SqlExceptionHints.IsMissingTable(ex))
            {
            }

            var assessment = await _db.Assessments
                .FirstAsync(x => x.Id == assessmentId);

            var isFirstSubmit = !AssessmentStatus.IsSubmitted(assessment.SubmittedAt);

            assessment.SubmittedAt = DateTimeOffset.UtcNow;
            await _db.SaveChangesAsync();

            if (isFirstSubmit)
            {
                await EnsureBaselineAsync(assessmentId, totalScore, sectionScores);
            }
            else if (priorOverall is not null)
            {
                // Overall-only safety net if revise opened without GetById baseline lock.
                // Section baselines must be captured before answers change (see GetByIdAsync).
                await EnsureBaselineAsync(assessmentId, priorOverall.Value, new List<CategoryScoreResult>());
            }

            return new SubmitResultModel
            {
                AssessmentId = assessmentId,
                TotalScore = totalScore
            };
        }

        // -------------------------
        // Final Result (WizardComplete)
        // -------------------------
        public async Task<AssessmentFinalResult> GetFinalResultAsync(Guid assessmentId)
        {
            var assessment = await _db.Assessments
                .AsNoTracking()
                .Where(x => x.Id == assessmentId)
                .Select(x => new AssessmentEntity
                {
                    Id = x.Id,
                    CustomerId = x.CustomerId,
                    SubmittedAt = x.SubmittedAt
                })
                .FirstAsync();

            string customerName = assessment.CustomerId;
            if (Guid.TryParse(assessment.CustomerId, out var customerGuid))
            {
                var customer = await _db.Customers
                    .AsNoTracking()
                    .Where(x => x.Id == customerGuid)
                    .Select(x => x.Name)
                    .FirstOrDefaultAsync();
                if (!string.IsNullOrWhiteSpace(customer))
                    customerName = customer!;
            }

            decimal totalScore = 0m;
            var sectionScores = await BuildSectionScoresAsync(assessmentId);
            if (sectionScores.Count > 0)
                totalScore = (decimal)Math.Round(sectionScores.Average(s => s.Percentage), 1);
            else
            {
                try
                {
                    totalScore = await _db.AssessmentScores
                        .AsNoTracking()
                        .Where(x => x.AssessmentId == assessmentId)
                        .SumAsync(x => (decimal?)x.TotalScore) ?? 0m;
                }
                catch (Exception ex) when (SqlExceptionHints.IsMissingTable(ex))
                {
                    totalScore = 0m;
                }

                if (totalScore == 0m)
                {
                    var answers = await GetAssessmentAnswersAsync(assessmentId);
                    totalScore = answers.Sum(a => a.PointsEarned);
                }
            }

            return new AssessmentFinalResult
            {
                AssessmentId = assessment.Id,
                CustomerId = assessment.CustomerId,
                CustomerName = customerName,
                CompletedAt = AssessmentStatus.Normalize(assessment.SubmittedAt) ?? DateTime.UtcNow,
                TotalScore = (int)Math.Round(totalScore),
                TotalMaxScore = 100,
                TotalPercentage = (double)totalScore,
                CategoryScores = sectionScores
            };
        }

        public async Task<List<AssessmentQuestionScoreDto>> GetAssessmentQuestionsWithScoresAsync(Guid assessmentId)
        {
            return await _db.AssessmentQuestions
                .AsNoTracking()
                .Where(q => q.IsActive)
                .OrderBy(q => q.Section)
                .ThenBy(q => q.DisplayOrder)
                .Select(q => new AssessmentQuestionScoreDto
                {
                    Section = q.Section,
                    Subsection = q.Subsection,
                    QuestionText = q.QuestionText,
                    Category = q.Category,
                    ScoreMax = q.Score,
                    ScoreValue = null
                })
                .ToListAsync();
        }

        public async Task<AssessmentEntity?> GetAssessmentWithResponsesAsync(Guid assessmentId)
        {
            var assessment = await _db.Assessments
                .AsNoTracking()
                .Where(x => x.Id == assessmentId)
                .Select(x => new AssessmentEntity
                {
                    Id = x.Id,
                    CustomerId = x.CustomerId,
                    SubmittedAt = x.SubmittedAt
                })
                .FirstOrDefaultAsync();

            if (assessment == null)
                return null;

            var responseList = await _db.AssessmentQuestions
                .AsNoTracking()
                .Where(q => q.IsActive)
                .OrderBy(q => q.Section)
                .ThenBy(q => q.DisplayOrder)
                .Select(q => new AssessmentResponseItem
                {
                    Section = q.Section,
                    Subsection = q.Subsection,
                    QuestionText = q.QuestionText,
                    Response = null,
                    Score = null
                })
                .ToListAsync();

            assessment.Responses = responseList;
            return assessment;
        }

        public async Task<Guid> CreateAssessmentAsync(AssessmentRequest request)
        {
            var id = request.Id == Guid.Empty ? Guid.NewGuid() : request.Id;

            var entity = new AssessmentEntity
            {
                Id = id,
                CustomerId = request.CustomerId,
                // Column is NOT NULL â€” use SQL datetime-safe sentinel for "not submitted yet"
                SubmittedAt = AssessmentStatus.NotSubmittedUtc
            };

            _db.Assessments.Add(entity);
            await _db.SaveChangesAsync();
            return id;
        }

        public async Task<AssessmentEntity?> GetByIdAsync(Guid id)
        {
            // Project only columns that exist on Azure Assessments table
            var assessment = await _db.Assessments
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new AssessmentEntity
                {
                    Id = a.Id,
                    CustomerId = a.CustomerId,
                    SubmittedAt = a.SubmittedAt
                })
                .FirstOrDefaultAsync();

            // Opening a submitted assessment to revise: lock onboarding scores before answers change.
            if (assessment != null && AssessmentStatus.IsSubmitted(assessment.SubmittedAt))
                await LockBaselineFromCurrentStateIfNeededAsync(id);

            return assessment;
        }

        public async Task<IReadOnlyList<AssessmentModel>> GetAssessmentsByCustomerAsync(Guid customerId)
        {
            var customerKey = customerId.ToString();

            var assessments = await _db.Assessments
                .AsNoTracking()
                .Where(a => a.CustomerId == customerKey)
                .OrderByDescending(a => a.SubmittedAt)
                .Select(a => new AssessmentModel
                {
                    Id = a.Id,
                    CustomerId = customerId,
                    SubmittedAt = AssessmentStatus.Normalize(a.SubmittedAt),
                    Status = AssessmentStatus.IsSubmitted(a.SubmittedAt) ? "Submitted" : "In Progress"
                })
                .ToListAsync();

            foreach (var item in assessments)
            {
                item.SectionScores = await BuildSectionScoresAsync(item.Id);
                item.TotalScore = item.SectionScores.Count == 0
                    ? 0
                    : (decimal)Math.Round(item.SectionScores.Average(s => s.Percentage), 1);

                await ApplyBaselineAsync(item);
            }

            return assessments;
        }

        private static bool IsEmptySectionBaselineJson(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return true;

            var trimmed = json.Trim();
            return trimmed == "[]" || trimmed == "null";
        }

        /// <summary>
        /// Before revise edits: snapshot current scores as onboarding if none exist yet.
        /// Also backfills empty section baselines when they still match the locked overall.
        /// </summary>
        private async Task LockBaselineFromCurrentStateIfNeededAsync(Guid assessmentId)
        {
            try
            {
                var sectionScores = await BuildSectionScoresAsync(assessmentId);
                if (sectionScores.Count == 0)
                    return;

                var totalScore = (decimal)Math.Round(sectionScores.Average(s => s.Percentage), 1);

                decimal? storedOverall = null;
                try
                {
                    storedOverall = await _db.AssessmentScores
                        .AsNoTracking()
                        .Where(x => x.AssessmentId == assessmentId)
                        .Select(x => (decimal?)x.TotalScore)
                        .FirstOrDefaultAsync();
                }
                catch (Exception ex) when (SqlExceptionHints.IsMissingTable(ex))
                {
                }

                var overallForBaseline = storedOverall ?? totalScore;
                await EnsureBaselineAsync(assessmentId, overallForBaseline, sectionScores);
            }
            catch (Exception ex) when (SqlExceptionHints.IsMissingTable(ex))
            {
            }
        }

        private async Task EnsureBaselineAsync(
            Guid assessmentId,
            decimal overallScore,
            List<CategoryScoreResult> sectionScores)
        {
            try
            {
                var existing = await _db.AssessmentScoreBaselines
                    .FirstOrDefaultAsync(x => x.AssessmentId == assessmentId);

                var baselineSections = sectionScores.Select(s => new CategoryScoreResult
                {
                    CategoryName = s.CategoryName,
                    TotalPointsEarned = s.TotalPointsEarned,
                    PotentialPoints = s.PotentialPoints,
                    Percentage = s.Percentage
                }).ToList();

                if (existing is not null)
                {
                    // Repair overall-only baselines when section snapshot still matches that overall
                    // (do not overwrite with later revised section scores).
                    if (sectionScores.Count > 0 &&
                        IsEmptySectionBaselineJson(existing.SectionScoresJson) &&
                        Math.Abs(existing.OverallScore - (decimal)Math.Round(sectionScores.Average(s => s.Percentage), 1)) < 0.15m)
                    {
                        existing.SectionScoresJson = JsonSerializer.Serialize(baselineSections);
                        await _db.SaveChangesAsync();
                    }

                    return;
                }

                _db.AssessmentScoreBaselines.Add(new AssessmentScoreBaseline
                {
                    AssessmentId = assessmentId,
                    CapturedAt = DateTimeOffset.UtcNow,
                    OverallScore = overallScore,
                    SectionScoresJson = JsonSerializer.Serialize(baselineSections)
                });
                await _db.SaveChangesAsync();
            }
            catch (Exception ex) when (
                SqlExceptionHints.IsMissingTable(ex) ||
                ex.Message.Contains("Invalid column name", StringComparison.OrdinalIgnoreCase) ||
                (ex.InnerException?.Message.Contains("Invalid column name", StringComparison.OrdinalIgnoreCase) ?? false))
            {
                // Table not created yet — dashboard still works without deltas
            }
        }

        private async Task ApplyBaselineAsync(AssessmentModel item)
        {
            try
            {
                var baseline = await _db.AssessmentScoreBaselines
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.AssessmentId == item.Id);

                if (baseline is null)
                    return;

                item.BaselineOverallScore = baseline.OverallScore;

                if (IsEmptySectionBaselineJson(baseline.SectionScoresJson))
                    return;

                var baselineSections = JsonSerializer.Deserialize<List<CategoryScoreResult>>(
                    baseline.SectionScoresJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new List<CategoryScoreResult>();

                foreach (var section in item.SectionScores)
                {
                    var match = baselineSections.FirstOrDefault(b =>
                        string.Equals(b.CategoryName, section.CategoryName, StringComparison.OrdinalIgnoreCase));
                    if (match is null)
                        continue;

                    // Recompute against current section max so older baselines (saved when every
                    // section was /100) still show the correct onboarding percentage.
                    var max = section.PotentialPoints > 0 ? section.PotentialPoints : 100m;
                    section.BaselinePercentage = max == 0
                        ? 0
                        : Math.Round((double)match.TotalPointsEarned / (double)max * 100.0, 1);
                }

                var sectionBaselines = item.SectionScores
                    .Where(s => s.BaselinePercentage is not null)
                    .Select(s => s.BaselinePercentage!.Value)
                    .ToList();
                if (sectionBaselines.Count > 0)
                    item.BaselineOverallScore = (decimal)Math.Round(sectionBaselines.Average(), 1);
            }
            catch (Exception ex) when (SqlExceptionHints.IsMissingTable(ex))
            {
            }
        }

        private async Task<List<CategoryScoreResult>> BuildSectionScoresAsync(Guid assessmentId)
        {
            var answers = await GetAssessmentAnswersAsync(assessmentId);
            if (answers.Count == 0)
                return new List<CategoryScoreResult>();

            var questionCats = await _db.AssessmentQuestions
                .AsNoTracking()
                .Where(q => q.IsActive)
                .Select(q => new { q.QuestionId, q.Category })
                .ToListAsync();

            var catByQuestion = questionCats.ToDictionary(
                q => q.QuestionId,
                q => string.IsNullOrWhiteSpace(q.Category) ? "Other" : q.Category,
                StringComparer.OrdinalIgnoreCase);

            var grouped = answers
                .GroupBy(a => catByQuestion.TryGetValue(a.QuestionId, out var cat) ? cat : "Other")
                .OrderBy(g => CategoryDisplayOrder(g.Key));

            var results = new List<CategoryScoreResult>();
            foreach (var g in grouped)
            {
                var earned = g.Sum(x => x.PointsEarned);
                var potential = CategoryMaxPoints(g.Key);
                var pct = potential == 0
                    ? 0
                    : Math.Round((double)earned / (double)potential * 100.0, 1);

                results.Add(new CategoryScoreResult
                {
                    CategoryName = FriendlyCategoryName(g.Key),
                    TotalPointsEarned = earned,
                    PotentialPoints = potential,
                    Percentage = pct
                });
            }

            return results;
        }

        /// <summary>Fixed section denominators for partner progress percentages.</summary>
        private static decimal CategoryMaxPoints(string category) => category switch
        {
            "Solutions_Specializations" => 100m,
            "CSP" => 100m,
            "CoSell" => 80m,
            "MarketPlace" => 65m,
            "Incentives" => 50m,
            _ => 100m
        };

        private static int CategoryDisplayOrder(string category) => category switch
        {
            "Solutions_Specializations" => 1,
            "CSP" => 2,
            "CoSell" => 3,
            "MarketPlace" => 4,
            "Incentives" => 5,
            _ => 99
        };

        private static string FriendlyCategoryName(string category) => category switch
        {
            "Solutions_Specializations" => "Solutions and Specializations",
            "CSP" => "Customers & CSP",
            "CoSell" => "Co-Sell & Referrals",
            "MarketPlace" => "Marketplace Offers",
            "Incentives" => "Incentives Usage",
            "Partner_Info" => "Partner Information",
            _ => category.Replace('_', ' ')
        };

        public async Task<bool> DeleteAssessmentAsync(Guid assessmentId)
        {
            var assessment = await _db.Assessments
                .FirstOrDefaultAsync(a => a.Id == assessmentId);

            if (assessment is null)
                return false;

            // Remove related wizard/score rows first (tables may be empty or missing)
            async Task SafeRemoveAsync(Func<Task> action)
            {
                try { await action(); }
                catch (Exception ex) when (
                    SqlExceptionHints.IsMissingTable(ex) ||
                    ex.Message.Contains("Invalid column name", StringComparison.OrdinalIgnoreCase) ||
                    (ex.InnerException?.Message.Contains("Invalid column name", StringComparison.OrdinalIgnoreCase) ?? false))
                { }
            }

            await SafeRemoveAsync(async () =>
            {
                var rows = await _db.PartnerInfos.Where(x => x.AssessmentId == assessmentId).ToListAsync();
                _db.PartnerInfos.RemoveRange(rows);
            });
            await SafeRemoveAsync(async () =>
            {
                var rows = await _db.CustomersCsp.Where(x => x.AssessmentId == assessmentId).ToListAsync();
                _db.CustomersCsp.RemoveRange(rows);
            });
            await SafeRemoveAsync(async () =>
            {
                var rows = await _db.CoSellReferrals.Where(x => x.AssessmentId == assessmentId).ToListAsync();
                _db.CoSellReferrals.RemoveRange(rows);
            });
            await SafeRemoveAsync(async () =>
            {
                var rows = await _db.MarketplaceUsage.Where(x => x.AssessmentId == assessmentId).ToListAsync();
                _db.MarketplaceUsage.RemoveRange(rows);
            });
            await SafeRemoveAsync(async () =>
            {
                var rows = await _db.IncentivesUsage.Where(x => x.AssessmentId == assessmentId).ToListAsync();
                _db.IncentivesUsage.RemoveRange(rows);
            });
            await SafeRemoveAsync(async () =>
            {
                var rows = await _db.GrowthMetrics.Where(x => x.AssessmentId == assessmentId).ToListAsync();
                _db.GrowthMetrics.RemoveRange(rows);
            });
            await SafeRemoveAsync(async () =>
            {
                await _db.Database.ExecuteSqlInterpolatedAsync(
                    $"DELETE FROM dbo.AssessmentScoreBaselines WHERE AssessmentId = {assessmentId}");
            });

            await SafeRemoveAsync(async () =>
            {
                await _db.Database.ExecuteSqlInterpolatedAsync(
                    $"DELETE FROM dbo.AssessmentAnswers WHERE AssessmentId = {assessmentId}");
            });

            await SafeRemoveAsync(async () =>
            {
                // Raw delete avoids EF selecting unmapped/missing score columns
                await _db.Database.ExecuteSqlInterpolatedAsync(
                    $"DELETE FROM dbo.AssessmentScores WHERE AssessmentId = {assessmentId}");
            });

            _db.Assessments.Remove(assessment);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task SaveAssessmentAnswersAsync(Guid assessmentId, IEnumerable<AssessmentQuestion> questions)
        {
            var list = questions?
                .Where(q => !string.IsNullOrWhiteSpace(q.QuestionId))
                .ToList()
                ?? new List<AssessmentQuestion>();

            var existing = await _db.AssessmentAnswers
                .Where(a => a.AssessmentId == assessmentId)
                .ToListAsync();

            var byQuestion = existing.ToDictionary(a => a.QuestionId, StringComparer.OrdinalIgnoreCase);

            // Merge saved + incoming answers so dependent formulas (e.g. "going to target")
            // can see sibling responses even when only one wizard step is posted.
            var allResponses = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            foreach (var row in existing)
                allResponses[row.QuestionId] = row.Response;
            foreach (var q in list)
                allResponses[q.QuestionId] = q.Response;

            // Enforce designation gates server-side (Have=Yes → Target cannot be Yes, etc.)
            QuestionAnswerGate.NormalizeAll(allResponses);
            foreach (var q in list)
            {
                if (allResponses.TryGetValue(q.QuestionId, out var normalized))
                    q.Response = normalized;
            }

            foreach (var q in list)
            {
                var points = AssessmentAnswerScorer.Score(q.QuestionId, q.Response, allResponses);

                if (byQuestion.TryGetValue(q.QuestionId, out var row))
                {
                    row.Response = q.Response;
                    row.PointsEarned = points;
                }
                else
                {
                    _db.AssessmentAnswers.Add(new AssessmentAnswer
                    {
                        Id = Guid.NewGuid(),
                        AssessmentId = assessmentId,
                        QuestionId = q.QuestionId,
                        Response = q.Response,
                        PointsEarned = points
                    });
                }
            }

            // Re-score dependent questions already stored (e.g. target after "have designation" changes)
            foreach (var row in existing)
            {
                if (list.Any(q => string.Equals(q.QuestionId, row.QuestionId, StringComparison.OrdinalIgnoreCase)))
                    continue;

                allResponses[row.QuestionId] = row.Response;
                row.PointsEarned = AssessmentAnswerScorer.Score(row.QuestionId, row.Response, allResponses);
            }

            await _db.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<AssessmentAnswerDto>> GetAssessmentAnswersAsync(Guid assessmentId)
        {
            try
            {
                return await _db.AssessmentAnswers
                    .AsNoTracking()
                    .Where(a => a.AssessmentId == assessmentId)
                    .Select(a => new AssessmentAnswerDto
                    {
                        QuestionId = a.QuestionId,
                        Response = a.Response,
                        PointsEarned = a.PointsEarned
                    })
                    .ToListAsync();
            }
            catch (Exception ex) when (SqlExceptionHints.IsMissingTable(ex))
            {
                return Array.Empty<AssessmentAnswerDto>();
            }
        }
}   // closes the AssessmentRepository class
}