# Numeracion SO-#### y direcciones de pedido - Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Persistir el numero de pedido como texto `SO-####`, mostrar el GUID del cliente y del vendedor junto con la direccion del cliente, y precargar la direccion de entrega al elegir cliente.

**Architecture:** Un helper estatico nuevo `PedidoNumero` concentra toda la regla de formato y validacion del numero, de modo que la pantalla, el servicio y las pruebas compartan una sola definicion. El modelo `Pedido.Numero` pasa de `int` a `string`; el consecutivo de `control.par1` sigue siendo `INT` y el prefijo/relleno se compone en C#. La direccion del cliente se resuelve en SQL con `COALESCE` para que la pantalla no arme la regla de precedencia. La migracion de esquema es un script separado que se escribe y revisa pero no se ejecuta.

**Tech Stack:** C# / .NET (WinForms + SunnyUI), SQL Server, xUnit + FluentAssertions.

**Spec:** `docs/superpowers/specs/2026-09-25-pedido-numero-so-direcciones-design.md`

## Global Constraints

- NO ejecutar escrituras ni pruebas de integracion contra la base de datos sin autorizacion explicita del usuario.
- NO ejecutar `Scripts/Migrar_Pedido_Numero_SO.sql`. Se escribe y se revueba, nada mas.
- NO modificar ni versionar `appsettings.Development.json`.
- NO ejecutar `git commit` ni `git push` sin autorizacion. Cada tarea termina en un checkpoint de revision, no en un commit. El comando de commit queda anotado pero commented.
- Los nuevos controles y campos se nombran SIN tildes: `direccion_cliente`, `uiTextBox10`, `uiTextBox11`, `uiTextBox12`.
- Comentarios, XML docs y mensajes de error en espanol, sin tildes (convencion existente del repo).
- Los textbox de solo lectura siguen el patron de `AplicarModo`: `ReadOnly = !esNuevo` para los editables (`uiRichTextBox1`, `uiRichTextBox3`), `ReadOnly = true` para los que nunca se editan (`uiRichTextBox2`, linea 131). Los tres textbox nuevos son informationales y por tanto `ReadOnly = true` en ambos modos.
- Convenciones de pruebas existentes: namespace `Ritrama2025.Tests;` con `file-scoped namespace`, clase `public class XTests`, atributo `[Trait("Categoria", "Unit")]`, aserciones FluentAssertions, nombres de prueba en espanol con el patron `Metodo_Condicion_Resultado`.
- Convenciones de codigo de produccion existentes: `namespace Ritrama2025.Services.PedidoService { ... }` con llaves, summary XML en clase y en cada miembro publico.

---

## Mapa de archivos

**Crear:**
- `Services/PedidoService/PedidoNumero.cs` - regla unica de formato/validacion/parseo del numero. Sin dependencias.
- `Ritrama2025.Tests/PedidoNumeroTests.cs` - pruebas de la regla unica.
- `Scripts/Migrar_Pedido_Numero_SO.sql` - migracion de esquema, NO ejecutar.

**Modificar:**
- `Models/Pedido.cs` - `Numero` de `int` a `string`.
- `Services/PedidoService/PedidoValidador.cs` - valida el formato textual.
- `Services/PedidoService/IPedidoService.cs` - 4 firmas cambian a `string`.
- `Services/PedidoService/PedidoService.cs` - 4 firmas + el consecutivo se formatea aqui.
- `R.cs` - columna `direccion_cliente` en el combo de clientes.
- `Forms/FrmPedidos.Designer.cs` - 3 textbox + 3 labels.
- `Forms/FrmPedidos.cs` - handlers de seleccion, prellenado de entrega, modo lectura.
- `Ritrama2025.Tests/PedidoValidadorTests.cs` - 2 adaptaciones.
- `Ritrama2025.Tests/PedidoServiceValidacionTests.cs` - 1 adaptacion.
- `Ritrama2025.Tests/PedidoServiceTests.cs` - firmas de integracion (no ejecutar).
- `Ritrama2025.Tests/PedidoDetalleMapperTests.cs` - tipo de la columna `numero` del DataTable de prueba.

---

### Task 1: Helper `PedidoNumero` con sus pruebas

**Files:**
- Create: `Ritrama2025.Tests/PedidoNumeroTests.cs`
- Create: `Services/PedidoService/PedidoNumero.cs`

**Interfaces:**
- Consumes: nada. Es la base de la cadena.
- Produces: `public static class PedidoNumero` con
  - `public const string Prefijo = "SO-";`
  - `public const int LargoNumero = 4;`
  - `public static string Formatear(int numero)` - `27` -> `"SO-0027"`. Lanza `ArgumentOutOfRangeException` si `numero <= 0` o `numero > 9999`.
  - `public static bool EsValido(string? numero)` - `null`, `""`, `"SO-"`, `"SO-abc"`, `"SO-12"`, `"SO-00001"`, `"so-0001"` -> `false`; `"SO-0027"` -> `true`.
  - `public static int ParteNumerica(string? numero)` - `"SO-0027"` -> `27`; cualquier valor invalido -> `0`. Nunca lanza.

> La clase es `public` y NO `internal`: el proyecto de pruebas no tiene `InternalsVisibleTo`, asi que un tipo `internal` seria invisible para las pruebas.

- [ ] **Step 1: Escribir las pruebas que fallan**

Crear `Ritrama2025.Tests/PedidoNumeroTests.cs`:

```csharp
using FluentAssertions;
using Ritrama2025.Services.PedidoService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Pruebas de la regla de formato del numero de pedido. Logica pura: no abren la base de datos.
/// </summary>
[Trait("Categoria", "Unit")]
public class PedidoNumeroTests
{
    [Theory]
    [InlineData(1, "SO-0001")]
    [InlineData(27, "SO-0027")]
    [InlineData(100, "SO-0100")]
    [InlineData(999, "SO-0999")]
    [InlineData(9999, "SO-9999")]
    public void Formatear_NumeroValido_AgregaPrefijoYRelenoACuatroDigitos(int numero, string esperado)
    {
        PedidoNumero.Formatear(numero).Should().Be(esperado);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(10000)]
    public void Formatear_NumeroFueraDeRango_Throws(int numero)
    {
        Action accion = () => PedidoNumero.Formatear(numero);

        accion.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("SO-")]
    [InlineData("SO-abc")]
    [InlineData("SO-12")]
    [InlineData("SO-00001")]
    [InlineData("so-0001")]
    [InlineData("  SO-0001")]
    [InlineData("PED-0001")]
    public void EsValido_FormatoInvalido_Rechazado(string? numero)
    {
        PedidoNumero.EsValido(numero).Should().BeFalse();
    }

    [Theory]
    [InlineData("SO-0001")]
    [InlineData("SO-0027")]
    [InlineData("SO-9999")]
    public void EsValido_FormatoValido_Aceptado(string numero)
    {
        PedidoNumero.EsValido(numero).Should().BeTrue();
    }

    [Theory]
    [InlineData("SO-0027", 27)]
    [InlineData("SO-0001", 1)]
    [InlineData("SO-9999", 9999)]
    [InlineData("SO-0000", 0)]
    public void ParteNumerica_FormatoValido_DevuelveLaParteNumerica(string numero, int esperado)
    {
        PedidoNumero.ParteNumerica(numero).Should().Be(esperado);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("basura")]
    [InlineData("SO-abc")]
    public void ParteNumerica_FormatoInvalido_DevuelveCeroSinLanzar(string? numero)
    {
        PedidoNumero.ParteNumerica(numero).Should().Be(0);
    }

    [Fact]
    public void EsValidoYParteNumerica_CuadranParaTodosLosDigitosValidos()
    {
        // Los tres miembros tienen que ser coherentes entre si: si Formatear produce algo que
        // EsValido rechaza, la pantalla dejaria de poder editar el pedido recien creado.
        for (int numero = 1; numero <= 9999; numero += 337)
        {
            string texto = PedidoNumero.Formatear(numero);

            PedidoNumero.EsValido(texto).Should().BeTrue(
                "el numero {0} debe producir un texto que el propio validador acepte", texto);
            PedidoNumero.ParteNumerica(texto).Should().Be(numero);
        }
    }
}
```

- [ ] **Step 2: Ejecutar las pruebas y verificar que fallan**

Run:

```powershell
dotnet test Ritrama2025.Tests/Ritrama2025.Tests.csproj --filter "FullyQualifiedName~PedidoNumeroTests"
```

Expected: FALLA la compilacion con `error CS0246: The type or namespace name 'PedidoNumero' could not be found`. Ese es el fallo esperado: la clase todavia no existe.

- [ ] **Step 3: Implementar el minimo para que las pruebas pasen**

Crear `Services/PedidoService/PedidoNumero.cs`:

```csharp
using System.Globalization;

namespace Ritrama2025.Services.PedidoService
{
    /// <summary>
    /// Regla unica del numero de pedido: formato SO-####, validacion y lectura de la parte
    /// numerica. Logica pura, sin base de datos ni interfaz, para que la pantalla, el servicio
    /// y las pruebas compartan la misma definicion y no se desincronicen.
    /// </summary>
    public static class PedidoNumero
    {
        /// <summary>Prefijo que precede a la parte numerica.</summary>
        public const string Prefijo = "SO-";

        /// <summary>Cantidad de digitos de la parte numerica, con relleno de ceros a la izquierda.</summary>
        public const int LargoNumero = 4;

        /// <summary>Valor maximo representable con el relleno de cuatro digitos.</summary>
        public const int MaximoNumero = 9999;

        private const int NumeroMinimo = 1;

        /// <summary>
        /// Compone el numero completo. Por ejemplo, 27 produce "SO-0027".
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Si el numero no cabe en <see cref="LargoNumero"/> digitos o no es positivo.
        /// </exception>
        public static string Formatear(int numero)
        {
            if (numero < NumeroMinimo || numero > MaximoNumero)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(numero),
                    numero,
                    "El numero de pedido debe estar entre 1 y " + MaximoNumero + " para usar el formato "
                        + Prefijo + "####.");
            }

            return Prefijo + numero.ToString(
                "D" + LargoNumero.ToString(CultureInfo.InvariantCulture),
                CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Indica si el texto tiene exactamente el formato SO-####, con digitos en la parte
        /// numerica. Rechaza nulo, vacio, otros prefijos y cualquier otra longitud.
        /// </summary>
        public static bool EsValido(string? numero)
        {
            if (numero is null || numero.Length != Prefijo.Length + LargoNumero)
            {
                return false;
            }

            if (!numero.StartsWith(Prefijo, StringComparison.Ordinal))
            {
                return false;
            }

            for (int i = Prefijo.Length; i < numero.Length; i++)
            {
                if (numero[i] is < '0' or > '9')
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Devuelve la parte numerica del numero, o cero si el texto no tiene el formato valido.
        /// Nunca lanza, para que un dato corrupto en la base no tumbe la pantalla.
        /// </summary>
        public static int ParteNumerica(string? numero)
        {
            if (!EsValido(numero))
            {
                return 0;
            }

            string parte = numero!.Substring(Prefijo.Length, LargoNumero);

            return int.TryParse(parte, NumberStyles.None, CultureInfo.InvariantCulture, out int valor)
                ? valor
                : 0;
        }
    }
}
```

- [ ] **Step 4: Ejecutar las pruebas y verificar que pasan**

Run:

```powershell
dotnet test Ritrama2025.Tests/Ritrama2025.Tests.csproj --filter "FullyQualifiedName~PedidoNumeroTests"
```

Expected: PASS, 29 casos en verde (5 de `Formatear` valido + 3 de `Formatear` fuera de rango + 9 de `EsValido` invalido + 3 de `EsValido` valido + 4 de `ParteNumerica` valido + 4 de `ParteNumerica` invalido + 1 de cuadre). Si el conteo que reporta xUnit difiere por como agrupa las Theory, lo importante es que no haya fallos ni errores.

- [ ] **Step 5: Checkpoint de revision**

```powershell
git diff --stat
```

Confirma que solo quedaron `Services/PedidoService/PedidoNumero.cs` y `Ritrama2025.Tests/PedidoNumeroTests.cs`. NO hacer commit sin autorizacion. Comando guardado para cuando se autorice:

```bash
# git add Services/PedidoService/PedidoNumero.cs Ritrama2025.Tests/PedidoNumeroTests.cs
# git commit -m "feat(pedido): agregar helper de formato SO-####"
```

---

### Task 2: Script de migracion (se escribe, NO se ejecuta)

**Files:**
- Create: `Scripts/Migrar_Pedido_Numero_SO.sql`

**Interfaces:**
- Consumes: nada de codigo. Documenta el contrato que el resto de tareas asume.
- Produces: el unico mecanismo para pasar `pedido.numero` de `INT` a `varchar(20)` con backfill `SO-####` y para realinear `pedido_detalle.numero`. NINGUNA tarea posterior lo ejecuta.

> Datos verificados con consultas de solo lectura: 20 pedidos, `MAX(numero) = 24`; `control.par1` para `PED` = 26; `customer_address` 0/199, `Customer_Dir` 178/199. La FK se llama `FK_PEDIDO_DETALLE_PEDIDO`.

- [ ] **Step 1: Escribir el script**

Crear `Scripts/Migrar_Pedido_Numero_SO.sql`:

```sql
/* ==========================================================================
   Migracion: pedido.numero de INT a texto con formato SO-####

   IMPORTANTE: este script NO debe ejecutarse sin respaldo previo y con la
   aplicacion cerrada. Convers Development y Produccion apuntan a la misma
   base real, asi que un doble clic aqui afecta datos de produccion.

   Que hace, en orden:
     1. Valida que se pueda correr (preflight). Cualquier fallo aborta sin tocar nada.
     2. Convierte pedido_detalle.numero a varchar(20) para poder soltar la FK.
     3. Convierte pedido.numero a varchar(20).
     4. Rellena ambos con 'SO-' + 4 digitos, de forma identica en las dos tablas
        para que la FK siga emparejando.
     5. Recrea la FK.
     6. Muestra el resultado para verificacion visual.

   Rollback: la conversion a texto es irreversible en el sentido de que SQL Server
   no recuerda de que tipo era la columna. Si algo sale mal, restaurar el respaldo.
   ========================================================================== */

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

/* --------------------------------------------------------------------------
   Paso 1 y 2-5: todo dentro de una transaccion. Si el preflight falla o algo
   revienta en medio, el ROLLBACK del CATCH deja la base exactamente como estaba.
   -------------------------------------------------------------------------- */
BEGIN TRY
    BEGIN TRANSACTION;

    /* ---- Preflight ------------------------------------------------------ */

    IF OBJECT_ID('dbo.pedido', 'U') IS NULL
    BEGIN
        THROW 50001, 'Migracion cancelada: no existe la tabla dbo.pedido. Nada se modifico.', 1;
    END

    IF OBJECT_ID('dbo.pedido_detalle', 'U') IS NULL
    BEGIN
        THROW 50002, 'Migracion cancelada: no existe la tabla dbo.pedido_detalle. Nada se modifico.', 1;
    END

    /* La columna ya no es INT: el script ya corrio en una pasada anterior. */
    IF EXISTS (
        SELECT 1
        FROM sys.columns AS c
        INNER JOIN sys.types AS t ON c.user_type_id = t.user_type_id
        WHERE c.object_id = OBJECT_ID('dbo.pedido')
          AND c.name = 'numero'
          AND t.name <> 'int'
    )
    BEGIN
        THROW 50003, 'Migracion cancelada: pedido.numero ya no es INT, la migracion ya fue ejecutada. Nada se modifico.', 1;
    END

    /* El relleno a 4 digitos trunaria un numero de 5 o mas. */
    IF EXISTS (SELECT 1 FROM dbo.pedido WHERE numero > 9999)
    BEGIN
        THROW 50004, 'Migracion cancelada: hay un pedido con numero mayor que 9999 y el relleno lo truncaria. Nada se modifico.', 1;
    END

    /* Un numero 0 o negativo se volveria SO-0000, que es un punto fijo del backfill:
       pasaria la verificacion LIKE, quedaria confirmado, y PedidoNumero.Formatear(0)
       lanza ArgumentOutOfRangeException. Se aborta antes de crear ese dato. */
    IF EXISTS (SELECT 1 FROM dbo.pedido WHERE numero < 1)
    BEGIN
        THROW 50006, 'Migracion cancelada: hay un pedido con numero menor que 1 y el relleno produciria SO-0000, que la aplicacion no puede volver a formatear. Nada se modifico.', 1;
    END

    /* Lineas de detalle sin pedido: la FK no las dejaria convertir. */
    IF EXISTS (
        SELECT 1
        FROM dbo.pedido_detalle AS d
        WHERE NOT EXISTS (SELECT 1 FROM dbo.pedido AS p WHERE p.numero = d.numero)
    )
    BEGIN
        THROW 50005, 'Migracion cancelada: pedido_detalle tiene lineas sin pedido correspondiente. Nada se modifico.', 1;
    END

    /* ---- Paso 2: convertir la hija para poder soltar la FK -------------- */

    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_PEDIDO_DETALLE_PEDIDO')
    BEGIN
        ALTER TABLE dbo.pedido_detalle DROP CONSTRAINT FK_PEDIDO_DETALLE_PEDIDO;
    END

    ALTER TABLE dbo.pedido_detalle ALTER COLUMN numero varchar(20) NOT NULL;

    /* ---- Paso 3: convertir la tabla padre ------------------------------- */

    ALTER TABLE dbo.pedido ALTER COLUMN numero varchar(20) NOT NULL;

    /* ---- Paso 4: backfill identico en las dos tablas -------------------- */

    UPDATE dbo.pedido
    SET numero = 'SO-' + RIGHT('0000' + CAST(numero AS varchar(10)), 4);

    UPDATE dbo.pedido_detalle
    SET numero = 'SO-' + RIGHT('0000' + CAST(numero AS varchar(10)), 4);

    /* ---- Paso 5: recrear la FK, validando datos existentes --------------- */

    ALTER TABLE dbo.pedido_detalle WITH CHECK
        ADD CONSTRAINT FK_PEDIDO_DETALLE_PEDIDO
        FOREIGN KEY (numero) REFERENCES dbo.pedido (numero);

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF TRANCOUNT > 0
    BEGIN
        ROLLBACK TRANSACTION;
    END

    /* El THROW del preflight y los errores del CATCH salen por aqui. */
    THROW;
END CATCH
GO

/* --------------------------------------------------------------------------
   Paso 6: verificacion. Todas estas consultas deben devolver 0 filas.
   -------------------------------------------------------------------------- */
SET NOCOUNT ON;

SELECT 'pedidos sin formato SO-####' AS revision, COUNT(*) AS fuera_de_rango
FROM dbo.pedido
WHERE numero NOT LIKE 'SO-[0-9][0-9][0-9][0-9]'
    OR numero IS NULL;

SELECT 'detalle sin formato SO-####' AS revision, COUNT(*) AS fuera_de_rango
FROM dbo.pedido_detalle
WHERE numero NOT LIKE 'SO-[0-9][0-9][0-9][0-9]'
    OR numero IS NULL;

SELECT 'lineas huerfanas despues del backfill' AS revision, COUNT(*) AS huerfanas
FROM dbo.pedido_detalle AS d
WHERE NOT EXISTS (SELECT 1 FROM dbo.pedido AS p WHERE p.numero = d.numero);

SELECT 'numeros duplicados' AS revision, COUNT(*) AS duplicados
FROM (
    SELECT numero FROM dbo.pedido GROUP BY numero HAVING COUNT(*) > 1
) AS d;

SELECT numero, fecha, estado, customer_name
FROM dbo.pedido
ORDER BY numero;

/* --------------------------------------------------------------------------
   Nota sobre otras tablas: orden_corte.pedido_id y despacho.pedido_id son
   INT NULL y hoy estan vacias. NO se convierten en este script. Cuando se
   conecten a Pedido, harvested bajo un ticket aparte con su propia FK.
   -------------------------------------------------------------------------- */
```

- [ ] **Step 2: Revisar el script sin ejecutarlo**

Confirma por lectura, sin abrir la base:

- `THROW` (no `RAISERROR`) con numeros 50001..50006, todos con el texto "Nada se modifico."
- La comprobacion de idempotencia mira `sys.types.name <> 'int'`, no un `LIKE` sobre una columna numerica.
- El backfill de `pedido` y el de `pedido_detalle` son **expresiones identicas**. Esto es lo que mantiene la FK valida: si difieren, el paso 5 falla.
- El orden es: hija -> padre -> backfill de ambas -> FK. Si se hiciera el backfill antes de convertir, no se podria escribir texto en la columna `int`.
- `orden_corte` y `despacho` quedan intactas, con la nota de por que.

- [ ] **Step 3: Checkpoint de revision**

```powershell
git status --short Scripts/Migrar_Pedido_Numero_SO.sql
```

Confirma que el archivo existe como untracked nuevo y que NO se ejecuto nada. NO hacer commit sin autorizacion.

---

### Task 3: El numero de pedido pasa a ser texto de punta a punta

Esta tarea fusiona lo que el plan dejaba en dos. No es una preferencia estetica: al cambiar
`Pedido.Numero` de `int` a `string` el compilador rompe seis consumidores, y dos de ellos
estan en el proyecto principal (`Forms/FrmPedidos.cs:896`). Separar el cambio de modelo del
cambio de las firmas del servicio produce una tarea cuyo criterio de aceptacion es imposible
de cumplir. Esta tarea termina con la solucion compilando y las unitarias en verde.

**Files:**
- Modify: `Models/Pedido.cs:5`
- Modify: `Services/PedidoService/PedidoValidador.cs:24-28`
- Modify: `Services/PedidoService/IPedidoService.cs:10,11,19,20`
- Modify: `Services/PedidoService/PedidoService.cs:94,107,120,132-133,233,245,257,279`
- Modify: `R.cs:53`
- Modify: `Forms/FrmPedidos.cs:26,320,331,335,341,791,896`
- Modify: `Ritrama2025.Tests/PedidoValidadorTests.cs:18,48-54`
- Modify: `Ritrama2025.Tests/PedidoServiceValidacionTests.cs:37`
- Modify: `Ritrama2025.Tests/PedidoServiceTests.cs:29-32,46-52,77,144,193,232`
- Modify: `Ritrama2025.Tests/PedidoDetalleMapperTests.cs:18`

**Interfaces:**
- Consumes: `PedidoNumero.Formatear(int)`, `PedidoNumero.EsValido(string?)` y
  `PedidoNumero.ParteNumerica(string)` de Task 1.
- Produces, y es el contrato que consumen las Tasks 5 y 6:
  - `public string Numero { get; set; } = string.Empty;` en `Models.Pedido`.
  - `PedidoValidador.EsValido` mantiene su firma `(Pedido? pedido, out string error)`.
  - `Task<DataTable> LoadDataPedidoDetalle(string numero, CancellationToken cancellationToken = default);`
  - `Task<string> GetNewNumeroPedido(CancellationToken cancellationToken = default);`
  - `bool AnularPedido(string numero);`
  - `bool ActualizarEstadoPedido(string numero, string estado);`
  - `SavePedidoCompleto(Pedido)` sin cambios de firma, y con los parametros `@p1`
    receiving `pedido.Numero` ya como texto.
  - `R.QUERY.COMERCIAL.SQL_SELECT_LOAD_CUSTOMER_COMBO` proyecta una columna
    `direccion_cliente`.
  - `FrmPedidos.ConstruirPedidoDesdeFormulario(string numero, DateTime fecha)`.

> `Ritrama2025.Tests/PedidoServiceTests.cs` es de integracion: abre conexiones reales. En esta
> tarea se **adapta** para que compile, pero **no se ejecuta**.

- [ ] **Step 1: Cambiar las pruebas del validador para que fallen**

En `Ritrama2025.Tests/PedidoValidadorTests.cs`, linea 18, cambiar `Numero = 1001,` por `Numero = "SO-1001",`.

En el mismo archivo, reemplazar el test de la linea 48:

```csharp
    [Fact]
    public void EsValido_NumeroSinFormato_SO_Rechazado()
    {
        Pedido pedido = PedidoValido();
        pedido.Numero = "1001";

        bool ok = PedidoValidador.EsValido(pedido, out string error);

        ok.Should().BeFalse();
        error.Should().Contain("SO-####");
    }

    [Fact]
    public void EsValido_NumeroVacio_Rechazado()
    {
        Pedido pedido = PedidoValido();
        pedido.Numero = string.Empty;

        bool ok = PedidoValidador.EsValido(pedido, out string error);

        ok.Should().BeFalse();
        error.Should().Contain("SO-####");
    }
```

Y anadir un caso de aceptacion, junto a los demas tests validos:

```csharp
    [Fact]
    public void EsValido_NumeroConFormato_SO_Aceptado()
    {
        Pedido pedido = PedidoValido();
        pedido.Numero = "SO-0027";

        bool ok = PedidoValidador.EsValido(pedido, out string error);

        ok.Should().BeTrue();
        error.Should().BeEmpty();
    }
```

En `Ritrama2025.Tests/PedidoServiceValidacionTests.cs`, linea 37, cambiar `Numero = 777001,` por `Numero = "SO-7770",`.

- [ ] **Step 2: Ejecutar y verificar que falla**

Run:

```powershell
dotnet test Ritrama2025.Tests/Ritrama2025.Tests.csproj --filter "FullyQualifiedName~PedidoValidadorTests"
```

Expected: FALLA la compilacion con `error CS0034: Cannot implicitly convert type 'string' to 'int'` sobre `Numero`. Ese es el fallo esperado: el modelo todavia declara `int`.

- [ ] **Step 3: Cambiar el modelo**

En `Models/Pedido.cs:5`, reemplazar `public int Numero { get; set; }` por:

```csharp
        /// <summary>
        /// Numero del pedido con formato SO-####. Se persiste como texto para que el prefijo
        /// viaje con el dato y no dependa de la capa de presentacion.
        /// </summary>
        public string Numero { get; set; } = string.Empty;
```

- [ ] **Step 4: Adaptar el validador**

En `Services/PedidoService/PedidoValidador.cs`, reemplazar el bloque de las lineas 24-28:

```csharp
            if (!PedidoNumero.EsValido(pedido.Numero))
            {
                error = "El numero de pedido debe tener el formato SO-#### (por ejemplo, SO-0027).";
                return false;
            }
```

El mensaje contiene `SO-####`, asi que las dos pruebas del paso 1 pasan. `PedidoNumero` vive
en el mismo namespace `Ritrama2025.Services.PedidoService`, asi que no hace falta `using` nuevo.

- [ ] **Step 5: Adaptar las firmas de la interfaz**

En `Services/PedidoService/IPedidoService.cs`, reemplazar las cuatro firmas affected:

```csharp
        Task<DataTable> LoadDataPedidoDetalle(string numero, CancellationToken cancellationToken = default);
        Task<string> GetNewNumeroPedido(CancellationToken cancellationToken = default);
        bool SavePedidoCompleto(Models.Pedido pedido);

        /// <summary>
        /// Motivo del ultimo fallo de una operacion de escritura, para que la pantalla pueda
        /// mostrarlo sin conocer la clase concreta.
        /// </summary>
        string? ErrorMsg { get; }

        bool AnularPedido(string numero);
        bool ActualizarEstadoPedido(string numero, string estado);
```

Las tres firmas que no cambian (`LoadDataPedidos`, `LoadDataCustomers`, `LoadDataVendors`) se
dejan exactamente como estan.

- [ ] **Step 6: Adaptar la implementacion del servicio**

En `Services/PedidoService/PedidoService.cs`, cuatro cambios y ningun otro.

**a) `GetNewNumeroPedido` (lineas 120, 132-133).** El metodo no es `async`: devuelve
`Task.FromResult`. Se conserva esa forma y solo cambian el tipo de retorno y el `return`. El
acceso a `control.par1` via `SQL_QUERY_CONSUMO_PEDIDO_CONSECUTIVO` no se toca: ya funciona y
esta verificado con `par1 = 26` para `PED`.

```csharp
        public Task<string> GetNewNumeroPedido(CancellationToken ct = default)
        {
            try
            {
                using SqlConnection conn = new(_conn);
                conn.Open();
                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = R.QUERY.COMMERCIAL.SQL_QUERY_CONSUMO_PEDIDO_CONSECUTIVO
                };
                int consecutivo = Convert.ToInt32(cmd.ExecuteScalar()!);
                return Task.FromResult(PedidoNumero.Formatear(consecutivo));
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al obtener el consecutivo del pedido: " + ex.Message);
                throw;
            }
        }
```

**b) `LoadDataPedidoDetalle` (linea 94).** Cambiar `int numero` por `string numero`. El
parametro de la linea 107 ya se pasa tal cual con `cmd.Parameters.Add(new SqlParameter("@p1", numero));`,
asi que no requiere ningun cambio mas.

**c) `AnularPedido` (linea 233) y `ActualizarEstadoPedido` (linea 257).** Cambiar `int numero`
por `string numero` en ambas firmas. Sus parametros (lineas 245 y 279) tambien se pasan tal
cual y no cambian.

**d) `SavePedidoCompleto` (linea 142): sin cambios.** Las lineas 167 y 192 ya hacen
`cmd.Parameters.Add(new SqlParameter("@p1", pedido.Numero));` sin `CONVERT` ni `CAST`. Al pasar
`Numero` de `int` a `string` esas dos lineas siguen siendo correctas. El mensaje de la linea
219 concatena `pedido.Numero` en un `string`, asi que tambien sigue compilando.

> Importante: en ningun punto de este archivo hay `CONVERT` ni `CAST` sobre `numero`. Si al
> trabajar aparece alguno, es que se agrego despues de esta planificacion y hay que revisarlo.

- [ ] **Step 7: Anadir la columna `direccion_cliente` al combo de clientes**

En `R.cs:53`, reemplazar la cadena completa:

```csharp
                internal static string SQL_SELECT_LOAD_CUSTOMER_COMBO = "SELECT customer_id,customer_name,COALESCE(customer_address, Customer_Dir, customer_zone, 'Sin especificar') AS direccion_cliente FROM customer WHERE anulado = 0 ORDER BY customer_name";
```

El alias es exactamente `direccion_cliente`, sin tilde, porque es el nombre que lee el
formulario. El `WHERE anulado = 0` y el `ORDER BY` quedan igual.

> `customer_address` esta vacia en los 199 clientes y `Customer_Dir` tiene 178 informadas, asi
> que en la practica casi siempre gana `Customer_Dir`. El `COALESCE` esta para cuando
> `customer_address` empiece a poblarse, que es la direccion que escriben en la ficha del
> cliente.

- [ ] **Step 8: Arreglar los siete puntos de compilacion en el formulario**

En `Forms/FrmPedidos.cs`. Estos siete cambios son los que exige el compilador; ninguno es
opcional y ninguno agrega comportamiento nuevo de UI.

**a) La linea 26.** `_ultimoPedidoDetalleConsulta` es el token de "ultima consulta ganadora",
no un numero de negocio, asi que pasa a texto nullable:

```csharp
    private string? _ultimoPedidoDetalleConsulta;
```

**b) La linea 320.** `CargarDetallePedidoAsync` arranca con `int.TryParse(numero, out int numeroPedido)`
y, si falla, limpia `_lineas` y el grid y retorna. Con el numero en formato `SO-0027` ese
parseo **siempre falla**, asi que el detalle de cualquier pedido se veria vacio en modo
consulta. Se valida el formato en vez de parsear a entero:

```csharp
            if (!PedidoNumero.EsValido(numero))
            {
                _lineas.Clear();
                uiDataGridView1.Rows.Clear();
                return;
            }
```

**c) La linea 331.** Se guarda el texto tal cual: `_ultimoPedidoDetalleConsulta = numero;`

**d) La linea 335.** Se pasa el texto directo, sin variable intermedia:

```csharp
                DataTable detalle = await _pedidoService.LoadDataPedidoDetalle(numero, cts.Token).ConfigureAwait(false);
```

**e) La linea 341.** La comparacion de "gano la ultima consulta" pasa a texto:
`if (_ultimoPedidoDetalleConsulta != numero)` seguido de su `{ return; }`.

El campo queda `string?` y por tanto necesita el operador `!=` de referencia, que es el que
hace el codigo. Al empezar vale `null`, distinto de cualquier numero, asi que la primera
consulta siempre pasa el filtro. El resto del metodo, desde `AplicarDetalle` hasta el
`finally`, no cambia.

**f) La linea 791.** Donde hoy dice `int numero = await _pedidoService.GetNewNumeroPedido();`,
cambiar a:

```csharp
        string numero = await _pedidoService.GetNewNumeroPedido();
```

El nombre del campo es `_pedidoService` (declarado en la linea 21). La llamada no lleva token
y debe seguir sin llevarlo.

**g) La linea 896.** En `ConstruirPedidoDesdeFormulario`, cambiar la firma de `int numero` a
`string numero` y dejar `Numero = numero,` sin conversions. Qeda:

```csharp
    private Pedido ConstruirPedidoDesdeFormulario(string numero, DateTime fecha)
    {
        return new Pedido
        {
            Numero = numero,
            // ... el resto de asignaciones sin cambios
        };
    }
```

- [ ] **Step 9: Adaptar las pruebas restantes para que el proyecto de pruebas compile**

En `Ritrama2025.Tests/PedidoDetalleMapperTests.cs:18`, cambiar
`tabla.Columns.Add("numero", typeof(int));` por:

```csharp
        tabla.Columns.Add("numero", typeof(string));
```

La columna del `DataTable` de prueba refleja lo que ahora devuelve SQL, que es `varchar`.

En `Ritrama2025.Tests/PedidoServiceTests.cs`, cambiar el helper de limpieza:

```csharp
    private void LimpiarPedido(string numero)
    {
        _fixture.ExecuteNonQuery("DELETE FROM pedido_detalle WHERE numero = @p1", ("@p1", numero));
        _fixture.ExecuteNonQuery("DELETE FROM pedido WHERE numero = @p1", ("@p1", numero));
    }
```

En cada prueba que hoy hace `int numero = await service.GetNewNumeroPedido();`, cambiar a:

```csharp
        string numero = await service.GetNewNumeroPedido();
```

En las lineas 77 y 144, que asignan `Numero = numero,` dentro de un `new Pedido { ... }`,
dejar `Numero = numero,` sin cambios: ya es el valor textual que devuelve el servicio.

En los `INSERT` manuales de las pruebas de anulacion y estado (lineas ~193 y ~232), cambiar el
valor de `@p1` de un entero a un texto con formato, por ejemplo `("SO-9901",)` y
`("SO-9902",)`, para que no choquen con el consecutivo real.

En el test `GetNewNumeroPedido_IncrementaConsecutivoPED` (lineas 46-52), las dos llamadas ahora
devuelven texto, asi que la comparacion pasa a verificar el formato y el orden:

```csharp
        string n1 = await service.GetNewNumeroPedido();
        string n2 = await service.GetNewNumeroPedido();

        PedidoNumero.EsValido(n1).Should().BeTrue();
        PedidoNumero.EsValido(n2).Should().BeTrue();
        PedidoNumero.ParteNumerica(n2).Should().Be(PedidoNumero.ParteNumerica(n1) + 1);
```

Agregar al final del bloque `using` correspondiente:

```csharp
using Ritrama2025.Services.PedidoService;
```

- [ ] **Step 10: Build limpio y suite unitaria en verde**

Run:

```powershell
dotnet build Ritrama2025.sln
```

Expected: 0 errores, 0 advertencias. Este es el criterio de aceptacion principal de la tarea:
la solucion completa compila con el numero ya textual.

Run:

```powershell
dotnet test Ritrama2025.Tests/Ritrama2025.Tests.csproj --filter "Categoria=Unit"
```

Expected: todas las unitarias en verde. La baseline antes de este trabajo era 32/32; ahora
deben ser 32 mas las 29 de `PedidoNumeroTests` mas las 3 nuevas de `PedidoValidadorTests`.
Reportar el conteo exacto.

Si el build falla con `error CS1503`, queda un consumidor de la firma vieja sin actualizar.
Localizarlo con:

```powershell
Select-String -Path "Ritrama2025.Tests\*.cs","Forms\*.cs","Services\**\*.cs" -Pattern "GetNewNumeroPedido|AnularPedido|ActualizarEstadoPedido|LoadDataPedidoDetalle"
```

- [ ] **Step 11: Checkpoint de revision**

```powershell
git diff Models/Pedido.cs Services/PedidoService/PedidoValidador.cs Services/PedidoService/IPedidoService.cs Services/PedidoService/PedidoService.cs R.cs Forms/FrmPedidos.cs Ritrama2025.Tests/PedidoValidadorTests.cs Ritrama2025.Tests/PedidoServiceValidacionTests.cs Ritrama2025.Tests/PedidoServiceTests.cs Ritrama2025.Tests/PedidoDetalleMapperTests.cs
```

Revisar que el diff de `Forms/FrmPedidos.cs` de esta tarea **solo** contiene los siete puntos
del paso 8, y nada de los textbox nuevos: esos son de la Task 4.

NO correr `dotnet test` sin filtro sobre `PedidoServiceTests`: abre conexiones a la BD real.
NO hacer commit sin autorizacion.

### Task 4: Controles de solo lectura en el Designer

**Files:**
- Modify: `Forms/FrmPedidos.Designer.cs` (declaraciones ~1013-1044, `InitializeComponent` tras el bloque de `uiLabel14` ~linea 898)

**Interfaces:**
- Consumes: nada de codigo. Solo creacion de controles.
- Produces, y es lo que consume la Task 6:
  - `uiTextBox10` = GUID del cliente. `Location = (758, 470)`, `Size = (145, 29)`.
  - `uiTextBox11` = GUID del vendedor. `Location = (758, 546)`, `Size = (145, 29)`.
  - `uiTextBox12` = direccion del cliente. `Location = (690, 147)`, `Size = (210, 113)`.
  - Tres labels nuevos: `uiLabel15` (ID Cliente), `uiLabel16` (ID Vendedor), `uiLabel17` (Direccion del Cliente).

> Coordenadas verificadas contra `FrmPedidos.Designer.cs` para que nada se solape:
> - `uiRichTextBox2` (contacto) ocupa `x 414..669, y 147..260`, asi que `x=690` queda libre.
> - `uiLabel13 "Itbis % :"` ocupa `x 762..842, y 511..534`, por eso `uiTextBox10` va en `y=470` y no en `y=488`, que lo invadiria por 6px.
> - `uiLabel12 "Total :"` esta en `x 383..497`, asi que `uiTextBox11` en `x=758` no tiene choque horizontal.

- [ ] **Step 1: Declarar los controles y los labels**

En la region de declaraciones de campos de `FrmPedidos.Designer.cs` (alrededor de la linea 1044, junto a las declaraciones de `UILabel` existentes), agregar exactamente estas seis lineas:

```csharp
    private Sunny.UI.UILabel uiLabel15;
    private Sunny.UI.UILabel uiLabel16;
    private Sunny.UI.UILabel uiLabel17;
    private Sunny.UI.UITextBox uiTextBox10;
    private Sunny.UI.UITextBox uiTextBox11;
    private Sunny.UI.UITextBox uiTextBox12;
```

No declarar ningun otro control nuevo: los tres textbox y los tres labels son lo unico que se agrega.

- [ ] **Step 2: Instanciarlos en `InitializeComponent`**

Agregar las instanciaciones junto a las de los demas controles nuevos, es decir despues del bloque `// uiLabel14` (~linea 898):

```csharp
        // uiLabel15
        uiLabel15.Font = new Font("Microsoft Sans Serif", 9F);
        uiLabel15.ForeColor = Color.FromArgb(48, 48, 48);
        uiLabel15.Location = new Point(655, 473);
        uiLabel15.Name = "uiLabel15";
        uiLabel15.Size = new Size(80, 23);
        uiLabel15.TabIndex = 25;
        uiLabel15.Text = "ID Cliente :";
        tabGeneral.Controls.Add(uiLabel15);
        // uiLabel16
        uiLabel16.Font = new Font("Microsoft Sans Serif", 9F);
        uiLabel16.ForeColor = Color.FromArgb(48, 48, 48);
        uiLabel16.Location = new Point(655, 549);
        uiLabel16.Name = "uiLabel16";
        uiLabel16.Size = new Size(80, 23);
        uiLabel16.TabIndex = 26;
        uiLabel16.Text = "ID Vendedor :";
        tabGeneral.Controls.Add(uiLabel16);
        // uiLabel17
        uiLabel17.Font = new Font("Microsoft Sans Serif", 12F);
        uiLabel17.ForeColor = Color.FromArgb(48, 48, 48);
        uiLabel17.Location = new Point(533, 147);
        uiLabel17.Name = "uiLabel17";
        uiLabel17.Size = new Size(140, 23);
        uiLabel17.TabIndex = 27;
        uiLabel17.Text = "Direccion Cliente :";
        tabGeneral.Controls.Add(uiLabel17);
        // uiTextBox10
        uiTextBox10.Font = new Font("Microsoft Sans Serif", 8F);
        uiTextBox10.Location = new Point(758, 470);
        uiTextBox10.Name = "uiTextBox10";
        uiTextBox10.ReadOnly = true;
        uiTextBox10.Size = new Size(145, 29);
        uiTextBox10.TabIndex = 28;
        tabGeneral.Controls.Add(uiTextBox10);
        // uiTextBox11
        uiTextBox11.Font = new Font("Microsoft Sans Serif", 8F);
        uiTextBox11.Location = new Point(758, 546);
        uiTextBox11.Name = "uiTextBox11";
        uiTextBox11.ReadOnly = true;
        uiTextBox11.Size = new Size(145, 29);
        uiTextBox11.TabIndex = 29;
        tabGeneral.Controls.Add(uiTextBox11);
        // uiTextBox12
        uiTextBox12.Font = new Font("Microsoft Sans Serif", 8F);
        uiTextBox12.Location = new Point(690, 147);
        uiTextBox12.Multiline = true;
        uiTextBox12.Name = "uiTextBox12";
        uiTextBox12.ReadOnly = true;
        uiTextBox12.Size = new Size(210, 113);
        uiTextBox12.TabIndex = 30;
        tabGeneral.Controls.Add(uiTextBox12);
```

`Multiline = true` es necesario: la direccion es texto largo y en un `UITextBox` de una sola linea el usuario solo veria la primera. El alto de 113 pixeles ya esta dimensionado para varias lineas.

`TabIndex` 25..30 continues la secuencia existente, cuyo maximo actual es 24 (`uiLabel14`).

- [ ] **Step 3: Compilar**

Run:

```powershell
dotnet build Ritrama2025.sln
```

Expected: compila con 0 errores. Los controles todavia no hacen nada; eso llega en la Task 6.

- [ ] **Step 4: Verificacion visual de colisiones (manual, con la app)**

Run: `dotnet run --project Ritrama2025/Ritrama2025.csproj`, abrir la pantalla de Pedidos, modo `Nuevo`, y confirmar:

- "ID Cliente" queda sobre su textbox, sin que "Itbis %" lo pise.
- "ID Vendedor" queda sobre su textbox, sin choque con "Total :".
- "Direccion Cliente" queda a la derecha del campo de contacto, sin encimarse.
- Los tres textbox se ven grises y no aceptan escritura.

Si algo se solapa, ajustar `Location` del control afectado y dejar anotado el valor final en el spec.

- [ ] **Step 5: Checkpoint de revision**

```powershell
git diff Forms/FrmPedidos.Designer.cs
```

Verificar que no se toco ningun control existente, solo adiciones. NO hacer commit sin autorizacion.

---

### Task 5: Comportamiento del formulario

**Files:**
- Modify: `Forms/FrmPedidos.cs` (`AplicarModo` ~123-160, `CargarPedidoEnGeneral` ~276-310, `LimpiarGeneral` ~931-950, y el cableado de eventos cerca de las lineas 66-68)

**Interfaces:**
- Consumes: la columna `direccion_cliente` de Task 3 Step 7; los tres textbox y los tres
  labels de Task 4; `PedidoNumero.EsValido(string?)` de Task 1, para el helper `AsGuid` no
  hace falta pero el archivo ya lo importa desde Task 1.
- Produces: el flujo de UI completo. La carga del detalle en modo consulta y la firma textual
  de `ConstruirPedidoDesdeFormulario` ya quedaron en Task 3 Step 8; esta tarea solo agrega la
  capa de presentacion de los tres campos informativos.

- [ ] **Step 1: Cablear el evento de seleccion del cliente**

En `Forms/FrmPedidos.cs`, junto a las lineas 66-68 donde ya se llama `ConfigurarComboFiltroIncremental(cbo_customers)`, agregar el handler:

```csharp
        cbo_customers.ValueChanged += CboCustomers_ValueChanged;
```

Y agregar el metodo cerca de `AsignarCombo` (linea 480):

```csharp
    /// <summary>
    /// Al elegir cliente, muestra su GUID y su direccion, y precarga la direccion de entrega
    /// con la del cliente. Solo precarga en modo nuevo: en consulta la direccion de entrega
    /// viene del propio pedido y no debe tocarse.
    /// </summary>
    private void CboCustomers_ValueChanged(object? sender, EventArgs e)
    {
        bool esNuevo = !uiRichTextBox1.ReadOnly;

        if (ValorCombo(cbo_customers) is not object valorCliente || !AsGuid(valorCliente, out Guid guidCliente))
        {
            uiTextBox10.Clear();
            uiTextBox10.TipText = string.Empty;
            uiTextBox12.Clear();
            uiTextBox12.TipText = string.Empty;

            if (esNuevo)
            {
                uiRichTextBox1.Clear();
            }

            return;
        }

        uiTextBox10.Text = guidCliente.ToString();
        uiTextBox10.TipText = guidCliente.ToString();

        DataRow? cliente = FilaDelCombo(cbo_customers);
        string direccion = cliente is not null
            ? Safe(cliente, "direccion_cliente")?.ToString() ?? string.Empty
            : string.Empty;

        if (string.IsNullOrWhiteSpace(direccion))
        {
            direccion = "Sin especificar";
        }

        uiTextBox12.Text = direccion;
        uiTextBox12.TipText = direccion;

        if (esNuevo)
        {
            uiRichTextBox1.Text = direccion;
        }
    }
```

> `esNuevo` se deduce de `uiRichTextBox1.ReadOnly`, que `AplicarModo` ya fija a `!esNuevo` en la linea 139. No hace falta un campo de modo nuevo. La guarda importa: en consulta, `CargarPedidoEnGeneral` asigna el combo en la linea 284 y despues carga la direccion de entrega en la 298, asi que un borrado sin guarda seria un borrado de ida y vuelta que hoy funciona solo por el orden de esas dos lineas. Con la guarda, el handler deja la direccion de entrega intacta y el orden deja de importar.

- [ ] **Step 2: Agregar los helpers que usa el handler**

Agregar junto a `ValorCombo` (linea 529) y `TablaDelCombo` (linea 515):

```csharp
    /// <summary>
    /// Convierte el valor crudo de un combo en Guid, aceptando el tipo con que lo entrega la
    /// columna de la base. Devuelve false si no es un identificador valido.
    /// </summary>
    private static bool AsGuid(object? valor, out Guid resultado)
    {
        switch (valor)
        {
            case Guid guid:
                resultado = guid;
                return true;
            case string texto when Guid.TryParse(texto, out Guid desdeTexto):
                resultado = desdeTexto;
                return true;
            default:
                resultado = Guid.Empty;
                return false;
        }
    }

    /// <summary>
    /// Devuelve la fila del combo que esta seleccionada, o null si no hay ninguna.
    /// </summary>
    private static DataRow? FilaDelCombo(UIComboBox combo)
    {
        DataTable? tabla = TablaDelCombo(combo);

        if (tabla is null || combo.SelectedIndex < 0 || combo.SelectedIndex >= tabla.Rows.Count)
        {
            return null;
        }

        return tabla.Rows[combo.SelectedIndex];
    }
```

`FilaDelCombo` reutiliza el `TablaDelCombo` que ya resuelve `DataTable` y `DataView`, para no duplicar esa logica.

- [ ] **Step 3: Mostrar el GUID del vendedor**

En el mismo lugar, agregar el handler del vendedor y cablearlo junto a `ConfigurarComboFiltroIncremental(uiComboBox1)` (linea 68):

```csharp
        uiComboBox1.ValueChanged += UiComboBox1_Vendedor_ValueChanged;
```

```csharp
    /// <summary>
    /// Al elegir vendedor, muestra su GUID en el campo de solo lectura.
    /// </summary>
    private void UiComboBox1_Vendedor_ValueChanged(object? sender, EventArgs e)
    {
        if (ValorCombo(uiComboBox1) is object valor && AsGuid(valor, out Guid guidVendedor))
        {
            uiTextBox11.Text = guidVendedor.ToString();
            uiTextBox11.TipText = guidVendedor.ToString();
            return;
        }

        uiTextBox11.Clear();
        uiTextBox11.TipText = string.Empty;
    }
```

- [ ] **Step 4: Cargar los tres campos en modo consulta**

En `CargarPedidoEnGeneral` (linea 276), junto a los `AsignarCombo` que ya existen (lineas 284-285), agregar despues del `AsignarCombo(cbo_customers, ...)`:

```csharp
        if (AsGuid(Safe(drv, "customer_id"), out Guid guidCliente))
        {
            uiTextBox10.Text = guidCliente.ToString();
            uiTextBox10.TipText = guidCliente.ToString();
        }
        else
        {
            uiTextBox10.Clear();
        }

        if (AsGuid(Safe(drv, "vendor_id"), out Guid guidVendedor))
        {
            uiTextBox11.Text = guidVendedor.ToString();
            uiTextBox11.TipText = guidVendedor.ToString();
        }
        else
        {
            uiTextBox11.Clear();
        }
```

Y asignar la direccion del cliente al `uiTextBox12`, junto a la linea 298 que ya carga `uiRichTextBox1` con `direccion_entrega`:

```csharp
        DataRow? clienteSeleccionado = FilaDelCombo(cbo_customers);
        uiTextBox12.Text = clienteSeleccionado is not null
            ? Safe(clienteSeleccionado, "direccion_cliente")?.ToString() ?? "Sin especificar"
            : "Sin especificar";
```

> Se lee del combo y no de una columna del `drv` porque `pedido` no guarda la direccion del cliente: la guarda el cliente. Asi consulta y modo nuevo muestran exactamente el mismo valor.

- [ ] **Step 5: Limpiar los tres textbox al cancelar y al limpiar**

En `LimpiarGeneral` (linea 931), junto al `AsignarCombo(cbo_customers, null, _valoresSel)` de la linea 933, agregar:

```csharp
        uiTextBox10.Clear();
        uiTextBox10.TipText = string.Empty;
        uiTextBox11.Clear();
        uiTextBox11.TipText = string.Empty;
        uiTextBox12.Clear();
        uiTextBox12.TipText = string.Empty;
```

> Limpiar el combo dispara `ValueChanged`, que ya vacia `uiTextBox10` y `uiTextBox12`. Se repiten las llamadas de forma explicita porque `AsignarCombo` puede no mover el valor si ya era nulo, y un textbox con GUID de un pedido anterior sobreviving a un `LimpiarGeneral` es un bug de datos cruzados.

- [ ] **Step 6: Fijar el modo de solo lectura en `AplicarModo`**

En `AplicarModo` (lineas 123-160), junto a las lineas que ya fijan `cbo_customers.ReadOnly` y `uiComboBox2.ReadOnly` (lineas 153-158), agregar:

```csharp
        // Informativos: se muestran pero el usuario no los edita en ningun modo.
        uiTextBox10.ReadOnly = true;
        uiTextBox11.ReadOnly = true;
        uiTextBox12.ReadOnly = true;
```

- [ ] **Step 7: Compilar y correr la suite unitaria**

Run:

```powershell
dotnet build Ritrama2025.sln
```

Expected: 0 errores, 0 advertencias.

Run:

```powershell
dotnet test Ritrama2025.Tests/Ritrama2025.Tests.csproj --filter "Categoria=Unit"
```

Expected: todas las unitarias en verde. La baseline antes de este trabajo era 32/32; ahora debe ser 32 mas las de `PedidoNumeroTests` mas las 2 nuevas de `PedidoValidadorTests`.

- [ ] **Step 8: Recorrido manual sin guardar contra la BD real**

Run: `dotnet run --project Ritrama2025/Ritrama2025.csproj`, pantalla de Pedidos.

Importante: **no pulsar Guardar en el recorrido de lectura**. Abrir la pantalla y cambiar de
pedido no escribe nada. El guardado en modo nuevo solo se prueba si el usuario lo pide.

**Modo Consulta:**

1. Buscar un pedido existente. El grid de listado debe mostrar el numero ya con `SO-####`.
2. Al seleccionarlo, el grid de detalle de abajo **debe traer las lineas**. Este es el punto critico: si aparece vacio, revas a Task 3 Step 8b, casi seguro la guarda `int.TryParse` no se cambio.
3. `ID Cliente` debe mostrar el GUID del cliente del pedido.
4. `ID Vendedor` debe mostrar el GUID del vendedor del pedido.
5. `Direccion Cliente` debe mostrar la direccion del cliente elegido.
6. La direccion de entrega debe seguir mostrando lo que tiene guardado el pedido, **no** la direccion del cliente. Si aparece la del cliente, se colo la precarga en modo consulta: falta la guarda `esNuevo`.
7. Los tres textbox nuevos no deben aceptar escritura.

**Modo Nuevo:**

8. Pulsar Nuevo. El numero debe venir ya como `SO-0027` (o el siguiente disponible) en `Numero Orden`. No debe aparecer un entero suelto.
9. Elegir un cliente. Deben llenarse `ID Cliente` y `Direccion Cliente`, y la direccion de entrega debe precargarse con la del cliente.
10. Elegir un cliente que tenga `Customer_Dir` vacia: `Direccion Cliente` debe decir `Sin especificar`.
11. Cambiar de cliente: los tres valores deben seguir a la nueva seleccion, sin residuos del cliente anterior.
12. Elegir un vendedor: `ID Vendedor` debe llenarse con su GUID.
13. Poner el mouse sobre `ID Cliente` y `ID Vendedor`: el tooltip debe mostrar el GUID completo, porque el textbox lo muestra truncado.
14. Limpiar el formulario: los tres textbox deben quedar vacios. Si sobrevive el GUID de un pedido anterior, falta el paso 5.
15. Cancelar: los tres textbox deben quedar vacios.

- [ ] **Step 9: Checkpoint de revision**

```powershell
git diff Forms/FrmPedidos.cs
```

NO hacer commit sin autorizacion.

---

### Task 6: Verificacion final

**Files:**
- No modifica archivos. Solo verifica.

**Interfaces:**
- Consumes: todo lo producido por las Tasks 1 a 5.
- Produces: evidencia de que la extension esta completa y no rompio el trabajo anterior.

- [ ] **Step 1: Build limpio de la solucion completa**

Run:

```powershell
dotnet build Ritrama2025.sln
```

Expected: **0 errores y 0 advertencias**. La extension anterior dejo el build limpio y este cambio no debe perderlo.

- [ ] **Step 2: Suite unitaria completa**

Run:

```powershell
dotnet test Ritrama2025.Tests/Ritrama2025.Tests.csproj --filter "Categoria=Unit"
```

Expected: 0 fallos. Reportar el conteo exacto en el resumen final.

- [ ] **Step 3: Confirmar que las pruebas de integracion NO se ejecutaron**

Run:

```powershell
git status --short
```

Confirma que `Ritrama2025.Tests/PedidoServiceTests.cs` quedo **modificado pero no ejecutado**. No correr `dotnet test` sin filtro sobre ese archivo: abre conexiones a la base real.

- [ ] **Step 4: Confirmar que el script de migracion quedo sin ejecutar**

Run:

```powershell
Select-String -Path "Scripts/Migrar_Pedido_Numero_SO.sql" -Pattern "THROW 5000"
```

Deben aparecer los 6 codigos de preflight (50001..50006). El archivo existe y esta listo para que el usuario lo ejecute manualmente, pero nadie lo ejecuto.

- [ ] **Step 5: Revisar el diff completo contra el spec**

Run:

```powershell
git diff --stat
git status --short
```

Contra el spec `docs/superpowers/specs/2026-09-25-pedido-numero-so-direcciones-design.md`, confirmar que cada requisito tiene su contraparte:

- numero persistido como `SO-####` -> Tasks 1, 3
- `control.par1` sigue `INT` -> Task 4 Step 2
- GUID crudo, sin `Identificacion` -> Tasks 4, 6
- ITBIS fijo 18, sin `customer.impuesto` -> sin cambios, confirmar que `ConstruirPedidoDesdeFormulario` sigue usando la constante
- direccion cargada al seleccionar cliente, no al abrir el datepicker -> Task 5 Steps 1 y 3
- `COALESCE` en el SQL, no en C# -> Task 4 Step 3
- tres textbox de solo lectura -> Tasks 4, 5
- `TipText` con el GUID completo -> Tasks 6 Steps 1 y 3
- migracion escrita y no ejecutada -> Task 2

- [ ] **Step 6: Informe de cierre**

Reportar explicitamente, sin adornos:

1. Numero de pruebas unitarias en verde.
2. Build con 0 errores y 0 advertencias.
3. Que `Scripts/Migrar_Pedido_Numero_SO.sql` quedo escrito y **no ejecutado**, y que falta la autorizacion del usuario mas un respaldo.
4. Que `PedidoServiceTests` quedo adaptada pero no ejecutada por tocar la BD real.
5. Que el recorrido manual de Task 5 Step 8 quedo pendiente o ejecutado, segun corresponda, y que no se guardo ningun pedido durante la lectura.
6. Que no se hizo ningun commit ni push.
7. Que el paso 5b de la Task 5 era obligatorio: sin el, el detalle se veia vacio en modo consulta.
