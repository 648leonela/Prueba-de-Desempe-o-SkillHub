using Microsoft.EntityFrameworkCore;
using SkillHub.Application.Interfaces.Repositories;
using SkillHub.Domain.Entities;
using SkillHub.Domain.Enums;
using SkillHub.Infrastructure.Persistence;

namespace SkillHub.Infrastructure.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly ApplicationDbContext _context;

    public EnrollmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Enrollment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Enrollments
            .Include(e => e.Workshop)
            .Include(e => e.Participant)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<bool> HasActiveEnrollmentAsync(Guid participantId, Guid workshopId, CancellationToken cancellationToken = default)
    {
        return await _context.Enrollments
            .AnyAsync(e => e.ParticipantId == participantId &&
                           e.WorkshopId == workshopId &&
                           e.Status == EnrollmentStatus.Active, cancellationToken);
    }

    public async Task<bool> HasScheduleConflictAsync(Guid participantId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.Enrollments
            .Where(e => e.ParticipantId == participantId && e.Status == EnrollmentStatus.Active)
            .AnyAsync(e => e.Workshop.StartDate < endDate && e.Workshop.EndDate > startDate, cancellationToken);
    }

    public async Task<IReadOnlyList<Enrollment>> GetByParticipantIdAsync(Guid participantId, CancellationToken cancellationToken = default)
    {
        return await _context.Enrollments
            .Include(e => e.Workshop)
            .Include(e => e.Participant)
            .Where(e => e.ParticipantId == participantId)
            .OrderByDescending(e => e.EnrolledAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Enrollment enrollment, CancellationToken cancellationToken = default)
    {
        await _context.Enrollments.AddAsync(enrollment, cancellationToken);
    }

    public Task UpdateAsync(Enrollment enrollment, CancellationToken cancellationToken = default)
    {
        _context.Enrollments.Update(enrollment);
        return Task.CompletedTask;
    }
}
