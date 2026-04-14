using System.Security.Claims;
using ControleFinanceiro.Application.DTOs.Dashboard;
using ControleFinanceiro.Application.Interfaces;

namespace ControleFinanceiro.API.Endpoints;

public static class DashboardEndpoints
{
    public static void MapDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/dashboard")
            .WithTags("Dashboard")
            .RequireAuthorization();

        group.MapGet("/", async (ClaimsPrincipal user, ITransactionService service) =>
        {
            var userId = GetUserId(user);
            var dashboard = await service.GetDashboardAsync(userId);
            return Results.Ok(dashboard);
        })
        .WithSummary("Dados do dashboard")
        .WithDescription("Retorna saldo atual, resumo mensal, top categorias de despesa e transações recentes.")
        .Produces<DashboardDto>();

        group.MapGet("/reports/monthly/{year:int}/{month:int}", async (
            int year, int month,
            ClaimsPrincipal user,
            ITransactionService service) =>
        {
            if (year < 2000 || year > 2100)
                return Results.BadRequest(new { message = "Ano inválido." });

            if (month < 1 || month > 12)
                return Results.BadRequest(new { message = "Mês inválido (1-12)." });

            var userId = GetUserId(user);
            var report = await service.GetMonthlyReportAsync(userId, year, month);
            return Results.Ok(report);
        })
        .WithSummary("Relatório mensal")
        .WithDescription("Retorna o relatório financeiro completo de um mês específico.")
        .Produces<MonthlyReportDto>()
        .Produces(StatusCodes.Status400BadRequest);
    }

    private static Guid GetUserId(ClaimsPrincipal user) =>
        Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
