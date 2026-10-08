using System.Reflection;
using FluentAssertions;
using Moq;
using RepositorioRemoto.Api;
using RepositorioRemoto.Cache.Common;
using RepositorioRemoto.Dto;
using RepositorioRemoto.Entity;
using RepositorioRemoto.Models;
using RepositorioRemoto.Repositories.Common;
using RepositorioRemoto.Services.Background;

namespace RepositorioRemoto.Tests.Services;

[TestFixture]
public class UserSyncBackgroundServiceTest
{
    [TestFixture]
    public class CasosValidos
    {
        private Mock<IServiceProvider> _provider = null!;
        private BackgroundService _service = null!;

        [SetUp]
        public void SetUp()
        {
            _provider = new Mock<IServiceProvider>();
            _service = new BackgroundService(_provider.Object);
        }

        [Test]
        public async Task StartAsync_TokenYaCancelado_TerminaSinLanzarExcepcion()
        {
            using var cancellation = new CancellationTokenSource();
            await cancellation.CancelAsync();

            var act = () => _service.StartAsync(cancellation.Token);

            await act.Should().NotThrowAsync();
        }

        [Test]
        public async Task Synchronize_LimpiaCacheYBdEInsertaLosUsuariosDeLaApi()
        {
            var repository = new Mock<IUserRepository>();
            var cache = new Mock<ICache<int, User>>();
            var api = new Mock<IJsonPlaceholderApi>();

            var dtos = new List<UserDto> { Dto(1), Dto(2) };
            api.Setup(a => a.GetAllAsync()).ReturnsAsync(dtos);
            cache.Setup(c => c.Clear()).Returns(Task.CompletedTask);
            repository.Setup(r => r.DeleteAllAsync()).Returns(Task.CompletedTask);
            repository.Setup(r => r.InsertRangeAsync(It.IsAny<IEnumerable<UserEntity>>())).Returns(Task.CompletedTask);

            await InvokeSynchronize(repository.Object, cache.Object, api.Object);

            cache.Verify(c => c.Clear(), Times.Once);
            repository.Verify(r => r.DeleteAllAsync(), Times.Once);
            api.Verify(a => a.GetAllAsync(), Times.Once);
            repository.Verify(r => r.InsertRangeAsync(It.Is<IEnumerable<UserEntity>>(e => e.Count() == 2)), Times.Once);
        }

        private async Task InvokeSynchronize(IUserRepository repository, ICache<int, User> cache, IJsonPlaceholderApi api)
        {
            var method = typeof(BackgroundService).GetMethod("Synchronize", BindingFlags.Instance | BindingFlags.NonPublic)!;
            await (Task)method.Invoke(_service, [repository, cache, api])!;
        }
    }

    [TestFixture]
    public class CasosInvalidos
    {
        private Mock<IServiceProvider> _provider = null!;
        private BackgroundService _service = null!;

        [SetUp]
        public void SetUp()
        {
            _provider = new Mock<IServiceProvider>();
            _service = new BackgroundService(_provider.Object);
        }

        [Test]
        public async Task StartAsync_SeCancelaDuranteLaEspera_LanzaOperationCanceledException()
        {
            using var cancellation = new CancellationTokenSource();
            var task = _service.StartAsync(cancellation.Token);
            await cancellation.CancelAsync();

            var act = () => task;

            await act.Should().ThrowAsync<OperationCanceledException>();
        }
    }

    private static UserDto Dto(int id) => new(
        id, "Nombre Test", "test", "test@test.com",
        new DireccionDto("Calle", "Suite", "Ciudad", "28000", new GeoDto("0", "0")),
        "600000000", "test.com",
        new CompaniaDto("Compañía", "Eslogan", "bs"));
}