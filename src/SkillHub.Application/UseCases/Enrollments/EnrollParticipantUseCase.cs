using SkillHub.Application.Common.Exceptions;
using SkillHub.Application.DTOs.Enrollments;
using SkillHub.Application.Interfaces.Repositories;
using SkillHub.Application.Interfaces.Services;
using SkillHub.Domain.Entities;

namespace SkillHub.Application.UseCases.Enrollments;

public class EnrollParticipantUseCase
{
    private readonly IWorkshopRepository _workshopRepository;
    private readonly IParticipantRepository _participantRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public EnrollParticipantUseCase(
        IWorkshopRepository workshopRepository,
        IParticipantRepository participantRepository,
        IEnrollmentRepository enrollmentRepository,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _workshopRepository = workshopRepository;
        _participantRepository = participantRepository;
        _enrollmentRepository = enrollmentRepository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> ExecuteAsync(CreateEnrollmentRequest request, CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.IsAuthenticated || string.IsNullOrWhiteSpace(_currentUserService.UserId))
            throw new ValidationException("El usuario debe estar autenticado para realizar una inscripción.");

        var participant = await _participantRepository.GetByIdentityUserIdAsync(_currentUserService.UserId, cancellationToken);
        if (participant == null)
            throw new NotFoundException("No se encontró el perfil de participante asociado al usuario autenticado.");

        var workshop = await _workshopRepository.GetByIdAsync(request.WorkshopId, cancellationToken);
        if (workshop == null)
            throw new NotFoundException($"El taller con ID '{request.WorkshopId}' no existe.");

        var now = _dateTimeProvider.UtcNow;

        if (workshop.HasStarted(now))
            throw new ValidationException("No es posible inscribirse a un taller que ya ha comenzado.");

        if (!workshop.HasAvailableCapacity)
            throw new ValidationException("El taller no cuenta con cupos disponibles.");

        var hasActiveEnrollment = await _enrollmentRepository.HasActiveEnrollmentAsync(participant.Id, workshop.Id, cancellationToken);
        if (hasActiveEnrollment)
            throw new ConflictException("Ya cuenta con una inscripción activa en este taller.");

        var hasScheduleConflict = await _enrollmentRepository.HasScheduleConflictAsync(
            participant.Id,
            workshop.StartDate,
            workshop.EndDate,
            cancellationToken);

        if (hasScheduleConflict)
            throw new ConflictException("Tiene un conflicto de horario con otro taller en el que se encuentra inscrito.");

        var enrollment = new Enrollment(participant.Id, workshop.Id, workshop.StartDate, now);

        await _enrollmentRepository.AddAsync(enrollment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return enrollment.Id;
    }
}
