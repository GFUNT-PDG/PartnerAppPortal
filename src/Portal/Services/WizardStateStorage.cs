using System.Text.Json;

namespace PartnerAppPortal.Portal.Services
{
    public class AssessmentWizardStorage
    {
        private const string StorageKey = "assessment_wizard_state";

        private readonly Dictionary<string, Dictionary<string, string>> _answers =
            new(StringComparer.OrdinalIgnoreCase);

        public AssessmentWizardStorage()
        {
            Load();
        }

        // Save an answer for a specific section + question
        public void SetAnswer(string section, string questionId, string value)
        {
            if (!_answers.ContainsKey(section))
                _answers[section] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            _answers[section][questionId] = value;
            Save();
        }

        // Retrieve an answer
        public string? GetAnswer(string section, string questionId)
        {
            if (_answers.TryGetValue(section, out var sectionAnswers))
            {
                if (sectionAnswers.TryGetValue(questionId, out var value))
                    return value;
            }

            return null;
        }

        // Returns all answers for scoring
        public IReadOnlyDictionary<string, Dictionary<string, string>> GetAllAnswers()
        {
            return _answers;
        }

        // Serialize to JSON and persist
        private void Save()
        {
            var json = JsonSerializer.Serialize(_answers);
            File.WriteAllText(StorageKey, json);
        }

        // Load from JSON if exists
        private void Load()
        {
            if (!File.Exists(StorageKey))
                return;

            var json = File.ReadAllText(StorageKey);

            var restored = JsonSerializer.Deserialize<
                Dictionary<string, Dictionary<string, string>>>(json);

            if (restored != null)
            {
                foreach (var section in restored)
                {
                    _answers[section.Key] = new Dictionary<string, string>(
                        section.Value, StringComparer.OrdinalIgnoreCase);
                }
            }
        }
    }
}
