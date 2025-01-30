namespace ImsMonitoring.Models
{
    public class ExternalSystem
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;  // e.g., "AIM"
        public string Version { get; set; } = string.Empty;  // e.g., "2.0"
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation property
        public ICollection<ImsInstanceConnection> Connections { get; set; } = new List<ImsInstanceConnection>();
    }
} 