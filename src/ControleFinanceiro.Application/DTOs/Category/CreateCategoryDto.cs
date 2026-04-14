namespace ControleFinanceiro.Application.DTOs.Category;

public record CreateCategoryDto(
    string Name,
    string Color,
    string Icon
);
