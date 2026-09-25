---
name: dotnet-backend-patterns
description: "C#/.NET backend patterns adapted for Ritrama2025: async/await, dependency injection, Result pattern, typed configuration, parameterized data access, and xUnit testing. Use when writing or reviewing services, data access, or business logic in this WinForms solution. Triggers: 'patrones backend', 'async', 'Result pattern', 'revisar services', 'acceso a datos', 'inyeccion de dependencias'."
---

# Patrones backend .NET para Ritrama2025

Patrones de negocio y datos para los servicios de Ritrama2025. Adaptados del skill `dotnet-backend-patterns` (wshobson/agents) a este proyecto: WinForms (`net10.0-windows`) + Core sin UI + SQL Server vía `Microsoft.Data.SqlClient`, sin EF Core/Dapper/Redis/MCP.

## Cuándo usar

- Escribir o revisar clases en `Services/<Area>Service/` (lógica de negocio, CRUD, reportes).
- Diseñar acceso a datos (SQL parametrizado).
- Configurar dependencias y opciones de configuración.
- Escribir o revisar tests en `Ritrama2025.Tests`.
- Revisar rendering/rendimiento/seguridad del código de servicios.

## Async/Await

- `async`/`await` para todo I/O: SQL, archivos, HTTP, impresoras.
- No bloquear con `.Result`/`.Wait()`/`.GetAwaiter().GetResult()`.
- No usar `async void` salvo handlers de eventos de controles.
- **En WinForms**: NO `ConfigureAwait(false)` cuando se continúa en el hilo UI tras el `await`. Solo usarlo en código sin UI (Core, data layer).
- Usar `CancellationToken` en métodos async nuevos si el flujo lo permite; no complicar la firma de métodos existentes sin necesidad.
- Evitar `Task.Run` innecesario para código ya async.
- Operaciones paralelas independientes: `Task.WhenAll`.
- Los `CancelEventArgs` de eventos de UI no se convierten en tokens; capturar/observar excepciones async correctamente.

## Inyección de dependencias

- Constructor clásica con `ArgumentNullException.ThrowIfNull(...)`.
- Registros en `Program.cs`: services `AddTransient` (habitual), `AddSingleton` solo para estado compartido; Forms `AddTransient`.
- No usar `ServiceLocator.Get<T>()` en código nuevo; migrarlo a inyección.
- Fábricas por configuración si se requiere selección condicional de implementación (poco frecuente aquí).
- `I{Area}Service` como contrato; implementación en `Services/<Area>Service/`.

## Result pattern (evitar excepciones como control de flujo)

```csharp
public sealed record Result<T>(bool IsSuccess, T? Value, string? Error, string? ErrorCode)
{
    public static Result<T> Success(T value) => new(true, value, null, null);
    public static Result<T> Failure(string error, string? code = null) => new(false, default, error, code);
}
```

- Utilizarlo en services para validaciones y reglas de negocio previsibles (stock insuficiente, datos no encontrados, validación fallida).
- Reservar excepciones para fallos de infraestructura/inesperados.
- En UI (Forms), traducir `Result.Error`/`ErrorCode` a mensaje de usuario vía `ServiceErrors.Notify` / dialogo SunnyUI, NO exponer detalles internos.

## Configuración tipada

- Clases de settings fuertemente tipadas con `IConfiguration.Bind` en `Program.cs` (o `IOptions<T>` si se añade).
- Definir en `appsettings.json` con overrides `appsettings.Development.json` / `appsettings.Production.json`.
- Validar con DataAnnotations (Required, range) donde tenga sentido.
- Valores sensibles (connection strings, claves) en User Secrets/entorno, nunca hardcodeados ni en el repo.
- No leer `IConfiguration` suelto dentro de services; recibir la opción tipada.

## Acceso a datos (Microsoft.Data.SqlClient)

- **Siempre** consultas parametrizadas (`SqlCommand.Parameters.AddWithValue`). Nunca concatenar valores en SQL.
- `using` sobre `SqlConnection`/`SqlCommand`/`SqlDataReader`; cerrar conexiones en `finally` si no se usa `using`.
- Proyecciones: seleccionar solo las columnas necesarias (evitar over-fetching, sobre todo en reportes grandes).
- Evitar N+1: cargar datos relacionados en una sola consulta cuando el escenario lo justifique.
- Los datasets tipados (`Ds*.xsd`) y reportes RDLC generan su propio acceso; no mezclarlos con queries ad-hoc si la funcionalidad ya está cubierta.
- Timeouts de conexión/command según el escenario (queries de reportes pueden requerir más tiempo).

## Testing (Ritrama2025.Tests)

- **xUnit** con `Assert.*` y **FluentAssertions** cuando aporten claridad.
- NO se usa Moq: para dependencias con I/O usar fixture real contra SQL Server de dev (`DatabaseFixture` + `TestConfiguration`), aislando datos sembrados/limpiados.
- Tests puros (cálculos tipo `CalculosDespacho`) directamente en Core, sin BD.
- Anular sinks de notificación en tests para no bloquear: `ServiceErrors.Report = _ => { };` y redirigir `ServiceLogger.Sink` si se valida logging.
- Cubrir feliz + fallo (cero, nulos, bordes); nombres `Metodo_Escenario_ResultadoEsperado`.

## Errores y logging

- Seguir `ServiceLogger`/`ServiceErrors`: logging nunca rompe la app; `Notify` redirigible para tests/UI.
- `try/catch` solo para fallos esperados; loggear con contexto (`ServiceLogger.LogError(contexto, ex)`).
- No filtrar detalles sensibles en mensajes al usuario.

## DO

1. `async`/`await` en toda la cadena de I/O.
2. Inyección por constructor con validación de null.
3. SQL parametrizado; `using`/dispose correctos.
4. Result pattern para reglas de negocio; excepciones solo para infraestructura.
5. Configuración tipada desde `appsettings.json`.
6. Tests xUnit sin MessageBox, usando `DatabaseFixture`.
7. `ArgumentNullException.ThrowIfNull` en públicos.
8. Lógica pura en `Ritrama2025.Core` para testearla sin UI ni BD.

## DON'T

1. No bloquear en async (`.Result`, `.Wait()`).
2. No `async void` salvo event handlers.
3. No `ConfigureAwait(false)` cuando se retoma la UI en WinForms.
4. No concatenar strings en SQL; usar parámetros.
5. No hardcodear connection strings ni claves.
6. No exponer datasets/DataTables crudos a la UI si la capa service los abstrae (mantener contrato `I{Area}Service`).
7. No usar `ServiceLocator` en código nuevo.
8. No dejar conexiones/disposables sin liberar.
9. No capturar `Exception` genérica sin loggear o tragar silenciosamente el error del usuario.
10. No duplicar lógica de negocio entre Forms — centralizar en `Services`/`Core`.

## Pitfalls comunes

- **N+1** en listados (despacho, inventario): una sola query tipada en vez de loops.
- **Memory leaks**: forms/streams/`ReportViewer`/recursos TSCLIB sin Dispose; desuscribir event handlers.
- **Deadlocks UI**: mezclar sync+async en el hilo UI; usar `await` y continuar en contexto.
- **Over-fetching** en reportes: proyectar solo columnas usadas.
- **Índices faltantes**: revisar planes para filtros habituales (búsquedas por código/rollo/fecha).
- **SQL ad-hoc sin parámetros** introduciendo inyección o errores de cultura (decimales con `,`).
- **Errores ocultos**: evitar `catch { }` mudo; loggear siempre vía `ServiceErrors`/`ServiceLogger`.