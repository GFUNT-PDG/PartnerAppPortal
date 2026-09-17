namespace PartnerAppPortal.Core.Models.Crm
{
    public class Customer
    {
        public string? AccountId { get; set; }
        public string? Name { get; set; }
        public string? PrimaryContact { get; set; }
        public string? AccountExecutive { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public DateTime? SyncedOn { get; set; }
        public Guid Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;

        // Added fields required by Step1
        public string? CustomerId { get; set; }
        
        public string? CustomerExperienceCX { get; set; }
        public string? DevelopmentDirector { get; set; }

        /// <summary>CRM Account field "Sales Accelerator Access" (Yes = true).</summary>
        public bool SalesAcceleratorAccess { get; set; }
    }
}
