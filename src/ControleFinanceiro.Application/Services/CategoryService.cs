using ControleFinanceiro.Application.DTOs.Category;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;
using ControleFinanceiro.Domain.Interfaces;

namespace ControleFinanceiro.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync(Guid userId)
    {
        var categories = await _categoryRepository.GetAllAsync(userId);
        return categories.Select(ToDto);
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id, Guid userId)
    {
        var category = await _categoryRepository.GetByIdAsync(id, userId);
        return category is null ? null : ToDto(category);
    }

    public async Task<CategoryDto> CreateAsync(Guid userId, CreateCategoryDto dto)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            Color = dto.Color,
            Icon = dto.Icon,
            IsDefault = false,
            UserId = userId
        };

        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();

        return ToDto(category);
    }

    public async Task<CategoryDto?> UpdateAsync(Guid id, Guid userId, UpdateCategoryDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(id, userId);
        if (category is null) return null;

        if (category.IsDefault)
            throw new InvalidOperationException("Categorias padrão do sistema não podem ser editadas.");

        category.Name = dto.Name.Trim();
        category.Color = dto.Color;
        category.Icon = dto.Icon;

        await _categoryRepository.UpdateAsync(category);
        await _categoryRepository.SaveChangesAsync();

        return ToDto(category);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var category = await _categoryRepository.GetByIdAsync(id, userId);
        if (category is null) return false;

        if (category.IsDefault)
            throw new InvalidOperationException("Categorias padrão do sistema não podem ser excluídas.");

        if (await _categoryRepository.HasTransactionsAsync(id, userId))
            throw new InvalidOperationException("Não é possível excluir uma categoria que possui transações.");

        await _categoryRepository.DeleteAsync(category);
        await _categoryRepository.SaveChangesAsync();

        return true;
    }

    private static CategoryDto ToDto(Category c) =>
        new(c.Id, c.Name, c.Color, c.Icon, c.IsDefault);
}
