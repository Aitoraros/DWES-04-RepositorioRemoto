using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RepositorioRemoto.Entity;
using RepositorioRemoto.Repositories.Sqlite;
using Testcontainers.PostgreSql;

namespace RepositorioRemoto.Tests.Repositories;

[TestFixture]
public abstract class UserEfcoreRepositoryTest
{
    private static PostgreSqlContainer _dbContainer = null!;

    private AppDbContext _context = null!;
    private UserEfcoreRepository _efcoreRepository = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        // Levanta el contenedor de base de datos una sola vez por clase de tests
        _dbContainer = new PostgreSqlBuilder()
            .WithDatabase("test_db")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        await _dbContainer.StartAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        // Detiene y elimina el contenedor Docker al finalizar los tests
        await _dbContainer.DisposeAsync();
    }

    [SetUp]
    public async Task SetUp()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_dbContainer.GetConnectionString())
            .Options;

        _context = new AppDbContext(options);

        // Asegura que el esquema de la BD esté creado antes de cada prueba
        await _context.Database.EnsureCreatedAsync();

        _efcoreRepository = new UserEfcoreRepository(_context);
    }

    [TearDown]
    public async Task TearDown()
    {
        // Limpia la base de datos entre pruebas para garantizar el aislamiento
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }

    private static UserEntity CrearUsuario(int id, string nombre, string email) => new()
    {
        Id = id,
        Alias = "alias",
        Nombre = nombre,
        Email = email,
        DireccionCalle = "Calle 1",
        DireccionSuite = "Apt. 1",
        DireccionCiudad = "Madrid",
        DireccionCodigoPostal = "28000",
        DireccionLatitud = 40.4,
        DireccionLongitud = -3.7,
        Telefono = "600000000",
        Web = "web.com",
        CompaniaNombre = "Compañía",
        CompaniaEslogan = "Eslogan",
        CompaniaBs = "bs"
    };

    [TestFixture]
    public class CasosValidos : UserEfcoreRepositoryTest
    {
        [Test]
        public async Task CreateAsync_UsuarioConId_LoGuardaConEseId()
        {
            // Arrange
            var usuario = CrearUsuario(7, "Ana", "ana@mail.com");

            // Act
            var creado = await _efcoreRepository.CreateAsync(usuario);
            var guardado = await _efcoreRepository.GetByIdAsync(7);

            // Assert
            creado.Id.Should().Be(7);
            guardado.Should().BeEquivalentTo(creado);
        }

        [Test]
        public async Task GetAllAsync_VariosUsuarios_LosDevuelveOrdenadosPorId()
        {
            // Arrange
            await _efcoreRepository.CreateAsync(CrearUsuario(2, "Luis", "luis@mail.com"));
            await _efcoreRepository.CreateAsync(CrearUsuario(1, "Ana", "ana@mail.com"));

            // Act
            var resultado = (await _efcoreRepository.GetAllAsync()).ToList();

            // Assert
            resultado.Select(u => u.Id).Should().Equal(1, 2);
        }

        [Test]
        public async Task UpdateAsync_UsuarioExistente_ActualizaLosCampos()
        {
            // Arrange
            await _efcoreRepository.CreateAsync(CrearUsuario(1, "Ana", "ana@mail.com"));
            var cambios = CrearUsuario(1, "Ana María", "nueva@mail.com");

            // Act
            var actualizado = await _efcoreRepository.UpdateAsync(cambios);
            _context.ChangeTracker.Clear();
            var guardado = await _efcoreRepository.GetByIdAsync(1);

            // Assert
            actualizado.Should().NotBeNull();
            guardado!.Id.Should().Be(1);
            guardado.Nombre.Should().Be("Ana María");
            guardado.Email.Should().Be("nueva@mail.com");
        }

        [Test]
        public async Task DeleteAsync_UsuarioExistente_DevuelveTrueYLoElimina()
        {
            // Arrange
            await _efcoreRepository.CreateAsync(CrearUsuario(1, "Ana", "ana@mail.com"));

            // Act
            var eliminado = await _efcoreRepository.DeleteAsync(1);
            var buscado = await _efcoreRepository.GetByIdAsync(1);

            // Assert
            eliminado.Should().BeTrue();
            buscado.Should().BeNull();
        }

        [Test]
        public async Task DeleteAllAsync_ConUsuarios_DejaLaTablaVacia()
        {
            // Arrange
            await _efcoreRepository.CreateAsync(CrearUsuario(1, "Ana", "ana@mail.com"));
            await _efcoreRepository.CreateAsync(CrearUsuario(2, "Luis", "luis@mail.com"));

            // Act
            await _efcoreRepository.DeleteAllAsync();
            var resultado = await _efcoreRepository.GetAllAsync();

            // Assert
            resultado.Should().BeEmpty();
        }

        [Test]
        public async Task InsertRangeAsync_VariosUsuarios_LosGuardaTodosConSusIds()
        {
            // Arrange
            var nuevos = new[]
            {
                CrearUsuario(10, "Luis", "luis@mail.com"),
                CrearUsuario(11, "Eva", "eva@mail.com")
            };

            // Act
            await _efcoreRepository.InsertRangeAsync(nuevos);
            var resultado = (await _efcoreRepository.GetAllAsync()).ToList();

            // Assert
            resultado.Select(u => u.Id).Should().Equal(10, 11);
        }
    }

    [TestFixture]
    public class CasosInvalidos : UserEfcoreRepositoryTest
    {
        [TestCase(0)]
        [TestCase(999)]
        public async Task GetByIdAsync_IdInexistente_DevuelveNull(int id)
        {
            // Act
            var resultado = await _efcoreRepository.GetByIdAsync(id);

            // Assert
            resultado.Should().BeNull();
        }

        [TestCase(999)]
        public async Task UpdateAsync_IdInexistente_DevuelveNull(int id)
        {
            // Arrange
            var cambios = CrearUsuario(id, "Ana", "ana@mail.com");

            // Act
            var resultado = await _efcoreRepository.UpdateAsync(cambios);

            // Assert
            resultado.Should().BeNull();
        }

        [TestCase(999)]
        public async Task DeleteAsync_IdInexistente_DevuelveFalse(int id)
        {
            // Act
            var resultado = await _efcoreRepository.DeleteAsync(id);

            // Assert
            resultado.Should().BeFalse();
        }
    }
}