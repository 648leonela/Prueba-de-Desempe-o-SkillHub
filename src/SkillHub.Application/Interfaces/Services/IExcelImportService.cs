using SkillHub.Application.DTOs.Imports;

namespace SkillHub.Application.Interfaces.Services;

public interface IExcelImportService
{
    Task<ExcelImportResultDto> ImportWorkshopsAsync(Stream fileStream, CancellationToken cancellationToken = default);
}
