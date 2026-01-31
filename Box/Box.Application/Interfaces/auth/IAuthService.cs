using Box.Application.Dtos;
using Box.Application.Common;

namespace Box.Application.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<AuthResponseDto>> LogInAsync(AuthRequestDto request);
    Task<ApiResponse<string>> LogOutAsync();
    Task<ApiResponse<string>> RegisterAsync(AuthRequestDto request);
}