using SkillHub.Application.Common.Exceptions;
using SkillHub.Application.DTOs.Workshops;
using SkillHub.Application.Interfaces.Repositories;
using SkillHub.Application.Interfaces.Services;

namespace SkillHub.Application.UseCases.Workshops;

public class GetWorkshopByIdUseCase
{
    private readonly IWorkshopRepository _workshopRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GetWorkshopByIdUseCase(IWorkshopRepository workshopRepository, IDateTimeProvider dateTimeProvider)
    {
        _workshopRepository = workshopRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<WorkshopDto> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var workshop = await _workshopRepository.GetByIdAsync(id, cancellationToken);
        if (workshop == null)
            throw new NotFoundException($"El taller con ID '{id}' no fue encontrado.");

        var now = _dateTimeProvider.UtcNow;

        return new WorkshopDto
        {
            Id = workshop.Id,
            Code = workshop.Code,
            Title = workshop.Title,
            Description = workshop.Description,
            StartDate = workshop.StartDate,
            EndDate = workshop.EndDate,
            Capacity = workshop.Capacity,
            ActiveEnrollmentsCount = workshop.ActiveEnrollmentsCount,
            AvailableCapacity = Math.Max(0, workshop.Capacity - workshop.ActiveEnrollmentsCount),
            HasStarted = workshop.HasStarted(now),
            InstructorId = workshop.InstructorId,
            InstructorName = workshop.Instructor != null ? workshop.Instructor.FullName : string.Empty
        };
    }
}
