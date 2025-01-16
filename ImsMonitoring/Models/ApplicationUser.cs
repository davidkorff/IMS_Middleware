using Microsoft.AspNetCore.Identity;

namespace ImsMonitoring.Models;

public class ApplicationUser : IdentityUser
{
    public bool NotificationsEnabled { get; set; } = true;
    public bool IsAdmin { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 