namespace PartnerAppPortal.Portal.Services;

/// <summary>
/// Partner Portal specialization / designation catalog for hot list + detail pages.
/// </summary>
public static class SpecializationCatalog
{
    public sealed record Item(string QuestionId, string Slug, string Title);

    /// <summary>
    /// Solution designation "going to target" questions (hot list source when Target = Yes).
    /// </summary>
    public static readonly IReadOnlyList<Item> DesignationTargets =
    [
        new("SOL-005", "business-applications", "Business Applications"),
        new("SOL-008", "data-ai", "Data & AI"),
        new("SOL-011", "digital-app-innovation", "Digital & App Innovation"),
        new("SOL-014", "infrastructure", "Infrastructure"),
        new("SOL-017", "modern-work", "Modern Work"),
        new("SOL-020", "security", "Security"),
    ];

    /// <summary>Advanced specialization pages (SOL-021 … SOL-052) for future content.</summary>
    public static readonly IReadOnlyList<Item> AdvancedSpecializations =
    [
        new("SOL-021", "ai-platform-azure", "AI Platform on Microsoft Azure"),
        new("SOL-022", "analytics-azure", "Analytics on Microsoft Azure"),
        new("SOL-023", "agentic-devops-azure-github", "Agentic DevOps with Microsoft Azure and GitHub"),
        new("SOL-024", "hybrid-cloud-azure-stack-hci", "Hybrid Cloud Infrastructure with Microsoft Azure Stack HCI"),
        new("SOL-025", "networking-services-azure", "Networking Services in Microsoft Azure"),
        new("SOL-026", "azure-vmware-solution", "Microsoft Azure VMware Solution"),
        new("SOL-027", "ai-apps-azure", "AI Apps on Microsoft Azure"),
        new("SOL-028", "data-warehouse-migration-azure", "Data Warehouse Migration to Microsoft Azure"),
        new("SOL-029", "infra-database-migration-azure", "Infra and Database Migration to Microsoft Azure"),
        new("SOL-030", "kubernetes-azure", "Kubernetes on Microsoft Azure"),
        new("SOL-031", "migrate-enterprise-apps-azure", "Migrate Enterprise Apps to Microsoft Azure"),
        new("SOL-032", "sap-azure", "SAP on Microsoft Azure"),
        new("SOL-033", "azure-virtual-desktop", "Microsoft Azure Virtual Desktop"),
        new("SOL-034", "finance", "Finance"),
        new("SOL-035", "low-code-application-development", "Microsoft Low Code Application Development"),
        new("SOL-036", "intelligent-automation", "Intelligent Automation"),
        new("SOL-037", "business-intelligence", "Business Intelligence"),
        new("SOL-038", "sales", "Sales"),
        new("SOL-039", "service", "Service"),
        new("SOL-040", "smb-management", "Small and Midsize Business Management"),
        new("SOL-041", "supply-chain", "Supply Chain"),
        new("SOL-042", "adoption-change-management", "Adoption and Change Management"),
        new("SOL-043", "calling-microsoft-teams", "Calling for Microsoft Teams"),
        new("SOL-044", "copilot", "Copilot"),
        new("SOL-045", "custom-teams-solutions", "Custom Teams Solutions"),
        new("SOL-046", "meetings-meeting-rooms-teams", "Meetings and Meeting Rooms for Microsoft Teams"),
        new("SOL-047", "modernize-end-points", "Modernize End Points"),
        new("SOL-048", "teamwork-deployment", "Teamwork Deployment"),
        new("SOL-049", "cloud-security", "Cloud Security"),
        new("SOL-050", "identity-access-management", "Identity and Access Management"),
        new("SOL-051", "data-security", "Data Security"),
        new("SOL-052", "threat-protection", "Threat Protection"),
    ];

    public static readonly IReadOnlyList<Item> All =
        DesignationTargets.Concat(AdvancedSpecializations).ToList();

    private static readonly Dictionary<string, Item> ByQuestionId =
        All.GroupBy(x => x.QuestionId, StringComparer.OrdinalIgnoreCase)
           .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

    private static readonly Dictionary<string, Item> BySlug =
        All.GroupBy(x => x.Slug, StringComparer.OrdinalIgnoreCase)
           .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

    public static Item? FindByQuestionId(string? questionId) =>
        !string.IsNullOrWhiteSpace(questionId) && ByQuestionId.TryGetValue(questionId, out var item)
            ? item
            : null;

    public static Item? FindBySlug(string? slug) =>
        !string.IsNullOrWhiteSpace(slug) && BySlug.TryGetValue(slug.Trim(), out var item)
            ? item
            : null;

    public static string DetailPath(string slug) => $"/partner/specializations/{slug}";
}
