using SkillHub.Domain.Exceptions;

namespace SkillHub.Domain.Entities;

public class Instructor
{
    public Guid Id { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Specialty { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<Workshop> _workshops = new();
    public IReadOnlyCollection<Workshop> Workshops => _workshops.AsReadOnly();

    protected Instructor() { }

    public Instructor(string fullName, string email, string? specialty = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("El nombre completo del instructor es obligatorio.");

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new DomainException("El correo electrónico del instructor no tiene un formato válido.");

        Id = Guid.NewGuid();
        FullName = fullName.Trim();
        Email = email.Trim().ToLowerInvariant();
        Specialty = specialty?.Trim();
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateProfile(string fullName, string email, string? specialty)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("El nombre completo del instructor es obligatorio.");

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new DomainException("El correo electrónico del instructor no tiene un formato válido.");

        FullName = fullName.Trim();
        Email = email.Trim().ToLowerInvariant();
        Specialty = specialty?.Trim();
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;
}
