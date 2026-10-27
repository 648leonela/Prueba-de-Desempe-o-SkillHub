using SkillHub.Domain.Entities;

namespace SkillHub.Application.Interfaces.Repositories;

public interface IWorkshopRepository
{
    Task<Workshop?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Workshop?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Workshop>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Workshop workshop, CancellationToken cancellationToken = default);
    Task UpdateAsync(Workshop workshop, CancellationToken cancellationToken = default);
}
