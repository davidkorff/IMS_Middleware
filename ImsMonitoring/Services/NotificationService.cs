using Microsoft.EntityFrameworkCore;
using ImsMonitoring.Data;
using ImsMonitoring.Models;

namespace ImsMonitoring.Services;

public interface INotificationService
{
    Task SendSubmissionNotificationAsync(Submission submission);
    Task SendErrorNotificationAsync(string error);
}

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly IEmailService _emailService;
    private readonly ApplicationDbContext _context;

    public NotificationService(
        ILogger<NotificationService> logger,
        IEmailService emailService,
        ApplicationDbContext context)
    {
        _logger = logger;
        _emailService = emailService;
        _context = context;
    }

    public async Task SendSubmissionNotificationAsync(Submission submission)
    {
        try
        {
            var users = await _context.Users
                .Where(u => u.NotificationsEnabled)
                .ToListAsync();

            foreach (var user in users)
            {
                await _emailService.SendEmailAsync(
                    user.Email,
                    "New IMS Submission",
                    $"A new submission ({submission.SubmissionId}) has been received.\n" +
                    $"Status: {submission.Status}\n" +
                    $"Date: {submission.SubmissionDate}\n" +
                    $"Company Line: {submission.CompanyLine}"
                );
            }

            submission.NotificationSent = true;
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send submission notification");
            throw;
        }
    }

    public async Task SendErrorNotificationAsync(string error)
    {
        try
        {
            var admins = await _context.Users
                .Where(u => u.IsAdmin && u.NotificationsEnabled)
                .ToListAsync();

            foreach (var admin in admins)
            {
                await _emailService.SendEmailAsync(
                    admin.Email,
                    "IMS Monitoring Error",
                    $"An error occurred in the IMS monitoring system:\n{error}"
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send error notification");
            throw;
        }
    }
} 