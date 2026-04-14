using ControleFinanceiro.Domain.Entities;
using ControleFinanceiro.Domain.Enums;
using ControleFinanceiro.Domain.Filters;
using ControleFinanceiro.Domain.Interfaces;
using ControleFinanceiro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _context;

    public TransactionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Transaction> Items, int TotalCount)> GetAllAsync(Guid userId, TransactionFilter filter)
    {
        var query = _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.UserId == userId);

        if (filter.CategoryId.HasValue)
            query = query.Where(t => t.CategoryId == filter.CategoryId.Value);

        if (filter.Type.HasValue)
            query = query.Where(t => t.Type == filter.Type.Value);

        if (filter.StartDate.HasValue)
            query = query.Where(t => t.Date >= filter.StartDate.Value);

        if (filter.EndDate.HasValue)
            query = query.Where(t => t.Date <= filter.EndDate.Value);

        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(t => t.Description.Contains(filter.Search) ||
                                     (t.Notes != null && t.Notes.Contains(filter.Search)));

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Transaction?> GetByIdAsync(Guid id, Guid userId) =>
        await _context.Transactions
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

    public async Task AddAsync(Transaction transaction) =>
        await _context.Transactions.AddAsync(transaction);

    public Task UpdateAsync(Transaction transaction)
    {
        _context.Transactions.Update(transaction);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Transaction transaction)
    {
        _context.Transactions.Remove(transaction);
        return Task.CompletedTask;
    }

    public async Task<decimal> GetTotalByTypeAsync(Guid userId, TransactionType type, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Transactions
            .Where(t => t.UserId == userId && t.Type == type);

        if (startDate.HasValue) query = query.Where(t => t.Date >= startDate.Value);
        if (endDate.HasValue) query = query.Where(t => t.Date <= endDate.Value);

        return await query.SumAsync(t => (decimal?)t.Amount) ?? 0m;
    }

    public async Task<IEnumerable<Transaction>> GetByMonthAsync(Guid userId, int year, int month)
    {
        var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = startDate.AddMonths(1);

        return await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.UserId == userId && t.Date >= startDate && t.Date < endDate)
            .OrderBy(t => t.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<(Guid CategoryId, string CategoryName, decimal Total)>> GetExpensesByCategoryAsync(
        Guid userId, DateTime startDate, DateTime endDate)
    {
        var results = await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.UserId == userId &&
                        t.Type == TransactionType.Expense &&
                        t.Date >= startDate &&
                        t.Date <= endDate)
            .GroupBy(t => new { t.CategoryId, t.Category.Name })
            .Select(g => new
            {
                g.Key.CategoryId,
                g.Key.Name,
                Total = g.Sum(t => t.Amount)
            })
            .OrderByDescending(x => x.Total)
            .ToListAsync();

        return results.Select(r => (r.CategoryId, r.Name, r.Total));
    }

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
