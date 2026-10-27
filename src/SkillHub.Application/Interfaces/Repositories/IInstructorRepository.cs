using SkillHub.Domain.Entities;

namespace SkillHub.Application.Interfaces.Repositories;

public interface IInstructorRepository
{
    Task<Instructor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Instructor>> GetAllActiveAsync(CancellationToken cancellationToken = default);
}
