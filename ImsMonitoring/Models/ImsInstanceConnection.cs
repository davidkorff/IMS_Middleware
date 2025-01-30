namespace ImsMonitoring.Models
{
    public class ImsInstanceConnection
    {
        public Guid Id { get; set; }
        public Guid ImsInstanceId { get; set; }
        public Guid ExternalSystemId { get; set; }
        
        // Connection details
        public string ConnectionString { get; set; } = string.Empty;
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? ApiKey { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastSuccessfulConnection { get; set; }
        public string? Notes { get; set; }

        // Navigation properties
        public ImsInstance ImsInstance { get; set; } = null!;
        public ExternalSystem ExternalSystem { get; set; } = null!;
    }
} 