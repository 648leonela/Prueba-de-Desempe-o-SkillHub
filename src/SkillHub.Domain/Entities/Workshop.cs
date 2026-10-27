using SkillHub.Domain.Enums;
using SkillHub.Domain.Exceptions;

namespace SkillHub.Domain.Entities;

public class Workshop
{
    public Guid Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int Capacity { get; private set; }

    public Guid InstructorId { get; private set; }
    public Instructor Instructor { get; private set; } = null!;

    private readonly List<Enrollment> _enrollments = new();
    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

    protected Workshop() { }

    public Workshop(
        string code,
        string title,
        DateTime startDate,
        DateTime endDate,
        int capacity,
        Guid instructorId,
        DateTime currentUtc,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("El código del taller es obligatorio.");

        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("El título del taller es obligatorio.");

        if (startDate <= currentUtc)
            throw new DomainException("La fecha de inicio del taller debe ser en el futuro.");

        if (startDate >= endDate)
            throw new DomainException("La fecha de inicio debe ser anterior a la fecha de finalización.");

        if (capacity <= 0)
            throw new DomainException("La capacidad del taller debe ser un número positivo mayor a cero.");

        if (instructorId == Guid.Empty)
            throw new DomainException("El identificador del instructor es obligatorio.");

        Id = Guid.NewGuid();
        Code = code.Trim().ToUpperInvariant();
        Title = title.Trim();
        Description = description?.Trim();
        StartDate = startDate;
        EndDate = endDate;
        Capacity = capacity;
        InstructorId = instructorId;
    }

    public int ActiveEnrollmentsCount => _enrollments.Count(e => e.Status == EnrollmentStatus.Active);
    public bool HasAvailableCapacity => ActiveEnrollmentsCount < Capacity;
    public bool HasStarted(DateTime currentUtc) => currentUtc >= StartDate;

    public void AssignInstructor(Guid newInstructorId)
    {
        if (newInstructorId == Guid.Empty)
            throw new DomainException("El identificador del instructor es obligatorio.");

        InstructorId = newInstructorId;
    }

    public void UpdateDetails(string title, string? description, int newCapacity)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("El título del taller es obligatorio.");

        if (newCapacity <= 0)
            throw new DomainException("La capacidad debe ser un número positivo mayor a cero.");

        if (newCapacity < ActiveEnrollmentsCount)
            throw new DomainException($"No se puede reducir la capacidad a {newCapacity} porque ya existen {ActiveEnrollmentsCount} inscripciones activas.");

        Title = title.Trim();
        Description = description?.Trim();
        Capacity = newCapacity;
    }

    public void Reschedule(DateTime newStartDate, DateTime newEndDate, DateTime currentUtc)
    {
        if (HasStarted(currentUtc))
            throw new DomainException("No se puede reprogramar un taller que ya ha comenzado.");

        if (newStartDate <= currentUtc)
            throw new DomainException("La nueva fecha de inicio debe ser en el futuro.");

        if (newStartDate >= newEndDate)
            throw new DomainException("La nueva fecha de inicio debe ser anterior a la de finalización.");

        StartDate = newStartDate;
        EndDate = newEndDate;
    }
}
