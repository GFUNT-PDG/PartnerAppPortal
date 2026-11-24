namespace PartnerAppPortal.Core.Models.Onboarding;

public enum OnboardingInput
{
    Text,
    YesNo,
    Date,
    Designation,
    CspType,
    Status,
    Territory
}

public enum OnboardingKind
{
    Text,
    YesNo,
    Dropdown,
    MultiSelect,
    Date,
    Number,
    Lookup
}

public sealed record OnboardingField(
    string Key,
    string Label,
    OnboardingInput Input,
    string LogicalName,
    OnboardingKind Kind,
    bool Repeating = false);

public sealed record OnboardingSection(string Title, IReadOnlyList<OnboardingField> Fields);

public sealed record ChecklistRow(string Key, string Label, string Value);

/// <summary>
/// Field list from the Dynamics Details Template and Activity Checklist workbook.
/// Dropdown labels match the approved lists on the How To Use sheet, including the
/// engagement status label "Engagment Launch".
/// </summary>
public static class OnboardingCatalog
{
    public const string AllDynamicsDetailsKey = "allDynamicsDetails";

    public static readonly string[] YesNo = ["", "Yes", "No"];
    public static readonly string[] Designations = ["", "Yes", "No", "Targeted", "Out of Scope"];
    public static readonly string[] CspTypes = ["", "Indirect CSP", "Direct CSP"];
    public static readonly string[] Statuses = ["", "Active", "Engagment Launch", "Renewal Period", "Complete"];
    public static readonly string[] Territories = ["", "Multi-National", "National", "Regional"];

    public static IReadOnlyList<OnboardingSection> Sections { get; } =
    [
        new("Account Information",
        [
            new("accountType", "Account Type", OnboardingInput.Text, "cr3f8_accounttype", OnboardingKind.Dropdown)
        ]),
        new("Internal Contacts",
        [
            new("accountExecutive", "Account Executive", OnboardingInput.Text, "cr3f8_accountexecutive", OnboardingKind.Lookup),
            new("developmentDirector", "Development Director", OnboardingInput.Text, "cr3f8_developmentdirector", OnboardingKind.Lookup),
            new("customerExperience", "Customer Experience (CS)", OnboardingInput.Text, "cr3f8_customerexperiencecx", OnboardingKind.Lookup)
        ]),
        new("Microsoft Partnership / PDG Engagement",
        [
            new("partnerType", "Partner Type", OnboardingInput.Text, "cr3f8_partnertype", OnboardingKind.MultiSelect),
            new("engagementType", "Engagement Type", OnboardingInput.Text, "cr3f8_engagementtype", OnboardingKind.Dropdown),
            new("salesAcceleratorAccess", "Sales Accelerator Access", OnboardingInput.YesNo, "cr3f8_salesacceleratoraccess", OnboardingKind.YesNo),
            new("engagementStatus", "Engagement Status", OnboardingInput.Status, "cr3f8_engagementstatus", OnboardingKind.Dropdown),
            new("engagementStart", "Engagement Start Date", OnboardingInput.Date, "cr3f8_engagementstartdate", OnboardingKind.Date),
            new("engagementEnd", "Engagement End Date", OnboardingInput.Date, "cr3f8_engagementenddate", OnboardingKind.Date),
            new("engagementLength", "Engagement Length", OnboardingInput.Text, "cr3f8_engagementlength", OnboardingKind.Dropdown)
        ]),
        new("CSP Details",
        [
            new("registeredCsp", "Registered CSP", OnboardingInput.YesNo, "cr3f8_registeredcsp", OnboardingKind.YesNo),
            new("cspEnrollmentType", "CSP Enrollment Type", OnboardingInput.CspType, "cr3f8_cspenrollmenttype", OnboardingKind.Dropdown),
            new("cspDistributor", "CSP Distributor", OnboardingInput.Text, "cr3f8_cspdistributor", OnboardingKind.Text)
        ]),
        new("Service Details",
        [
            new("serviceTerritory", "Service Territory", OnboardingInput.Territory, "cr3f8_serviceterritory", OnboardingKind.Dropdown),
            new("serviceTerritoryNotes", "Service Territory Notes", OnboardingInput.Text, "cr3f8_serviceterritorynotes", OnboardingKind.Text)
        ]),
        new("Customer Industries",
        [
            new("industry1", "Customer Industry 1", OnboardingInput.Text, "cr3f8_customerindustry1", OnboardingKind.Text, true),
            new("industry2", "Customer Industry 2", OnboardingInput.Text, "cr3f8_customerindustry2", OnboardingKind.Text, true),
            new("industry3", "Customer Industry 3", OnboardingInput.Text, "cr3f8_customerindustry3", OnboardingKind.Text, true),
            new("industry4", "Customer Industry 4", OnboardingInput.Text, "cr3f8_customerindustry4", OnboardingKind.Text, true),
            new("industry5", "Customer Industry 5", OnboardingInput.Text, "cr3f8_customerindustry5", OnboardingKind.Text, true)
        ]),
        new("Primary Competition",
        [
            new("competitor1", "Competitor 1", OnboardingInput.Text, "cr3f8_competitor1", OnboardingKind.Text, true),
            new("competitor2", "Competitor 2", OnboardingInput.Text, "cr3f8_competitor2", OnboardingKind.Text, true),
            new("competitor3", "Competitor 3", OnboardingInput.Text, "cr3f8_competitor3", OnboardingKind.Text, true)
        ]),
        new("Customer Sizing",
        [
            new("smb", "Small Business (SMB)", OnboardingInput.YesNo, "cr3f8_smallbusinesssmb", OnboardingKind.YesNo, true),
            new("smc", "Mid-Market (SMC)", OnboardingInput.YesNo, "cr3f8_midmarketsmc", OnboardingKind.YesNo, true),
            new("ent", "Enterprise (ENT)", OnboardingInput.YesNo, "cr3f8_enterpriseent", OnboardingKind.YesNo, true)
        ]),
        new("Customer Details",
        [
            new("customer1", "Primary Customer 1", OnboardingInput.Text, "cr3f8_primarycustomer1", OnboardingKind.Text, true),
            new("customer2", "Primary Customer 2", OnboardingInput.Text, "cr3f8_primarycustomer2", OnboardingKind.Text, true),
            new("customer3", "Primary Customer 3", OnboardingInput.Text, "cr3f8_primarycustomer3", OnboardingKind.Text, true),
            new("customer4", "Primary Customer 4", OnboardingInput.Text, "cr3f8_primarycustomer4", OnboardingKind.Text, true),
            new("customer5", "Primary Customer 5", OnboardingInput.Text, "cr3f8_primarycustomer5", OnboardingKind.Text, true),
            new("customer6", "Primary Customer 6", OnboardingInput.Text, "cr3f8_primarycustomer6", OnboardingKind.Text, true)
        ]),
        new("Microsoft Partner IDs",
        [
            new("globalPartnerId", "Global Partner ID", OnboardingInput.Text, "cr3f8_globalpartnerid", OnboardingKind.Text),
            new("registeredSupplier", "Registered Supplier", OnboardingInput.YesNo, "cr3f8_programsregisteredsupplier", OnboardingKind.YesNo),
            new("ecifSupplierId", "ECIF Supplier ID", OnboardingInput.Text, "cr3f8_idecifsupplierid", OnboardingKind.Text)
        ]),
        new("Commercial Marketplace",
        [
            new("publisherId", "Publisher ID", OnboardingInput.Text, "cr3f8_idpublisherid", OnboardingKind.Text)
        ]),
        new("Designations",
        [
            new("desSecurity", "Security", OnboardingInput.Designation, "cr3f8_designationsecurity", OnboardingKind.Dropdown),
            new("desModernWork", "Modern Work", OnboardingInput.Designation, "cr3f8_designationmodernwork", OnboardingKind.Dropdown),
            new("desInfrastructure", "Infrastructure", OnboardingInput.Designation, "cr3f8_designationinfrastructure", OnboardingKind.Dropdown),
            new("desDataAi", "Data & AI", OnboardingInput.Designation, "cr3f8_designationdataai", OnboardingKind.Dropdown),
            new("desDigitalApp", "Digital & App Innovation", OnboardingInput.Designation, "cr3f8_designationdigitalappinnovation", OnboardingKind.Dropdown),
            new("desBusinessApps", "Business Applications", OnboardingInput.Designation, "cr3f8_designationbusinessapplications", OnboardingKind.Dropdown)
        ]),
        new("Specializations",
        [
            new("specCompleted", "Specialization - Completed", OnboardingInput.Text, "cr3f8_specializationcompleted", OnboardingKind.Number),
            new("specInProgress", "Specialization - In-Progress", OnboardingInput.Text, "cr3f8_specializationinprogress", OnboardingKind.Number)
        ]),
        new("Microsoft Programs",
        [
            new("isvSuccess", "ISV Success Program", OnboardingInput.YesNo, "cr3f8_programsisvsuccessprogram", OnboardingKind.YesNo),
            new("misa", "MISA Membership", OnboardingInput.YesNo, "cr3f8_misamembership", OnboardingKind.YesNo),
            new("copilotSpec", "Copilot Specialization", OnboardingInput.YesNo, "cr3f8_programscopilotspecialization", OnboardingKind.YesNo),
            new("supportDesignation", "Support Designation", OnboardingInput.YesNo, "cr3f8_programssupportdesignation", OnboardingKind.YesNo),
            new("fastTrack", "FastTrack Ready", OnboardingInput.YesNo, "cr3f8_programsfasttrackready", OnboardingKind.YesNo),
            new("fabricFeatured", "Fabric Featured", OnboardingInput.YesNo, "cr3f8_programsfabricfeatured", OnboardingKind.YesNo),
            new("trainingServices", "Training Services", OnboardingInput.YesNo, "cr3f8_programstrainingservices", OnboardingKind.YesNo),
            new("surfaceReseller", "Surface Reseller", OnboardingInput.YesNo, "cr3f8_programssurfacereseller", OnboardingKind.YesNo)
        ])
    ];

    public static IReadOnlyList<OnboardingField> InputFields { get; } =
        Sections.SelectMany(s => s.Fields).ToList();

    public static IReadOnlyList<(string Key, string Label)> ChecklistItems { get; } =
    [
        (AllDynamicsDetailsKey, "All Dynamics Details"),
        ("partnerCenterAssessment", "Partner Center Assessment"),
        ("marketplaceOfferingPlan", "Marketplace Offering Plan"),
        ("partnerSuccessPlan", "Partner Success Plan"),
        ("superPowerSlide", "SuperPower Slide"),
        ("winWireTemplates", "WinWire Templates"),
        ("caseStudyTemplate", "Case Study Template"),
        ("salesPitchOnePager", "Sales Pitch 1-Pager")
    ];

    public static string[] Choices(OnboardingInput input) => input switch
    {
        OnboardingInput.YesNo => YesNo,
        OnboardingInput.Designation => Designations,
        OnboardingInput.CspType => CspTypes,
        OnboardingInput.Status => Statuses,
        OnboardingInput.Territory => Territories,
        _ => YesNo
    };

    public static bool DynamicsDetailsComplete(IReadOnlyDictionary<string, string?> fields)
    {
        foreach (var section in Sections)
        {
            var repeating = section.Fields.Where(field => field.Repeating).ToList();
            if (repeating.Count > 0 && repeating.All(field => string.IsNullOrWhiteSpace(ValueOf(fields, field.Key))))
                return false;

            foreach (var field in section.Fields.Where(field => !field.Repeating))
            {
                if (string.IsNullOrWhiteSpace(ValueOf(fields, field.Key)))
                    return false;
            }
        }

        return Sections.Count > 0;
    }

    private static string? ValueOf(IReadOnlyDictionary<string, string?> fields, string key) =>
        fields.TryGetValue(key, out var value) ? value : null;

    public static bool DynamicsDetailsStarted(IReadOnlyDictionary<string, string?> fields) =>
        InputFields.Any(field => fields.TryGetValue(field.Key, out var value) && !string.IsNullOrWhiteSpace(value));

    public static bool IsChecklistKey(string? key) =>
        ChecklistItems.Any(item => string.Equals(item.Key, key, StringComparison.OrdinalIgnoreCase));

    public static IReadOnlyList<ChecklistRow> BuildChecklist(
        IReadOnlyDictionary<string, string?> checklist)
    {
        var rows = new List<ChecklistRow>(ChecklistItems.Count);
        foreach (var item in ChecklistItems)
        {
            checklist.TryGetValue(item.Key, out var value);
            rows.Add(new ChecklistRow(item.Key, item.Label, NormalizeYesNo(value)));
        }

        return rows;
    }

    public static PartnerOnboardingChecklistView ToChecklistView(PartnerOnboardingDocument document)
    {
        var rows = BuildChecklist(document.Checklist).ToList();
        var detailsComplete = DynamicsDetailsComplete(document.Fields);
        if (document.FieldsFromDynamics)
        {
            var index = rows.FindIndex(row =>
                string.Equals(row.Key, AllDynamicsDetailsKey, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
                rows[index] = rows[index] with { Value = detailsComplete ? "Yes" : "No" };
        }

        document.Fields.TryGetValue("engagementType", out var engagementType);
        document.Fields.TryGetValue("engagementStart", out var engagementStart);
        return new PartnerOnboardingChecklistView
        {
            AccountId = document.AccountId,
            AccountName = document.AccountName,
            EngagementType = engagementType,
            EngagementStart = engagementStart,
            Items = rows,
            ItemsComplete = ItemsComplete(rows),
            PercentComplete = PercentComplete(rows),
            DynamicsDetailsComplete = detailsComplete
        };
    }

    public static int ItemsComplete(IReadOnlyList<ChecklistRow> rows) =>
        rows.Count(row => string.Equals(row.Value, "Yes", StringComparison.OrdinalIgnoreCase));

    public static int PercentComplete(IReadOnlyList<ChecklistRow> rows) =>
        rows.Count == 0 ? 0 : (int)Math.Round(ItemsComplete(rows) * 100d / rows.Count);

    public static string NormalizeYesNo(string? value)
    {
        if (string.Equals(value, "Yes", StringComparison.OrdinalIgnoreCase))
            return "Yes";
        if (string.Equals(value, "No", StringComparison.OrdinalIgnoreCase))
            return "No";
        return "";
    }
}
