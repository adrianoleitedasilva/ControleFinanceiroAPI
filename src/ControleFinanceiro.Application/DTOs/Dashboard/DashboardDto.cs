namespace ControleFinanceiro.Application.DTOs.Dashboard;

public record DashboardDto(
    decimal Balance,
    decimal TotalIncome,
    decimal TotalExpense,
    decimal MonthlyIncome,
    decimal MonthlyExpense,
    decimal MonthlyBalance,
    decimal SavingsRate,
    IEnumerable<CategoryExpenseDto> TopExpenseCategories,
    IEnumerable<TransactionSummaryDto> RecentTransactions
);

public record CategoryExpenseDto(
    Guid CategoryId,
    string CategoryName,
    string CategoryColor,
    decimal Total,
    decimal Percentage
);

public record TransactionSummaryDto(
    Guid Id,
    string Description,
    decimal Amount,
    DateTime Date,
    string Type,
    string CategoryName,
    string CategoryColor
);
