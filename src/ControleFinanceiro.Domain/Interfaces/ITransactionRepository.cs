using ControleFinanceiro.Domain.Entities;
using ControleFinanceiro.Domain.Filters;

namespace ControleFinanceiro.Domain.Interfaces;

public interface ITransactionRepository
{
    Task<(IEnumerable<Transaction> Items, int TotalCount)> GetAllAsync(Guid userId, TransactionFilter filter);
    Task<Transaction?> GetByIdAsync(Guid id, Guid userId);
    Task AddAsync(Transaction transaction);
    Task UpdateAsync(Transaction transaction);
    Task DeleteAsync(Transaction transaction);
    Task<decimal> GetTotalByTypeAsync(Guid userId, Domain.Enums.TransactionType type, DateTime? startDate = null, DateTime? endDate = null);
    Task<IEnumerable<Transaction>> GetByMonthAsync(Guid userId, int year, int month);
    Task<IEnumerable<(Guid CategoryId, string CategoryName, decimal Total)>> GetExpensesByCategoryAsync(Guid userId, DateTime startDate, DateTime endDate);
    Task SaveChangesAsync();
}
