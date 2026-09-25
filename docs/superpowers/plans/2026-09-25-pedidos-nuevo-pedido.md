# Plan: flujo de creación de pedido nuevo en FrmPedidos

> **Para trabajadores agénticos:** SUB-SKILL REQUERIDO: usa `superpowers:subagent-driven-development` (recomendado) o `superpowers:executing-plans` para implementar este plan tarea por tarea. Los pasos usan casillas (`- [ ]`) para dar seguimiento.

**Objetivo:** Que el usuario pueda crear un pedido nuevo completo (encabezado + líneas) desde `FrmPedidos`, escribiendo solo cuando entra explícitamente al modo Nuevo y volviendo a solo lectura al cancelar o guardar.

**Arquitectura:** La aritmética de importes, la validación de negocio y el mapeo del detalle salen del formulario a tres clases puras y testeables sin base de datos (`PedidoCalculos`, `PedidoValidador`, `PedidoDetalleMapper`). El formulario gana un modo explícito (`Consulta` / `Nuevo`) aplicado por un único método `AplicarModo`, y una lista `List<PedidoDetalle> _lineas` como fuente de verdad del detalle. El servicio valida antes de abrir la conexión.

**Stack técnico:** C# / .NET 10 (`net10.0-windows`), WinForms, SunnyUI 3.9.8, `Microsoft.Data.SqlClient` 7.0.2, xUnit 2.9.2 + FluentAssertions 8.2.0.

**Spec:** `docs/superpowers/specs/2026-09-25-pedidos-modos-nuevo-editar-design.md`

## Alcance de este plan

Cubre **solo la creación de un pedido nuevo**. Quedan fuera y serán planes separados:

- Editar un pedido existente (modo `Editar`, botón `btnEditar` y `ActualizarPedidoCompleto`).
- Anular un pedido desde la pantalla (botón `btnAnular`).
- El avance automático de estado por OC/Despacho (diferido por decisión del usuario: ningún código escribe `orden_corte.pedido_id` ni `despacho.pedido_id`).

Por esa razón `btnEditar` queda deshabilitado durante este plan y el enumerado de modo tiene solo dos valores.

## Restricciones globales

- La pestaña General ya quedó **solo lectura por defecto** en el diseñador. Ninguna tarea puede habilitar un control fuera de `AplicarModo`.
- **No hacer `git commit` ni `git push` sin autorización explícita del usuario.** El árbol de trabajo ya contiene modificaciones y archivos sin rastrear ajenos a este trabajo.
- **Las pruebas que escriben en la base de datos no se ejecutan sin confirmación del usuario.** Solo se ejecuta `dotnet test --filter "Categoria=Unit"`, que no abre conexiones.
- No modificar ni versionar `appsettings.Development.json` ni `Ritrama2025.Tests/appsettings.Test.json` (contienen credenciales).
- Los identificadores nuevos van sin tildes ni `ñ` (convención del repositorio: `numero`, `descripcion`, `anulado`). Los textos de interfaz y comentarios sí llevan tildes.
- Los mensajes de error para el usuario van en español.
- No inventar nombres de recursos de imagen: los disponibles relevantes son `add_file_32px`, `edit_24px`, `delete_row_24px`, `cancel_24px`.
- La clave de configuración de la conexión es `ConnectionStringsEnvironment:{Ambiente}` con `Ambiente` = `Desarrollo` (`Services/ProduccionService/ConexionResolver.cs:8`).
- `ServiceErrors.Report` y `ServiceErrors.Notify` son estáticos globales mutables y las colecciones de pruebas corren en paralelo (`xunit.runner.json`). Las pruebas unitarias nuevas no deben depender de su valor.

---

### Tarea 1: `PedidoCalculos` — aritmética de importes

**Archivos:**
- Crear: `Services/PedidoService/PedidoCalculos.cs`
- Crear: `Ritrama2025.Tests/PedidoCalculosTests.cs`

**Interfaces:**
- Produce: `public static class PedidoCalculos` con
  - `public static decimal TotalRenglon(decimal cant, decimal? precio)`
  - `public static (decimal SubTotal, decimal MontoItbis, decimal Total) Calcular(IEnumerable<PedidoDetalle> lineas, decimal porcItbis)`
- No consume nada de otras tareas.

- [x] **Paso 1: Escribir la prueba que falla**

Crear `Ritrama2025.Tests/PedidoCalculosTests.cs`:

```csharp
using FluentAssertions;
using Ritrama2025.Models;
using Ritrama2025.Services.PedidoService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Pruebas unitarias del calculo de importes. No abren conexiones a la base de datos.
/// </summary>
[Trait("Categoria", "Unit")]
public class PedidoCalculosTests
{
    [Fact]
    public void Calcular_DosLineas_SumaCantPorPrecio()
    {
        List<PedidoDetalle> lineas = new()
        {
            new PedidoDetalle { Cant = 2m, Precio = 50m },
            new PedidoDetalle { Cant = 3m, Precio = 25.50m }
        };

        (decimal subTotal, decimal montoItbis, decimal total) = PedidoCalculos.Calcular(lineas, 0m);

        subTotal.Should().Be(176.50m);
        montoItbis.Should().Be(0m);
        total.Should().Be(176.50m);
    }

    [Fact]
    public void Calcular_SinLineas_DevuelveCeros()
    {
        (decimal subTotal, decimal montoItbis, decimal total) =
            PedidoCalculos.Calcular(new List<PedidoDetalle>(), 18m);

        subTotal.Should().Be(0m);
        montoItbis.Should().Be(0m);
        total.Should().Be(0m);
    }

    [Fact]
    public void Calcular_AplicaItbisDe18PorCiento()
    {
        List<PedidoDetalle> lineas = new() { new PedidoDetalle { Cant = 1m, Precio = 100m } };

        (decimal subTotal, decimal montoItbis, decimal total) = PedidoCalculos.Calcular(lineas, 18m);

        subTotal.Should().Be(100m);
        montoItbis.Should().Be(18m);
        total.Should().Be(118m);
    }

    [Fact]
    public void Calcular_RespetaPorcentajeItbisEditado()
    {
        List<PedidoDetalle> lineas = new() { new PedidoDetalle { Cant = 1m, Precio = 200m } };

        (decimal subTotal, decimal montoItbis, decimal total) = PedidoCalculos.Calcular(lineas, 10m);

        montoItbis.Should().Be(20m);
        total.Should().Be(220m);
    }

    [Fact]
    public void Calcular_RedondeaASoloDosDecimales()
    {
        List<PedidoDetalle> lineas = new() { new PedidoDetalle { Cant = 1m, Precio = 0.125m } };

        var (subTotal, _, _) = PedidoCalculos.Calcular(lineas, 0m);

        subTotal.Should().Be(0.13m);
    }

    [Fact]
    public void Calcular_LineaSinPrecio_CuentaComoCero()
    {
        List<PedidoDetalle> lineas = new() { new PedidoDetalle { Cant = 4m, Precio = null } };

        var (subTotal, _, _) = PedidoCalculos.Calcular(lineas, 18m);

        subTotal.Should().Be(0m);
    }

    [Fact]
    public void TotalRenglon_MultiplicaCantidadPorPrecio()
    {
        PedidoCalculos.TotalRenglon(3m, 12.50m).Should().Be(37.50m);
    }

    [Fact]
    public void TotalRenglon_PrecioNuloDevuelveCero()
    {
        PedidoCalculos.TotalRenglon(3m, null).Should().Be(0m);
    }
}
```

- [x] **Paso 2: Ejecutar y verificar que falla**

```bash
dotnet test Ritrama2025.Tests/Ritrama2025.Tests.csproj --no-restore --filter "FullyQualifiedName~PedidoCalculosTests"
```

Esperado: error de compilación `CS0103: El nombre 'PedidoCalculos' no existe en el contexto actual`.

> Nota: al escribir pruebas que descartan parte de la tupla, usar `var (subTotal, _, _) = ...`.
> `(decimal subTotal, _, _)` no compila: C# no permite mezclar tipos explícitos con
> descartes en una desestructuración (CS8130 / CS8183).

- [x] **Paso 3: Implementación mínima**

Crear `Services/PedidoService/PedidoCalculos.cs`:

```csharp
using Ritrama2025.Models;

namespace Ritrama2025.Services.PedidoService
{
    /// <summary>
    /// Calculo de los importes de un pedido. Logica pura: no toca la base de datos ni la
    /// interfaz, de modo que la pantalla y las pruebas comparten una sola regla de redondeo.
    /// </summary>
    public static class PedidoCalculos
    {
        /// <summary>
        /// Importe de una linea del detalle. Un precio nulo equivale a cero.
        /// </summary>
        public static decimal TotalRenglon(decimal cant, decimal? precio)
        {
            return Math.Round(cant * (precio ?? 0m), 2, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Subtotal, ITBIS y total del pedido a partir de sus lineas y el porcentaje de ITBIS.
        /// </summary>
        public static (decimal SubTotal, decimal MontoItbis, decimal Total) Calcular(
            IEnumerable<PedidoDetalle> lineas,
            decimal porcItbis)
        {
            decimal acumulado = 0m;
            if (lineas != null)
            {
                foreach (PedidoDetalle linea in lineas)
                {
                    acumulado += linea.Cant * (linea.Precio ?? 0m);
                }
            }

            decimal subTotal = Math.Round(acumulado, 2, MidpointRounding.AwayFromZero);
            decimal montoItbis = Math.Round(subTotal * porcItbis / 100m, 2, MidpointRounding.AwayFromZero);
            return (subTotal, montoItbis, subTotal + montoItbis);
        }
    }
}
```

- [x] **Paso 4: Ejecutar y verificar que pasa**

```bash
dotnet test Ritrama2025.Tests/Ritrama2025.Tests.csproj --no-restore --filter "FullyQualifiedName~PedidoCalculosTests"
```

Esperado: `8 pruebas correctas`.

- [ ] **Paso 5: Commit (solo con autorización del usuario)**

```bash
git add Services/PedidoService/PedidoCalculos.cs Ritrama2025.Tests/PedidoCalculosTests.cs
git commit -m "feat(pedidos): calculo de importes con pruebas unitarias"
```

---

### Tarea 2: `PedidoValidador` — reglas de negocio del encabezado y las líneas

**Archivos:**
- Crear: `Services/PedidoService/PedidoValidador.cs`
- Crear: `Ritrama2025.Tests/PedidoValidadorTests.cs`

**Interfaces:**
- Consume: `Ritrama2025.Models.Pedido`, `Ritrama2025.Models.PedidoDetalle`, `Ritrama2025.Models.PedidoEstado`
- Produce: `public static class PedidoValidador` con
  - `public static bool EsValido(Pedido? pedido, out string error)`
  - `public static bool EsEstadoValido(string? estado)`

- [x] **Paso 1: Escribir las pruebas que fallan**

Crear `Ritrama2025.Tests/PedidoValidadorTests.cs`:

```csharp
using FluentAssertions;
using Ritrama2025.Models;
using Ritrama2025.Services.PedidoService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Pruebas unitarias de las reglas de negocio del pedido. No abren conexiones.
/// </summary>
[Trait("Categoria", "Unit")]
public class PedidoValidadorTests
{
    private static Pedido PedidoValido()
    {
        return new Pedido
        {
            Numero = 1001,
            Fecha = new DateTime(2026, 9, 25),
            Customer_Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Estado = PedidoEstado.Creado,
            SubTotal = 100m,
            Porc_Itbis = 18m,
            Monto_Itbis = 18m,
            Total = 118m,
            Detalle =
            {
                new PedidoDetalle
                {
                    Product_id = "P001",
                    Product_name = "Producto Test",
                    Cant = 1m,
                    Precio = 100m,
                    Total_Renglon = 100m
                }
            }
        };
    }

    [Fact]
    public void EsValido_PedidoCompleto_EsValido()
    {
        PedidoValidador.EsValido(PedidoValido(), out string error).Should().BeTrue();
        error.Should().BeEmpty();
    }

    [Fact]
    public void EsValido_NumeroCero_Rechazado()
    {
        Pedido pedido = PedidoValido();
        pedido.Numero = 0;

        PedidoValidador.EsValido(pedido, out string error).Should().BeFalse();
        error.Should().Contain("numero");
    }

    [Fact]
    public void EsValido_SinCliente_Rechazado()
    {
        Pedido pedido = PedidoValido();
        pedido.Customer_Id = Guid.Empty;

        PedidoValidador.EsValido(pedido, out string error).Should().BeFalse();
        error.Should().Contain("cliente");
    }

    [Fact]
    public void EsValido_SinLineas_Rechazado()
    {
        Pedido pedido = PedidoValido();
        pedido.Detalle.Clear();

        PedidoValidador.EsValido(pedido, out string error).Should().BeFalse();
        error.Should().Contain("linea");
    }

    [Fact]
    public void EsValido_LineaSinProducto_Rechazado()
    {
        Pedido pedido = PedidoValido();
        pedido.Detalle[0].Product_id = "  ";

        PedidoValidador.EsValido(pedido, out string error).Should().BeFalse();
        error.Should().Contain("producto");
    }

    [Fact]
    public void EsValido_CantidadCero_Rechazado()
    {
        Pedido pedido = PedidoValido();
        pedido.Detalle[0].Cant = 0m;

        PedidoValidador.EsValido(pedido, out string error).Should().BeFalse();
        error.Should().Contain("cantidad");
    }

    [Fact]
    public void EsValido_PrecioNegativo_Rechazado()
    {
        Pedido pedido = PedidoValido();
        pedido.Detalle[0].Precio = -1m;

        PedidoValidador.EsValido(pedido, out string error).Should().BeFalse();
        error.Should().Contain("precio");
    }

    [Fact]
    public void EsValido_PorcItbisNegativo_Rechazado()
    {
        Pedido pedido = PedidoValido();
        pedido.Porc_Itbis = -1m;

        PedidoValidador.EsValido(pedido, out string error).Should().BeFalse();
        error.Should().Contain("ITBIS");
    }

    [Fact]
    public void EsValido_EstadoDesconocido_Rechazado()
    {
        Pedido pedido = PedidoValido();
        pedido.Estado = "inventado";

        PedidoValidador.EsValido(pedido, out string error).Should().BeFalse();
        error.Should().Contain("estado");
    }

    [Fact]
    public void EsValido_EstadoVacio_AceptadoYSeCompletaAlGuardar()
    {
        Pedido pedido = PedidoValido();
        pedido.Estado = null;

        PedidoValidador.EsValido(pedido, out string error).Should().BeTrue();
        error.Should().BeEmpty();
    }

    [Theory]
    [InlineData("creado")]
    [InlineData("en producción")]
    [InlineData("pickeado")]
    [InlineData("despachado")]
    [InlineData("devuelto")]
    public void EsEstadoValido_EstadosConocidos_Aceptados(string estado)
    {
        PedidoValidador.EsEstadoValido(estado).Should().BeTrue();
    }

    [Fact]
    public void EsEstadoValido_Desconocido_Rechazado()
    {
        PedidoValidador.EsEstadoValido("inventado").Should().BeFalse();
        PedidoValidador.EsEstadoValido("").Should().BeFalse();
    }
}
```

- [x] **Paso 2: Ejecutar y verificar que falla**

```bash
dotnet test Ritrama2025.Tests/Ritrama2025.Tests.csproj --no-restore --filter "FullyQualifiedName~PedidoValidadorTests"
```

Esperado: `CS0246` sobre `PedidoValidador`.

- [x] **Paso 3: Implementación mínima**

Crear `Services/PedidoService/PedidoValidador.cs`:

```csharp
using Ritrama2025.Models;

namespace Ritrama2025.Services.PedidoService
{
    /// <summary>
    /// Reglas de negocio del encabezado y las lineas de un pedido. Logica pura: el mismo
    /// validador corre en la pantalla antes de guardar y en el servicio como red de seguridad.
    /// </summary>
    public static class PedidoValidador
    {
        /// <summary>
        /// Indica si el pedido puede guardarse y, cuando no, devuelve el motivo en español.
        /// </summary>
        public static bool EsValido(Pedido? pedido, out string error)
        {
            error = string.Empty;

            if (pedido == null)
            {
                error = "El pedido es nulo.";
                return false;
            }

            if (pedido.Numero <= 0)
            {
                error = "El numero de pedido debe ser mayor que cero.";
                return false;
            }

            if (pedido.Customer_Id == Guid.Empty)
            {
                error = "Debe seleccionar un cliente.";
                return false;
            }

            if (pedido.Porc_Itbis < 0m)
            {
                error = "El porcentaje de ITBIS no puede ser negativo.";
                return false;
            }

            if (pedido.SubTotal < 0m || pedido.Monto_Itbis < 0m || pedido.Total < 0m)
            {
                error = "Los importes del pedido no pueden ser negativos.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(pedido.Estado) && !EsEstadoValido(pedido.Estado))
            {
                error = $"El estado '{pedido.Estado}' no es valido.";
                return false;
            }

            if (pedido.Detalle == null || pedido.Detalle.Count == 0)
            {
                error = "Debe agregar al menos una linea de producto.";
                return false;
            }

            for (int i = 0; i < pedido.Detalle.Count; i++)
            {
                PedidoDetalle linea = pedido.Detalle[i];
                int numeroLinea = i + 1;

                if (string.IsNullOrWhiteSpace(linea.Product_id))
                {
                    error = $"La linea {numeroLinea} no tiene producto asignado.";
                    return false;
                }

                if (linea.Cant <= 0m)
                {
                    error = $"La cantidad de la linea {numeroLinea} debe ser mayor que cero.";
                    return false;
                }

                if (linea.Precio.HasValue && linea.Precio.Value < 0m)
                {
                    error = $"El precio de la linea {numeroLinea} no puede ser negativo.";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Indica si el estado corresponde a uno de los estados validos del pedido.
        /// </summary>
        public static bool EsEstadoValido(string? estado)
        {
            string normalizado = (estado ?? string.Empty).Trim().ToLowerInvariant();
            return normalizado == PedidoEstado.Creado
                || normalizado == PedidoEstado.EnProduccion
                || normalizado == PedidoEstado.Pickeado
                || normalizado == PedidoEstado.Despachado
                || normalizado == PedidoEstado.Devuelto;
        }
    }
}
```

- [x] **Paso 4: Ejecutar y verificar que pasa**

```bash
dotnet test Ritrama2025.Tests/Ritrama2025.Tests.csproj --no-restore --filter "FullyQualifiedName~PedidoValidadorTests"
```

Esperado: `16 pruebas correctas` (10 `[Fact]` de `EsValido` + 5 casos de la `[Theory]` + 1 `[Fact]` de `EsEstadoValido`).

- [ ] **Paso 5: Commit (solo con autorización del usuario)**

```bash
git add Services/PedidoService/PedidoValidador.cs Ritrama2025.Tests/PedidoValidadorTests.cs
git commit -m "feat(pedidos): reglas de validacion del pedido con pruebas unitarias"
```

---

### Tarea 3: `PedidoDetalleMapper` — conserva el `product_id` que hoy se pierde

**Archivos:**
- Crear: `Services/PedidoService/PedidoDetalleMapper.cs`
- Crear: `Ritrama2025.Tests/PedidoDetalleMapperTests.cs`

**Contexto:** `LlenarGridDetalle` (`Forms/FrmPedidos.cs:267-283`) proyecta la fila a celdas del grid y descarta `product_id`, que es `NOT NULL` en `pedido_detalle` (`Scripts/Setup_Pedidos.sql`). Guardar esas líneas falla por violación de la columna.

**Interfaces:**
- Consume: `System.Data.DataTable` con las columnas de `R.QUERY.COMMERCIAL.SQL_SELECT_PEDIDO_DETALLE` (`R.cs:47`)
- Produce: `public static class PedidoDetalleMapper` con `public static List<PedidoDetalle> Mapear(DataTable tabla)`

- [x] **Paso 1: Escribir las pruebas que fallan**

Crear `Ritrama2025.Tests/PedidoDetalleMapperTests.cs`:

```csharp
using System.Data;
using FluentAssertions;
using Ritrama2025.Models;
using Ritrama2025.Services.PedidoService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Pruebas unitarias del mapeo del detalle. No abren conexiones: la tabla se arma en memoria.
/// </summary>
[Trait("Categoria", "Unit")]
public class PedidoDetalleMapperTests
{
    private static DataTable TablaDetalle()
    {
        DataTable tabla = new();
        tabla.Columns.Add("numero", typeof(int));
        tabla.Columns.Add("product_id", typeof(string));
        tabla.Columns.Add("product_name", typeof(string));
        tabla.Columns.Add("cant", typeof(decimal));
        tabla.Columns.Add("unidad", typeof(string));
        tabla.Columns.Add("width", typeof(decimal));
        tabla.Columns.Add("lenght", typeof(decimal));
        tabla.Columns.Add("msi", typeof(decimal));
        tabla.Columns.Add("precio", typeof(decimal));
        tabla.Columns.Add("total_renglon", typeof(decimal));
        tabla.Columns.Add("notas", typeof(string));
        return tabla;
    }

    [Fact]
    public void Mapear_ConservaElProductId()
    {
        DataTable tabla = TablaDetalle();
        tabla.Rows.Add(1001, "P-0001", "Rollo 60", 3m, "Rollo", 60m, 1000m, 1m, 25.50m, 76.50m, "corte especial");

        List<PedidoDetalle> lineas = PedidoDetalleMapper.Mapear(tabla);

        lineas.Should().HaveCount(1);
        lineas[0].Product_id.Should().Be("P-0001");
        lineas[0].Product_name.Should().Be("Rollo 60");
        lineas[0].Cant.Should().Be(3m);
        lineas[0].Unidad.Should().Be("Rollo");
        lineas[0].Precio.Should().Be(25.50m);
        lineas[0].Total_Renglon.Should().Be(76.50m);
        lineas[0].Notas.Should().Be("corte especial");
    }

    [Fact]
    public void Mapear_ValoresDBNull_LosConvierteEnCeroONulo()
    {
        DataTable tabla = TablaDetalle();
        tabla.Rows.Add(1001, "P-0001", "Rollo 60", 1m, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value);

        List<PedidoDetalle> lineas = PedidoDetalleMapper.Mapear(tabla);

        lineas[0].Unidad.Should().BeNull();
        lineas[0].Width.Should().Be(0m);
        lineas[0].Precio.Should().BeNull();
        lineas[0].Total_Renglon.Should().BeNull();
    }

    [Fact]
    public void Mapear_TablaVacia_DevuelveListaVacia()
    {
        PedidoDetalleMapper.Mapear(TablaDetalle()).Should().BeEmpty();
    }

    [Fact]
    public void Mapear_TablaNula_DevuelveListaVacia()
    {
        PedidoDetalleMapper.Mapear(null!).Should().BeEmpty();
    }
}
```

- [x] **Paso 2: Ejecutar y verificar que falla**

```bash
dotnet test Ritrama2025.Tests/Ritrama2025.Tests.csproj --no-restore --filter "FullyQualifiedName~PedidoDetalleMapperTests"
```

Esperado: `CS0246` sobre `PedidoDetalleMapper`.

- [x] **Paso 3: Implementación mínima**

Crear `Services/PedidoService/PedidoDetalleMapper.cs`:

```csharp
using System.Data;
using Ritrama2025.Models;

namespace Ritrama2025.Services.PedidoService
{
    /// <summary>
    /// Convierte el resultado de SQL_SELECT_PEDIDO_DETALLE en lineas de negocio. Conserva el
    /// product_id, que es NOT NULL en la base y se perdia al proyectar la fila al grid.
    /// </summary>
    public static class PedidoDetalleMapper
    {
        public static List<PedidoDetalle> Mapear(DataTable tabla)
        {
            List<PedidoDetalle> lineas = new();
            if (tabla == null)
            {
                return lineas;
            }

            foreach (DataRow row in tabla.Rows)
            {
                lineas.Add(new PedidoDetalle
                {
                    Product_id = Texto(row, "product_id"),
                    Product_name = Texto(row, "product_name"),
                    Cant = Decimal(row, "cant"),
                    Unidad = Texto(row, "unidad"),
                    Width = Decimal(row, "width"),
                    Lenght = Decimal(row, "lenght"),
                    Msi = Decimal(row, "msi"),
                    Precio = DecimalNulo(row, "precio"),
                    Total_Renglon = DecimalNulo(row, "total_renglon"),
                    Notas = Texto(row, "notas")
                });
            }

            return lineas;
        }

        private static string? Texto(DataRow row, string columna)
        {
            return TieneColumna(row, columna) ? row[columna]?.ToString() : null;
        }

        private static decimal Decimal(DataRow row, string columna)
        {
            return DecimalNulo(row, columna) ?? 0m;
        }

        private static decimal? DecimalNulo(DataRow row, string columna)
        {
            if (!TieneColumna(row, columna))
            {
                return null;
            }

            object? valor = row[columna];
            if (valor == null || valor == DBNull.Value)
            {
                return null;
            }

            return decimal.TryParse(valor.ToString(), out decimal monto) ? monto : null;
        }

        private static bool TieneColumna(DataRow row, string columna)
        {
            return row.Table.Columns.Contains(columna);
        }
    }
}
```

- [x] **Paso 4: Ejecutar y verificar que pasa**

```bash
dotnet test Ritrama2025.Tests/Ritrama2025.Tests.csproj --no-restore --filter "FullyQualifiedName~PedidoDetalleMapperTests"
```

Esperado: `4 pruebas correctas`.

- [ ] **Paso 5: Commit (solo con autorización del usuario)**

```bash
git add Services/PedidoService/PedidoDetalleMapper.cs Ritrama2025.Tests/PedidoDetalleMapperTests.cs
git commit -m "fix(pedidos): mapeo del detalle que conserva el product_id"
```

---

### Tarea 4: `SavePedidoCompleto` valida antes de abrir la conexión

**Archivos:**
- Modificar: `Services/PedidoService/PedidoService.cs:142-202`
- Crear: `Ritrama2025.Tests/PedidoServiceValidacionTests.cs`

**Contexto:** hoy `SavePedidoCompleto` abre la conexión y escribe sin validar nada, y traga el rollback con `catch { }` vacío (`PedidoService.cs:197`). Un `product_id` nulo revienta la transacción por `NOT NULL` en vez de devolver un mensaje claro.

**Interfaces:**
- Consume: `PedidoValidador.EsValido(Pedido, out string)` de la Tarea 2
- Produce: `SavePedidoCompleto(Pedido)` conserva su firma; ahora devuelve `false` y llena `ErrorMsg` con el motivo cuando el pedido es inválido, sin abrir conexión.

- [x] **Paso 1: Escribir las pruebas que fallan**

Crear `Ritrama2025.Tests/PedidoServiceValidacionTests.cs`. El servicio se construye con una cadena de conexión inalcanzable a propósito: si el método valida antes de abrir, devuelve `false` de inmediato y la prueba no toca ninguna base de datos.

```csharp
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Models;
using Ritrama2025.Services.PedidoService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Pruebas del camino de rechazo de SavePedidoCompleto. El servicio se apunta a un servidor
/// inexistente: si el metodo devuelve false es porque valido antes de abrir la conexion,
/// asi que estas pruebas nunca tocan una base de datos real.
/// </summary>
[Trait("Categoria", "Unit")]
public class PedidoServiceValidacionTests
{
    private const string ConexionInexistente =
        "Server=localhost;Database=Ritrama_NoExiste_Pruebas;User Id=sa;Password=NoAbre;TrustServerCertificate=True;Connect Timeout=2";

    private static PedidoService CrearServicio()
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Ambiente"] = "Desarrollo",
                ["ConnectionStringsEnvironment:Desarrollo"] = ConexionInexistente
            })
            .Build();

        return new PedidoService(config);
    }

    private static Pedido PedidoCon(PedidoDetalle? linea)
    {
        Pedido pedido = new()
        {
            Numero = 777001,
            Fecha = new DateTime(2026, 9, 25),
            Customer_Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Estado = PedidoEstado.Creado
        };

        if (linea != null)
        {
            pedido.Detalle.Add(linea);
        }

        return pedido;
    }

    private static PedidoDetalle LineaValida()
    {
        return new PedidoDetalle { Product_id = "P001", Cant = 1m, Precio = 10m };
    }

    [Fact]
    public void SavePedidoCompleto_SinCliente_DevuelveFalseConMotivo()
    {
        PedidoService service = CrearServicio();
        Pedido pedido = PedidoCon(LineaValida());
        pedido.Customer_Id = Guid.Empty;

        bool resultado = service.SavePedidoCompleto(pedido);

        resultado.Should().BeFalse();
        service.ErrorMsg.Should().Contain("cliente");
    }

    [Fact]
    public void SavePedidoCompleto_SinLineas_DevuelveFalseConMotivo()
    {
        PedidoService service = CrearServicio();

        bool resultado = service.SavePedidoCompleto(PedidoCon(null));

        resultado.Should().BeFalse();
        service.ErrorMsg.Should().Contain("linea");
    }

    [Fact]
    public void SavePedidoCompleto_LineaSinProducto_DevuelveFalseConMotivo()
    {
        PedidoService service = CrearServicio();
        PedidoDetalle linea = LineaValida();
        linea.Product_id = null;

        bool resultado = service.SavePedidoCompleto(PedidoCon(linea));

        resultado.Should().BeFalse();
        service.ErrorMsg.Should().Contain("producto");
    }

    [Fact]
    public void SavePedidoCompleto_CantidadCero_DevuelveFalseConMotivo()
    {
        PedidoService service = CrearServicio();
        PedidoDetalle linea = LineaValida();
        linea.Cant = 0m;

        bool resultado = service.SavePedidoCompleto(PedidoCon(linea));

        resultado.Should().BeFalse();
        service.ErrorMsg.Should().Contain("cantidad");
    }
}
```

- [x] **Paso 2: Ejecutar y verificar que falla**

```bash
dotnet test Ritrama2025.Tests/Ritrama2025.Tests.csproj --no-restore --filter "FullyQualifiedName~PedidoServiceValidacionTests"
```

Esperado: las pruebas **no pasan**. El código actual llama `conn.Open()` **fuera** del `try` (`PedidoService.cs:144-146`), así que la conexión inalcanzable lanza `SqlException` y la prueba revienta con esa excepción en vez de un mensaje de negocio. Ese es el RED esperado: con el paso 3, ninguna de las cuatro pruebas abre conexión. Si alguna devuelve `true` o tarda más de 2 segundos, se está abriendo la conexión: la implementación del paso 3 es obligatoria.

- [x] **Paso 3: Implementación mínima**

En `Services/PedidoService/PedidoService.cs`, reemplazar el método `SavePedidoCompleto` completo (líneas 142 a 202) por:

```csharp
        public bool SavePedidoCompleto(Pedido pedido)
        {
            if (!PedidoValidador.EsValido(pedido, out string error))
            {
                // No se reporta por ServiceErrors: es un resultado esperado y la pantalla
                // ya muestra el motivo al usuario.
                ErrorMsg = error;
                return false;
            }

            using SqlConnection conn = new SqlConnection(_conn);
            SqlTransaction? tran = null;
            try
            {
                // Abrir primero y recien después iniciar la transacción: al revés, SqlConnection
                // lanza InvalidOperationException porque no hay conexión abierta.
                conn.Open();
                tran = conn.BeginTransaction();

                using (SqlCommand cmd = new SqlCommand(
                    R.QUERY.COMMERCIAL.SQL_INSERT_PEDIDO,
                    conn, tran))
                {
                    cmd.Parameters.Add(new SqlParameter("@p1", pedido.Numero));
                    cmd.Parameters.Add(new SqlParameter("@p2", pedido.Fecha));
                    cmd.Parameters.Add(new SqlParameter("@p3", pedido.Customer_Id) { SqlDbType = SqlDbType.UniqueIdentifier });
                    cmd.Parameters.Add(new SqlParameter("@p4", (object?)pedido.Customer_Name ?? DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@p5", pedido.Vendor_Id.HasValue ? pedido.Vendor_Id.Value : (object)DBNull.Value) { SqlDbType = SqlDbType.UniqueIdentifier });
                    cmd.Parameters.Add(new SqlParameter("@p6", (object?)pedido.Persona_Contacto ?? DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@p7", (object?)pedido.Tipo_venta ?? DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@p8", pedido.Fecha_entrega.HasValue ? pedido.Fecha_entrega.Value : (object)DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@p9", (object?)pedido.Condiciones_pago ?? DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@p10", (object?)pedido.Direccion_entrega ?? DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@p11", string.IsNullOrEmpty(pedido.Estado) ? PedidoEstado.Creado : pedido.Estado));
                    cmd.Parameters.Add(new SqlParameter("@p12", (object?)pedido.Notas ?? DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@p13", pedido.Anulado));
                    cmd.Parameters.Add(new SqlParameter("@p14", pedido.SubTotal));
                    cmd.Parameters.Add(new SqlParameter("@p15", pedido.Porc_Itbis));
                    cmd.Parameters.Add(new SqlParameter("@p16", pedido.Monto_Itbis));
                    cmd.Parameters.Add(new SqlParameter("@p17", pedido.Total));
                    cmd.ExecuteNonQuery();
                }

                foreach (PedidoDetalle det in pedido.Detalle)
                {
                    using SqlCommand cmd = new SqlCommand(
                        R.QUERY.COMMERCIAL.SQL_INSERT_PEDIDO_DETALLE,
                        conn, tran);
                    cmd.Parameters.Add(new SqlParameter("@p1", pedido.Numero));
                    cmd.Parameters.Add(new SqlParameter("@p2", (object?)det.Product_id ?? DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@p3", (object?)det.Product_name ?? DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@p4", det.Cant));
                    cmd.Parameters.Add(new SqlParameter("@p5", (object?)det.Unidad ?? DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@p6", det.Width));
                    cmd.Parameters.Add(new SqlParameter("@p7", det.Lenght));
                    cmd.Parameters.Add(new SqlParameter("@p8", det.Msi));
                    cmd.Parameters.Add(new SqlParameter("@p9", det.Precio.HasValue ? det.Precio.Value : (object)DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@p10", det.Total_Renglon.HasValue ? det.Total_Renglon.Value : (object)DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@p11", (object?)det.Notas ?? DBNull.Value));
                    cmd.ExecuteNonQuery();
                }

                tran.Commit();
                return true;
            }
            catch (Exception ex)
            {
                if (tran != null)
                {
                    try
                    {
                        tran.Rollback();
                    }
                    catch (Exception rollbackEx)
                    {
                        ServiceLogger.Log("No se pudo revertir la transaccion del pedido " + pedido.Numero + ": " + rollbackEx.Message);
                    }
                }

                ErrorMsg = ex.Message;
                ServiceErrors.Report("Error al grabar el pedido: " + ex.Message);
                return false;
            }
            finally
            {
                tran?.Dispose();
            }
        }
```

- [x] **Paso 4: Ejecutar y verificar que pasa**

```bash
dotnet test Ritrama2025.Tests/Ritrama2025.Tests.csproj --no-restore --filter "FullyQualifiedName~PedidoServiceValidacionTests"
```

Esperado: `4 pruebas correctas` y ninguna conexión abierta (la suite completa tarda lo mismo que antes).

- [x] **Paso 5: Compilar la solución completa**

```bash
dotnet build Ritrama2025.sln --no-restore --verbosity minimal
```

Esperado: `0 Errores`. Las 5 advertencias de nulabilidad preexistentes en `Forms/FrmPedidos.cs` siguen ahí y no son nuevas.

- [ ] **Paso 6: Commit (solo con autorización del usuario)**

```bash
git add Services/PedidoService/PedidoService.cs Ritrama2025.Tests/PedidoServiceValidacionTests.cs
git commit -m "feat(pedidos): valida el pedido antes de abrir la conexion"
```

---

### Tarea 5: Endurecer los números de pedido de las pruebas de integración existentes

**Archivos:**
- Modificar: `Ritrama2025.Tests/PedidoServiceTests.cs:178-206` y `:215-243`

**Contexto:** `AnularPedido_MarcaAnulado` usa el número fijo `950000` y `ActualizarEstadoPedido_TransicionesValidasCambiaEstado` usa `950001`, y ambos hacen `DELETE FROM pedido WHERE numero = @p1` en el `finally`. Si esos números existen en la base de desarrollo, las pruebas borran datos reales.

**Interfaces:**
- Consume: `IPedidoService.GetNewNumeroPedido()` y `_fixture.ExecuteNonQuery`
- Produce: ninguno; es una corrección de seguridad de las pruebas.

- [x] **Paso 1: Cambiar el número fijo por uno generado**

En `AnularPedido_MarcaAnulado`, reemplazar el número fijo y restaurar el consecutivo. El método pasa a ser `async Task`:

```csharp
    [SkippableFact]
    public async Task AnularPedido_MarcaAnulado()
    {
        IPedidoService service = CrearServicio();

        object? customerIdObj = _fixture.ExecuteScalar("SELECT TOP 1 customer_id FROM customer");
        Skip.If(customerIdObj == null, "no hay clientes; validado en prueba manual");
        Guid customerId = Guid.Parse(customerIdObj.ToString()!);

        int numero = await service.GetNewNumeroPedido();
        try
        {
            _fixture.ExecuteNonQuery(
                "INSERT INTO pedido (numero,fecha,customer_id,customer_name,estado) VALUES (@p1,@p2,@p3,@p4,@p5)",
                ("@p1", numero),
                ("@p2", DateTime.Now),
                ("@p3", customerId),
                ("@p4", "Cliente Test"),
                ("@p5", "creado"));

            service.AnularPedido(numero).Should().BeTrue();
            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT anulado FROM pedido WHERE numero = @p1", ("@p1", numero))!).Should().Be(1);
        }
        finally
        {
            LimpiarPedido(numero);
            _fixture.ExecuteNonQuery("UPDATE control SET par1 = par1 - 1 WHERE filter='PED'");
        }
    }
```

- [x] **Paso 2: Repetir el cambio en la prueba de estado**

En `ActualizarEstadoPedido_TransicionesValidasCambiaEstado`, aplicar el mismo criterio: `int numero = await service.GetNewNumeroPedido();` y añadir `_fixture.ExecuteNonQuery("UPDATE control SET par1 = par1 - 1 WHERE filter='PED'");` dentro del `finally`, después de `LimpiarPedido(numero);`. El método también pasa a `async Task` y su firma queda `public async Task ActualizarEstadoPedido_TransicionesValidasCambiaEstado()`.

- [x] **Paso 3: Confirmar que no quedan números fijos**

```bash
Select-String -Path Ritrama2025.Tests/PedidoServiceTests.cs -Pattern "950000|950001"
```

Esperado: sin resultados.

- [x] **Paso 4: Compilar sin ejecutar las pruebas de integración**

```bash
dotnet build Ritrama2025.sln --no-restore --verbosity minimal
```

Esperado: `0 Errores`. **No** ejecutar `dotnet test` sin filtro: estas pruebas siguen escribiendo en la base configurada y solo se corren con confirmación del usuario.

- [ ] **Paso 5: Commit (solo con autorización del usuario)**

```bash
git add Ritrama2025.Tests/PedidoServiceTests.cs
git commit -m "test(pedidos): numeros de pedido generados en vez de fijos"
```

---

### Tarea 6: Controles nuevos en el diseñador

**Archivos:**
- Modificar: `Forms/FrmPedidos.Designer.cs`

**Interfaces:**
- Produce en el formulario: `uiTextBox7` (% ITBIS), `uiTextBox8` (precio de línea), `uiTextBox9` (notas de línea), `uiLabel13`, `uiLabel14`, `btnGuardar`, `btnCancelar`
- Todos se declaran con `ReadOnly = true` / deshabilitados, igual que los existentes.

- [x] **Paso 1: Declarar los controles nuevos**

En el bloque de declaraciones privadas (junto a `uiTextBox6`, alrededor de la línea 909 del archivo), agregar:

```csharp
        private Sunny.UI.UITextBox uiTextBox7;
        private Sunny.UI.UITextBox uiTextBox8;
        private Sunny.UI.UITextBox uiTextBox9;
        private Sunny.UI.UILabel uiLabel13;
        private Sunny.UI.UILabel uiLabel14;
        private System.Windows.Forms.ToolStripButton btnGuardar;
        private System.Windows.Forms.ToolStripButton btnCancelar;
```

Y en el bloque `InitializeComponent`, junto a las instanciaciones existentes (después de `this.uiTextBox6 = new Sunny.UI.UITextBox();`):

```csharp
            this.uiTextBox7 = new Sunny.UI.UITextBox();
            this.uiTextBox8 = new Sunny.UI.UITextBox();
            this.uiTextBox9 = new Sunny.UI.UITextBox();
            this.uiLabel13 = new Sunny.UI.UILabel();
            this.uiLabel14 = new Sunny.UI.UILabel();
            this.btnGuardar = new ToolStripButton();
            this.btnCancelar = new ToolStripButton();
```

- [x] **Paso 2: Inicializar los controles nuevos**

Agregar el bloque de propiedades, respetando el estilo del archivo (propiedades en orden alfabético, `ReadOnly = true` como default):

```csharp
            // 
            // uiLabel13
            // 
            uiLabel13.Font = new Font("Microsoft Sans Serif", 12F);
            uiLabel13.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel13.Location = new Point(762, 511);
            uiLabel13.Name = "uiLabel13";
            uiLabel13.Size = new Size(80, 23);
            uiLabel13.TabIndex = 23;
            uiLabel13.Text = "Itbis % :";
            // 
            // uiLabel14
            // 
            uiLabel14.Font = new Font("Microsoft Sans Serif", 12F);
            uiLabel14.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel14.Location = new Point(655, 276);
            uiLabel14.Name = "uiLabel14";
            uiLabel14.Size = new Size(70, 23);
            uiLabel14.TabIndex = 24;
            uiLabel14.Text = "Price :";
            // 
            // uiTextBox7
            // 
            uiTextBox7.Font = new Font("Microsoft Sans Serif", 12F);
            uiTextBox7.Location = new Point(847, 505);
            uiTextBox7.Margin = new Padding(4, 5, 4, 5);
            uiTextBox7.MinimumSize = new Size(1, 16);
            uiTextBox7.Name = "uiTextBox7";
            uiTextBox7.Padding = new Padding(5);
            uiTextBox7.ReadOnly = true;
            uiTextBox7.ShowText = false;
            uiTextBox7.Size = new Size(58, 29);
            uiTextBox7.TabIndex = 25;
            uiTextBox7.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox7.Watermark = "";
            // 
            // uiTextBox8
            // 
            uiTextBox8.Font = new Font("Microsoft Sans Serif", 12F);
            uiTextBox8.Location = new Point(730, 270);
            uiTextBox8.Margin = new Padding(4, 5, 4, 5);
            uiTextBox8.MinimumSize = new Size(1, 16);
            uiTextBox8.Name = "uiTextBox8";
            uiTextBox8.Padding = new Padding(5);
            uiTextBox8.ReadOnly = true;
            uiTextBox8.ShowText = false;
            uiTextBox8.Size = new Size(85, 29);
            uiTextBox8.TabIndex = 26;
            uiTextBox8.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox8.Watermark = "Precio";
            // 
            // uiTextBox9
            // 
            uiTextBox9.Font = new Font("Microsoft Sans Serif", 12F);
            uiTextBox9.Location = new Point(823, 270);
            uiTextBox9.Margin = new Padding(4, 5, 4, 5);
            uiTextBox9.MinimumSize = new Size(1, 16);
            uiTextBox9.Name = "uiTextBox9";
            uiTextBox9.Padding = new Padding(5);
            uiTextBox9.ReadOnly = true;
            uiTextBox9.ShowText = false;
            uiTextBox9.Size = new Size(82, 29);
            uiTextBox9.TabIndex = 27;
            uiTextBox9.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox9.Watermark = "Notas";
            // 
            // btnGuardar
            // 
            btnGuardar.AutoSize = false;
            btnGuardar.BackColor = Color.Transparent;
            btnGuardar.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnGuardar.ForeColor = Color.FromArgb(80, 80, 80);
            btnGuardar.ImageAlign = ContentAlignment.MiddleLeft;
            btnGuardar.ImageScaling = ToolStripItemImageScaling.None;
            btnGuardar.Margin = new Padding(4, 1, 0, 2);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(92, 36);
            btnGuardar.Text = "Guardar";
            btnGuardar.ToolTipText = "Guardar el pedido nuevo";
            btnGuardar.Visible = false;
            // 
            // btnCancelar
            // 
            btnCancelar.AutoSize = false;
            btnCancelar.BackColor = Color.Transparent;
            btnCancelar.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnCancelar.ForeColor = Color.FromArgb(80, 80, 80);
            btnCancelar.Image = Properties.Resources.cancel_24px;
            btnCancelar.ImageAlign = ContentAlignment.MiddleLeft;
            btnCancelar.ImageScaling = ToolStripItemImageScaling.None;
            btnCancelar.Margin = new Padding(4, 1, 0, 2);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(92, 36);
            btnCancelar.Text = "Cancelar";
            btnCancelar.ToolTipText = "Descartar el pedido nuevo";
            btnCancelar.Visible = false;
```

- [x] **Paso 3: Agregar los controles al tab y a la barra**

En la lista `tabGeneral.Controls.Add(...)` (después de `tabGeneral.Controls.Add(uiLabel9);`):

```csharp
            tabGeneral.Controls.Add(uiTextBox9);
            tabGeneral.Controls.Add(uiTextBox8);
            tabGeneral.Controls.Add(uiLabel14);
            tabGeneral.Controls.Add(uiTextBox7);
            tabGeneral.Controls.Add(uiLabel13);
```

En la barra, reemplazar la línea de items:

```csharp
            barraHerramientas.Items.AddRange(new ToolStripItem[] { btnNuevo, btnEditar, btnGuardar, btnCancelar });
```

- [x] **Paso 4: Deshabilitar btnEditar en esta iteración**

En el bloque de `btnEditar`, agregar `btnEditar.Enabled = false;` después de `btnEditar.AutoSize = false;`. El modo Editar llega en el plan siguiente.

- [x] **Paso 5: Compilar**

```bash
dotnet build Ritrama2025.sln --no-restore --verbosity minimal
```

Esperado: `0 Errores`. Si el diseñador se quejara de controles superpuestos al abrir en Visual Studio, ajustar los puntos `Location`/`Size` de `uiTextBox8`, `uiTextBox9`, `uiLabel13`, `uiLabel14` y `uiTextBox7` manteniendo las zonas libres descritas arriba (x 650-905 en la fila y 264-299, y x 760-905 entre y 460 y 580).

- [ ] **Paso 6: Commit (solo con autorización del usuario)**

```bash
git add Forms/FrmPedidos.Designer.cs
git commit -m "feat(pedidos): controles de porcentaje ITBIS, precio y notas de linea"
```

---

### Tarea 7: Fuente de verdad del detalle y editor de línea

**Archivos:**
- Modificar: `Forms/FrmPedidos.cs`

**Interfaces:**
- Consume: `PedidoDetalleMapper.Mapear(DataTable)` (Tarea 3), `PedidoCalculos.TotalRenglon` y `PedidoCalculos.Calcular` (Tarea 1)
- Produce en el formulario:
  - `private readonly List<PedidoDetalle> _lineas = new();`
  - `private void ProyectarLineasEnGrid()`
  - `private PedidoDetalle? LineaSeleccionada()`
  - `private void AgregarLinea()`, `private void CargarLineaEnEditor(PedidoDetalle linea)`, `private void EditarLinea()`, `private void EliminarLinea()`, `private void LimpiarEditorLinea()`
  - `private void ActualizarTotalesEnPantalla()`
  - `private decimal PorcItbisActual()`

- [x] **Paso 1: Agregar el campo y la proyección**

Agregar el campo junto a los existentes del formulario (después de `private readonly Dictionary<UIComboBox, object?> _valoresSel = new();`):

```csharp
        private readonly List<PedidoDetalle> _lineas = new();
```

- [x] **Paso 2: Reemplazar la carga del detalle para respetar `_lineas`**

En `CargarDetallePedidoAsync`, reemplazar la llamada a `LlenarGridDetalle(detalle)` por:

```csharp
                _lineas.Clear();
                _lineas.AddRange(PedidoDetalleMapper.Mapear(detalle));
                if (uiDataGridView1.InvokeRequired)
                {
                    uiDataGridView1.BeginInvoke((Action)(ProyectarLineasEnGrid));
                }
                else
                {
                    ProyectarLineasEnGrid();
                }
```

- [x] **Paso 3: Reemplazar `LlenarGridDetalle` por la proyección**

Reemplazar el método `LlenarGridDetalle` completo (`Forms/FrmPedidos.cs:267-283`) por:

```csharp
        /// <summary>
        /// Proyecta las lineas del pedido sobre el grid. Cada fila guarda su linea en Tag,
        /// de modo que agregar, editar y eliminar no dependan del indice visible de la fila.
        /// </summary>
        private void ProyectarLineasEnGrid()
        {
            uiDataGridView1.Rows.Clear();
            int renglon = 0;
            foreach (PedidoDetalle linea in _lineas)
            {
                renglon++;
                int indice = uiDataGridView1.Rows.Add(
                    renglon,
                    linea.Product_name ?? string.Empty,
                    linea.Unidad ?? string.Empty,
                    linea.Cant.ToString("N2"),
                    linea.Notas ?? string.Empty,
                    linea.Precio.HasValue ? linea.Precio.Value.ToString("N2") : string.Empty,
                    linea.Total_Renglon.HasValue ? linea.Total_Renglon.Value.ToString("N2") : string.Empty);
                uiDataGridView1.Rows[indice].Tag = linea;
            }

            uiDataGridView1.ClearSelection();
        }

        /// <summary>
        /// Devuelve la linea seleccionada en el grid de detalle, o null si no hay seleccion.
        /// </summary>
        private PedidoDetalle? LineaSeleccionada()
        {
            return uiDataGridView1.CurrentRow?.Tag as PedidoDetalle;
        }
```

- [x] **Paso 4: Implementar el editor de línea**

Agregar los métodos. Reemplazan a `BtnAddProducto_Click`, que hoy escribe en columnas equivocadas (`Forms/FrmPedidos.cs:450`):

```csharp
        /// <summary>
        /// Agrega al borrador la linea del producto, cantidad, precio y notas del editor.
        /// </summary>
        private void AgregarLinea()
        {
            if (!TryGetProductoEditor(out DataRow? producto))
            {
                return;
            }

            if (!decimal.TryParse(uiTextBox3.Text?.Trim(), out decimal cant) || cant <= 0m)
            {
                MessageBox.Show("La cantidad debe ser un número mayor que cero.", "Pedido nuevo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal? precio = decimal.TryParse(uiTextBox8.Text?.Trim(), out decimal p) ? p : null;
            if (precio.HasValue && precio.Value < 0m)
            {
                MessageBox.Show("El precio no puede ser negativo.", "Pedido nuevo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _lineas.Add(new PedidoDetalle
            {
                Product_id = producto!["product_id"]?.ToString(),
                Product_name = producto!["product_name"]?.ToString(),
                Unidad = producto!["tipo"]?.ToString(),
                Cant = cant,
                Precio = precio,
                Total_Renglon = PedidoCalculos.TotalRenglon(cant, precio),
                Notas = uiTextBox9.Text?.Trim()
            });

            ProyectarLineasEnGrid();
            ActualizarTotalesEnPantalla();
            LimpiarEditorLinea();
        }

        /// <summary>
        /// Carga la linea seleccionada en los controles del editor para poder modificarla.
        /// </summary>
        private void CargarLineaEnEditor(PedidoDetalle linea)
        {
            AsignarCombo(uiComboBox2, linea.Product_id, _valoresSel);
            uiTextBox3.Text = linea.Cant.ToString("N2");
            uiTextBox8.Text = linea.Precio.HasValue ? linea.Precio.Value.ToString("N2") : string.Empty;
            uiTextBox9.Text = linea.Notas ?? string.Empty;
        }

        /// <summary>
        /// Reemplaza la linea seleccionada por los valores del editor.
        /// </summary>
        private void EditarLinea()
        {
            PedidoDetalle? linea = LineaSeleccionada();
            if (linea == null)
            {
                MessageBox.Show("Seleccione la linea que desea editar.", "Pedido nuevo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!TryGetProductoEditor(out DataRow? producto))
            {
                return;
            }

            if (!decimal.TryParse(uiTextBox3.Text?.Trim(), out decimal cant) || cant <= 0m)
            {
                MessageBox.Show("La cantidad debe ser un número mayor que cero.", "Pedido nuevo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal? precio = decimal.TryParse(uiTextBox8.Text?.Trim(), out decimal p) ? p : null;
            linea.Product_id = producto!["product_id"]?.ToString();
            linea.Product_name = producto!["product_name"]?.ToString();
            linea.Unidad = producto!["tipo"]?.ToString();
            linea.Cant = cant;
            linea.Precio = precio;
            linea.Total_Renglon = PedidoCalculos.TotalRenglon(cant, precio);
            linea.Notas = uiTextBox9.Text?.Trim();

            ProyectarLineasEnGrid();
            ActualizarTotalesEnPantalla();
            LimpiarEditorLinea();
        }

        /// <summary>
        /// Quita del borrador la linea seleccionada, previa confirmacion.
        /// </summary>
        private void EliminarLinea()
        {
            PedidoDetalle? linea = LineaSeleccionada();
            if (linea == null)
            {
                MessageBox.Show("Seleccione la linea que desea eliminar.", "Pedido nuevo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult confirmacion = MessageBox.Show(
                "¿Eliminar la linea " + (linea.Product_name ?? string.Empty) + " del borrador?",
                "Pedido nuevo",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            _lineas.Remove(linea);
            ProyectarLineasEnGrid();
            ActualizarTotalesEnPantalla();
            LimpiarEditorLinea();
        }

        /// <summary>
        /// Vacia los controles del editor de linea.
        /// </summary>
        private void LimpiarEditorLinea()
        {
            AsignarCombo(uiComboBox2, null, _valoresSel);
            uiTextBox3.Clear();
            uiTextBox8.Clear();
            uiTextBox9.Clear();
        }

        /// <summary>
        /// Devuelve el producto elegido en el combo del editor, o false si no hay ninguno.
        /// </summary>
        private bool TryGetProductoEditor(out DataRow? producto)
        {
            producto = null;
            if (_dtProductos == null)
            {
                return false;
            }

            object? productIdRaw = ValorCombo(uiComboBox2);
            if (productIdRaw == null || string.IsNullOrEmpty(productIdRaw.ToString()))
            {
                MessageBox.Show("Seleccione un producto.", "Pedido nuevo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            DataRow[] filas = _dtProductos.Select($"product_id = '{EscapeLike(productIdRaw.ToString() ?? string.Empty)}'");
            if (filas.Length == 0)
            {
                MessageBox.Show("El producto seleccionado no existe en el catálogo.", "Pedido nuevo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            producto = filas[0];
            return true;
        }
```

- [x] **Paso 5: Implementar el recálculo de totales**

```csharp
        /// <summary>
        /// Porcentaje de ITBIS indicado en pantalla; 18 por defecto si no es un numero valido.
        /// </summary>
        private decimal PorcItbisActual()
        {
            return decimal.TryParse(uiTextBox7.Text?.Trim(), out decimal porcentaje) && porcentaje >= 0m
                ? porcentaje
                : 18m;
        }

        /// <summary>
        /// Recalcula los importes del borrador y los muestra en solo lectura.
        /// </summary>
        private void ActualizarTotalesEnPantalla()
        {
            decimal porcItbis = PorcItbisActual();
            (decimal subTotal, decimal montoItbis, decimal total) = PedidoCalculos.Calcular(_lineas, porcItbis);
            uiTextBox4.Text = subTotal.ToString("N2");
            uiTextBox5.Text = montoItbis.ToString("N2");
            uiTextBox6.Text = total.ToString("N2");
        }
```

- [x] **Paso 6: Conectar los handlers de línea en el constructor**

En el constructor, reemplazar:

```csharp
            // Acciones sobre las líneas de producto del detalle.
            btnAddProducto.Click += BtnAddProducto_Click;
            btnBuscarProducto.Click += BtnBuscarProducto_Click;
```

por:

```csharp
            // Acciones sobre las lineas de producto del detalle.
            btnAddProducto.Click += (_, _) => AgregarLinea();
            btnEditarProducto.Click += (_, _) => EditarLinea();
            btnEliminarProducto.Click += (_, _) => EliminarLinea();
            btnBuscarProducto.Click += BtnBuscarProducto_Click;
            uiTextBox7.TextChanged += (_, _) => ActualizarTotalesEnPantalla();
```

- [x] **Paso 7: Eliminar el código muerto del agregar**

Borrar el método `BtnAddProducto_Click` completo (líneas 424-452 del archivo original): quedó reemplazado por `AgregarLinea`. Borrar también el método vacío `btnBuscarProducto_Click_1` (líneas 478-481).

- [x] **Paso 8: Compilar**

```bash
dotnet build Ritrama2025.sln --no-restore --verbosity minimal
```

Esperado: `0 Errores`. La columna `tipo` existe en el DataTable de productos porque `SELECT_QUERY_PRODUCTS` (`R.cs:102`) la calcula.

- [ ] **Paso 9: Commit (solo con autorización del usuario)**

```bash
git add Forms/FrmPedidos.cs
git commit -m "feat(pedidos): editor de lineas del borrador con product_id y totales"
```

---

### Tarea 8: Modo Nuevo, Guardar y Cancelar

**Archivos:**
- Modificar: `Forms/FrmPedidos.cs`

**Interfaces:**
- Consume: `PedidoValidador.EsValido` (Tarea 2), `IPedidoService.GetNewNumeroPedido`, `IPedidoService.SavePedidoCompleto`, `PermisoHelper.PuedeCrear`
- Produce en el formulario:
  - `private enum ModoFormulario { Consulta, Nuevo }`
  - `private ModoFormulario _modo = ModoFormulario.Consulta;`
  - `private void AplicarModo(ModoFormulario modo)`
  - `private async void BtnNuevo_Click(object? sender, EventArgs e)`
  - `private void BtnGuardar_Click(object? sender, EventArgs e)`
  - `private void BtnCancelar_Click(object? sender, EventArgs e)`
  - `private void LimpiarGeneral()`
  - `private Pedido ConstruirPedidoDesdeFormulario(int numero, DateTime fecha)`

- [x] **Paso 1: Agregar el modo y su aplicador**

Agregar después de `AplicarTemaVerde`:

```csharp
        /// <summary>
        /// Estado del formulario. Consulta es el estado por defecto: la pestaña General es de
        /// solo lectura y solo se habilita al entrar explicitamente a Nuevo.
        /// </summary>
        private enum ModoFormulario
        {
            Consulta,
            Nuevo
        }

        private ModoFormulario _modo = ModoFormulario.Consulta;

        /// <summary>
        /// Unico metodo que cambia ReadOnly o Enabled. Al volver a Consulta deja toda la
        /// pestaña General en solo lectura.
        /// </summary>
        private void AplicarModo(ModoFormulario modo)
        {
            _modo = modo;
            bool esNuevo = modo == ModoFormulario.Nuevo;

            // Encabezado siempre de solo lectura.
            uiTextBox1.ReadOnly = true;
            uiTextBox2.ReadOnly = true;
            uiRichTextBox2.ReadOnly = true;
            uiTextBox4.ReadOnly = true;
            uiTextBox5.ReadOnly = true;
            uiTextBox6.ReadOnly = true;

            // Editables solo en modo Nuevo (textos y areas de texto: ReadOnly ya impide escribir).
            uiDatetimePicker2.ReadOnly = !esNuevo;
            uiRichTextBox1.ReadOnly = !esNuevo;
            uiRichTextBox3.ReadOnly = !esNuevo;
            uiTextBox7.ReadOnly = !esNuevo;
            uiTextBox3.ReadOnly = !esNuevo;
            uiTextBox8.ReadOnly = !esNuevo;
            uiTextBox9.ReadOnly = !esNuevo;

            // Combos y fechas: ReadOnly solo impide escribir. Verificado en el IL de SunnyUI
            // 3.9.8 que UIDropControl.ReadOnly solo escribe en el TextBoxBase interno
            // (edit.ReadOnly), mientras UIComboBox.ListBox_Click y Edit_KeyDown siguen
            // cambiando el valor, y el desplegable se sigue abriendo con el raton. Por eso en
            // solo lectura se deshabilitan: Enabled = false no entrega ni teclado ni raton.
            uiDatetimePicker1.Enabled = false;
            uiDatetimePicker2.Enabled = esNuevo;
            cbo_customers.ReadOnly = !esNuevo;
            cbo_customers.Enabled = esNuevo;
            uiComboBox1.ReadOnly = !esNuevo;
            uiComboBox1.Enabled = esNuevo;
            uiComboBox2.ReadOnly = !esNuevo;
            uiComboBox2.Enabled = esNuevo;

            btnAddProducto.Visible = esNuevo;
            btnEditarProducto.Visible = esNuevo;
            btnEliminarProducto.Visible = esNuevo;
            btnBuscarProducto.Visible = esNuevo;
            btnAddProducto.Enabled = esNuevo;
            btnEditarProducto.Enabled = esNuevo;
            btnEliminarProducto.Enabled = esNuevo;
            btnBuscarProducto.Enabled = esNuevo;

            // Barra de herramientas. btnEditar sigue deshabilitado: el modo Editar llega en
            // el plan siguiente.
            btnNuevo.Visible = !esNuevo;
            btnGuardar.Visible = esNuevo;
            btnCancelar.Visible = esNuevo;

            gridPedidos.Enabled = !esNuevo;
        }
```

- [x] **Paso 2: Proteger la selección del listado en modo Nuevo**

En `GridPedidos_SelectionChanged`, agregar la guarda al inicio del método, antes de la validación de filas seleccionadas:

```csharp
            if (_modo != ModoFormulario.Consulta)
            {
                return;
            }
```

- [x] **Paso 3: Implementar Nuevo**

```csharp
        /// <summary>
        /// Entra al modo Nuevo: consume el consecutivo, limpia la pantalla y habilita la
        /// escritura de la cabecera y las lineas.
        /// </summary>
        private async void BtnNuevo_Click(object? sender, EventArgs e)
        {
            if (!PermisoHelper.PuedeCrear("Pedidos"))
            {
                MessageBox.Show("No tiene permiso para crear pedidos.", "Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int numero = await _pedidoService.GetNewNumeroPedido();
                LimpiarGeneral();
                _lineas.Clear();
                ProyectarLineasEnGrid();

                uiTextBox1.Text = numero.ToString();
                uiDatetimePicker1.Value = DateTime.Now;
                uiDatetimePicker2.Value = DateTime.Now;
                uiTextBox2.Text = PedidoEstado.Creado;
                uiTextBox7.Text = "18";
                AplicarModo(ModoFormulario.Nuevo);
                ActualizarTotalesEnPantalla();
                cbo_customers.Focus();
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al iniciar el pedido nuevo: " + ex.Message);
            }
        }
```

- [x] **Paso 4: Implementar Guardar**

```csharp
        /// <summary>
        /// Valida el borrador y lo guarda. Al terminar vuelve a solo lectura.
        /// </summary>
        private void BtnGuardar_Click(object? sender, EventArgs e)
        {
            if (_modo != ModoFormulario.Nuevo)
            {
                return;
            }

            if (!int.TryParse(uiTextBox1.Text, out int numero))
            {
                MessageBox.Show("El número de pedido no es válido.", "Pedido nuevo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Pedido pedido = ConstruirPedidoDesdeFormulario(numero, uiDatetimePicker1.Value);
            if (!PedidoValidador.EsValido(pedido, out string error))
            {
                MessageBox.Show(error, "Pedido nuevo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_pedidoService.SavePedidoCompleto(pedido))
            {
                MessageBox.Show("No se pudo guardar el pedido: " + _pedidoService.ErrorMsg, "Pedido nuevo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Pedido " + numero + " guardado.", "Pedido nuevo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LimpiarGeneral();
            AplicarModo(ModoFormulario.Consulta);
            _ = RecargarListadoAsync();
        }
```

- [x] **Paso 5: Implementar Cancelar y la recarga del listado**

```csharp
        /// <summary>
        /// Descarta el borrador y vuelve a solo lectura.
        /// </summary>
        private void BtnCancelar_Click(object? sender, EventArgs e)
        {
            DialogResult confirmacion = MessageBox.Show(
                "¿Descartar el pedido en borrador?",
                "Pedido nuevo",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            LimpiarGeneral();
            _lineas.Clear();
            ProyectarLineasEnGrid();
            AplicarModo(ModoFormulario.Consulta);
            _ = RecargarListadoAsync();
        }

        /// <summary>
        /// Vuelve a cargar el listado de pedidos y recalcula el contador.
        /// </summary>
        private async Task RecargarListadoAsync()
        {
            try
            {
                _dtPedidos = await _pedidoService.LoadDataPedidos();
                gridPedidos.DataSource = _dtPedidos;
                gridPedidos.ClearSelection();
                gridPedidos.CurrentCell = null;
                ActualizarContador();
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al recargar los pedidos: " + ex.Message);
            }
        }
```

- [x] **Paso 6: Construir el pedido desde la pantalla**

```csharp
        /// <summary>
        /// Arma el pedido con los valores de la pantalla y las lineas del borrador.
        /// </summary>
        private Pedido ConstruirPedidoDesdeFormulario(int numero, DateTime fecha)
        {
            Pedido pedido = new()
            {
                Numero = numero,
                Fecha = fecha,
                Fecha_entrega = uiDatetimePicker2.Value.Date,
                Estado = string.IsNullOrWhiteSpace(uiTextBox2.Text) ? PedidoEstado.Creado : uiTextBox2.Text.Trim(),
                Direccion_entrega = uiRichTextBox1.Text?.Trim(),
                Notas = uiRichTextBox3.Text?.Trim(),
                Anulado = false,
                Porc_Itbis = PorcItbisActual(),
                Detalle = new List<PedidoDetalle>(_lineas)
            };

            if (ValorCombo(cbo_customers) is object valorCliente
                && Guid.TryParse(valorCliente.ToString(), out Guid clienteId))
            {
                pedido.Customer_Id = clienteId;
                pedido.Customer_Name = cbo_customers.Text;
            }

            if (ValorCombo(uiComboBox1) is object valorVendedor
                && Guid.TryParse(valorVendedor.ToString(), out Guid vendedorId))
            {
                pedido.Vendor_Id = vendedorId;
            }

            (decimal subTotal, decimal montoItbis, decimal total) = PedidoCalculos.Calcular(pedido.Detalle, pedido.Porc_Itbis);
            pedido.SubTotal = subTotal;
            pedido.Monto_Itbis = montoItbis;
            pedido.Total = total;

            return pedido;
        }
```

- [x] **Paso 7: Limpiar la pestaña General**

```csharp
        /// <summary>
        /// Deja la pestaña General sin datos, lista para ver un pedido o empezar uno nuevo.
        /// </summary>
        private void LimpiarGeneral()
        {
            AsignarCombo(cbo_customers, null, _valoresSel);
            AsignarCombo(uiComboBox1, null, _valoresSel);
            uiTextBox1.Clear();
            uiDatetimePicker1.Value = DateTime.Now;
            uiDatetimePicker2.Value = DateTime.Now;
            uiTextBox2.Clear();
            uiRichTextBox1.Clear();
            uiRichTextBox2.Clear();
            uiRichTextBox3.Clear();
            uiTextBox4.Clear();
            uiTextBox5.Clear();
            uiTextBox6.Clear();
            uiTextBox7.Clear();
            LimpiarEditorLinea();
        }
```

- [x] **Paso 8: Conectar los handlers de la barra**

En el constructor, después de `btnBuscarProducto.Click += BtnBuscarProducto_Click;`:

```csharp
            btnNuevo.Click += BtnNuevo_Click;
            btnGuardar.Click += BtnGuardar_Click;
            btnCancelar.Click += BtnCancelar_Click;
            AplicarModo(ModoFormulario.Consulta);
```

- [x] **Paso 9: Compilar**

```bash
dotnet build Ritrama2025.sln --no-restore --verbosity minimal
```

Esperado: `0 Errores`.

- [x] **Paso 10: Recorrido manual de verificación**

Resultado: ejecutado por el usuario el 2026-09-25. Se confirmaron los pedidos nuevos desde la pantalla. La lista se conserva como criterio de regresión manual:

Como la pantalla no tiene arnés de pruebas automatizadas, validar a mano siguiendo esta lista:

1. Abrir Pedidos: todos los textos y áreas de texto de General en solo lectura, `btnNuevo` visible, `btnEditar` deshabilitado, los botones de línea ocultos, y los combos de cliente/vendedor/producto y las fechas atenuados y sin responder al clic ni al teclado.
2. En solo lectura, intentar cambiar el cliente desde el desplegable del combo: no se abre la lista ni cambia el valor. Es el caso que `ReadOnly` por sí solo no cubría.
3. Pulsar `btnNuevo`: aparece el número siguiente, la fecha de hoy, estado `creado`, ITBIS `18`, y los controles editables se habilitan. `btnGuardar` y `btnCancelar` sustituyen a `btnNuevo` en la barra.
4. Elegir cliente y vendedor: quedan seleccionados.
5. Elegir producto y cantidad y pulsar el botón de agregar: la línea aparece en el grid con producto, unidad, cantidad, precio y total; los totales de arriba se recalculan.
6. Cambiar el ITBIS a `0`: el monto de ITBIS y el total se actualizan al instante.
7. Seleccionar la línea y pulsar editar: sus valores vuelven al editor; cambiar la cantidad y guardar: el grid muestra la cantidad nueva y el total de la línea cambia.
8. Seleccionar la línea y pulsar eliminar: aparece la confirmación y la línea desaparece.
9. Guardar sin líneas: aparece el mensaje de que hay que agregar al menos una.
10. Guardar con una línea: aparece la confirmación y el formulario vuelve a solo lectura con el listado actualizado, con el pedido nuevo arriba.
11. Volver a entrar en Nuevo, agregar una línea y cancelar: el formulario queda limpio, en solo lectura, y el listado conserva el pedido del paso 10.
12. Entrar a un pedido existente del listado: todos los controles vuelven a solo lectura y el detalle se muestra completo, incluida la columna de precio y total de cada línea.

- [ ] **Paso 11: Commit (solo con autorización del usuario)**

```bash
git add Forms/FrmPedidos.cs
git commit -m "feat(pedidos): modo Nuevo, guardar y cancelar"
```

---

### Tarea 9: Prueba de integración del ITBIS editable

**Archivos:**
- Modificar: `Ritrama2025.Tests/PedidoServiceTests.cs`

**Interfaces:**
- Consume: `IPedidoService.SavePedidoCompleto`, `IPedidoService.GetNewNumeroPedido`, `_fixture.ExecuteScalar`
- Produce: cobertura de que un porcentaje de ITBIS distinto de 18 se persiste con los importes que calculó la pantalla.

- [ ] **Paso 1: Escribir la prueba**

Agregar al final de la clase, antes de la llave de cierre:

```csharp
    [SkippableFact]
    public async Task SavePedidoCompleto_PorcItbisEditado_PersisteLosImportes()
    {
        IPedidoService service = CrearServicio();

        object? customerIdObj = _fixture.ExecuteScalar("SELECT TOP 1 customer_id FROM customer");
        Skip.If(customerIdObj == null, "no hay clientes; validado en prueba manual");
        object? productIdObj = _fixture.ExecuteScalar("SELECT TOP 1 product_id FROM producto");
        Skip.If(productIdObj == null, "no hay productos; validado en prueba manual");

        Guid customerId = Guid.Parse(customerIdObj.ToString()!);
        string productId = productIdObj.ToString()!;
        int numero = await service.GetNewNumeroPedido();

        try
        {
            Pedido pedido = new Pedido
            {
                Numero = numero,
                Fecha = DateTime.Now,
                Customer_Id = customerId,
                Customer_Name = "Cliente Test",
                Estado = PedidoEstado.Creado,
                Porc_Itbis = 10m,
                SubTotal = 200m,
                Monto_Itbis = 20m,
                Total = 220m,
                Detalle =
                {
                    new PedidoDetalle
                    {
                        Product_id = productId,
                        Product_name = "Producto Test",
                        Cant = 1m,
                        Unidad = "Rollo",
                        Precio = 200m,
                        Total_Renglon = 200m
                    }
                }
            };

            service.SavePedidoCompleto(pedido).Should().BeTrue();

            Convert.ToDecimal(_fixture.ExecuteScalar(
                "SELECT porc_itbis FROM pedido WHERE numero = @p1", ("@p1", numero))!).Should().Be(10m);
            Convert.ToDecimal(_fixture.ExecuteScalar(
                "SELECT itbis FROM pedido WHERE numero = @p1", ("@p1", numero))!).Should().Be(20m);
            Convert.ToDecimal(_fixture.ExecuteScalar(
                "SELECT total$ FROM pedido WHERE numero = @p1", ("@p1", numero))!).Should().Be(220m);
        }
        finally
        {
            LimpiarPedido(numero);
            _fixture.ExecuteNonQuery("UPDATE control SET par1 = par1 - 1 WHERE filter='PED'");
        }
    }
```

- [ ] **Paso 2: Pedir autorización antes de ejecutarla**

Esta prueba **escribe en la base configurada** en `Ritrama2025.Tests/appsettings.Test.json`. Antes de correrla hay que confirmar con el usuario que esa base es una base de pruebas descartable. Sin esa confirmación no se ejecuta; el resto del plan no depende de ella.

- [ ] **Paso 3: Ejecutar solo esa prueba, con la base autorizada**

```bash
dotnet test Ritrama2025.Tests/Ritrama2025.Tests.csproj --no-restore --filter "FullyQualifiedName~SavePedidoCompleto_PorcItbisEditado"
```

Esperado: `1 prueba correcta`. Confirmar también que no quedó rastro: el `finally` borra el pedido y devuelve el consecutivo.

- [ ] **Paso 4: Commit (solo con autorización del usuario)**

```bash
git add Ritrama2025.Tests/PedidoServiceTests.cs
git commit -m "test(pedidos): persistencia del ITBIS editado"
```

---

## Verificación final del plan

```bash
dotnet build Ritrama2025.sln --no-restore --verbosity minimal
dotnet test Ritrama2025.Tests/Ritrama2025.Tests.csproj --no-restore --filter "Categoria=Unit"
```

Esperado: compilación con `0 Errores` y `0 Advertencia(s)`, y las 32 pruebas unitarias en verde (8 de `PedidoCalculosTests`, 16 de `PedidoValidadorTests`, 4 de `PedidoDetalleMapperTests`, 4 de `PedidoServiceValidacionTests`), sin abrir ninguna conexión.

Resultado de la última ejecución: `0 Errores`, `0 Advertencia(s)`, 32/32 pruebas en verde.

## Defectos encontrados durante la ejecución

Se corrigieron al implementar, aunque el plan original no los anticipaba:

1. `AsignarCombo` solo resoltaba el texto cuando el combo estaba enlazado a un `DataTable`. El combo de producto está enlazado a un `DataView` filtrado por `anulado = 0`, así que seleccionar una fila del detalle dejaba el combo de producto en blanco y rompía el paso 7 del recorrido manual. Se agrego `TablaDelCombo`, que resuelve ambos casos y busca sobre la tabla completa, para que el nombre siga visible si el producto fue anulado despues.
2. `CargarDetallePedidoAsync` modificaba `_lineas` en el hilo de fondo y despues la proyectaba en el hilo de la interfaz. `_lineas` es la fuente de verdad compartida, asi que ahora la mutacion y la proyeccion ocurren juntas en el hilo de la interfaz.
3. `BtnGuardar_Click` limpiaba el encabezado pero dejaba las lineas del borrador en el detalle. Ahora limpia tambien `_lineas` y reproyecta, igual que Cancelar.
4. `AgregarLinea` y `EditarLinea` duplicaban la validacion y el copiado de campos. Se extrajeron `LeerValoresEditor` y `CopiarValoresEditor` para que no se separen los campos que se guardan.
5. `_dtPedidos` no acepta nulos y se dejaba en `CS8618`; los tres accesos a `gridPedidos.Columns[...]` estaban en `CS8602`, igual que `Safe`. Se corrigieron los tres puntos, por lo que la solucion compila sin advertencias.

## Fuera de alcance de este plan

- Modo `Editar` y `ActualizarPedidoCompleto` (plan siguiente).
- Botón `btnAnular` desde la pantalla.
- `SQL_UPDATE_PEDIDO` y `SQL_DELETE_PEDIDO_DETALLE` en `R.cs`: solo hacen falta para el modo Editar.
- Correcciones de la revisión que no tocan este flujo: corchetes en `EscapeLike`, y el paralelismo del fixture de pruebas. Las advertencias de nulabilidad de `FrmPedidos.cs` si se corrigieron, porque los cinco puntos caian en el flujo que este plan reescribe.
