using ControleFinanceiro.Application.DTOs.Auth;
using ControleFinanceiro.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Autenticação");

        group.MapPost("/register", async ([FromBody] RegisterDto dto, IUserService service) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return Results.BadRequest(new { message = "Nome é obrigatório." });

            if (string.IsNullOrWhiteSpace(dto.Email) || !dto.Email.Contains('@'))
                return Results.BadRequest(new { message = "E-mail inválido." });

            if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
                return Results.BadRequest(new { message = "A senha deve ter no mínimo 6 caracteres." });

            try
            {
                var result = await service.RegisterAsync(dto);
                return Results.Created("/api/auth/me", result);
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { message = ex.Message });
            }
        })
        .WithSummary("Cadastrar novo usuário")
        .WithDescription("Cria uma nova conta de usuário e retorna o token JWT.")
        .Produces<AuthResponseDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status409Conflict);

        group.MapPost("/login", async ([FromBody] LoginDto dto, IUserService service) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return Results.BadRequest(new { message = "E-mail e senha são obrigatórios." });

            try
            {
                var result = await service.LoginAsync(dto);
                return Results.Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Results.Unauthorized();
            }
        })
        .WithSummary("Login")
        .WithDescription("Autentica o usuário e retorna o token JWT.")
        .Produces<AuthResponseDto>()
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}
