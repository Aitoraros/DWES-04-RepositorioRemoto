using Microsoft.EntityFrameworkCore;

namespace RepositorioRemoto.Entity;

/// <summary>Contexto de EF Core para la base de datos local SQLite.</summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    /// <summary>Tabla de usuarios.</summary>
    public DbSet<UserEntity> Users => Set<UserEntity>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).ValueGeneratedNever();   // el id lo da la API, no la BD
        });
    }
}