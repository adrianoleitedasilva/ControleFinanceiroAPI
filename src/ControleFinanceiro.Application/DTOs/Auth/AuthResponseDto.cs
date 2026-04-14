namespace ControleFinanceiro.Application.DTOs.Auth;

public record AuthResponseDto(
    Guid UserId,
    string Name,
    string Email,
    string Token,
    DateTime ExpiresAt
);
