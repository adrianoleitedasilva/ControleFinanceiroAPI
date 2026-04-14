using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ControleFinanceiro.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ITransactionService, TransactionService>();

        return services;
    }
}
