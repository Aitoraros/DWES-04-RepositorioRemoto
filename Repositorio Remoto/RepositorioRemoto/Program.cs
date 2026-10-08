using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using RepositorioRemoto.Config;
using RepositorioRemoto.Dto;
using RepositorioRemoto.Errors.Common;
using RepositorioRemoto.Infrastructure;
using RepositorioRemoto.Models;
using RepositorioRemoto.Services.Background;
using RepositorioRemoto.Services.Notificactions;
using RepositorioRemoto.Services.Users;

Console.WriteLine("=== Arrancando RepositorioRemoto ===");
if (AppConfig.IsDevelopment)
    File.Delete("usuarios.db");
var provider = DependenciesProvider.BuildServiceProvider();
using var scope = provider.CreateScope();

// Suscribirse ANTES de operar: es un flujo caliente y se pierden los eventos ya emitidos
var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
using var suscripcion = notificationService.Observable.Subscribe(n =>
    Console.WriteLine($"  >> [{n.Timestamp:HH:mm:ss}] [{n.Type}] {n.Message}"));

using var cts = new CancellationTokenSource();
var syncService = scope.ServiceProvider.GetRequiredService<BackgroundService>();
_ = syncService.StartAsync(cts.Token);

var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

// ---------- GetAll ----------
Titulo("GetAll (BD vacía -> API, luego BD)");
var todos = await userService.GetAllAsync();
Console.WriteLine($"Total de usuarios: {todos.Count()}");

// ---------- GetById ----------
Titulo("GetById existente (1ª vez: BD o API)");
Mostrar(await userService.GetByIdAsync(1), u => $"Usuario 1: {u.Nombre}");

Titulo("GetById existente (2ª vez: debe salir de caché)");
Mostrar(await userService.GetByIdAsync(1), u => $"Usuario 1: {u.Nombre}");

Titulo("GetById inexistente");
Mostrar(await userService.GetByIdAsync(9999), u => "Encontrado (no debería)");

// ---------- Create ----------
Titulo("Create inválido (debe fallar la validación)");
Mostrar(await userService.CreateAsync(NuevoRequest(nombre: "", email: "no-es-un-email")),
    u => "Creado (no debería)");

Titulo("Create válido");
var creado = await userService.CreateAsync(NuevoRequest("Nick Test", "nick@test.com"));
Mostrar(creado, u => $"Creado con Id: {u.Id}");

// ---------- Update ----------
var usuario1 = (await userService.GetByIdAsync(1)).Value;

Titulo("Update válido (usuario 1)");
Mostrar(await userService.UpdateAsync(1, ActualizarRequest(1, usuario1, "Leanne Actualizada")),
    u => $"Actualizado: {u.Nombre}");

Titulo("Update con id de ruta distinto al del cuerpo");
Mostrar(await userService.UpdateAsync(1, ActualizarRequest(2, usuario1, "Otro")),
    u => "Actualizado (no debería)");

Titulo("Update de usuario inexistente");
Mostrar(await userService.UpdateAsync(9999, ActualizarRequest(9999, usuario1, "Fantasma")),
    u => "Actualizado (no debería)");

// ---------- Delete ----------
if (creado.IsSuccess)
{
    Titulo("Delete del usuario creado");
    Mostrar(await userService.DeleteAsync(creado.Value.Id), u => $"Eliminado: {u.Nombre}");

    Titulo("Delete del mismo usuario otra vez (ya no existe)");
    Mostrar(await userService.DeleteAsync(creado.Value.Id), u => "Eliminado (no debería)");
}

// ---------- Export ----------
Titulo("Export");
Mostrar(await userService.ExportAsync(), ruta => $"Exportado en: {ruta}");

// Margen para que lleguen las últimas notificaciones a la consola
await Task.Delay(500);

cts.Cancel();
Console.WriteLine("\n=== Fin de la prueba ===");

// ====================== helpers ======================

static void Titulo(string texto) => Console.WriteLine($"\n=== {texto} ===");

static void Mostrar<T>(Result<T, DomainError> resultado, Func<T, string> alExito) =>
    Console.WriteLine(resultado.IsSuccess ? $"OK: {alExito(resultado.Value)}" : $"Error: {resultado.Error.Message}");

static CreateUserRequest NuevoRequest(string nombre, string email) => new(
    Nombre: nombre, Alias: "nickt", Email: email,
    DireccionCalle: "Calle Falsa", DireccionSuite: "1A", DireccionCiudad: "Madrid",
    DireccionCodigoPostal: "28000", DireccionLatitud: 40.4, DireccionLongitud: -3.7,
    Telefono: "600000000", Web: "nick.dev",
    CompaniaNombre: "NickCorp", CompaniaEslogan: "Haciendo cosas", CompaniaBs: "bs");

static UpdateUserRequest ActualizarRequest(int id, User baseUser, string nombre) => new(
    Id: id, Nombre: nombre, Alias: baseUser.Alias, Email: baseUser.Email,
    DireccionCalle: baseUser.DireccionCalle, DireccionSuite: baseUser.DireccionSuite,
    DireccionCiudad: baseUser.DireccionCiudad, DireccionCodigoPostal: baseUser.DireccionCodigoPostal,
    DireccionLatitud: baseUser.DireccionLatitud, DireccionLongitud: baseUser.DireccionLongitud,
    Telefono: baseUser.Telefono, Web: baseUser.Web,
    CompaniaNombre: baseUser.CompaniaNombre, CompaniaEslogan: baseUser.CompaniaEslogan,
    CompaniaBs: baseUser.CompaniaBs);