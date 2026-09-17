namespace PartnerAppPortal.Core.Services.SalesAccelerator;

public sealed class ParsedSalesAcceleratorRow
{
    public int LineNumber { get; init; }
    public string CompanyName { get; init; } = string.Empty;
    public string Domain { get; init; } = string.Empty;
    public string NormalizedDomain { get; init; } = string.Empty;
    public string? AccountType { get; init; }
    public string? Error { get; init; }
    public bool IsValid => string.IsNullOrEmpty(Error);
}

public static class SalesAcceleratorCsvParser
{
    public static string TemplateCsv => "Company Name,Domain,Account Type\r\n";

    public static IReadOnlyList<ParsedSalesAcceleratorRow> Parse(string csvText)
    {
        if (string.IsNullOrWhiteSpace(csvText))
            return Array.Empty<ParsedSalesAcceleratorRow>();

        var lines = SplitLines(csvText);
        if (lines.Count == 0)
            return Array.Empty<ParsedSalesAcceleratorRow>();

        var delimiter = DetectDelimiter(lines[0]);
        var header = ParseLine(lines[0], delimiter);
        var hasHeader = LooksLikeHeader(header);
        var companyIdx = hasHeader ? IndexOf(header, "company name", "companyname", "company") : 0;
        var domainIdx = hasHeader ? IndexOf(header, "domain") : 1;
        var typeIdx = hasHeader ? IndexOf(header, "account type", "accounttype", "type") : 2;

        if (hasHeader && (companyIdx < 0 || domainIdx < 0))
            throw new InvalidOperationException(
                "CSV header must include Company Name and Domain (Account Type is optional).");

        var start = hasHeader ? 1 : 0;
        var rows = new List<ParsedSalesAcceleratorRow>();

        for (var i = start; i < lines.Count; i++)
        {
            var lineNumber = i + 1;
            var cells = ParseLine(lines[i], delimiter);
            if (cells.All(string.IsNullOrWhiteSpace))
                continue;

            var company = Cell(cells, companyIdx);
            var domain = Cell(cells, domainIdx);
            var accountType = typeIdx >= 0 ? Cell(cells, typeIdx) : string.Empty;
            var normalized = NormalizeDomain(domain);

            string? error = null;
            if (string.IsNullOrWhiteSpace(company) || string.IsNullOrWhiteSpace(normalized))
                error = "Company Name and Domain are required.";

            rows.Add(new ParsedSalesAcceleratorRow
            {
                LineNumber = lineNumber,
                CompanyName = company,
                Domain = domain.Trim(),
                NormalizedDomain = normalized,
                AccountType = string.IsNullOrWhiteSpace(accountType) ? null : accountType.Trim(),
                Error = error
            });
        }

        return rows;
    }

    public static string NormalizeDomain(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var s = value.Trim().ToLowerInvariant();
        if (s.StartsWith("https://", StringComparison.Ordinal))
            s = s["https://".Length..];
        else if (s.StartsWith("http://", StringComparison.Ordinal))
            s = s["http://".Length..];

        if (s.StartsWith("www.", StringComparison.Ordinal))
            s = s[4..];

        var cut = s.IndexOfAny(['/', '?', '#']);
        if (cut >= 0)
            s = s[..cut];

        return s.Trim().TrimEnd('.');
    }

    private static bool LooksLikeHeader(IReadOnlyList<string> cells)
    {
        var joined = string.Join(" ", cells).ToLowerInvariant();
        return joined.Contains("company") || joined.Contains("domain") || joined.Contains("account type");
    }

    private static int IndexOf(IReadOnlyList<string> header, params string[] names)
    {
        for (var i = 0; i < header.Count; i++)
        {
            var key = Compact(header[i]);
            if (names.Any(n => Compact(n) == key))
                return i;
        }

        return -1;
    }

    private static string Compact(string value) =>
        new string(value.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();

    private static string Cell(IReadOnlyList<string> cells, int index) =>
        index >= 0 && index < cells.Count ? cells[index].Trim() : string.Empty;

    private static char DetectDelimiter(string headerLine)
    {
        var commas = headerLine.Count(c => c == ',');
        var semis = headerLine.Count(c => c == ';');
        return semis > commas ? ';' : ',';
    }

    private static List<string> SplitLines(string text)
    {
        return text
            .Replace("\r\n", "\n")
            .Replace('\r', '\n')
            .Split('\n', StringSplitOptions.None)
            .Select(l => l.Trim())
            .Where(l => l.Length > 0)
            .ToList();
    }

    private static List<string> ParseLine(string line, char delimiter)
    {
        var cells = new List<string>();
        var current = new System.Text.StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == delimiter && !inQuotes)
            {
                cells.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        cells.Add(current.ToString());
        return cells;
    }
}
