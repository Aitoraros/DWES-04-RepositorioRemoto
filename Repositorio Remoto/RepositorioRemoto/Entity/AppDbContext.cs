using Microsoft.EntityFrameworkCore;

namespace RepositorioRemoto.Entity;

/// <summary>Contexto de EF Core para la base de datos local SQLite.</summary>
public class AppDbContext : DbContext
{
    private readonly string _connectionString;

    public AppDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        _connectionString = "";
    }

    /// <summary>Tabla de usuarios.</summary>
    public DbSet<UserEntity> Users { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<UserEntity>(entity => {
            entity.ToTable("users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).ValueGeneratedNever(); // El ID lo proporciona la API remoto, no SQLite
        });
    }

    public void EnsureCreated()
    {
        Database.EnsureCreated();
    }
}