namespace PartnerAppPortal.Infrastructure;

/// <summary>
/// Assessments.SubmittedAt is NOT NULL datetimeoffset in Azure SQL.
/// Use a sentinel for in-progress rows.
/// </summary>
public static class AssessmentStatus
{
    // SQL-safe sentinel for "not submitted yet"
    public static readonly DateTimeOffset NotSubmittedUtc =
        new(1753, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public static bool IsSubmitted(DateTimeOffset? submittedAt) =>
        submittedAt.HasValue && submittedAt.Value > NotSubmittedUtc;

    public static DateTime? Normalize(DateTimeOffset? submittedAt) =>
        IsSubmitted(submittedAt) ? submittedAt!.Value.UtcDateTime : null;
}
