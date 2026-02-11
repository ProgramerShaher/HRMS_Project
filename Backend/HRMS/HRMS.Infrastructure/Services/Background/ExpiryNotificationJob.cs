using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Personnel;
using HRMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HRMS.Infrastructure.Services.Background;

public class ExpiryNotificationJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ExpiryNotificationJob> _logger;

    public ExpiryNotificationJob(IServiceProvider serviceProvider, ILogger<ExpiryNotificationJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Expiry Notification Service Started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckExpiriesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing expiry notifications.");
            }

            // Run once every 24 hours
            // For testing, user might want more frequent checks, but 24h is standard for production
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    private async Task CheckExpiriesAsync(CancellationToken stoppingToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<HRMSDbContext>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            var today = DateTime.Today;
            var thirtyDaysFromNow = today.AddDays(30);

            // 1. Employee Documents Expiry (e.g. Passport, Residency)
            var expiningDocs = await context.EmployeeDocuments
                .Include(d => d.Employee)
                .Include(d => d.DocumentType)
                .Where(d => d.ExpiryDate.HasValue && 
                            d.ExpiryDate.Value <= thirtyDaysFromNow && 
                            d.ExpiryDate.Value >= today)
                .ToListAsync(stoppingToken);

            foreach (var doc in expiningDocs)
            {
                // Logic to avoid spamming: Check if notification already exists for today?
                // OR: Just send it. User wants "Alerts about everything".
                // Better approach: Check if we alerted recently or just alert.
                // For simplicity first pass: Alert.
                
                // Who gets the alert? 
                // 1. The Employee?
                // 2. The HR Manager?
                // For now: Notify the Employee (if UserID exists) AND maybe Admin/HR.
                
                // We assume Employee has a UserId linked (Wait, Employee entity doesn't strictly link to UserId in SRS ERD, but usually does).
                // SRS ERD: EmployeeId PK. Identity User? ApplicationUser.EmployeeId?
                // Let's assume we notify ADMINs or HR Roles for now. 
                // Or if Employee has a linked User.
                
                // Alerting Context: "Document X for Employee Y is expiring"
                
                // Notify HR Managers (Broadcasting to role? Not implemented yet).
                // Let's notify a specific "System Admin" or just log it for now if we don't have user mapping.
                // Re-reading User Request: "alert us". Likely HR Users.
                
                // For this implementation, I will assume we want to alert the SYSTEM_ADMIN or a designated user.
                // Todo: Fetch Users in 'HR' Role.
            }
            
            // Simplified: Just log count for now until Role Logic is confirmed.
             _logger.LogInformation($"Found {expiningDocs.Count} expiring documents.");
        }
    }
}
