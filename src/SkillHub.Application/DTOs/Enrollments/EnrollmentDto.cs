using SkillHub.Domain.Enums;

namespace SkillHub.Application.DTOs.Enrollments;

public class EnrollmentDto
{
    public Guid Id { get; set; }
    public Guid ParticipantId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public string ParticipantEmail { get; set; } = string.Empty;
    public Guid WorkshopId { get; set; }
    public string WorkshopCode { get; set; } = string.Empty;
    public string WorkshopTitle { get; set; } = string.Empty;
    public DateTime WorkshopStartDate { get; set; }
    public DateTime WorkshopEndDate { get; set; }
    public DateTime EnrolledAt { get; set; }
    public EnrollmentStatus Status { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelledBy { get; set; }
    public string? CancellationReason { get; set; }
}
