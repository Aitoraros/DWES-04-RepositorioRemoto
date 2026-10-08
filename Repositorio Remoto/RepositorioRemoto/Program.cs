using Microsoft.Extensions.DependencyInjection;
using RepositorioRemoto.Dto;
using RepositorioRemoto.Services.Background;
using RepositorioRemoto.Services.Notificactions;
using RepositorioRemoto.Services.Users;

Console.WriteLine("=== Arrancando RepositorioRemoto ===\n");

var provider = DependenciesProvider.BuildServiceProvider();
var scope = provider.CreateScope();

// ---------- Notificaciones: suscribirse ANTES de hacer nada ----------
// Es un flujo caliente: si te suscribes tarde, pierdes los eventos ya emitidos
var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
notificationService.Observable.Subscribe(n =>
    Console.WriteLine($"  >> [{n.Timestamp:HH:mm:ss}] [{n.Type}] {n.Message}"));

// ---------- Sincronización en segundo plano ----------
using var cts = new CancellationTokenSource();
var syncService = scope.ServiceProvider.GetRequiredService<BackgroundService>();
_ = syncService.StartAsync(cts.Token);

var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

// ---------- GetAll ----------
Console.WriteLine("=== GetAll ===");
var todos = await userService.GetAllAsync();
Console.WriteLine($"Total de usuarios: {todos.Count()}");

// ---------- GetById (existente) ----------
Console.WriteLine("\n=== GetById (existente) ===");
var uno = await userService.GetByIdAsync(1);
Console.WriteLine(uno.IsSuccess ? $"Usuario 1: {uno.Value.Nombre}" : $"Error: {uno.Error.Message}");

// ---------- GetById (inexistente) ----------
Console.WriteLine("\n=== GetById (inexistente) ===");
var noExiste = await userService.GetByIdAsync(9999);
Console.WriteLine(noExiste.IsSuccess ? "Encontrado" : $"Error: {noExiste.Error.Message}");

// ---------- Create ----------
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
    // ---------- Update (sobre un usuario que SÍ existe de verdad en la API) ----------
    Console.WriteLine("\n=== Update (usuario 1, existente real) ===");
    var existente = await userService.GetByIdAsync(1);

    if (existente.IsSuccess)
    {
        var actualizar = new UpdateUserRequest(
            Id: 1, Nombre: "Leanne Actualizada", Alias: existente.Value.Alias, Email: existente.Value.Email,
            DireccionCalle: existente.Value.DireccionCalle, DireccionSuite: existente.Value.DireccionSuite,
            DireccionCiudad: existente.Value.DireccionCiudad, DireccionCodigoPostal: existente.Value.DireccionCodigoPostal,
            DireccionLatitud: existente.Value.DireccionLatitud, DireccionLongitud: existente.Value.DireccionLongitud,
            Telefono: existente.Value.Telefono, Web: existente.Value.Web,
            CompaniaNombre: existente.Value.CompaniaNombre, CompaniaEslogan: existente.Value.CompaniaEslogan,
            CompaniaBs: existente.Value.CompaniaBs);

        var actualizado = await userService.UpdateAsync(1, actualizar);
        Console.WriteLine(actualizado.IsSuccess ? $"Actualizado: {actualizado.Value.Nombre}" : $"Error: {actualizado.Error.Message}");
    }

    // ---------- Delete ----------
    Console.WriteLine("\n=== Delete ===");
    var eliminado = await userService.DeleteAsync(creado.Value.Id);
    Console.WriteLine(eliminado.IsSuccess ? "Eliminado correctamente" : $"Error: {eliminado.Error.Message}");
}

// ---------- Dejar un momento para ver las notificaciones en consola ----------
await Task.Delay(500);

cts.Cancel();
Console.WriteLine("\n=== Fin de la prueba ===");