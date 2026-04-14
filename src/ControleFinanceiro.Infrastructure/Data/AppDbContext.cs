using ControleFinanceiro.Domain.Entities;
using ControleFinanceiro.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.Name).IsRequired().HasMaxLength(100);
            e.Property(u => u.Email).IsRequired().HasMaxLength(200);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.PasswordHash).IsRequired();
        });

        // Category
        modelBuilder.Entity<Category>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Name).IsRequired().HasMaxLength(50);
            e.Property(c => c.Color).IsRequired().HasMaxLength(20);
            e.Property(c => c.Icon).IsRequired().HasMaxLength(50);

            e.HasOne(c => c.User)
             .WithMany(u => u.Categories)
             .HasForeignKey(c => c.UserId)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Transaction
        modelBuilder.Entity<Transaction>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Description).IsRequired().HasMaxLength(200);
            e.Property(t => t.Amount).HasColumnType("decimal(18,2)");
            e.Property(t => t.Notes).HasMaxLength(500);
            e.Property(t => t.Type).HasConversion<int>();

            e.HasOne(t => t.Category)
             .WithMany(c => c.Transactions)
             .HasForeignKey(t => t.CategoryId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(t => t.User)
             .WithMany(u => u.Transactions)
             .HasForeignKey(t => t.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(t => new { t.UserId, t.Date });
        });

        SeedDefaultCategories(modelBuilder);
    }

    private static void SeedDefaultCategories(ModelBuilder modelBuilder)
    {
        var defaultCategories = new List<Category>
        {
            // Receitas
            new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Name = "Salário", Color = "#22C55E", Icon = "briefcase", IsDefault = true, UserId = null },
            new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Name = "Freelance", Color = "#10B981", Icon = "laptop", IsDefault = true, UserId = null },
            new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), Name = "Investimentos", Color = "#059669", Icon = "trending-up", IsDefault = true, UserId = null },
            new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), Name = "Outros (Receita)", Color = "#34D399", Icon = "plus-circle", IsDefault = true, UserId = null },
            // Despesas
            new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000001"), Name = "Alimentação", Color = "#F97316", Icon = "utensils", IsDefault = true, UserId = null },
            new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000002"), Name = "Transporte", Color = "#3B82F6", Icon = "car", IsDefault = true, UserId = null },
            new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000003"), Name = "Moradia", Color = "#8B5CF6", Icon = "home", IsDefault = true, UserId = null },
            new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000004"), Name = "Saúde", Color = "#EF4444", Icon = "heart", IsDefault = true, UserId = null },
            new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000005"), Name = "Educação", Color = "#F59E0B", Icon = "book", IsDefault = true, UserId = null },
            new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000006"), Name = "Lazer", Color = "#EC4899", Icon = "gamepad", IsDefault = true, UserId = null },
            new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000007"), Name = "Vestuário", Color = "#14B8A6", Icon = "shirt", IsDefault = true, UserId = null },
            new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000008"), Name = "Outros (Despesa)", Color = "#94A3B8", Icon = "minus-circle", IsDefault = true, UserId = null },
        };

        modelBuilder.Entity<Category>().HasData(defaultCategories);
    }
}
