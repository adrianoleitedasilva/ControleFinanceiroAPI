using ControleFinanceiro.Domain.Enums;

namespace ControleFinanceiro.Application.DTOs.Transaction;

public record TransactionFilterDto(
    Guid? CategoryId = null,
    TransactionType? Type = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    string? Search = null,
    int Page = 1,
    int PageSize = 20
);
