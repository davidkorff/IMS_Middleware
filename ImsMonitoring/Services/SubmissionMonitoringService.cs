using Microsoft.Extensions.Hosting;
using ImsMonitoring.Data;
using ImsMonitoring.Models;

namespace ImsMonitoring.Services;

public class SubmissionMonitoringService : BackgroundService
{
    private readonly ILogger<SubmissionMonitoringService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    public int ProcessedCount { get; private set; }

    public SubmissionMonitoringService(
        ILogger<SubmissionMonitoringService> logger,
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                // TODO: Implement actual IMS submission monitoring
                _logger.LogInformation("Checking for new submissions...");
                ProcessedCount++;

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while monitoring submissions");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}