using ControleFinanceiro.Domain.Entities;

namespace ControleFinanceiro.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    DateTime GetExpirationDate();
}
