using System.Security.Claims;
using ControleFinanceiro.Application.DTOs.Category;
using ControleFinanceiro.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro.API.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/categories")
            .WithTags("Categorias")
            .RequireAuthorization();

        group.MapGet("/", async (ClaimsPrincipal user, ICategoryService service) =>
        {
            var userId = GetUserId(user);
            var categories = await service.GetAllAsync(userId);
            return Results.Ok(categories);
        })
        .WithSummary("Listar categorias")
        .WithDescription("Retorna todas as categorias disponíveis para o usuário (padrão + personalizadas).")
        .Produces<IEnumerable<CategoryDto>>();

        group.MapGet("/{id:guid}", async (Guid id, ClaimsPrincipal user, ICategoryService service) =>
        {
            var userId = GetUserId(user);
            var category = await service.GetByIdAsync(id, userId);
            return category is null ? Results.NotFound(new { message = "Categoria não encontrada." }) : Results.Ok(category);
        })
        .WithSummary("Buscar categoria por ID")
        .Produces<CategoryDto>()
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async ([FromBody] CreateCategoryDto dto, ClaimsPrincipal user, ICategoryService service) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return Results.BadRequest(new { message = "Nome é obrigatório." });

            var userId = GetUserId(user);
            var category = await service.CreateAsync(userId, dto);
            return Results.Created($"/api/categories/{category.Id}", category);
        })
        .WithSummary("Criar categoria")
        .Produces<CategoryDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateCategoryDto dto, ClaimsPrincipal user, ICategoryService service) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return Results.BadRequest(new { message = "Nome é obrigatório." });

            var userId = GetUserId(user);
            try
            {
                var category = await service.UpdateAsync(id, userId, dto);
                return category is null ? Results.NotFound(new { message = "Categoria não encontrada." }) : Results.Ok(category);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        })
        .WithSummary("Atualizar categoria")
        .Produces<CategoryDto>()
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", async (Guid id, ClaimsPrincipal user, ICategoryService service) =>
        {
            var userId = GetUserId(user);
            try
            {
                var deleted = await service.DeleteAsync(id, userId);
                return deleted ? Results.NoContent() : Results.NotFound(new { message = "Categoria não encontrada." });
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        })
        .WithSummary("Excluir categoria")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);
    }

    private static Guid GetUserId(ClaimsPrincipal user) =>
        Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
