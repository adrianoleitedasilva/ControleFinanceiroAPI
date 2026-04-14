using ControleFinanceiro.Domain.Enums;

namespace ControleFinanceiro.Application.DTOs.Transaction;

public record TransactionDto(
    Guid Id,
    string Description,
    decimal Amount,
    DateTime Date,
    TransactionType Type,
    string TypeLabel,
    string? Notes,
    bool IsRecurring,
    Guid CategoryId,
    string CategoryName,
    string CategoryColor,
    string CategoryIcon,
    DateTime CreatedAt
);
