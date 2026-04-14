namespace ControleFinanceiro.Application.DTOs.Dashboard;

public record MonthlyReportDto(
    int Year,
    int Month,
    string MonthName,
    decimal TotalIncome,
    decimal TotalExpense,
    decimal Balance,
    IEnumerable<CategoryExpenseDto> ExpensesByCategory,
    IEnumerable<DailyBalanceDto> DailyBalances
);

public record DailyBalanceDto(
    DateTime Date,
    decimal Income,
    decimal Expense,
    decimal Balance
);
