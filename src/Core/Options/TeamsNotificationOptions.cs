namespace PartnerAppPortal.Core.Options;

public class TeamsNotificationOptions
{
    public const string SectionName = "Teams";

    /// <summary>Incoming webhook for new Sales Accelerator template submissions.</summary>
    public string PartnerDevelopmentTeamWebhook { get; set; } = string.Empty;

    /// <summary>Incoming webhook for duplicate-domain skip alerts.</summary>
    public string PartnerDevelopmentChannelWebhook { get; set; } = string.Empty;
}
