using FluentAssertions;
using RepositorioRemoto.Dto;
using RepositorioRemoto.Errors.User;
using RepositorioRemoto.Validators;

namespace RepositorioRemoto.Tests.Validators;

/// <summary>Tests de UpdateUserRequestValidator. SetUp compartido por las clases internas.</summary>
public abstract class UpdateUserRequestValidatorTest
{
    protected UpdateUserRequestValidator _validator = null!;
    protected UpdateUserRequest _requestValido = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new UpdateUserRequestValidator();
        _requestValido = new UpdateUserRequest(
            Id: 0,
            Nombre: "Leanne Graham",
            Alias: "Bret",
            Email: "Sincere@april.biz",
            DireccionCalle: "Kulas Light",
            DireccionSuite: "Apt. 556",
            DireccionCiudad: "Gwenborough",
            DireccionCodigoPostal: "92998-3874",
            DireccionLatitud: -37.3159,
            DireccionLongitud: 81.1496,
            Telefono: "1-770-736-8031 x56442",
            Web: "hildegard.org",
            CompaniaNombre: "Romaguera-Crona",
            CompaniaEslogan: "Multi-layered client-server neural-net",
            CompaniaBs: "harness real-time e-markets");
    }

    [TestFixture]
    public class CasosValidos : UpdateUserRequestValidatorTest
    {
        [Test]
        public void Validar_UsuarioValido_DevuelveSuccessConLaMismaPeticion()
        {
            // Arrange 

            // Act
            var result = _validator.Validar(_requestValido);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(_requestValido);
        }

        [TestCase("")]
        [TestCase("   ")]
        public void Validar_CamposOpcionalesVacios_DevuelveSuccess(string? valorVacio)
        {
            // Arrange
            var request = _requestValido with
            {
                DireccionCalle = valorVacio,
                DireccionSuite = valorVacio,
                DireccionCiudad = valorVacio,
                DireccionCodigoPostal = valorVacio,
                Telefono = valorVacio,
                Web = valorVacio,
                CompaniaNombre = valorVacio,
                CompaniaEslogan = valorVacio,
                CompaniaBs = valorVacio
            };

            // Act
            var result = _validator.Validar(request);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [TestCase(90, 180)]
        [TestCase(-90, -180)]
        [TestCase(0, 0)]
        public void Validar_LatitudYLongitudEnLosLimites_DevuelveSuccess(double latitud, double longitud)
        {
            // Arrange
            var request = _requestValido with { DireccionLatitud = latitud, DireccionLongitud = longitud };

            // Act
            var result = _validator.Validar(request);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }
    }

    [TestFixture]
    public class CasosInvalidos : UpdateUserRequestValidatorTest
    {
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("A")]
        public void Validar_NombreVacioOCorto_DevuelveError(string? nombre)
        {
            // Arrange
            var request = _requestValido with { Nombre = nombre! };

            // Act
            var result = _validator.Validar(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeOfType<UserError.Validation>().Which.Errors.Should()
                .Contain("El nombre es obligatorio y debe tener entre 2 y 50 caracteres.");
        }

        [TestCase(51)]
        public void Validar_NombreDemasiadoLargo_DevuelveError(int longitud)
        {
            // Arrange
            var request = _requestValido with { Nombre = new string('a', longitud) };

            // Act
            var result = _validator.Validar(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeOfType<UserError.Validation>().Which.Errors.Should()
                .Contain("El nombre es obligatorio y debe tener entre 2 y 50 caracteres.");
        }

        [TestCase("ab")]
        [TestCase("con espacio")]
        public void Validar_AliasCortoOConEspacios_DevuelveError(string alias)
        {
            // Arrange
            var request = _requestValido with { Alias = alias };

            // Act
            var result = _validator.Validar(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeOfType<UserError.Validation>().Which.Errors.Should()
                .Contain("El alias es obligatorio, debe tener entre 3 y 20 caracteres y no puede contener espacios.");
        }

        [TestCase("")]
        [TestCase("sin-arroba")]
        [TestCase("sin@punto")]
        public void Validar_EmailVacioOSinFormato_DevuelveError(string? email)
        {
            // Arrange
            var request = _requestValido with { Email = email! };

            // Act
            var result = _validator.Validar(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeOfType<UserError.Validation>().Which.Errors.Should()
                .Contain("El email es obligatorio y tiene que seguir el formato 'xxx@xxx.xxx'.");
        }

        [TestCase(101)]
        public void Validar_CalleDemasiadoLarga_DevuelveError(int longitud)
        {
            // Arrange
            var request = _requestValido with { DireccionCalle = new string('a', longitud) };

            // Act
            var result = _validator.Validar(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeOfType<UserError.Validation>().Which.Errors.Should()
                .Contain("La calle no puede superar los 100 caracteres.");
        }

        [TestCase(51)]
        public void Validar_SuiteDemasiadoLarga_DevuelveError(int longitud)
        {
            // Arrange
            var request = _requestValido with { DireccionSuite = new string('a', longitud) };

            // Act
            var result = _validator.Validar(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeOfType<UserError.Validation>().Which.Errors.Should()
                .Contain("La suite no puede superar los 50 caracteres.");
        }

        [TestCase(51)]
        public void Validar_CiudadDemasiadoLarga_DevuelveError(int longitud)
        {
            // Arrange
            var request = _requestValido with { DireccionCiudad = new string('a', longitud) };

            // Act
            var result = _validator.Validar(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeOfType<UserError.Validation>().Which.Errors.Should()
                .Contain("La ciudad no puede superar los 50 caracteres.");
        }

        [TestCase("1234")]
        [TestCase("123456")]
        [TestCase("abcde")]
        public void Validar_CodigoPostalConFormatoIncorrecto_DevuelveError(string codigoPostal)
        {
            // Arrange
            var request = _requestValido with { DireccionCodigoPostal = codigoPostal };

            // Act
            var result = _validator.Validar(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeOfType<UserError.Validation>().Which.Errors.Should()
                .Contain("El código postal debe tener el formato '12345' o '12345-6789'.");
        }

        [TestCase(91)]
        [TestCase(-91)]
        public void Validar_LatitudFueraDeRango_DevuelveError(double latitud)
        {
            // Arrange
            var request = _requestValido with { DireccionLatitud = latitud };

            // Act
            var result = _validator.Validar(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeOfType<UserError.Validation>().Which.Errors.Should()
                .Contain("La latitud debe estar entre -90 y 90.");
        }

        [TestCase(181)]
        [TestCase(-181)]
        public void Validar_LongitudFueraDeRango_DevuelveError(double longitud)
        {
            // Arrange
            var request = _requestValido with { DireccionLongitud = longitud };

            // Act
            var result = _validator.Validar(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeOfType<UserError.Validation>().Which.Errors.Should()
                .Contain("La longitud debe estar entre -180 y 180.");
        }

        [TestCase("abc")]
        [TestCase("12#34")]
        public void Validar_TelefonoConCaracteresNoPermitidos_DevuelveError(string telefono)
        {
            // Arrange
            var request = _requestValido with { Telefono = telefono };

            // Act
            var result = _validator.Validar(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeOfType<UserError.Validation>().Which.Errors.Should()
                .Contain("El teléfono solo puede contener números, espacios y los símbolos + - ( ) . x (máximo 25 caracteres).");
        }

        [TestCase(26)]
        public void Validar_TelefonoDemasiadoLargo_DevuelveError(int longitud)
        {
            // Arrange
            var request = _requestValido with { Telefono = new string('1', longitud) };

            // Act
            var result = _validator.Validar(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeOfType<UserError.Validation>().Which.Errors.Should()
                .Contain("El teléfono solo puede contener números, espacios y los símbolos + - ( ) . x (máximo 25 caracteres).");
        }

        [TestCase("no valida")]
        [TestCase("sinpunto")]
        public void Validar_WebConFormatoIncorrecto_DevuelveError(string web)
        {
            // Arrange
            var request = _requestValido with { Web = web };

            // Act
            var result = _validator.Validar(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeOfType<UserError.Validation>().Which.Errors.Should()
                .Contain("La web debe tener un formato válido, por ejemplo 'midominio.com'.");
        }

        [TestCase(101)]
        public void Validar_CompaniaNombreDemasiadoLargo_DevuelveError(int longitud)
        {
            // Arrange
            var request = _requestValido with { CompaniaNombre = new string('a', longitud) };

            // Act
            var result = _validator.Validar(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeOfType<UserError.Validation>().Which.Errors.Should()
                .Contain("El nombre de la compañía no puede superar los 100 caracteres.");
        }

        [TestCase(151)]
        public void Validar_CompaniaEsloganDemasiadoLargo_DevuelveError(int longitud)
        {
            // Arrange
            var request = _requestValido with { CompaniaEslogan = new string('a', longitud) };

            // Act
            var result = _validator.Validar(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeOfType<UserError.Validation>().Which.Errors.Should()
                .Contain("El eslogan no puede superar los 150 caracteres.");
        }

        [TestCase(151)]
        public void Validar_CompaniaBsDemasiadoLargo_DevuelveError(int longitud)
        {
            // Arrange
            var request = _requestValido with { CompaniaBs = new string('a', longitud) };

            // Act
            var result = _validator.Validar(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeOfType<UserError.Validation>().Which.Errors.Should()
                .Contain("El campo 'bs' de la compañía no puede superar los 150 caracteres.");
        }
    }
}