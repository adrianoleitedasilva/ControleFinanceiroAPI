using ControleFinanceiro.Domain.Entities;
using ControleFinanceiro.Domain.Interfaces;
using ControleFinanceiro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllAsync(Guid userId) =>
        await _context.Categories
            .Where(c => c.UserId == null || c.UserId == userId)
            .OrderBy(c => c.IsDefault ? 0 : 1)
            .ThenBy(c => c.Name)
            .ToListAsync();

    public async Task<Category?> GetByIdAsync(Guid id, Guid userId) =>
        await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && (c.UserId == null || c.UserId == userId));

    public async Task AddAsync(Category category) =>
        await _context.Categories.AddAsync(category);

    public Task UpdateAsync(Category category)
    {
        _context.Categories.Update(category);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Category category)
    {
        _context.Categories.Remove(category);
        return Task.CompletedTask;
    }

    public async Task<bool> HasTransactionsAsync(Guid categoryId, Guid userId) =>
        await _context.Transactions
            .AnyAsync(t => t.CategoryId == categoryId && t.UserId == userId);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
