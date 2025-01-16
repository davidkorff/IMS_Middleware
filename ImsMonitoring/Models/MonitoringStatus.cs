namespace ImsMonitoring.Models;

public class MonitoringStatus
{
    public bool IsRunning { get; set; }
    public DateTime LastCheck { get; set; }
    public int SubmissionsProcessed { get; set; }
    public string CurrentStatus { get; set; } = string.Empty;
    public List<string> RecentErrors { get; set; } = new();
} 