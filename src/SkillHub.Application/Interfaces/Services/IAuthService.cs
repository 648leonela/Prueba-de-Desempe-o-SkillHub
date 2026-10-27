using SkillHub.Application.DTOs.Auth;

namespace SkillHub.Application.Interfaces.Services;

public interface IAuthService
{
    Task<string> RegisterUserAsync(string email, string password, string role, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
