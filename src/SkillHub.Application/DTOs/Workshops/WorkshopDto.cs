namespace SkillHub.Application.DTOs.Workshops;

public class WorkshopDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Capacity { get; set; }
    public int ActiveEnrollmentsCount { get; set; }
    public int AvailableCapacity { get; set; }
    public bool HasStarted { get; set; }
    public Guid InstructorId { get; set; }
    public string InstructorName { get; set; } = string.Empty;
}
