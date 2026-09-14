using System.Text.Json;
using PartnerAppPortal.Core.Interfaces.Repositories;
using PartnerAppPortal.Core.Models.Assessments;
using PartnerAppPortal.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace PartnerAppPortal.Infrastructure.Repositories
{
    public class AssessmentSubmissionRepository : IAssessmentSubmissionRepository
    {
        private readonly PortalDbContext _db;

        public AssessmentSubmissionRepository(PortalDbContext db)
        {
            _db = db;
        }

        public async Task<Guid> SaveAssessmentAsync(AssessmentSubmission submission, AssessmentResult result)
        {
            // Create Assessment entity (matches SQL table "Assessments")
            var assessmentEntity = new AssessmentEntity
            {
                Id = Guid.NewGuid(),
                CustomerId = submission.CustomerId,
                SubmittedAt = DateTimeOffset.UtcNow
            };

            _db.Assessments.Add(assessmentEntity);

            // Create AssessmentScore entity (matches SQL table "AssessmentScores")
            var scoreEntity = new AssessmentScore
            {
                Id = Guid.NewGuid(),
                AssessmentId = assessmentEntity.Id,
                TotalScore = result.TotalScore,
                CategoryScoresJson = JsonSerializer.Serialize(result.CategoryScores)
            };

            _db.AssessmentScores.Add(scoreEntity);

            await _db.SaveChangesAsync();

            return assessmentEntity.Id;
        }
    }
}
