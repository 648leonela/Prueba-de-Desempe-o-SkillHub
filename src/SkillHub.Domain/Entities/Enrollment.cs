using SkillHub.Domain.Enums;
using SkillHub.Domain.Exceptions;

namespace SkillHub.Domain.Entities;

public class Enrollment
{
    public Guid Id { get; private set; }

    public Guid ParticipantId { get; private set; }
    public Participant Participant { get; private set; } = null!;

    public Guid WorkshopId { get; private set; }
    public Workshop Workshop { get; private set; } = null!;

    public DateTime EnrolledAt { get; private set; }
    public EnrollmentStatus Status { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public string? CancelledBy { get; private set; } // "Participant" o "Employee"
    public string? CancellationReason { get; private set; }

    protected Enrollment() { }

    public Enrollment(Guid participantId, Guid workshopId, DateTime workshopStartDate, DateTime currentUtc)
    {
        if (participantId == Guid.Empty)
            throw new DomainException("El identificador del participante es obligatorio.");

        if (workshopId == Guid.Empty)
            throw new DomainException("El identificador del taller es obligatorio.");

        if (currentUtc >= workshopStartDate)
            throw new DomainException("No es posible inscribirse a un taller que ya ha comenzado.");

        Id = Guid.NewGuid();
        ParticipantId = participantId;
        WorkshopId = workshopId;
        EnrolledAt = currentUtc;
        Status = EnrollmentStatus.Active;
    }

    public bool IsActive => Status == EnrollmentStatus.Active;

    public void CancelByParticipant(DateTime workshopStartDate, DateTime currentUtc, string? reason = null)
    {
        EnsureCanBeCancelled();

        if (currentUtc > workshopStartDate.AddHours(-24))
            throw new DomainException("El participante solo puede cancelar la inscripción con al menos 24 horas de anticipación al inicio del taller.");

        ApplyCancellation("Participant", currentUtc, reason);
    }

    public void CancelByEmployee(DateTime workshopStartDate, DateTime currentUtc, string? reason = null)
    {
        EnsureCanBeCancelled();

        if (currentUtc >= workshopStartDate)
            throw new DomainException("El empleado no puede cancelar una inscripción si el taller ya comenzó.");

        ApplyCancellation("Employee", currentUtc, reason);
    }

    private void EnsureCanBeCancelled()
    {
        if (Status == EnrollmentStatus.Cancelled)
            throw new DomainException("La inscripción ya se encuentra cancelada.");
    }

    private void ApplyCancellation(string cancelledBy, DateTime currentUtc, string? reason)
    {
        Status = EnrollmentStatus.Cancelled;
        CancelledAt = currentUtc;
        CancelledBy = cancelledBy;
        CancellationReason = reason;
    }
}
