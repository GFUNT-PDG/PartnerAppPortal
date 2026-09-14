namespace PartnerAppPortal.Core.Models.Crm
{
    /// <summary>Synced CRM contact row (dbo.CrmContacts).</summary>
    public class CrmContact
    {
        public string ContactId { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        /// <summary>AccountId as string (matches CrmAccounts.AccountId).</summary>
        public string? AccountId { get; set; }
    }
}
