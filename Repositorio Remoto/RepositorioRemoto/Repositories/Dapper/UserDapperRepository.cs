using Dapper;
using Npgsql;
using RepositorioRemoto.Entity;
using RepositorioRemoto.Repositories.Common;
using Serilog;

namespace RepositorioRemoto.Repositories.Dapper;

public class UserDapperRepository(string connectionString) : IUserRepository {
    
    private readonly ILogger _logger = Log.ForContext<UserDapperRepository>();

    public async Task<IEnumerable<UserEntity>> GetAllAsync() {
        await using var connection = new NpgsqlConnection(connectionString);
        const string sql = """
                           SELECT id, alias, nombre, email,
                                  direccion_calle, direccion_suite, direccion_ciudad, direccion_codigo_postal,
                                  direccion_latitud, direccion_longitud, telefono, web,
                                  compania_nombre, compania_eslogan, compania_bs
                           FROM users
                           ORDER BY id
                           """;
        return await connection.QueryAsync<UserEntity>(sql);
    }

    public async Task<UserEntity?> GetByIdAsync(int id) {
        await using var connection = new NpgsqlConnection(connectionString);
        const string sql = """
                           SELECT id, alias, nombre, email,
                                  direccion_calle, direccion_suite, direccion_ciudad, direccion_codigo_postal,
                                  direccion_latitud, direccion_longitud, telefono, web,
                                  compania_nombre, compania_eslogan, compania_bs
                           FROM users
                           WHERE id = @Id
                           """;
        return await connection.QuerySingleOrDefaultAsync<UserEntity>(sql, new { Id = id });
    }

    public async Task<UserEntity> CreateAsync(UserEntity user) {
        await using var connection = new NpgsqlConnection(connectionString);
        const string sql = """
                           INSERT INTO users (id, alias, nombre, email,
                                              direccion_calle, direccion_suite, direccion_ciudad, direccion_codigo_postal,
                                              direccion_latitud, direccion_longitud, telefono, web,
                                              compania_nombre, compania_eslogan, compania_bs)
                           VALUES (@Id, @Alias, @Nombre, @Email,
                                   @DireccionCalle, @DireccionSuite, @DireccionCiudad, @DireccionCodigoPostal,
                                   @DireccionLatitud, @DireccionLongitud, @Telefono, @Web,
                                   @CompaniaNombre, @CompaniaEslogan, @CompaniaBs)
                           """;

        await connection.ExecuteAsync(sql, user);
        return user;
    }

    public async Task<UserEntity?> UpdateAsync(UserEntity user) {
        await using var connection = new NpgsqlConnection(connectionString);
        const string sql = """
                           UPDATE users
                           SET alias = @Alias, nombre = @Nombre, email = @Email,
                               direccion_calle = @DireccionCalle, direccion_suite = @DireccionSuite,
                               direccion_ciudad = @DireccionCiudad, direccion_codigo_postal = @DireccionCodigoPostal,
                               direccion_latitud = @DireccionLatitud, direccion_longitud = @DireccionLongitud,
                               telefono = @Telefono, web = @Web,
                               compania_nombre = @CompaniaNombre, compania_eslogan = @CompaniaEslogan, compania_bs = @CompaniaBs
                           WHERE id = @Id
                           """;
        var rows = await connection.ExecuteAsync(sql, user);
        return rows > 0 ? user : null;
    }

    public async Task<bool> DeleteAsync(int id) {
        await using var connection = new NpgsqlConnection(connectionString);
        const string sql = "DELETE FROM users WHERE id = @Id";
        var rows = await connection.ExecuteAsync(sql, new { Id = id });
        return rows > 0;
    }

    public async Task InsertRangeAsync(IEnumerable<UserEntity> users) {
        await using var connection = new NpgsqlConnection(connectionString);
        const string sql = """
                           INSERT INTO users (id, alias, nombre, email,
                                               direccion_calle, direccion_suite, direccion_ciudad, direccion_codigo_postal,
                                               direccion_latitud, direccion_longitud, telefono, web,
                                               compania_nombre, compania_eslogan, compania_bs)
                           VALUES (@Id, @Alias, @Nombre, @Email,
                                   @DireccionCalle, @DireccionSuite, @DireccionCiudad, @DireccionCodigoPostal,
                                   @DireccionLatitud, @DireccionLongitud, @Telefono, @Web,
                                   @CompaniaNombre, @CompaniaEslogan, @CompaniaBs)
                           """;
        await connection.ExecuteAsync(sql, users);
    }

    public async Task DeleteAllAsync() {
        await using var connection = new NpgsqlConnection(connectionString);
        const string sql = "DELETE FROM users";
        await connection.ExecuteAsync(sql);
    }

    /// <summary>Crea la tabla users si no existe</summary>
    public async Task EnsureCreatedAsync() {
        await using var connection = new NpgsqlConnection(connectionString);
        const string sql = """
                           CREATE TABLE IF NOT EXISTS users (
                               id INT PRIMARY KEY,
                               alias VARCHAR(20) NOT NULL,
                               nombre VARCHAR(50) NOT NULL,
                               email TEXT NOT NULL,
                               direccion_calle VARCHAR(100) NOT NULL,
                               direccion_suite VARCHAR(50) NOT NULL,
                               direccion_ciudad VARCHAR(50) NOT NULL,
                               direccion_codigo_postal TEXT NOT NULL,
                               direccion_latitud DOUBLE PRECISION NOT NULL,
                               direccion_longitud DOUBLE PRECISION NOT NULL,
                               telefono VARCHAR(25) NOT NULL,
                               web TEXT NOT NULL,
                               compania_nombre VARCHAR(100) NOT NULL,
                               compania_eslogan VARCHAR(150) NOT NULL,
                               compania_bs VARCHAR(150) NOT NULL
                           );
                           """;
        await connection.ExecuteAsync(sql);
    }
}