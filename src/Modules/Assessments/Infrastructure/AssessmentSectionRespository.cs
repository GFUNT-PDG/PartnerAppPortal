namespace PartnerAppPortal.Modules.Assessments.Infrastructure;

public class AssessmentSectionRepository
{
    public List<string> LoadSections()
    {
        return new() { "Solutions", "CSP", "Co-Sell", "Marketplace", "Incentives" };
    }
}
