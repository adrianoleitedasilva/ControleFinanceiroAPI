using ControleFinanceiro.Domain.Enums;

namespace ControleFinanceiro.Domain.Filters;

public class TransactionFilter
{
    public Guid? CategoryId { get; set; }
    public TransactionType? Type { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
