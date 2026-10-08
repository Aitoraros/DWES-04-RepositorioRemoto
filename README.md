# 🚀 REST User Management Service — Multi-Level Caching & Sync Architecture

Un servicio de procesamiento e integración en **.NET 9 / C# 14** diseñado con **arquitectura limpia, almacenamiento híbrido en tres niveles, sincronización en segundo plano y notificación de eventos mediante Programación Reactiva**.

---

## 🎯 Objetivos de la Práctica

El objetivo principal de esta práctica ha sido construir un servicio REST resiliente, asíncrono y de alto rendimiento capaz de gestionar usuarios a través de una API remota, sincronizando los datos en almacenamiento local y garantizando un acceso ultrarrápido mediante estrategias de almacenamiento en caché.

---

## 🌟 Características Principales y Arquitectura

### 1. 🏗️ Almacenamiento en 3 Niveles (Multi-Tier Storage)
* **Nivel 1 — Caché de Lectura:** Acceso ultrarrápido implementando dos estrategias intercambiables:
  * **MemoryCache Local** (con políticas LRU / expiración).
  * **Redis Server** distribuido (mediante Docker).
* **Nivel 2 — Base de Datos Local / Persistencia:**
  * Soporte dual con **SQLite (vía Entity Framework Core)** para entorno local ligero y **PostgreSQL (vía Dapper)** para máxima velocidad de lectura y consultas optimizadas.
* **Nivel 3 — API Remota (Upstream):**
  * Integración con la API de [JSONPlaceholder](https://jsonplaceholder.typicode.com/) como fuente primaria mediante **Refit** (cliente HTTP tipado asíncrono).

### 2. 🔄 Sincronización y Tareas en Segundo Plano
* **Startup Sync:** Limpieza automática y recarga inicial desde la API REST remota al arrancar la aplicación.
* **BackgroundService:** Worker en segundo plano ejecutado cada **60 segundos** para sincronizar el estado local con la API remota e invalidar la caché.

### 3. 📢 Notificaciones Reactivas (Rx.NET)
* Publicación/Suscripción de eventos de dominio (`Created`, `Updated`, `Deleted`) utilizando `System.Reactive` (`IObservable<T>` / `Subject<T>`).
* Inyección del servicio de eventos en la capa de negocio (`UserService`) y suscripción explícita al inicio de la aplicación (`Program.cs`) para logging y auditoría.

### 4. 🛡️ Robustez y Arquitectura Limpia
* **Programación Funcional:** Manejo de errores de dominio explícitos mediante `Result<T, DomainError>` (**CSharpFunctionalExtensions**).
* **Funcionalidades C# 14:** Utilización de *Primary Constructors* y tipos de dominio inmutables.
* **Logging Estructurado:** Integración de **Serilog** con sinks hacia Consola y Archivo con rotación automática.

---

## 🧪 Estrategia de Testing

* **Pruebas Unitarias:** Cobertura de la lógica de negocio (`UserService`) mockeando dependencias mediante **NUnit**, **Moq** y afirmaciones con **FluentAssertions** (patrón AAA).
* **Pruebas de Integración:** Verificación real de repositorios y bases de datos utilizando **Testcontainers** para levantar contenedores efímeros durante la ejecución del suite de tests.

---

## 🛠️ Tecnologías Utilizadas

| Categoría | Tecnologías / Librerías |
| :--- | :--- |
| **Framework** | .NET 9 / C# 14 |
| **Persistencia** | Entity Framework Core, Dapper, SQLite, PostgreSQL |
| **Caché** | `Microsoft.Extensions.Caching.Memory`, Redis |
| **HTTP Client** | Refit + `HttpClientFactory` |
| **Programación Reactiva** | System.Reactive (Rx.NET) |
| **Manejo de Errores** | CSharpFunctionalExtensions (`Result<T, Error>`) |
| **Testing** | NUnit, Moq, FluentAssertions, Testcontainers |
| **Infraestructura** | Docker, Docker Compose, Serilog |

---

## ⚙️ Configuración (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "SQLite": "Data Source=usuarios.db",
    "PostgreSQL": "Host=localhost;Port=5432;Database=usuarios;Username=postgres;Password=postgres",
    "Redis": "localhost:6379"
  },
  "JsonPlaceholderApi": {
    "BaseUrl": "[https://jsonplaceholder.typicode.com](https://jsonplaceholder.typicode.com)"
  },
  "CacheOptions": {
    "Provider": "MemoryCache",
    "AbsoluteExpirationInSeconds": 600
  }
}
```

# Iniciar infraestructura (PostgreSQL + Redis)
```
docker-compose up -d
```

# Ejecutar la aplicación .NET
```
dotnet run --project RepositorioRemoto
```
# Ejecutar la suite completa de tests unitarios e integración
```
dotnet test
```
