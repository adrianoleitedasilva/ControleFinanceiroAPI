using ControleFinanceiro.Application.DTOs.Auth;

namespace ControleFinanceiro.Application.Interfaces;

public interface IUserService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}
