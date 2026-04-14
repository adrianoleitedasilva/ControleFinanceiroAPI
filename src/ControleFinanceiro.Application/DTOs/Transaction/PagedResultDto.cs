namespace ControleFinanceiro.Application.DTOs.Transaction;

public record PagedResultDto<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);
