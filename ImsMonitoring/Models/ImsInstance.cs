using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using ImsMonitoring.Models;

namespace ImsMonitoring.Models
{
    public class ImsInstance
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Url]
        public string BaseUrl { get; set; } = string.Empty;  

        [Required]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "Program Code must be exactly 5 characters")]
        public string ProgramCode { get; set; } = string.Empty;  

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastSuccessfulConnection { get; set; }
        public string? Notes { get; set; }
        
        // Changed from Guid to string to match AspNetUsers.Id
        public string UserId { get; set; } = string.Empty;
        // Make the navigation property optional
        public ApplicationUser? User { get; set; }

        // Add this navigation property
        public ICollection<ImsInstanceConnection> Connections { get; set; } = new List<ImsInstanceConnection>();
    }
} 