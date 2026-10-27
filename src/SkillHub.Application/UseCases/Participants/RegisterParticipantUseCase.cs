using SkillHub.Application.Common.Exceptions;
using SkillHub.Application.DTOs.Participants;
using SkillHub.Application.Interfaces.Repositories;
using SkillHub.Application.Interfaces.Services;
using SkillHub.Domain.Entities;

namespace SkillHub.Application.UseCases.Participants;

public class RegisterParticipantUseCase
{
    private readonly IParticipantRepository _participantRepository;
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterParticipantUseCase(
        IParticipantRepository participantRepository,
        IAuthService authService,
        IEmailService emailService,
        IUnitOfWork unitOfWork)
    {
        _participantRepository = participantRepository;
        _authService = authService;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> ExecuteAsync(RegisterParticipantRequest request, CancellationToken cancellationToken = default)
    {
        var emailExists = await _participantRepository.ExistsByEmailAsync(request.Email, cancellationToken);
        if (emailExists)
            throw new ConflictException($"Ya existe un participante registrado con el correo '{request.Email.ToLowerInvariant()}'.");

        // Registra las credenciales en el proveedor de identidad
        var identityUserId = await _authService.RegisterUserAsync(request.Email, request.Password, "Participant", cancellationToken);

        // Crea el perfil de participante en el dominio
        var participant = new Participant(request.FullName, request.Email, identityUserId);

        await _participantRepository.AddAsync(participant, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Envío de correo informativo (no bloquea el registro si el servicio no está disponible)
        try
        {
            await _emailService.SendWelcomeEmailAsync(participant.Email, participant.FullName, cancellationToken);
        }
        catch
        {
            // El fallo de envío no cancela el registro
        }

        return participant.Id;
    }
}
