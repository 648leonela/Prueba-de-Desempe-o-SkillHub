using SkillHub.Application.Common.Exceptions;
using SkillHub.Application.DTOs.Workshops;
using SkillHub.Application.Interfaces.Repositories;
using SkillHub.Application.Interfaces.Services;
using SkillHub.Domain.Entities;

namespace SkillHub.Application.UseCases.Workshops;

public class CreateWorkshopUseCase
{
    private readonly IWorkshopRepository _workshopRepository;
    private readonly IInstructorRepository _instructorRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public CreateWorkshopUseCase(
        IWorkshopRepository workshopRepository,
        IInstructorRepository instructorRepository,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _workshopRepository = workshopRepository;
        _instructorRepository = instructorRepository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> ExecuteAsync(CreateWorkshopRequest request, CancellationToken cancellationToken = default)
    {
        var codeExists = await _workshopRepository.ExistsByCodeAsync(request.Code, cancellationToken);
        if (codeExists)
            throw new ConflictException($"Ya existe un taller registrado con el código '{request.Code.ToUpperInvariant()}'.");

        var instructor = await _instructorRepository.GetByIdAsync(request.InstructorId, cancellationToken);
        if (instructor == null)
            throw new NotFoundException($"El instructor con ID '{request.InstructorId}' no existe.");

        if (!instructor.IsActive)
            throw new ValidationException("No se puede asignar un taller a un instructor inactivo.");

        var workshop = new Workshop(
            request.Code,
            request.Title,
            request.StartDate,
            request.EndDate,
            request.Capacity,
            request.InstructorId,
            _dateTimeProvider.UtcNow,
            request.Description);

        await _workshopRepository.AddAsync(workshop, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return workshop.Id;
    }
}
