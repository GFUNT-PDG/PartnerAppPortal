namespace PartnerAppPortal.Portal.Services;

public sealed class SiteReportFiles
{
    public const string SuccessPlan = "SuccessPlan";
    public const string AssessmentHealth = "AssessmentHealth";
    public const string MarketplaceMaturity = "MarketplaceMaturity";
    public const string SuperPowerSlidePdf = "SuperPowerSlidePdf";
    public const string SuperPowerSlidePptx = "SuperPowerSlidePptx";
    public const string SalesOverviewPdf = "SalesOverviewPdf";
    public const string SalesOverviewPptx = "SalesOverviewPptx";

    private readonly Dictionary<string, string> _reports;

    public SiteReportFiles(IConfiguration configuration)
    {
        _reports = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [SuccessPlan] = configuration["SiteReports:Reports:SuccessPlan"]
                ?? "Portal Artifacts/{CustomerName} - Partner Success Plan.html",
            [AssessmentHealth] = configuration["SiteReports:Reports:AssessmentHealth"]
                ?? "Portal Artifacts/AssessmentHealth.html",
            [MarketplaceMaturity] = configuration["SiteReports:Reports:MarketplaceMaturity"]
                ?? "Portal Artifacts/{CustomerName} - Marketplace Maturity Score.html",
            [SuperPowerSlidePdf] = configuration["SiteReports:Reports:SuperPowerSlidePdf"]
                ?? "Sales Artifacts/{CustomerName} - SuperPower Slide.pdf",
            [SuperPowerSlidePptx] = configuration["SiteReports:Reports:SuperPowerSlidePptx"]
                ?? "Sales Artifacts/{CustomerName} - SuperPower Slide.pptx",
            [SalesOverviewPdf] = configuration["SiteReports:Reports:SalesOverviewPdf"]
                ?? "Sales Artifacts/{CustomerName} - Partner Pitch.pdf",
            [SalesOverviewPptx] = configuration["SiteReports:Reports:SalesOverviewPptx"]
                ?? "Sales Artifacts/{CustomerName} - Partner Pitch.pptx"
        };
    }

    public string RelativePathFor(string reportKey, string customerName)
    {
        if (!_reports.TryGetValue(reportKey, out var name) || string.IsNullOrWhiteSpace(name))
            throw new InvalidOperationException($"Unknown site report '{reportKey}'.");

        var partner = (customerName ?? "").Trim();
        var relative = name
            .Replace("{CustomerName}", partner, StringComparison.OrdinalIgnoreCase)
            .Replace("{PartnerName}", partner, StringComparison.OrdinalIgnoreCase)
            .Replace('\\', '/')
            .Trim()
            .TrimStart('/');

        if (relative.Length == 0 || relative.Contains("..", StringComparison.Ordinal) || relative.Contains(':'))
            throw new InvalidOperationException("That site report file path is not allowed.");

        return relative;
    }

    public string FileNameFor(string reportKey, string customerName)
    {
        var relative = RelativePathFor(reportKey, customerName);
        return relative.Split('/').Last();
    }

    public string FileHref(Guid customerId, string reportKey, string customerName, bool download = false)
    {
        var encoded = EncodeRelativePath(RelativePathFor(reportKey, customerName));
        var href = $"/partner/site-reports/file/{customerId}/{encoded}";
        return download ? href + "?download=true" : href;
    }

    public string AssetBaseHref(Guid customerId, string reportKey, string customerName)
    {
        var relative = RelativePathFor(reportKey, customerName);
        var slash = relative.LastIndexOf('/');
        var folder = slash < 0 ? "" : relative[..slash];
        var encoded = string.IsNullOrEmpty(folder) ? "" : EncodeRelativePath(folder) + "/";
        return $"/partner/site-reports/file/{customerId}/{encoded}";
    }

    public static string EncodeRelativePath(string relative)
    {
        var parts = (relative ?? "")
            .Replace('\\', '/')
            .Split('/', StringSplitOptions.RemoveEmptyEntries);
        return string.Join("/", parts.Select(Uri.EscapeDataString));
    }

    public static string PrepareForFrame(string html, string baseHref)
    {
        if (html.IndexOf("<html", StringComparison.OrdinalIgnoreCase) < 0)
        {
            html = "<!DOCTYPE html><html><head><meta charset=\"utf-8\"></head><body>"
                + html
                + "</body></html>";
        }

        var baseTag = $"<base href=\"{baseHref}\">";
        var head = html.IndexOf("<head", StringComparison.OrdinalIgnoreCase);
        if (head >= 0)
        {
            var end = html.IndexOf('>', head);
            if (end >= 0)
                return html.Insert(end + 1, baseTag);
        }

        return baseTag + html;
    }

    public static string ContentTypeFor(string path, string? fromSource)
    {
        if (!string.IsNullOrWhiteSpace(fromSource) &&
            fromSource.IndexOf("octet-stream", StringComparison.OrdinalIgnoreCase) < 0)
            return fromSource;

        return Path.GetExtension(path).ToLowerInvariant() switch
        {
            ".html" or ".htm" => "text/html; charset=utf-8",
            ".css" => "text/css; charset=utf-8",
            ".js" => "text/javascript; charset=utf-8",
            ".json" => "application/json; charset=utf-8",
            ".svg" => "image/svg+xml",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".pdf" => "application/pdf",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ".ppt" => "application/vnd.ms-powerpoint",
            _ => "application/octet-stream"
        };
    }
}
