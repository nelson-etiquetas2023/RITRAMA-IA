---
name: dotnet-design-pattern-review
description: "Review C#/.NET code in Ritrama2025 for design pattern implementation and suggest improvements without editing. Use when asked to review code, architecture, or design patterns of this solution. Triggers: 'revisar patrones', 'design pattern review', 'revisar arquitectura', 'mejorar el diseño', 'review del diseño'."
---

# Revisión de patrones de diseño para Ritrama2025

Revisar el código C#/.NET indicado evaluando la implementación de patrones de diseño y convenciones del proyecto. **No modificar código**: entregar únicamente un review con recomendaciones accionables y concretas.

## Patrones y convenciones relevantes del proyecto

- **Abstracción por área**: cada área expone `I{Area}Service` (ej. `IProduccionService`, `IDespachoService`) con implementación en `Services/<Area>Service/`.
- **Lógica pura en Core**: cálculos y transformaciones sin I/O van en `Ritrama2025.Core` como clases/estáticos testables (ej. `CalculosDespacho`).
- **Dependency Injection**: por constructor clásica con `ArgumentNullException.ThrowIfNull(...)`; registros `AddTransient`/`AddSingleton` en `Program.cs`. Formularios resueltos por DI.
- **ServiceLocator**: está en fase de desmantelamiento (ver comentario en `Program.cs`); señalar cualquier uso nuevo, no proponerlo como patrón.
- **Factory**: solo para creación compleja (reportes, exportación con ClosedXML, etc.).
- **Provider**: abstracciones de recursos externos con contratos claros y configuración; en este proyecto externos son SQL Server (`Microsoft.Data.SqlClient`), impresoras TSCLIB, reportes RDLC/ReportViewer.
- **Recursos y localización**: `.resx` (Resources) + SunnyUI, cultura `es-ES`; no proponer `ResourceManager` manual con `LogMessages`/`ErrorMessages`.
- **Logging/errores**: `ServiceLogger.Log`/`LogError` y `ServiceErrors.Notify`/`Report` (sinks redirigibles, nunca romper la app).

## Checklist de revisión

- **Patrones GoF**: ¿Command/Strategy/Template Method/Factory están bien aplicados donde corresponda? ¿Algún patrón sería útil y falta?
- **Abstracciones**: ¿`I{Area}Service` coherentes y con responsabilidad única? ¿Dependencias abstraídas vía interfaz para testabilidad?
- **Arquitectura**: ¿respeta los namespaces `Ritrama2025.*` (Core sin UI, Services por área, Forms, Models, Helpers)? ¿Hay acoplamiento indebido entre capas (Core referenciando Forms/UI)?
- **DI**: ¿validación de null en contructores, lifetimes correctos (transient vs singleton), sin `new` de servicios donde debería haber inyección?
- **SOLID**: violaciones de SRP, Open/Closed, Liskov, segregación de interfaces o inversión de dependencias.
- **Async**: ¿async/await en I/O? En WinForms, ¿se usa `ConfigureAwait(false)` donde rompe el contexto UI? ¿`async void` fuera de handlers de eventos?
- **Rendimiento/dispose**: `using`/`IDisposable` correctos en conexiones SQL, streams, ReportViewer y recursos nativos TSCLIB; conexiones abiertas sin cerrar.
- **Seguridad**: consultas SQL parametrizadas (`Parameters.AddWithValue`), validación/sanitización de entradas, credenciales fuera del repo (User Secrets / env vars), excepciones que no filtren detalles sensibles al usuario.
- **Mantenibilidad**: manejo de errores consistente (`ServiceErrors`/`ServiceLogger`), configuración tipada y validada (DataAnnotations), sin duplicación.
- **Testabilidad**: código puro en Core testable con xUnit + FluentAssertions sin BD; uso de `DatabaseFixture`/`TestConfiguration` para integración; tests sin MessageBox (anular `ServiceErrors.Report`).
- **Documentación XML**: comentarios XML en API públicas (no en código generado `*.Designer.cs`), `param`/`returns` descritos.
- **Claridad y clean code**: nombres de dominio (despacho, producción, inventario, materia prima, etiquetas), métodos enfocados, complejidad mínima, sin duplicación.
- **Código generado**: no sugerir ediciones manuales a `*.Designer.cs`, `.xsd`/datasets o resx generados.

## Entregable

Reportar hallazgos enmarcados en la arquitectura real del proyecto y las mejores prácticas .NET (`net10.0`, C# 13+). Cada hallazgo debe ser específico: archivo/símbolo afectado, problema, impacto y recomendación concreta. Priorizar por severidad. No inventar patrones ni refactors que contradigan la estructura del proyecto.