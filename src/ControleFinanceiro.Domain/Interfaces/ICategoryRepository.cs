using ControleFinanceiro.Domain.Entities;

namespace ControleFinanceiro.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync(Guid userId);
    Task<Category?> GetByIdAsync(Guid id, Guid userId);
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(Category category);
    Task<bool> HasTransactionsAsync(Guid categoryId, Guid userId);
    Task SaveChangesAsync();
}
