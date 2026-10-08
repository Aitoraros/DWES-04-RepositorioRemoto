using System.Text.Json;
using FluentAssertions;
using RepositorioRemoto.Entity;
using RepositorioRemoto.Models;
using RepositorioRemoto.Storage;

namespace RepositorioRemoto.Tests.Storage;

[TestFixture]
public abstract class JsonUserStorageTest
{
    protected string raiz = null!;
    protected string directorio = null!;
    protected UserStorage storage = null!;

    [SetUp]
    public void SetUp()
    {
        // La carpeta de exportación NO se crea aquí: el storage debe crearla
        raiz = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(raiz);
        directorio = Path.Combine(raiz, "exports");
        storage = new UserStorage(directorio);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(raiz)) Directory.Delete(raiz, true);
    }

    protected static List<User> CrearUsuarios(int cantidad) =>
        Enumerable.Range(1, cantidad)
            .Select(i => new User
            {
                Id = i,
                Alias = $"alias{i}",
                Nombre = $"Usuario {i}",
                Email = $"user{i}@mail.com"
            })
            .ToList();

    [TestFixture]
    public class CasosValidos : JsonUserStorageTest
    {
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(3)]
        public async Task ExportAsync_ConNUsuarios_CreaFicheroJsonConNUsuarios(int cantidad)
        {
            // Arrange
            var usuarios = CrearUsuarios(cantidad);

            // Act
            var ruta = await storage.ExportAsync(usuarios);
            var leidos = JsonSerializer.Deserialize<List<UserEntity>>(await File.ReadAllTextAsync(ruta));

            // Assert
            File.Exists(ruta).Should().BeTrue();
            Path.GetDirectoryName(ruta).Should().Be(Path.GetFullPath(directorio));
            Path.GetExtension(ruta).Should().Be(".json");
            leidos.Should().HaveCount(cantidad);
        }

        [Test]
        public async Task ExportAsync_ConUsuarios_ElContenidoSeRecuperaIgual()
        {
            // Arrange
            var usuarios = CrearUsuarios(2);

            // Act
            var ruta = await storage.ExportAsync(usuarios);
            var leidos = JsonSerializer.Deserialize<List<UserEntity>>(await File.ReadAllTextAsync(ruta));

            // Assert
            leidos.Should().BeEquivalentTo(usuarios);
        }
    }

    [TestFixture]
    public class CasosInvalidos : JsonUserStorageTest
    {
        [Test]
        public async Task ExportAsync_UsuariosNull_LanzaArgumentNullException()
        {
            // Arrange
            IEnumerable<User> usuarios = null!;

            // Act
            Func<Task> accion = () => storage.ExportAsync(usuarios);

            // Assert
            await accion.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public async Task ExportAsync_DirectorioEsUnFichero_LanzaIOException()
        {
            // Arrange
            var ocupado = Path.Combine(raiz, "ocupado.txt");
            await File.WriteAllTextAsync(ocupado, "no soy una carpeta");
            var storageInvalido = new UserStorage(ocupado);

            // Act
            Func<Task> accion = () => storageInvalido.ExportAsync(CrearUsuarios(1));

            // Assert
            await accion.Should().ThrowAsync<IOException>();
        }
    }
}