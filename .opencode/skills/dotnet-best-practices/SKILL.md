---
name: dotnet-best-practices
description: "Ensure .NET/C# code in Ritrama2025 meets the project's best practices. Use when writing, reviewing, or refactoring C# code in this solution (services, forms, models, core, tests). Triggers: 'best practices', 'convenciones', 'revisar codigo', 'refactor', 'solucion net', 'proyecto .NET'."
---

# Best Practices .NET/C# para Ritrama2025

Aplicar estas convenciones al escribir, revisar o refactorizar código C# de la solución Ritrama2025. Son específicas del proyecto y priman sobre las best practices genéricas.

## Documentación y estructura

- Crear comentarios XML para clases, interfaces, métodos y propiedades públicas que no sean Forms o código generado por el diseñador.
- Incluir descripción de parámetros (`<param>`) y de retorno (`<returns>`) en los comentarios XML.
- Seguir la estructura de namespaces real del proyecto: `Ritrama2025`, `Ritrama2025.Core`, `Ritrama2025.Forms`, `Ritrama2025.Models`, `Ritrama2025.Services.<Area>Service`, `Ritrama2025.Helpers`, `Ritrama2025.Reports`.
- El proyecto principal es WinForms (`net10.0-windows`); `Ritrama2025.Core` es la biblioteca sin UI (`net10.0`). No acoplar Core a System.Windows.Forms.

## Arquitectura y diseño

- Cada área de negocio expone una interfaz `I{Area}Service` (ej. `IProduccionService`, `IDespachoService`) y su implementación en `Services/<Area>Service/`.
- Usar inyección de dependencias por constructor con validación `ArgumentNullException.ThrowIfNull(...)`.
- Los nuevos Forms se resuelven por DI (registrados como `AddTransient` en `Program.cs`). NO usar `ServiceLocator.Get<T>()` en código nuevo; migrar hacia inyección por constructor.
- Mantener segregación de interfaces y nombres que reflejen el dominio.
- Usar el patrón Factory solo para creación compleja (ej. objetos de reporte, exportación).

## Inyección de dependencias y servicios

- Registrar servicios en `Program.cs` con `Microsoft.Extensions.DependencyInjection`, lifetime explícito (`AddTransient`, `AddScoped`, `AddSingleton`).
- Usar `AddSingleton` solo cuando sea seguro compartir estado; los services de datos habituales van `AddTransient`.
- Los services aislables y puros (cálculos, transformaciones) que no requieren I/O se ponen en `Ritrama2025.Core` y se exponen como métodos estáticos (ej. `CalculosDespacho`) o clases con interfaces para testearlos sin base de datos.

## Recursos y localización

- La app corre en cultura `es-ES` (definida en `Program.cs`). Respetar ese comportamiento: no forzar `InvariantCulture` en UI.
- Texto de interfaz va en archivos `.resx` (ej. `Properties/Resources.resx`) o en los recursos de SunnyUI (`SunnyUIResourcesEs.cs`); no hardcodear literales de UI repetidos.
- Separar mensajes de error de UI (que se muestran al usuario) de los detalles internos de logging.

## UI WinForms (regla obligatoria del proyecto)

- Toda la UI usa controles **SunnyUI** (`Sunny.UI.UITextBox`, `UIButton`, `UIDataGridView`, `UITabControl`, `UIForm`, `UIDatePicker`, `UIComboBox`, `UICheckBox`, `UILabel`, etc.): se ve mas trabajada y profesional. NO usar controles WinForms estandar en pantallas nuevas ni al modificar existentes (migrarlos a su equivalente SunnyUI).
- Verificar la API contra el DLL referenciado (`~/.nuget/packages/sunnyui/<version>/lib/.../SunnyUI.dll`): no todos los miembros WinForms existen (ej. `UIComboBox.DropDownStyle` es `Sunny.UI.UIDropDownStyle`, `UITextBox` usa `ShowScrollBar` en vez de `ScrollBars`).
- Contenedores de layout invisibles (`SplitContainer`, `Panel`) se conservan: la regla aplica a controles visibles.
- Tema: `UIStyleManager` + fuente global; formularios con estilo propio implementan `IFormTemaClaro` (`Helpers/IFormTemaClaro.cs`).
- Recursos en español registrados en `SunnyUIResourcesEs.cs` (SunnyUI solo trae zh-CN/en-US).

## Async/Await

- Usar `async`/`await` para I/O: SQL (`Microsoft.Data.SqlClient`), archivos, HTTP.
- En WinForms NO usar `ConfigureAwait(false)` cuando haya que continuar en el hilo UI tras el await. Usarlo solo en código que no toca la UI (Core, layers de datos) o cuando se continúa en contexto sin sincronización.
- Capturar/observar excepciones async correctamente; evitar `async void` salvo en event handlers de controles.

## Testing (proyecto Ritrama2025.Tests)

- Framework: **xUnit** (`xunit` 2.9+), NO MSTest ni NUnit. `Assert.Equal`/`Assert.True`, etc.
- Usar **FluentAssertions** para asserts que ganen claridad (`Should().Be(...)`).
- Patrón AAA (Arrange, Act, Assert) y nombres de test descriptivos: `Metodo_Escenario_ResultadoEsperado` (ej. `CalcularItbis_ConSubtotal_Devuelve18PorCiento`).
- NO se usa Moq actualmente; para dependencias como servicio de producción, probar con fixture real contra SQL Server de dev (ver `DatabaseFixture` + `TestConfiguration`).
- Los tests no deben abrir MessageBox: anular `ServiceErrors.Report`/`ServiceErrors.Notify` en fixtures (ver `DatabaseFixture`).
- Cubrir tanto el escenario feliz como los fallos (ceros, nulos, bordes).

## Configuración

- Configuración fuertemente tipada con clases de settings + `IOptions`/`IConfiguration.Bind`, definidas en `appsettings.json` con overrides por entorno (`appsettings.Development.json`, `appsettings.Production.json`).
- Cargar la configuración vía `builder.Configuration` en `Program.cs` (ya incluye User Secrets en Development y variables de entorno).
- Secrets (cadenas de conexión sensibles, claves) van en User Secrets o variables de entorno, nunca en el repo.

## Manejo de errores y logging

- No romper nunca el flujo por fallos de logging. Seguir el patrón existente: `ServiceLogger.Log`/`LogError` con `Action<string>? Sink` redirigible y try/catch interno.
- Notificar al usuario vía `ServiceErrors.Notify` (MessageBox por defecto) que es redirigible para tests.
- Lanzar excepciones específicas (o validación con `ArgumentException`) con mensajes descriptivos; usar `try/catch` solo en escenarios de fallo esperados.
- Si se introduce Microsoft.Extensions.Logging a futuro, usarlo con logging estructurado y contexto; mantener compatibilidad con los tests anulando los sinks.

## Rendimiento y seguridad

- Código actual: `net10.0` / C# 13+. Usar Features y optimizaciones de la TFM del proyecto (no apuntar a versiones antiguas).
- Toda consulta SQL con parámetros (`SqlCommand.Parameters.AddWithValue`), nunca concatenación de strings en SQL.
- Validar y sanitizar entradas de usuario antes de usarlas.
- Implementar `IDisposable`/`using` correctos para conexiones, streams, ReportViewer y recursos nativos (TSCLIB).
- En WinForms liberar recursos de controles/forms correctamente (Dispose, event handlers desuscritos).

## Calidad de código

- Cumplir SOLID.
- Evitar duplicación: extraer a base/utilitarios en `Ritrama2025.Core` o `Helpers`.
- Nombres significativos que reflejen el dominio (despacho, producción, inventario, materia prima, etiquetas).
- Métodos enfocados y cohesivos; evitar métodos gigantes en Forms.
- Código generado por diseñador (`*.Designer.cs`, datasets `.xsd`) no se toca a mano; los cambios van en el archivo de código asociado.