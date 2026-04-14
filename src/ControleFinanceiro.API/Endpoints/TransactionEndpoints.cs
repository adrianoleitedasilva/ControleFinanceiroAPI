using System.Security.Claims;
using ControleFinanceiro.Application.DTOs.Transaction;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro.API.Endpoints;

public static class TransactionEndpoints
{
    public static void MapTransactionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/transactions")
            .WithTags("Transações")
            .RequireAuthorization();

        group.MapGet("/", async (
            ClaimsPrincipal user,
            ITransactionService service,
            [FromQuery] Guid? categoryId = null,
            [FromQuery] TransactionType? type = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20) =>
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var userId = GetUserId(user);
            var filter = new TransactionFilterDto(categoryId, type, startDate, endDate, search, page, pageSize);
            var result = await service.GetAllAsync(userId, filter);
            return Results.Ok(result);
        })
        .WithSummary("Listar transações")
        .WithDescription("Retorna as transações do usuário com suporte a filtros e paginação.")
        .Produces<PagedResultDto<TransactionDto>>();

        group.MapGet("/{id:guid}", async (Guid id, ClaimsPrincipal user, ITransactionService service) =>
        {
            var userId = GetUserId(user);
            var transaction = await service.GetByIdAsync(id, userId);
            return transaction is null
                ? Results.NotFound(new { message = "Transação não encontrada." })
                : Results.Ok(transaction);
        })
        .WithSummary("Buscar transação por ID")
        .Produces<TransactionDto>()
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async ([FromBody] CreateTransactionDto dto, ClaimsPrincipal user, ITransactionService service) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Description))
                return Results.BadRequest(new { message = "Descrição é obrigatória." });

            if (dto.Amount <= 0)
                return Results.BadRequest(new { message = "O valor deve ser maior que zero." });

            if (dto.CategoryId == Guid.Empty)
                return Results.BadRequest(new { message = "Categoria é obrigatória." });

            var userId = GetUserId(user);
            try
            {
                var transaction = await service.CreateAsync(userId, dto);
                return Results.Created($"/api/transactions/{transaction.Id}", transaction);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        })
        .WithSummary("Criar transação")
        .Produces<TransactionDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateTransactionDto dto, ClaimsPrincipal user, ITransactionService service) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Description))
                return Results.BadRequest(new { message = "Descrição é obrigatória." });

            if (dto.Amount <= 0)
                return Results.BadRequest(new { message = "O valor deve ser maior que zero." });

            var userId = GetUserId(user);
            try
            {
                var transaction = await service.UpdateAsync(id, userId, dto);
                return transaction is null
                    ? Results.NotFound(new { message = "Transação não encontrada." })
                    : Results.Ok(transaction);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        })
        .WithSummary("Atualizar transação")
        .Produces<TransactionDto>()
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", async (Guid id, ClaimsPrincipal user, ITransactionService service) =>
        {
            var userId = GetUserId(user);
            var deleted = await service.DeleteAsync(id, userId);
            return deleted ? Results.NoContent() : Results.NotFound(new { message = "Transação não encontrada." });
        })
        .WithSummary("Excluir transação")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }

    private static Guid GetUserId(ClaimsPrincipal user) =>
        Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
