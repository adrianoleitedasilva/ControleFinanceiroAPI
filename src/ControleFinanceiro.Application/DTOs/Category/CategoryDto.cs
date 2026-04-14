namespace ControleFinanceiro.Application.DTOs.Category;

public record CategoryDto(
    Guid Id,
    string Name,
    string Color,
    string Icon,
    bool IsDefault
);
