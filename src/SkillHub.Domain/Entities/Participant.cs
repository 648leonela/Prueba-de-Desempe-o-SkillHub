using SkillHub.Domain.Exceptions;

namespace SkillHub.Domain.Entities;

public class Participant
{
    public Guid Id { get; private set; }
    public string? IdentityUserId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    private readonly List<Enrollment> _enrollments = new();
    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

    protected Participant() { }

    public Participant(string fullName, string email, string? identityUserId = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("El nombre completo del participante es obligatorio.");

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new DomainException("El correo electrónico del participante no tiene un formato válido.");

        Id = Guid.NewGuid();
        FullName = fullName.Trim();
        Email = email.Trim().ToLowerInvariant();
        IdentityUserId = identityUserId?.Trim();
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateProfile(string fullName, string email)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("El nombre completo del participante es obligatorio.");

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new DomainException("El correo electrónico del participante no tiene un formato válido.");

        FullName = fullName.Trim();
        Email = email.Trim().ToLowerInvariant();
    }

    public void LinkIdentityUser(string identityUserId)
    {
        if (string.IsNullOrWhiteSpace(identityUserId))
            throw new DomainException("El identificador de identidad no puede ser vacío.");

        IdentityUserId = identityUserId.Trim();
    }
}
