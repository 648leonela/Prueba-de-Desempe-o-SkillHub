using SkillHub.Application.DTOs.Certificates;

namespace SkillHub.Application.Interfaces.Services;

public interface IPdfService
{
    Task<CertificateFileDto> GenerateCertificateAsync(Guid enrollmentId, CancellationToken cancellationToken = default);
}
