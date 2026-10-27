using SkillHub.Application.Common.Exceptions;
using SkillHub.Application.DTOs.Enrollments;
using SkillHub.Application.Interfaces.Repositories;
using SkillHub.Application.Interfaces.Services;

namespace SkillHub.Application.UseCases.Enrollments;

public class CancelEnrollmentUseCase
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IWorkshopRepository _workshopRepository;
    private readonly IParticipantRepository _participantRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public CancelEnrollmentUseCase(
        IEnrollmentRepository enrollmentRepository,
        IWorkshopRepository workshopRepository,
        IParticipantRepository participantRepository,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _enrollmentRepository = enrollmentRepository;
        _workshopRepository = workshopRepository;
        _participantRepository = participantRepository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid enrollmentId, CancelEnrollmentRequest request, CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.IsAuthenticated || string.IsNullOrWhiteSpace(_currentUserService.UserId))
            throw new ValidationException("El usuario debe estar autenticado para cancelar una inscripción.");

        var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId, cancellationToken);
        if (enrollment == null)
            throw new NotFoundException($"La inscripción con ID '{enrollmentId}' no fue encontrada.");

        var workshop = await _workshopRepository.GetByIdAsync(enrollment.WorkshopId, cancellationToken);
        if (workshop == null)
            throw new NotFoundException($"El taller asociado a la inscripción no fue encontrado.");

        var now = _dateTimeProvider.UtcNow;

        if (_currentUserService.IsEmployee)
        {
            // Empleado cancela antes del inicio del taller sin restricción de 24 horas
            enrollment.CancelByEmployee(workshop.StartDate, now, request.Reason);
        }
        else
        {
            // Participante solo puede cancelar sus propias inscripciones y con al menos 24 horas de anticipación
            var participant = await _participantRepository.GetByIdentityUserIdAsync(_currentUserService.UserId, cancellationToken);
            if (participant == null || participant.Id != enrollment.ParticipantId)
                throw new ValidationException("No tiene permisos para cancelar una inscripción de otro participante.");

            enrollment.CancelByParticipant(workshop.StartDate, now, request.Reason);
        }

        await _enrollmentRepository.UpdateAsync(enrollment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
