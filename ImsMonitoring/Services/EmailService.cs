namespace ImsMonitoring.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body)
    {
        _logger.LogInformation("Email sent to {to}: {subject}", to, subject);
        // TODO: Implement actual email sending
        return Task.CompletedTask;
    }
} 