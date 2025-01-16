namespace ImsMonitoring.Models;

public class Submission
{
    public Guid Id { get; set; }
    public string SubmissionId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime SubmissionDate { get; set; }
    public string InsuredGuid { get; set; } = string.Empty;
    public string QuoteGuid { get; set; } = string.Empty;
    public string CompanyLine { get; set; } = string.Empty;
    public bool NotificationSent { get; set; }
}