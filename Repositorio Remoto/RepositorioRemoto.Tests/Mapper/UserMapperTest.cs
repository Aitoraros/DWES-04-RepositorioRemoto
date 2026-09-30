using FluentAssertions;
using RepositorioRemoto.Dto;
using RepositorioRemoto.Entity;
using RepositorioRemoto.Mappers;
using RepositorioRemoto.Models;

namespace RepositorioRemoto.Tests.Mapper;

[TestFixture]
[TestOf(typeof(UserMapper))]
public class UserMapperTest {

    [TestFixture]
    public class CasosCorrectos {
        
        private User _user = null!;
        private UserDto _userDto = null!;
        private UserEntity _userEntity = null!;

        [SetUp]
        public void SetUp() {
            
            _user = new User {
                Id = 1,
                Alias = "Bret",
                Nombre = "Leanne Graham",
                Email = "sincere@april.biz",
                DireccionCalle = "Kulas Light",
                DireccionSuite = "Apt. 556",
                DireccionCiudad = "Gwenborough",
                DireccionCodigoPostal = "92998-3874",
                DireccionLatitud = -37.3159,
                DireccionLongitud = 81.1496,
                Telefono = "1-770-736-8031",
                Web = "hildegard.org",
                CompaniaNombre = "Romaguera-Crona",
                CompaniaEslogan = "Multi-layered client-server neural-net",
                CompaniaBs = "harness real-time e-markets"
            };
            
            _userDto = new UserDto(
                1,
                "Leanne Graham",
                "Bret",
                "sincere@april.biz",
                new DireccionDto(
                    "Kulas Light",
                    "Apt. 556",
                    "Gwenborough",
                    "92998-3874",
                    new GeoDto("-37.3159", "81.1496")
                ),
                "1-770-736-8031",
                "hildegard.org",
                new CompaniaDto(
                    "Romaguera-Crona",
                    "Multi-layered client-server neural-net",
                    "harness real-time e-markets"
                )
            );
            
            _userEntity = new UserEntity {
                Id = 1,
                Alias = "Bret",
                Nombre = "Leanne Graham",
                Email = "sincere@april.biz",
                DireccionCalle = "Kulas Light",
                DireccionSuite = "Apt. 556",
                DireccionCiudad = "Gwenborough",
                DireccionCodigoPostal = "92998-3874",
                DireccionLatitud = -37.3159,
                DireccionLongitud = 81.1496,
                Telefono = "1-770-736-8031",
                Web = "hildegard.org",
                CompaniaNombre = "Romaguera-Crona",
                CompaniaEslogan = "Multi-layered client-server neural-net",
                CompaniaBs = "harness real-time e-markets"
            };
        }

        [Test]
        public void ToModelFromDto_DeberiaAplanarYParsearCoordenadas() {
            
            // act
            var res = _userDto.ToModel();

            // assert
            res.Should().NotBeNull();
            res.Id.Should().Be(1);
            res.Alias.Should().Be("Bret");
            res.Nombre.Should().Be("Leanne Graham");
            res.Email.Should().Be("sincere@april.biz");
            res.DireccionCalle.Should().Be("Kulas Light");
            res.DireccionSuite.Should().Be("Apt. 556");
            res.DireccionCiudad.Should().Be("Gwenborough");
            res.DireccionCodigoPostal.Should().Be("92998-3874");
            res.DireccionLatitud.Should().Be(-37.3159);
            res.DireccionLongitud.Should().Be(81.1496);
            res.Telefono.Should().Be("1-770-736-8031");
            res.Web.Should().Be("hildegard.org");
            res.CompaniaNombre.Should().Be("Romaguera-Crona");
            res.CompaniaEslogan.Should().Be("Multi-layered client-server neural-net");
            res.CompaniaBs.Should().Be("harness real-time e-markets");
        }

        [Test]
        public void ToDtoFromModel_DeberiaAnidarYConvertirCoordenadasATexto() {
            
            // act
            var res = _user.ToDto();

            // assert
            res.Should().BeEquivalentTo(_userDto);
            res.Direccion.Coordenadas.Latitud.Should().Be("-37.3159");
            res.Direccion.Coordenadas.Longitud.Should().Be("81.1496");
        }

        [Test]
        public void ToModelFromCreateRequest_SinId_DeberiaDejarIdACero() {
            
            // arrange
            var request = new CreateUserRequest(
                "Leanne Graham", "Bret", "sincere@april.biz", "Kulas Light", "Apt. 556",
                "Gwenborough", "92998-3874", -37.3159, 81.1496, "1-770-736-8031",
                "hildegard.org", "Romaguera-Crona", "Multi-layered client-server neural-net",
                "harness real-time e-markets");

            // act
            var res = request.ToModel();

            // assert
            res.Id.Should().Be(0);
            res.Should().BeEquivalentTo(_user, o => o.Excluding(u => u.Id));
        }

        [Test]
        public void ToModelFromCreateRequest_ConId_DeberiaAsignarElId() {
            
            // arrange
            var request = new CreateUserRequest(
                "Leanne Graham", "Bret", "sincere@april.biz", "Kulas Light", "Apt. 556",
                "Gwenborough", "92998-3874", -37.3159, 81.1496, "1-770-736-8031",
                "hildegard.org", "Romaguera-Crona", "Multi-layered client-server neural-net",
                "harness real-time e-markets");

            // act
            var res = request.ToModel(11);

            // assert
            res.Id.Should().Be(11);
            res.Should().BeEquivalentTo(_user, o => o.Excluding(u => u.Id));
        }

        [Test]
        public void ToModelFromUpdateRequest_DeberiaConservarElIdYTodosLosCampos() {
            
            // arrange
            var request = new UpdateUserRequest(
                1, "Leanne Graham", "Bret", "sincere@april.biz", "Kulas Light", "Apt. 556",
                "Gwenborough", "92998-3874", -37.3159, 81.1496, "1-770-736-8031",
                "hildegard.org", "Romaguera-Crona", "Multi-layered client-server neural-net",
                "harness real-time e-markets");

            // act
            var res = request.ToModel();

            // assert
            res.Should().BeEquivalentTo(_user);
        }

        [Test]
        public void ToModelFromEntity_DeberiaSerSuccess() {
            
            // act
            var res = _userEntity.ToModel();

            // assert
            res.Should().NotBeNull();
            res.Id.Should().Be(1);
            res.Alias.Should().Be("Bret");
            res.Nombre.Should().Be("Leanne Graham");
            res.Email.Should().Be("sincere@april.biz");
            res.DireccionCalle.Should().Be("Kulas Light");
            res.DireccionSuite.Should().Be("Apt. 556");
            res.DireccionCiudad.Should().Be("Gwenborough");
            res.DireccionCodigoPostal.Should().Be("92998-3874");
            res.DireccionLatitud.Should().Be(-37.3159);
            res.DireccionLongitud.Should().Be(81.1496);
            res.Telefono.Should().Be("1-770-736-8031");
            res.Web.Should().Be("hildegard.org");
            res.CompaniaNombre.Should().Be("Romaguera-Crona");
            res.CompaniaEslogan.Should().Be("Multi-layered client-server neural-net");
            res.CompaniaBs.Should().Be("harness real-time e-markets");
        }

        [Test]
        public void ToEntityFromModel_DeberiaSerSuccess() {
            
            // act
            var res = _user.ToEntity();

            // assert
            res.Should().NotBeNull();
            res.Id.Should().Be(1);
            res.Alias.Should().Be("Bret");
            res.Nombre.Should().Be("Leanne Graham");
            res.Email.Should().Be("sincere@april.biz");
            res.DireccionCalle.Should().Be("Kulas Light");
            res.DireccionSuite.Should().Be("Apt. 556");
            res.DireccionCiudad.Should().Be("Gwenborough");
            res.DireccionCodigoPostal.Should().Be("92998-3874");
            res.DireccionLatitud.Should().Be(-37.3159);
            res.DireccionLongitud.Should().Be(81.1496);
            res.Telefono.Should().Be("1-770-736-8031");
            res.Web.Should().Be("hildegard.org");
            res.CompaniaNombre.Should().Be("Romaguera-Crona");
            res.CompaniaEslogan.Should().Be("Multi-layered client-server neural-net");
            res.CompaniaBs.Should().Be("harness real-time e-markets");
        }

        [Test]
        [SetCulture("es-ES")]
        public void ToModelFromDto_ConCulturaEspanola_DeberiaParsearPuntoDecimal() {
            
            // act
            var res = _userDto.ToModel();

            // assert
            res.DireccionLatitud.Should().Be(-37.3159);
            res.DireccionLongitud.Should().Be(81.1496);
        }

        [Test]
        [SetCulture("es-ES")]
        public void ToDtoFromModel_ConCulturaEspanola_DeberiaUsarPuntoDecimal()
        {
            // act
            var res = _user.ToDto();

            // assert
            res.Direccion.Coordenadas.Latitud.Should().Be("-37.3159");
            res.Direccion.Coordenadas.Longitud.Should().Be("81.1496");
        }
    }

    [TestFixture]
    public class CasosIncorrectos {
        
        private UserDto _userDto = null!;

        [SetUp]
        public void SetUp() {
            
            _userDto = new UserDto(
                1,
                "Leanne Graham",
                "Bret",
                "sincere@april.biz",
                new DireccionDto(
                    "Kulas Light",
                    "Apt. 556",
                    "Gwenborough",
                    "92998-3874",
                    new GeoDto("-37.3159", "81.1496")
                ),
                "1-770-736-8031",
                "hildegard.org",
                new CompaniaDto(
                    "Romaguera-Crona",
                    "Multi-layered client-server neural-net",
                    "harness real-time e-markets"
                )
            );
        }

        [TestCase("12.3.4")]
        public void ToModelFromDto_CuandoLatitudEsInvalida_DeberiaLanzarFormatException(string latitud) {
            
            // arrange
            var dto = _userDto with
            {
                Direccion = _userDto.Direccion with { Coordenadas = new GeoDto(latitud, "81.1496") }
            };

            // act
            Action act = () => dto.ToModel();

            // assert
            act.Should().Throw<FormatException>();
        }

        [TestCase("12.3.4")]
        public void ToModelFromDto_CuandoLongitudEsInvalida_DeberiaLanzarFormatException(string longitud) {
            
            // arrange
            var dto = _userDto with {
                Direccion = _userDto.Direccion with { Coordenadas = new GeoDto("-37.3159", longitud) }
            };

            // act
            Action act = () => dto.ToModel();

            // assert
            act.Should().Throw<FormatException>();
        }

        [Test]
        public void ToModelFromDto_CuandoLatitudEsNull_DeberiaLanzarArgumentNullException() {
            
            // arrange
            var dto = _userDto with
            {
                Direccion = _userDto.Direccion with { Coordenadas = new GeoDto(null!, "81.1496") }
            };

            // act
            Action act = () => dto.ToModel();

            // assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void ToModelFromDto_CuandoDireccionEsNull_DeberiaLanzarNullReferenceException() {
            
            // arrange
            var dto = _userDto with { Direccion = null! };

            // act
            Action act = () => dto.ToModel();

            // assert
            act.Should().Throw<NullReferenceException>();
        }

        [Test]
        public void ToModelFromDto_CuandoCompaniaEsNull_DeberiaLanzarNullReferenceException() {
            
            // arrange
            var dto = _userDto with { Compania = null! };

            // act
            Action act = () => dto.ToModel();

            // assert
            act.Should().Throw<NullReferenceException>();
        }
    }
}