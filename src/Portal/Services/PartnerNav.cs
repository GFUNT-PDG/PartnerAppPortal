namespace PartnerAppPortal.Portal.Services
{
    /// <summary>Partner portal route helpers (customer-scoped query string).</summary>
    public static class PartnerNav
    {
        public static string WithCustomer(string path, Guid? customerId, string? email = null)
        {
            var result = path;
            if (customerId is Guid id && id != Guid.Empty)
            {
                var sep = result.Contains('?', StringComparison.Ordinal) ? "&" : "?";
                result = $"{result}{sep}customerId={id}";
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                var sep = result.Contains('?', StringComparison.Ordinal) ? "&" : "?";
                result = $"{result}{sep}email={Uri.EscapeDataString(email)}";
            }

            return result;
        }

        public static string AppBoard(Guid? customerId, string? email = null) => WithCustomer("/dashboard/partner", customerId, email);
        public static string Assessment(Guid? customerId, string? email = null) => WithCustomer("/partner/assessment", customerId, email);
        public static string Designations(Guid? customerId, string? email = null) => WithCustomer("/partner/designations", customerId, email);
        public static string ActionPlans(Guid? customerId, string? email = null) => WithCustomer("/partner/action-plans", customerId, email);
        public static string Progress(Guid? customerId, string? email = null) => WithCustomer("/partner/progress", customerId, email);
        public static string Documents(Guid? customerId, string? email = null) => WithCustomer("/partner/documents", customerId, email);
        public static string Teams(Guid? customerId, string? email = null) => WithCustomer("/partner/teams", customerId, email);
        public static string SalesAccelerator(Guid? customerId, string? email = null) => WithCustomer("/partner/sales-accelerator", customerId, email);
    }
}
