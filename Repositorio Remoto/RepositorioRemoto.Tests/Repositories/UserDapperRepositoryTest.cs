using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RepositorioRemoto.Entity;
using RepositorioRemoto.Repositories.Dapper;
using Testcontainers.PostgreSql;

namespace RepositorioRemoto.Tests.Repositories;

public class UserDapperRepositoryTest {
    
    private PostgreSqlContainer _container = null!;
    private static UserDapperRepository _repository = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp() {
        
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:17-alpine")
            .WithDatabase("test_db")
            .WithUsername("test")
            .WithPassword("test")
            .Build();

        await _container.StartAsync();

        await using var context = new AppDbContext(
            new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(_container.GetConnectionString())
                .Options);
        await context.Database.EnsureCreatedAsync();
        await context.DisposeAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _container.DisposeAsync();
    }

    [SetUp]
    public async Task SetUp() {
        
        _repository = new UserDapperRepository(_container.GetConnectionString());

        using var context = new AppDbContext(
            new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(_container.GetConnectionString())
                .Options);
        await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Users\"");
    }

    private static UserEntity CrearUsuario(int id, string alias = "test") => new() {
        Id = id,
        Alias = alias,
        Nombre = "Nombre Test",
        Email = "test@test.com",
        DireccionCalle = "Calle Falsa 123",
        DireccionSuite = "Apt. 1",
        DireccionCiudad = "Ciudad",
        DireccionCodigoPostal = "28000",
        DireccionLatitud = 0,
        DireccionLongitud = 0,
        Telefono = "600000000",
        Web = "test.com",
        CompaniaNombre = "Compañía Test",
        CompaniaEslogan = "Eslogan",
        CompaniaBs = "bs"
    };

    [TestFixture]
    public class CasosPositivos : UserDapperRepositoryTest {
        
        [Test]
        public async Task Create_UsuarioValido_DeberiaCrearConId() {
            
            // Arrange
            var usuario = CrearUsuario(1);

            // Act
            var resultado = await _repository.CreateAsync(usuario);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(1);
            resultado.Alias.Should().Be("test");
        }

        [Test]
        public async Task GetAll_ConDatos_DeberiaRetornarTodos() {
            
            // Arrange
            await _repository.CreateAsync(CrearUsuario(1, "uno"));
            await _repository.CreateAsync(CrearUsuario(2, "dos"));

            // Act
            var resultados = await _repository.GetAllAsync();

            // Assert
            resultados.Should().HaveCount(2);
        }

        [Test]
        public async Task GetById_Existente_DeberiaRetornarUsuario() {
            
            // Arrange
            var creado = await _repository.CreateAsync(CrearUsuario(1, "nick"));

            // Act
            var encontrado = await _repository.GetByIdAsync(creado.Id);

            // Assert
            encontrado.Should().NotBeNull();
            encontrado!.Alias.Should().Be("nick");
        }

        [Test]
        public async Task Update_Existente_DeberiaActualizar() {
            
            // Arrange
            var creado = await _repository.CreateAsync(CrearUsuario(1));
            creado.Nombre = "Nombre Actualizado";

            // Act
            var resultado = await _repository.UpdateAsync(creado);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Nombre.Should().Be("Nombre Actualizado");
        }

        [Test]
        public async Task Delete_Existente_DeberiaEliminar() {
            
            // Arrange
            var creado = await _repository.CreateAsync(CrearUsuario(1));

            // Act
            var eliminado = await _repository.DeleteAsync(creado.Id);

            // Assert
            eliminado.Should().BeTrue();
            var verificacion = await _repository.GetByIdAsync(creado.Id);
            verificacion.Should().BeNull();
        }

        [Test]
        public async Task InsertRange_DeberiaInsertarTodos() {
            
            // Arrange
            var usuarios = new[] { CrearUsuario(1, "uno"), CrearUsuario(2, "dos") };

            // Act
            await _repository.InsertRangeAsync(usuarios);

            // Assert
            var resultados = await _repository.GetAllAsync();
            resultados.Should().HaveCount(2);
        }

        [Test]
        public async Task DeleteAll_DeberiaEliminarTodos() {
            
            // Arrange
            await _repository.CreateAsync(CrearUsuario(1));
            await _repository.CreateAsync(CrearUsuario(2));

            // Act
            await _repository.DeleteAllAsync();

            // Assert
            var resultados = await _repository.GetAllAsync();
            resultados.Should().BeEmpty();
        }
    }

    [TestFixture]
    public class CasosNegativos : UserDapperRepositoryTest {
        
        [Test]
        public async Task GetById_Inexistente_DeberiaRetornarNull() {
            
            // Arrange & Act
            var resultado = await _repository.GetByIdAsync(9999);

            // Assert
            resultado.Should().BeNull();
        }

        [Test]
        public async Task Update_Inexistente_DeberiaRetornarNull() {
            
            // Arrange
            var usuario = CrearUsuario(9999);

            // Act
            var resultado = await _repository.UpdateAsync(usuario);

            // Assert
            resultado.Should().BeNull();
        }

        [Test]
        public async Task Delete_Inexistente_DeberiaRetornarFalse() {
            
            // Arrange & Act
            var eliminado = await _repository.DeleteAsync(9999);

            // Assert
            eliminado.Should().BeFalse();
        }
    }
}