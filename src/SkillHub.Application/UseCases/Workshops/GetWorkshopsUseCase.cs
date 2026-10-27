using SkillHub.Application.DTOs.Workshops;
using SkillHub.Application.Interfaces.Repositories;
using SkillHub.Application.Interfaces.Services;

namespace SkillHub.Application.UseCases.Workshops;

public class GetWorkshopsUseCase
{
    private readonly IWorkshopRepository _workshopRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GetWorkshopsUseCase(IWorkshopRepository workshopRepository, IDateTimeProvider dateTimeProvider)
    {
        _workshopRepository = workshopRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<IReadOnlyList<WorkshopDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var workshops = await _workshopRepository.GetAllAsync(cancellationToken);
        var now = _dateTimeProvider.UtcNow;

        return workshops.Select(w => new WorkshopDto
        {
            Id = w.Id,
            Code = w.Code,
            Title = w.Title,
            Description = w.Description,
            StartDate = w.StartDate,
            EndDate = w.EndDate,
            Capacity = w.Capacity,
            ActiveEnrollmentsCount = w.ActiveEnrollmentsCount,
            AvailableCapacity = Math.Max(0, w.Capacity - w.ActiveEnrollmentsCount),
            HasStarted = w.HasStarted(now),
            InstructorId = w.InstructorId,
            InstructorName = w.Instructor != null ? w.Instructor.FullName : string.Empty
        }).ToList();
    }
}
