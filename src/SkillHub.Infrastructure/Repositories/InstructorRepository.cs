using Microsoft.EntityFrameworkCore;
using SkillHub.Application.Interfaces.Repositories;
using SkillHub.Domain.Entities;
using SkillHub.Infrastructure.Persistence;

namespace SkillHub.Infrastructure.Repositories;

public class InstructorRepository : IInstructorRepository
{
    private readonly ApplicationDbContext _context;

    public InstructorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Instructor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Instructors
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Instructor>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Instructors
            .Where(i => i.IsActive)
            .OrderBy(i => i.FullName)
            .ToListAsync(cancellationToken);
    }
}
