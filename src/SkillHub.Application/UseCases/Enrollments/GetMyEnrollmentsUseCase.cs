using SkillHub.Application.Common.Exceptions;
using SkillHub.Application.DTOs.Enrollments;
using SkillHub.Application.Interfaces.Repositories;
using SkillHub.Application.Interfaces.Services;

namespace SkillHub.Application.UseCases.Enrollments;

public class GetMyEnrollmentsUseCase
{
    private readonly IParticipantRepository _participantRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMyEnrollmentsUseCase(
        IParticipantRepository participantRepository,
        IEnrollmentRepository enrollmentRepository,
        ICurrentUserService currentUserService)
    {
        _participantRepository = participantRepository;
        _enrollmentRepository = enrollmentRepository;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<EnrollmentDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.IsAuthenticated || string.IsNullOrWhiteSpace(_currentUserService.UserId))
            throw new ValidationException("El usuario debe estar autenticado para consultar sus inscripciones.");

        var participant = await _participantRepository.GetByIdentityUserIdAsync(_currentUserService.UserId, cancellationToken);
        if (participant == null)
            throw new NotFoundException("No se encontró el perfil de participante asociado al usuario autenticado.");

        var enrollments = await _enrollmentRepository.GetByParticipantIdAsync(participant.Id, cancellationToken);

        return enrollments.Select(e => new EnrollmentDto
        {
            Id = e.Id,
            ParticipantId = e.ParticipantId,
            ParticipantName = e.Participant != null ? e.Participant.FullName : participant.FullName,
            ParticipantEmail = e.Participant != null ? e.Participant.Email : participant.Email,
            WorkshopId = e.WorkshopId,
            WorkshopCode = e.Workshop != null ? e.Workshop.Code : string.Empty,
            WorkshopTitle = e.Workshop != null ? e.Workshop.Title : string.Empty,
            WorkshopStartDate = e.Workshop != null ? e.Workshop.StartDate : DateTime.MinValue,
            WorkshopEndDate = e.Workshop != null ? e.Workshop.EndDate : DateTime.MinValue,
            EnrolledAt = e.EnrolledAt,
            Status = e.Status,
            CancelledAt = e.CancelledAt,
            CancelledBy = e.CancelledBy,
            CancellationReason = e.CancellationReason
        }).ToList();
    }
}
