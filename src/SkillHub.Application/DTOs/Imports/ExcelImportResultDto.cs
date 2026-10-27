namespace SkillHub.Application.DTOs.Imports;

public class ExcelRowErrorDto
{
    public int RowNumber { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}

public class ExcelImportResultDto
{
    public int TotalRows { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount => Errors.Count;
    public List<ExcelRowErrorDto> Errors { get; set; } = new();
}
