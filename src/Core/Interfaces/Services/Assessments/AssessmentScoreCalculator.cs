using PartnerAppPortal.Core.Models.Assessments;

namespace PartnerAppPortal.Core.Services.Assessments
{
    public class AssessmentScoreCalculator
    {
        /// <summary>
        /// Calculates total score and category scores based on answers and questions.
        /// </summary>
        public AssessmentResult Calculate(
            IReadOnlyDictionary<string, Dictionary<string, string>> answers,
            IReadOnlyList<AssessmentQuestion> questions)
        {
            var categoryScores = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var totalScore = 0;

            foreach (var question in questions)
            {
                // Skip inactive questions
                if (!question.IsActive)
                    continue;

                // Check if the user answered this question
                if (!answers.TryGetValue(question.Section, out var sectionAnswers))
                    continue;

                if (!sectionAnswers.TryGetValue(question.QuestionId, out var answerValue))
                    continue;

                // Score logic: if the answer is "yes", apply the question's score
                // You can expand this later for multi-choice, numeric, etc.
                var normalized = answerValue.Trim().ToLowerInvariant();

                var earned = normalized == "yes" ? question.Score : 0;

                // Add to category
                if (!categoryScores.ContainsKey(question.Category))
                    categoryScores[question.Category] = 0;

                categoryScores[question.Category] += earned;

                // Add to total
                totalScore += earned;
            }

            return new AssessmentResult
            {
                TotalScore = totalScore,
                CategoryScores = categoryScores
            };
        }
    }
}
