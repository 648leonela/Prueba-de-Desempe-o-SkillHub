namespace SkillHub.Application.DTOs.Workshops;

public class CreateWorkshopRequest
{
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Capacity { get; set; }
    public Guid InstructorId { get; set; }
}
