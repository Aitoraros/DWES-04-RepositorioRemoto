using System.Net;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Moq;
using Refit;
using RepositorioRemoto.Api;
using RepositorioRemoto.Cache.Common;
using RepositorioRemoto.Dto;
using RepositorioRemoto.Entity;
using RepositorioRemoto.Errors.Common;
using RepositorioRemoto.Errors.User;
using RepositorioRemoto.Models;
using RepositorioRemoto.Repositories.Common;
using RepositorioRemoto.Services.Users;
using RepositorioRemoto.Validators;

namespace RepositorioRemoto.Tests.Services;

public abstract class UserServiceTest
{
    [TestFixture]
    public class CasosValidos {
        private Mock<IValidator<CreateUserRequest>> _createValidator = null!;
        private Mock<IValidator<UpdateUserRequest>> _updateValidator = null!;
        private Mock<IUserRepository> _repository = null!;
        private Mock<ICache<int, User>> _cache = null!;
        private Mock<IJsonPlaceholderApi> _api = null!;
        private UserService _service = null!;

        [SetUp]
        public void SetUp() {
            _createValidator = new Mock<IValidator<CreateUserRequest>>();
            _updateValidator = new Mock<IValidator<UpdateUserRequest>>();
            _repository = new Mock<IUserRepository>();
            _cache = new Mock<ICache<int, User>>();
            _api = new Mock<IJsonPlaceholderApi>();
            _service = new UserService(_createValidator.Object, _updateValidator.Object, _repository.Object, _cache.Object, _api.Object);

            _createValidator.Setup(v => v.Validar(It.IsAny<CreateUserRequest>()))
                .Returns((CreateUserRequest r) => Result.Success<CreateUserRequest, DomainError>(r));
            _updateValidator.Setup(v => v.Validar(It.IsAny<UpdateUserRequest>()))
                .Returns((UpdateUserRequest r) => Result.Success<UpdateUserRequest, DomainError>(r));
        }

        [Test]
        public async Task GetAllAsync_ConDatosEnBd_NoConsultaApi() {
            _repository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<UserEntity> { Entity(1), Entity(2) });

            var resultado = await _service.GetAllAsync();

            resultado.Should().HaveCount(2);
            _api.Verify(a => a.GetAllAsync(), Times.Never);
        }

        [Test]
        public async Task GetAllAsync_BdVaciaYApiOk_ConsultaApiYGuardaEnBd() {
            _repository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<UserEntity>());
            _api.Setup(a => a.GetAllAsync()).ReturnsAsync([Dto(1), Dto(2)]);

            var resultado = await _service.GetAllAsync();

            resultado.Should().HaveCount(2);
            _repository.Verify(r => r.InsertRangeAsync(It.Is<IEnumerable<UserEntity>>(e => e.Count() == 2)), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_CacheHit_NoConsultaBdNiApi() {
            _cache.Setup(c => c.Get(7)).ReturnsAsync(Model(7));

            var resultado = await _service.GetByIdAsync(7);

            resultado.IsSuccess.Should().BeTrue();
            _repository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
            _api.Verify(a => a.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task GetByIdAsync_CacheMissBdHit_GuardaEnCache() {
            _cache.Setup(c => c.Get(7)).ReturnsAsync((User?)null);
            _repository.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(Entity(7));

            var resultado = await _service.GetByIdAsync(7);

            resultado.IsSuccess.Should().BeTrue();
            _cache.Verify(c => c.Add(7, It.IsAny<User>()), Times.Once);
            _api.Verify(a => a.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task GetByIdAsync_SoloExisteEnApi_GuardaEnBdYCache() {
            _cache.Setup(c => c.Get(7)).ReturnsAsync((User?)null);
            _repository.Setup(r => r.GetByIdAsync(7)).ReturnsAsync((UserEntity?)null);
            _api.Setup(a => a.GetByIdAsync(7)).ReturnsAsync(Dto(7));

            var resultado = await _service.GetByIdAsync(7);

            resultado.IsSuccess.Should().BeTrue();
            _repository.Verify(r => r.CreateAsync(It.IsAny<UserEntity>()), Times.Once);
            _cache.Verify(c => c.Add(7, It.IsAny<User>()), Times.Once);
        }

        [Test]
        public async Task CreateAsync_Valido_GuardaEnBdYCacheConIdDeLaApi() {
            _api.Setup(a => a.CreateAsync(It.IsAny<UserDto>())).ReturnsAsync(Dto(12));
            _repository.Setup(r => r.CreateAsync(It.IsAny<UserEntity>())).ReturnsAsync((UserEntity e) => e);

            var resultado = await _service.CreateAsync(Request());

            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Id.Should().Be(12);
        }

        [Test]
        public async Task UpdateAsync_Valido_ActualizaCache() {
            _repository.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(Entity(4));
            _api.Setup(a => a.UpdateAsync(4, It.IsAny<UserDto>())).ReturnsAsync(Dto(4));
            _repository.Setup(r => r.UpdateAsync(It.IsAny<UserEntity>())).ReturnsAsync((UserEntity e) => e);

            var resultado = await _service.UpdateAsync(4, UpdateRequest(4));

            resultado.IsSuccess.Should().BeTrue();
            _cache.Verify(c => c.Add(4, It.IsAny<User>()), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_RepositorioDevuelveNull_DevuelveNotFound() {
            _repository.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(Entity(4));
            _api.Setup(a => a.UpdateAsync(4, It.IsAny<UserDto>())).ReturnsAsync(Dto(4));
            _repository.Setup(r => r.UpdateAsync(It.IsAny<UserEntity>())).ReturnsAsync((UserEntity?)null);

            var resultado = await _service.UpdateAsync(4, UpdateRequest(4));

            resultado.IsFailure.Should().BeTrue();
        }

        [Test]
        public async Task DeleteAsync_Valido_EliminaDeBdYCache() {
            _repository.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(Entity(4));

            var resultado = await _service.DeleteAsync(4);

            resultado.IsSuccess.Should().BeTrue();
            _repository.Verify(r => r.DeleteAsync(4), Times.Once);
            _cache.Verify(c => c.Remove(4), Times.Once);
        }
    }

    [TestFixture]
    public class CasosInvalidos {
        private Mock<IValidator<CreateUserRequest>> _createValidator = null!;
        private Mock<IValidator<UpdateUserRequest>> _updateValidator = null!;
        private Mock<IUserRepository> _repository = null!;
        private Mock<ICache<int, User>> _cache = null!;
        private Mock<IJsonPlaceholderApi> _api = null!;
        private UserService _service = null!;

        [SetUp]
        public void SetUp() {
            _createValidator = new Mock<IValidator<CreateUserRequest>>();
            _updateValidator = new Mock<IValidator<UpdateUserRequest>>();
            _repository = new Mock<IUserRepository>();
            _cache = new Mock<ICache<int, User>>();
            _api = new Mock<IJsonPlaceholderApi>();
            _service = new UserService(_createValidator.Object, _updateValidator.Object, _repository.Object, _cache.Object, _api.Object);

            _createValidator.Setup(v => v.Validar(It.IsAny<CreateUserRequest>()))
                .Returns((CreateUserRequest r) => Result.Success<CreateUserRequest, DomainError>(r));
            _updateValidator.Setup(v => v.Validar(It.IsAny<UpdateUserRequest>()))
                .Returns((UpdateUserRequest r) => Result.Success<UpdateUserRequest, DomainError>(r));
        }

        [Test]
        public async Task GetAllAsync_ApiFalla_DevuelveListaVacia() {
            _repository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<UserEntity>());
            _api.Setup(a => a.GetAllAsync()).ThrowsAsync(await CrearApiException(HttpStatusCode.InternalServerError));

            var resultado = await _service.GetAllAsync();

            resultado.Should().BeEmpty();
        }

        [Test]
        public async Task GetByIdAsync_NoExisteEnNingunSitio_DevuelveNotFound() {
            _cache.Setup(c => c.Get(99)).ReturnsAsync((User?)null);
            _repository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((UserEntity?)null);
            _api.Setup(a => a.GetByIdAsync(99)).ThrowsAsync(await CrearApiException(HttpStatusCode.NotFound));

            var resultado = await _service.GetByIdAsync(99);

            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<UserError.NotFound>();
        }

        [Test]
        public async Task GetByIdAsync_ApiDevuelveOtroError_DevuelveBadResponse() {
            _cache.Setup(c => c.Get(5)).ReturnsAsync((User?)null);
            _repository.Setup(r => r.GetByIdAsync(5)).ReturnsAsync((UserEntity?)null);
            _api.Setup(a => a.GetByIdAsync(5)).ThrowsAsync(await CrearApiException(HttpStatusCode.InternalServerError));

            var resultado = await _service.GetByIdAsync(5);

            resultado.IsFailure.Should().BeTrue();
        }

        [Test]
        public async Task CreateAsync_ValidacionFalla_NoLlamaApi() {
            _createValidator.Setup(v => v.Validar(It.IsAny<CreateUserRequest>()))
                .Returns(Result.Failure<CreateUserRequest, DomainError>(UserErrors.Validation(["error"])));

            var resultado = await _service.CreateAsync(Request());

            resultado.IsFailure.Should().BeTrue();
            _api.Verify(a => a.CreateAsync(It.IsAny<UserDto>()), Times.Never);
        }

        [Test]
        public async Task CreateAsync_ApiFalla_NoGuardaEnBd() {
            _api.Setup(a => a.CreateAsync(It.IsAny<UserDto>())).ThrowsAsync(await CrearApiException(HttpStatusCode.InternalServerError));

            var resultado = await _service.CreateAsync(Request());

            resultado.IsFailure.Should().BeTrue();
            _repository.Verify(r => r.CreateAsync(It.IsAny<UserEntity>()), Times.Never);
        }

        [Test]
        public async Task UpdateAsync_IdNoCoincide_NoValida() {
            var resultado = await _service.UpdateAsync(1, UpdateRequest(2));

            resultado.IsFailure.Should().BeTrue();
            _updateValidator.Verify(v => v.Validar(It.IsAny<UpdateUserRequest>()), Times.Never);
        }

        [Test]
        public async Task UpdateAsync_ValidacionFalla_NoComprueabaExistencia() {
            _updateValidator.Setup(v => v.Validar(It.IsAny<UpdateUserRequest>()))
                .Returns(Result.Failure<UpdateUserRequest, DomainError>(UserErrors.Validation(["error"])));

            var resultado = await _service.UpdateAsync(1, UpdateRequest(1));

            resultado.IsFailure.Should().BeTrue();
            _repository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task UpdateAsync_NoExisteLocalmente_NoLlamaApi() {
            _repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((UserEntity?)null);

            var resultado = await _service.UpdateAsync(1, UpdateRequest(1));

            resultado.IsFailure.Should().BeTrue();
            _api.Verify(a => a.UpdateAsync(It.IsAny<int>(), It.IsAny<UserDto>()), Times.Never);
        }

        [Test]
        public async Task UpdateAsync_ApiFalla_NoActualizaBd() {
            _repository.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(Entity(4));
            _api.Setup(a => a.UpdateAsync(4, It.IsAny<UserDto>())).ThrowsAsync(await CrearApiException(HttpStatusCode.InternalServerError));

            var resultado = await _service.UpdateAsync(4, UpdateRequest(4));

            resultado.IsFailure.Should().BeTrue();
            _repository.Verify(r => r.UpdateAsync(It.IsAny<UserEntity>()), Times.Never);
        }

        [Test]
        public async Task DeleteAsync_NoExisteLocalmente_NoLlamaApi() {
            _repository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((UserEntity?)null);

            var resultado = await _service.DeleteAsync(99);

            resultado.IsFailure.Should().BeTrue();
            _api.Verify(a => a.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task DeleteAsync_ApiFalla_NoEliminaDeBd() {
            _repository.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(Entity(4));
            _api.Setup(a => a.DeleteAsync(4)).ThrowsAsync(await CrearApiException(HttpStatusCode.InternalServerError));

            var resultado = await _service.DeleteAsync(4);

            resultado.IsFailure.Should().BeTrue();
            _repository.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }
    }

    // único helper, solo para simular fallos de la API
    private static Task<ApiException> CrearApiException(HttpStatusCode statusCode) =>
        ApiException.Create(new HttpRequestMessage(), HttpMethod.Get, new HttpResponseMessage(statusCode), new RefitSettings());

    // helpers
    private static UserEntity Entity(int id) => new() {
        Id = id, Alias = "test", Nombre = "Nombre Test", Email = "test@test.com",
        DireccionCalle = "Calle", DireccionSuite = "Suite", DireccionCiudad = "Ciudad",
        DireccionCodigoPostal = "28000", DireccionLatitud = 0, DireccionLongitud = 0,
        Telefono = "600000000", Web = "test.com",
        CompaniaNombre = "Compañía", CompaniaEslogan = "Eslogan", CompaniaBs = "bs"
    };

    private static User Model(int id) => new() {
        Id = id, Alias = "test", Nombre = "Nombre Test", Email = "test@test.com",
        DireccionCalle = "Calle", DireccionSuite = "Suite", DireccionCiudad = "Ciudad",
        DireccionCodigoPostal = "28000", DireccionLatitud = 0, DireccionLongitud = 0,
        Telefono = "600000000", Web = "test.com",
        CompaniaNombre = "Compañía", CompaniaEslogan = "Eslogan", CompaniaBs = "bs"
    };

    private static UserDto Dto(int id) => new(
        id, "Nombre Test", "test", "test@test.com",
        new DireccionDto("Calle", "Suite", "Ciudad", "28000", new GeoDto("0", "0")),
        "600000000", "test.com",
        new CompaniaDto("Compañía", "Eslogan", "bs"));

    private static CreateUserRequest Request() => new(
        Nombre: "Nombre Test", Alias: "test", Email: "test@test.com",
        DireccionCalle: "Calle", DireccionSuite: "Suite", DireccionCiudad: "Ciudad",
        DireccionCodigoPostal: "28000", DireccionLatitud: 0, DireccionLongitud: 0,
        Telefono: "600000000", Web: "test.com",
        CompaniaNombre: "Compañía", CompaniaEslogan: "Eslogan", CompaniaBs: "bs");

    private static UpdateUserRequest UpdateRequest(int id) => new(
        Id: id, Nombre: "Nombre Test", Alias: "test", Email: "test@test.com",
        DireccionCalle: "Calle", DireccionSuite: "Suite", DireccionCiudad: "Ciudad",
        DireccionCodigoPostal: "28000", DireccionLatitud: 0, DireccionLongitud: 0,
        Telefono: "600000000", Web: "test.com",
        CompaniaNombre: "Compañía", CompaniaEslogan: "Eslogan", CompaniaBs: "bs");
}