namespace SkillHub.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendWelcomeEmailAsync(string toEmail, string fullName, CancellationToken cancellationToken = default);
    Task SendEnrollmentNotificationAsync(string toEmail, string workshopTitle, DateTime startDate, CancellationToken cancellationToken = default);
}
