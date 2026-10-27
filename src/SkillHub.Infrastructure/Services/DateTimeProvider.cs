using SkillHub.Application.Interfaces.Services;

namespace SkillHub.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
