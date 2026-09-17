using Microsoft.EntityFrameworkCore;
using PartnerAppPortal.Core.Models.Assessments;
using PartnerAppPortal.Core.Models.Crm;
using PartnerAppPortal.Core.Models.SalesAccelerator;

namespace PartnerAppPortal.Infrastructure.Data
{
    public class PortalDbContext : DbContext
    {
        public PortalDbContext(DbContextOptions<PortalDbContext> options)
            : base(options)
        {
        }

        // ------------------------------------------------------------
        // EXISTING TABLES (UNCHANGED)
        // ------------------------------------------------------------
        public DbSet<AssessmentQuestion> AssessmentQuestions { get; set; } = default!;
        public DbSet<Customer> Customers { get; set; } = default!;
        public DbSet<CrmContact> CrmContacts { get; set; } = default!;
        public DbSet<AssessmentEntity> Assessments { get; set; } = default!;
        public DbSet<AssessmentScore> AssessmentScores { get; set; } = default!;
        public DbSet<AssessmentAnswer> AssessmentAnswers { get; set; } = default!;
        public DbSet<AssessmentScoreBaseline> AssessmentScoreBaselines { get; set; } = default!;

        // ------------------------------------------------------------
        // NEW TABLES REQUIRED BY REPOSITORY (ADD ONLY)
        // ------------------------------------------------------------
        public DbSet<PartnerInfo> PartnerInfos { get; set; } = default!;
        public DbSet<CustomersCsp> CustomersCsp { get; set; } = default!;
        public DbSet<CoSellReferrals> CoSellReferrals { get; set; } = default!;
        public DbSet<MarketPlaceUsage> MarketplaceUsage { get; set; } = default!;
        public DbSet<IncentivesUsage> IncentivesUsage { get; set; } = default!;
        public DbSet<GrowthMetrics> GrowthMetrics { get; set; } = default!;
        public DbSet<SalesAcceleratorSubmission> SalesAcceleratorSubmissions { get; set; } = default!;
        public DbSet<SalesAcceleratorAccount> SalesAcceleratorAccounts { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // DTO used in projections only — never a database entity
            modelBuilder.Ignore<AssessmentResponseItem>();

            // ------------------------------------------------------------
            // AssessmentQuestions mapping (UNCHANGED)
            // ------------------------------------------------------------
            modelBuilder.Entity<AssessmentQuestion>(entity =>
            {
                entity.ToTable("AssessmentQuestions");

                entity.HasKey(q => q.Id);

                entity.Property(q => q.Id).HasColumnName("Id");
                entity.Property(q => q.QuestionId).HasColumnName("QuestionId");
                entity.Property(q => q.Section).HasColumnName("Section");
                entity.Property(q => q.Category).HasColumnName("Category");
                entity.Property(q => q.QuestionText).HasColumnName("QuestionText");
                entity.Property(q => q.IsActive).HasColumnName("IsActive");
                entity.Property(q => q.DisplayOrder).HasColumnName("DisplayOrder");
                entity.Property(q => q.Score).HasColumnName("Score");
                entity.Property(q => q.AnswerType).HasColumnName("AnswerType");
                entity.Property(q => q.Subsection).HasColumnName("Subsection");
                entity.Property(q => q.OptionsJson).HasColumnName("OptionsJson");

                // UI-only fields — not columns on AssessmentQuestions
                entity.Ignore(q => q.Response);
                entity.Ignore(q => q.ResponseType);
            });

          // ------------------------------------------------------------
// Customer mapping (map to CrmAccounts table)
// ------------------------------------------------------------
modelBuilder.Entity<Customer>(entity =>
{
    entity.ToTable("CrmAccounts");

    // SQL PK is AccountId (uniqueidentifier) — expose it as Customer.Id for the app
    entity.HasKey(c => c.Id);
    entity.Property(c => c.Id).HasColumnName("AccountId");
    entity.Property(c => c.Name).HasColumnName("Name");

    // Present on CrmAccounts — used for partner portal login
    entity.Property(c => c.Email).HasColumnName("Email");
    entity.Property(c => c.PrimaryContact).HasColumnName("PrimaryContact");
    entity.Property(c => c.SalesAcceleratorAccess).HasColumnName("SalesAcceleratorAccess");

    // Not needed for current portal features
    entity.Ignore(c => c.AccountId);
    entity.Ignore(c => c.AccountExecutive);
    entity.Ignore(c => c.Phone);
    entity.Ignore(c => c.ModifiedOn);
    entity.Ignore(c => c.SyncedOn);
    entity.Ignore(c => c.AccountNumber);
    entity.Ignore(c => c.CustomerId);
    entity.Ignore(c => c.CustomerExperienceCX);
    entity.Ignore(c => c.DevelopmentDirector);
});

modelBuilder.Entity<CrmContact>(entity =>
{
    entity.ToTable("CrmContacts");
    entity.HasKey(c => c.ContactId);
    entity.Property(c => c.ContactId).HasColumnName("ContactId");
    entity.Property(c => c.FirstName).HasColumnName("FirstName");
    entity.Property(c => c.LastName).HasColumnName("LastName");
    entity.Property(c => c.Email).HasColumnName("Email");
    entity.Property(c => c.Phone).HasColumnName("Phone");
    entity.Property(c => c.AccountId).HasColumnName("AccountId");
});



            // ------------------------------------------------------------
            // Assessments mapping (UNCHANGED)
            // ------------------------------------------------------------
            modelBuilder.Entity<AssessmentEntity>(entity =>
            {
                entity.ToTable("Assessments");

                entity.HasKey(a => a.Id);

                entity.Property(a => a.Id).HasColumnName("Id");
                entity.Property(a => a.CustomerId).HasColumnName("CustomerId");
                entity.Property(a => a.SubmittedAt).HasColumnName("SubmittedAt");

                // Not present on Azure Assessments table
                entity.Ignore(a => a.AnswersJson);
                entity.Ignore(a => a.TotalScore);
                entity.Ignore(a => a.Responses);
            });

            // ------------------------------------------------------------
            // AssessmentScores mapping (UNCHANGED)
            // ------------------------------------------------------------
            modelBuilder.Entity<AssessmentScore>(entity =>
            {
                entity.ToTable("AssessmentScores");

                entity.HasKey(s => s.Id);

                entity.Property(s => s.Id).HasColumnName("Id");
                entity.Property(s => s.AssessmentId).HasColumnName("AssessmentId");
                entity.Property(s => s.TotalScore).HasColumnName("TotalScore");

                // Not present on Azure AssessmentScores — do not map
                entity.Ignore(s => s.CategoryScoresJson);
                entity.Ignore(s => s.Section);
                entity.Ignore(s => s.Score);
                entity.Ignore(s => s.QuestionId);
                entity.Ignore(s => s.Response);
                entity.Ignore(s => s.Result);
                entity.Ignore(s => s.Notes);

                entity.HasOne<AssessmentEntity>()
                      .WithMany()
                      .HasForeignKey(s => s.AssessmentId);
            });

            modelBuilder.Entity<AssessmentAnswer>(entity =>
            {
                entity.ToTable("AssessmentAnswers");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("Id");
                entity.Property(x => x.AssessmentId).HasColumnName("AssessmentId");
                entity.Property(x => x.QuestionId).HasColumnName("QuestionId").HasMaxLength(100);
                entity.Property(x => x.Response).HasColumnName("Response");
                entity.Property(x => x.PointsEarned).HasColumnName("PointsEarned");
                entity.HasIndex(x => new { x.AssessmentId, x.QuestionId }).IsUnique();
            });

            modelBuilder.Entity<AssessmentScoreBaseline>(entity =>
            {
                entity.ToTable("AssessmentScoreBaselines");
                entity.HasKey(x => x.AssessmentId);
                entity.Property(x => x.AssessmentId).HasColumnName("AssessmentId");
                entity.Property(x => x.CapturedAt).HasColumnName("CapturedAt");
                entity.Property(x => x.OverallScore).HasColumnName("OverallScore");
                entity.Property(x => x.SectionScoresJson).HasColumnName("SectionScoresJson");
            });

            // ------------------------------------------------------------
            // NEW ENTITY MAPPINGS (ADD ONLY)
            // ------------------------------------------------------------
            modelBuilder.Entity<PartnerInfo>(entity =>
            {
                entity.ToTable("PartnerInfo");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id).HasColumnName("Id");
                entity.Property(x => x.AssessmentId).HasColumnName("AssessmentId");
                entity.Property(x => x.PartnerName).HasColumnName("PartnerName");
                entity.Property(x => x.MpnId).HasColumnName("MpnId");
                entity.Property(x => x.Country).HasColumnName("Country");
                entity.Property(x => x.Segment).HasColumnName("Segment");

                // Present on DTO/UI model — not assumed columns unless synced later
                entity.Ignore(x => x.Tenure);
                entity.Ignore(x => x.CommitmentLength);
                entity.Ignore(x => x.Region);
                entity.Ignore(x => x.Industry);
            });

            modelBuilder.Entity<CustomersCsp>(entity =>
            {
                entity.ToTable("CustomersCsp");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id).HasColumnName("Id");
                entity.Property(x => x.AssessmentId).HasColumnName("AssessmentId");
                entity.Property(x => x.CspTier).HasColumnName("CspTier");
                entity.Property(x => x.ActiveSubscriptions).HasColumnName("ActiveSubscriptions");
                entity.Property(x => x.AzureConsumption).HasColumnName("AzureConsumption");
                entity.Property(x => x.MarketplaceUsage).HasColumnName("MarketplaceUsage");
                entity.Property(x => x.IncentivesUsage).HasColumnName("IncentivesUsage");
            });

            modelBuilder.Entity<CoSellReferrals>(entity =>
            {
                entity.ToTable("CoSellReferrals");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id).HasColumnName("Id");
                entity.Property(x => x.AssessmentId).HasColumnName("AssessmentId");
                entity.Property(x => x.CoSellOpportunities).HasColumnName("CoSellOpportunities");
                entity.Property(x => x.ReferralsSubmitted).HasColumnName("ReferralsSubmitted");
                entity.Property(x => x.ReferralsWon).HasColumnName("ReferralsWon");
                entity.Property(x => x.PipelineValue).HasColumnName("PipelineValue");
            });

            modelBuilder.Entity<MarketPlaceUsage>(entity =>
            {
                entity.ToTable("MarketplaceUsage");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id).HasColumnName("Id");
                entity.Property(x => x.AssessmentId).HasColumnName("AssessmentId");
                entity.Property(x => x.Offers).HasColumnName("Offers");
                entity.Property(x => x.Transactions).HasColumnName("Transactions");
                entity.Property(x => x.Revenue).HasColumnName("Revenue");
                entity.Property(x => x.Growth).HasColumnName("Growth");
            });

            modelBuilder.Entity<IncentivesUsage>(entity =>
            {
                entity.ToTable("IncentivesUsage");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id).HasColumnName("Id");
                entity.Property(x => x.AssessmentId).HasColumnName("AssessmentId");
                entity.Property(x => x.Earned).HasColumnName("Earned");
                entity.Property(x => x.Redeemed).HasColumnName("Redeemed");
                entity.Property(x => x.Balance).HasColumnName("Balance");
                entity.Property(x => x.Growth).HasColumnName("Growth");
            });

            modelBuilder.Entity<GrowthMetrics>(entity =>
            {
                entity.ToTable("GrowthMetrics");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id).HasColumnName("Id");
                entity.Property(x => x.AssessmentId).HasColumnName("AssessmentId");
                entity.Property(x => x.YearOverYear).HasColumnName("YearOverYear");
                entity.Property(x => x.CustomerAdds).HasColumnName("CustomerAdds");
                entity.Property(x => x.RevenueGrowth).HasColumnName("RevenueGrowth");
                entity.Property(x => x.PipelineGrowth).HasColumnName("PipelineGrowth");
            });

            modelBuilder.Entity<SalesAcceleratorSubmission>(entity =>
            {
                entity.ToTable("SalesAcceleratorSubmissions");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("Id");
                entity.Property(x => x.PartnerCustomerId).HasColumnName("PartnerCustomerId");
                entity.Property(x => x.Partner).HasColumnName("Partner").HasMaxLength(256);
                entity.Property(x => x.SubmittedBy).HasColumnName("SubmittedBy").HasMaxLength(256);
                entity.Property(x => x.PartnerUploadDate).HasColumnName("PartnerUploadDate");
                entity.Property(x => x.OriginalFileName).HasColumnName("OriginalFileName").HasMaxLength(260);
                entity.Property(x => x.ContentType).HasColumnName("ContentType").HasMaxLength(128);
                entity.Property(x => x.FileContent).HasColumnName("FileContent");
                entity.Property(x => x.InsertedCount).HasColumnName("InsertedCount");
                entity.Property(x => x.DuplicateCount).HasColumnName("DuplicateCount");
            });

            modelBuilder.Entity<SalesAcceleratorAccount>(entity =>
            {
                entity.ToTable("SalesAcceleratorAccounts");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("Id");
                entity.Property(x => x.SubmissionId).HasColumnName("SubmissionId");
                entity.Property(x => x.PartnerCustomerId).HasColumnName("PartnerCustomerId");
                entity.Property(x => x.CompanyName).HasColumnName("CompanyName").HasMaxLength(256);
                entity.Property(x => x.Domain).HasColumnName("Domain").HasMaxLength(256);
                entity.Property(x => x.NormalizedDomain).HasColumnName("NormalizedDomain").HasMaxLength(256);
                entity.Property(x => x.AccountType).HasColumnName("AccountType").HasMaxLength(128);
                entity.Property(x => x.SubmittedBy).HasColumnName("SubmittedBy").HasMaxLength(256);
                entity.Property(x => x.Partner).HasColumnName("Partner").HasMaxLength(256);
                entity.Property(x => x.PartnerUploadDate).HasColumnName("PartnerUploadDate");
                entity.Property(x => x.Tpid).HasColumnName("Tpid").HasMaxLength(128);
                entity.Property(x => x.SentToMitch).HasColumnName("SentToMitch");
                entity.Property(x => x.DateSubmitted).HasColumnName("DateSubmitted");
                entity.Property(x => x.DateVerified).HasColumnName("DateVerified");
                entity.HasIndex(x => x.NormalizedDomain).IsUnique();
                entity.HasOne<SalesAcceleratorSubmission>()
                      .WithMany()
                      .HasForeignKey(x => x.SubmissionId);
            });
        }
    }
}
