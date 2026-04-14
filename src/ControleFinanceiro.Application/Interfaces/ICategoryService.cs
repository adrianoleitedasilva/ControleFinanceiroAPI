using ControleFinanceiro.Application.DTOs.Category;

namespace ControleFinanceiro.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync(Guid userId);
    Task<CategoryDto?> GetByIdAsync(Guid id, Guid userId);
    Task<CategoryDto> CreateAsync(Guid userId, CreateCategoryDto dto);
    Task<CategoryDto?> UpdateAsync(Guid id, Guid userId, UpdateCategoryDto dto);
    Task<bool> DeleteAsync(Guid id, Guid userId);
}
