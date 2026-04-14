using ControleFinanceiro.Domain.Enums;

namespace ControleFinanceiro.Application.DTOs.Transaction;

public record CreateTransactionDto(
    string Description,
    decimal Amount,
    DateTime Date,
    TransactionType Type,
    Guid CategoryId,
    string? Notes,
    bool IsRecurring
);
