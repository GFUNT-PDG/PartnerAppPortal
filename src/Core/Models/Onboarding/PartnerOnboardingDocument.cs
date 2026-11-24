namespace PartnerAppPortal.Core.Models.Onboarding;

public sealed class PartnerOnboardingDocument
{
    public Guid AccountId { get; set; }
    public string AccountName { get; set; } = "";
    public Dictionary<string, string?> Fields { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string?> Checklist { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public bool FolderCreated { get; set; }
    public string? ContactCardBlob { get; set; }
    public bool FieldsFromDynamics { get; set; }
    public List<string> UnmappedLabels { get; set; } = new();
    public Dictionary<string, List<string>> FieldChoices { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class PartnerOnboardingChecklistView
{
    public Guid AccountId { get; set; }
    public string AccountName { get; set; } = "";
    public string? EngagementType { get; set; }
    public string? EngagementStart { get; set; }
    public List<ChecklistRow> Items { get; set; } = new();
    public int ItemsComplete { get; set; }
    public int PercentComplete { get; set; }
    public bool DynamicsDetailsComplete { get; set; }
    public string? CrmWarning { get; set; }
}

public sealed class PartnerOnboardingSaveRequest
{
    public Dictionary<string, string?> Fields { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string?> Checklist { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public string? UpdatedBy { get; set; }
}

public sealed class ChecklistItemUpdate
{
    public string Key { get; set; } = "";
    public string? Value { get; set; }
    public string? UpdatedBy { get; set; }
}

public sealed class ContactCardResult
{
    public string BlobName { get; set; } = "";
    public string AccountName { get; set; } = "";
}

public sealed class PartnerOnboardingRecord
{
    public Guid AccountId { get; set; }
    public string ProfileJson { get; set; } = "{}";
    public string ChecklistJson { get; set; } = "{}";
    public bool FolderCreated { get; set; }
    public string? ContactCardBlob { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }
}
