namespace ControleFinanceiro.Domain.Entities;

public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#6366F1";
    public string Icon { get; set; } = "tag";
    public bool IsDefault { get; set; }

    // null = categoria do sistema (disponível a todos os usuários)
    public Guid? UserId { get; set; }
    public User? User { get; set; }

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
