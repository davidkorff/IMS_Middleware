using System.ComponentModel.DataAnnotations;

namespace ImsMonitoring.Models
{
    public class AimInstance
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Url]
        public string BaseUrl { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastSuccessfulConnection { get; set; }
        public string? Notes { get; set; }
        public string? CurrentToken { get; set; }
        public DateTime? TokenExpiration { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
    }
} 