using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RepositorioRemoto.Entity;
using RepositorioRemoto.Repositories.Dapper;
using Testcontainers.PostgreSql;

namespace RepositorioRemoto.Tests.Repositories;

public class UserDapperRepositoryTest {
    
    private PostgreSqlContainer _container = null!;
    private UserDapperRepository _repository = null!;

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
    public async Task OneTimeTearDown() {
        await _container.DisposeAsync();
    }

    [SetUp]
    public async Task SetUp() {
        
        _repository = new UserDapperRepository(_container.GetConnectionString());

        using var context = new AppDbContext(
            new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(_container.GetConnectionString())
                .Options);
        await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"users\"");
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
            
            // arrange
            var usuario = CrearUsuario(1);

            // act
            var resultado = await _repository.CreateAsync(usuario);

            // assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(1);
            resultado.Alias.Should().Be("test");
        }

        [Test]
        public async Task GetAll_ConDatos_DeberiaRetornarTodos() {
            
            // arrange
            await _repository.CreateAsync(CrearUsuario(1, "uno"));
            await _repository.CreateAsync(CrearUsuario(2, "dos"));

            // act
            var resultados = await _repository.GetAllAsync();

            // assert
            resultados.Should().HaveCount(2);
        }

        [Test]
        public async Task GetById_Existente_DeberiaRetornarUsuario() {
            
            // arrange
            var creado = await _repository.CreateAsync(CrearUsuario(1, "nick"));

            // act
            var encontrado = await _repository.GetByIdAsync(creado.Id);

            // assert
            encontrado.Should().NotBeNull();
            encontrado!.Alias.Should().Be("nick");
        }

        [Test]
        public async Task Update_Existente_DeberiaActualizar() {
            
            // arrange
            var creado = await _repository.CreateAsync(CrearUsuario(1));
            creado.Nombre = "Nombre Actualizado";

            // act
            var resultado = await _repository.UpdateAsync(creado);

            // assert
            resultado.Should().NotBeNull();
            resultado!.Nombre.Should().Be("Nombre Actualizado");
        }

        [Test]
        public async Task Delete_Existente_DeberiaEliminar() {
            
            // arrange
            var creado = await _repository.CreateAsync(CrearUsuario(1));

            // act
            var eliminado = await _repository.DeleteAsync(creado.Id);

            // assert
            eliminado.Should().BeTrue();
            var verificacion = await _repository.GetByIdAsync(creado.Id);
            verificacion.Should().BeNull();
        }

        [Test]
        public async Task InsertRange_DeberiaInsertarTodos() {
            
            // arrange
            var usuarios = new[] { CrearUsuario(1, "uno"), CrearUsuario(2, "dos") };

            // act
            await _repository.InsertRangeAsync(usuarios);

            // assert
            var resultados = await _repository.GetAllAsync();
            resultados.Should().HaveCount(2);
        }

        [Test]
        public async Task DeleteAll_DeberiaEliminarTodos() {
            
            // arrange
            await _repository.CreateAsync(CrearUsuario(1));
            await _repository.CreateAsync(CrearUsuario(2));

            // act
            await _repository.DeleteAllAsync();

            // assert
            var resultados = await _repository.GetAllAsync();
            resultados.Should().BeEmpty();
        }
    }

    [TestFixture]
    public class CasosNegativos : UserDapperRepositoryTest {
        
        [Test]
        public async Task GetById_Inexistente_DeberiaRetornarNull() {
            
            // arrange & act
            var resultado = await _repository.GetByIdAsync(9999);

            // assert
            resultado.Should().BeNull();
        }

        [Test]
        public async Task Update_Inexistente_DeberiaRetornarNull() {
            
            // arrange
            var usuario = CrearUsuario(9999);

            // act
            var resultado = await _repository.UpdateAsync(usuario);

            // assert
            resultado.Should().BeNull();
        }

        [Test]
        public async Task Delete_Inexistente_DeberiaRetornarFalse() {
            
            // arrange & act
            var eliminado = await _repository.DeleteAsync(9999);

            // assert
            eliminado.Should().BeFalse();
        }
        
        [Test]
        public async Task EnsureCreatedAsync_DeberiaEjecutarseSinLanzarExcepciones() {
            
            // act
            var act = async () => await _repository.EnsureCreatedAsync();

            // assert
            await act.Should().NotThrowAsync();
        }
    }
}