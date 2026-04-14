using ControleFinanceiro.Application.DTOs.Dashboard;
using ControleFinanceiro.Application.DTOs.Transaction;

namespace ControleFinanceiro.Application.Interfaces;

public interface ITransactionService
{
    Task<PagedResultDto<TransactionDto>> GetAllAsync(Guid userId, TransactionFilterDto filter);
    Task<TransactionDto?> GetByIdAsync(Guid id, Guid userId);
    Task<TransactionDto> CreateAsync(Guid userId, CreateTransactionDto dto);
    Task<TransactionDto?> UpdateAsync(Guid id, Guid userId, UpdateTransactionDto dto);
    Task<bool> DeleteAsync(Guid id, Guid userId);
    Task<DashboardDto> GetDashboardAsync(Guid userId);
    Task<MonthlyReportDto> GetMonthlyReportAsync(Guid userId, int year, int month);
}
