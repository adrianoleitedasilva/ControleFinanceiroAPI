using ControleFinanceiro.Application.DTOs.Dashboard;
using ControleFinanceiro.Application.DTOs.Transaction;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;
using ControleFinanceiro.Domain.Enums;
using ControleFinanceiro.Domain.Filters;
using ControleFinanceiro.Domain.Interfaces;

namespace ControleFinanceiro.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICategoryRepository _categoryRepository;

    public TransactionService(ITransactionRepository transactionRepository, ICategoryRepository categoryRepository)
    {
        _transactionRepository = transactionRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<PagedResultDto<TransactionDto>> GetAllAsync(Guid userId, TransactionFilterDto filterDto)
    {
        var filter = new TransactionFilter
        {
            CategoryId = filterDto.CategoryId,
            Type = filterDto.Type,
            StartDate = filterDto.StartDate,
            EndDate = filterDto.EndDate,
            Search = filterDto.Search,
            Page = filterDto.Page,
            PageSize = filterDto.PageSize
        };

        var (items, totalCount) = await _transactionRepository.GetAllAsync(userId, filter);
        var totalPages = (int)Math.Ceiling(totalCount / (double)filterDto.PageSize);

        return new PagedResultDto<TransactionDto>(
            items.Select(ToDto),
            totalCount,
            filterDto.Page,
            filterDto.PageSize,
            totalPages
        );
    }

    public async Task<TransactionDto?> GetByIdAsync(Guid id, Guid userId)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id, userId);
        return transaction is null ? null : ToDto(transaction);
    }

    public async Task<TransactionDto> CreateAsync(Guid userId, CreateTransactionDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId, userId)
            ?? throw new InvalidOperationException("Categoria não encontrada.");

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Description = dto.Description.Trim(),
            Amount = Math.Abs(dto.Amount),
            Date = dto.Date,
            Type = dto.Type,
            CategoryId = dto.CategoryId,
            UserId = userId,
            Notes = dto.Notes?.Trim(),
            IsRecurring = dto.IsRecurring,
            CreatedAt = DateTime.UtcNow
        };

        await _transactionRepository.AddAsync(transaction);
        await _transactionRepository.SaveChangesAsync();

        transaction.Category = category;
        return ToDto(transaction);
    }

    public async Task<TransactionDto?> UpdateAsync(Guid id, Guid userId, UpdateTransactionDto dto)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id, userId);
        if (transaction is null) return null;

        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId, userId)
            ?? throw new InvalidOperationException("Categoria não encontrada.");

        transaction.Description = dto.Description.Trim();
        transaction.Amount = Math.Abs(dto.Amount);
        transaction.Date = dto.Date;
        transaction.Type = dto.Type;
        transaction.CategoryId = dto.CategoryId;
        transaction.Notes = dto.Notes?.Trim();
        transaction.IsRecurring = dto.IsRecurring;
        transaction.UpdatedAt = DateTime.UtcNow;

        await _transactionRepository.UpdateAsync(transaction);
        await _transactionRepository.SaveChangesAsync();

        transaction.Category = category;
        return ToDto(transaction);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id, userId);
        if (transaction is null) return false;

        await _transactionRepository.DeleteAsync(transaction);
        await _transactionRepository.SaveChangesAsync();

        return true;
    }

    public async Task<DashboardDto> GetDashboardAsync(Guid userId)
    {
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endOfMonth = startOfMonth.AddMonths(1).AddTicks(-1);

        var totalIncome = await _transactionRepository.GetTotalByTypeAsync(userId, TransactionType.Income);
        var totalExpense = await _transactionRepository.GetTotalByTypeAsync(userId, TransactionType.Expense);
        var monthlyIncome = await _transactionRepository.GetTotalByTypeAsync(userId, TransactionType.Income, startOfMonth, endOfMonth);
        var monthlyExpense = await _transactionRepository.GetTotalByTypeAsync(userId, TransactionType.Expense, startOfMonth, endOfMonth);

        var balance = totalIncome - totalExpense;
        var monthlyBalance = monthlyIncome - monthlyExpense;
        var savingsRate = monthlyIncome > 0 ? Math.Round((monthlyIncome - monthlyExpense) / monthlyIncome * 100, 2) : 0;

        var expensesByCategory = await _transactionRepository.GetExpensesByCategoryAsync(userId, startOfMonth, endOfMonth);
        var categoryExpenses = expensesByCategory.Select(e => new CategoryExpenseDto(
            e.CategoryId,
            e.CategoryName,
            "#6366F1",
            e.Total,
            monthlyExpense > 0 ? Math.Round(e.Total / monthlyExpense * 100, 2) : 0
        )).OrderByDescending(x => x.Total).Take(5);

        var recentFilter = new TransactionFilter { Page = 1, PageSize = 5 };
        var (recentItems, _) = await _transactionRepository.GetAllAsync(userId, recentFilter);
        var recentTransactions = recentItems.Select(t => new TransactionSummaryDto(
            t.Id,
            t.Description,
            t.Amount,
            t.Date,
            t.Type == TransactionType.Income ? "Receita" : "Despesa",
            t.Category?.Name ?? "",
            t.Category?.Color ?? ""
        ));

        return new DashboardDto(
            balance,
            totalIncome,
            totalExpense,
            monthlyIncome,
            monthlyExpense,
            monthlyBalance,
            savingsRate,
            categoryExpenses,
            recentTransactions
        );
    }

    public async Task<MonthlyReportDto> GetMonthlyReportAsync(Guid userId, int year, int month)
    {
        var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = startDate.AddMonths(1).AddTicks(-1);

        var income = await _transactionRepository.GetTotalByTypeAsync(userId, TransactionType.Income, startDate, endDate);
        var expense = await _transactionRepository.GetTotalByTypeAsync(userId, TransactionType.Expense, startDate, endDate);

        var expensesByCategory = await _transactionRepository.GetExpensesByCategoryAsync(userId, startDate, endDate);
        var categoryExpenses = expensesByCategory.Select(e => new CategoryExpenseDto(
            e.CategoryId,
            e.CategoryName,
            "#6366F1",
            e.Total,
            expense > 0 ? Math.Round(e.Total / expense * 100, 2) : 0
        )).OrderByDescending(x => x.Total);

        var transactions = await _transactionRepository.GetByMonthAsync(userId, year, month);
        var dailyBalances = transactions
            .GroupBy(t => t.Date.Date)
            .Select(g => new DailyBalanceDto(
                g.Key,
                g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount),
                g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount) -
                g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)
            ))
            .OrderBy(d => d.Date);

        return new MonthlyReportDto(
            year,
            month,
            startDate.ToString("MMMM", new System.Globalization.CultureInfo("pt-BR")),
            income,
            expense,
            income - expense,
            categoryExpenses,
            dailyBalances
        );
    }

    private static TransactionDto ToDto(Transaction t) =>
        new(
            t.Id,
            t.Description,
            t.Amount,
            t.Date,
            t.Type,
            t.Type == TransactionType.Income ? "Receita" : "Despesa",
            t.Notes,
            t.IsRecurring,
            t.CategoryId,
            t.Category?.Name ?? "",
            t.Category?.Color ?? "",
            t.Category?.Icon ?? "",
            t.CreatedAt
        );
}
