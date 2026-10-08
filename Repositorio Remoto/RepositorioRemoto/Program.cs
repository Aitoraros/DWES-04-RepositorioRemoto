using Microsoft.Extensions.DependencyInjection;
using RepositorioRemoto.Dto;
using RepositorioRemoto.Infrastructure;
using RepositorioRemoto.Services.Background;
using RepositorioRemoto.Services.Users;

var provider = DependenciesProvider.BuildServiceProvider();
var scope = provider.CreateScope();

using var cts = new CancellationTokenSource();
var syncService = scope.ServiceProvider.GetRequiredService<BackgroundService>();
_ = syncService.StartAsync(cts.Token);

var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

Console.WriteLine("=== GetAll ===");
var todos = await userService.GetAllAsync();
Console.WriteLine($"Total de usuarios: {todos.Count()}");

Console.WriteLine("\n=== GetById (existente) ===");
var uno = await userService.GetByIdAsync(1);
Console.WriteLine(uno.IsSuccess ? $"Usuario 1: {uno.Value.Nombre}" : $"Error: {uno.Error.Message}");

Console.WriteLine("\n=== GetById (inexistente) ===");
var noExiste = await userService.GetByIdAsync(9999);
Console.WriteLine(noExiste.IsSuccess ? "Encontrado" : $"Error: {noExiste.Error.Message}");

Console.WriteLine("\n=== Create ===");
var creado = await userService.CreateAsync(new CreateUserRequest(
    Nombre: "Nick Test", Alias: "nickt", Email: "nick@test.com",
    DireccionCalle: "Calle Falsa", DireccionSuite: "1A", DireccionCiudad: "Madrid",
    DireccionCodigoPostal: "28000", DireccionLatitud: 40.4, DireccionLongitud: -3.7,
    Telefono: "600000000", Web: "nick.dev",
    CompaniaNombre: "NickCorp", CompaniaEslogan: "Haciendo cosas", CompaniaBs: "bs"));

Console.WriteLine(creado.IsSuccess ? $"Creado con Id: {creado.Value.Id}" : $"Error: {creado.Error.Message}");

if (creado.IsSuccess)
{
    Console.WriteLine("\n=== Update ===");
    var actualizar = new UpdateUserRequest(
        Id: creado.Value.Id, Nombre: "Nick Actualizado", Alias: creado.Value.Alias, Email: creado.Value.Email,
        DireccionCalle: creado.Value.DireccionCalle, DireccionSuite: creado.Value.DireccionSuite,
        DireccionCiudad: creado.Value.DireccionCiudad, DireccionCodigoPostal: creado.Value.DireccionCodigoPostal,
        DireccionLatitud: creado.Value.DireccionLatitud, DireccionLongitud: creado.Value.DireccionLongitud,
        Telefono: creado.Value.Telefono, Web: creado.Value.Web,
        CompaniaNombre: creado.Value.CompaniaNombre, CompaniaEslogan: creado.Value.CompaniaEslogan, CompaniaBs: creado.Value.CompaniaBs);

    var actualizado = await userService.UpdateAsync(creado.Value.Id, actualizar);
    Console.WriteLine(actualizado.IsSuccess ? $"Actualizado: {actualizado.Value.Nombre}" : $"Error: {actualizado.Error.Message}");

    Console.WriteLine("\n=== Delete ===");
    var eliminado = await userService.DeleteAsync(creado.Value.Id);
    Console.WriteLine(eliminado.IsSuccess ? "Eliminado correctamente" : $"Error: {eliminado.Error.Message}");
}

cts.Cancel();