# Pestaña «Inventario» en FrmProductos — plan de implementación

Fecha: 2026-10-06
Estado: en ejecución (SDD)

## Contexto

`FrmProductos` (rediseño 30/70) tiene `tabDetalle` (Sunny.UI.UITabControl) con una sola
pestaña, `tabDetalleProducto` ("Detalle"). Se añade una segunda pestaña **«Inventario»**
que muestra en un grid los masters (rollos) del producto seleccionado, con el mismo
lenguaje visual que el módulo `Frm_Inventarios`: barra de "% Disponible" con colores y
colores de estado.

Datos: igualdad exacta `Part_Number = @codigo` sobre la misma consulta que usa Inventario
(`R.QUERY.PRODUCTION.SQL_QUERY_SELECT_LOAD_ROLL_ID_INVENTARIO`, iniciales + compras,
compras primero, deduplicado por rollid). NO se reutiliza el `LIKE` de
`BuscarMasterInventario`: los códigos comparten prefijo (0075 y 00753) y mezclaría
productos distintos.

Carga perezosa: la consulta solo se hace al entrar en la pestaña. Si el producto cambia
con la pestaña abierta, se recarga. Estado vacío: un único mensaje
«Este producto no tiene masters en inventario» (sin producto seleccionado, sin masters o
error de consulta). Grid siempre visible (pestaña siempre presente, vacía si no hay datos).

Aprobado por el usuario: diseño acotado con 9 columnas, y entre los extras solo se eligió
la columna «Documento OC» (ya incluida en las 9).

## Global Constraints (vinculan TODAS las tareas)

1. **Build**: `dotnet build Ritrama2025.csproj --nologo -v q -p:BaseOutputPath=bin_check/`
   → **0 errores y 0 advertencias nuevas**. La aplicación está abierta: `bin\...Ritrama2025.exe`
   está bloqueado, por eso SIEMPRE `-p:BaseOutputPath=bin_check/`. Nunca `dotnet build` a secas.
2. **Tests**: `dotnet test Ritrama2025.Tests\Ritrama2025.Tests.csproj -p:BaseOutputPath=bin_test/ --nologo -v q`
   → suite en verde. El proyecto de tests arrastra warnings PREEXISTENTES (CS8600/CS8602 en
   `FrmMateriaPrimaLayoutTests`, `FrmUsuariosDatosTests`, `ProductsImportServiceTests.cs(66,9)`):
   no son de este cambio y no se tocan.
3. **Prohibido modificar**: `R.cs` (`SQL_QUERY_SELECT_LOAD_ROLL_ID` y `..._INVENTARIO`),
   `Frm_Inventarios.*`, y cualquier SQL de inventario / OC / reportes.
4. **Estilo**: comentarios y textos de UI en español; XML docs en los miembros nuevos tal
   como el resto de `FrmProductos`; cero `MessageBox` en el formulario (avisos por
   `MostrarAviso`, errores de servicio vía `ServiceErrors.Report`).
5. **Sin subagentes desde dentro de una tarea**: las revisiones las despacha el controlador.
6. El repositorio está en la rama `main` con cambios sin commitear del usuario:
   **NO hacer commit, stash, reset ni clean**, y no tocar ficheros ajenos a la tarea.
7. El grid nuevo es **solo lectura** (`ReadOnly = true`,
   `AllowUserToAddRows = false`, `AllowUserToDeleteRows = false`, `MultiSelect = false`).

## Task 1 — Servicio: `BuscarMastersDeProducto`

Ficheros: `Services/InventarioService/IInventarioService.cs`, `Services/InventarioService/InventarioService.cs`.

1. En la interfaz, junto a `BuscarMasterInventario` (línea 18), añadir:

```csharp
Task<DataTable?> BuscarMastersDeProducto(string productId);
```

con XML doc que explique: masters de UN producto para la pestaña Inventario de Productos;
igualdad exacta sobre `Part_Number` (el `LIKE` de `BuscarMasterInventario` casaría con
prefijos como 0075/00753); devuelve `null` si falla la consulta.

2. En `InventarioService`, implementar siguiendo el molde exacto de
   `BuscarMasterInventario` (líneas 217-252):

   - `string baseSql = R.QUERY.PRODUCTION.SQL_QUERY_SELECT_LOAD_ROLL_ID_INVENTARIO.TrimEnd();`
     (el fichero ya tiene `using static Ritrama2025.R.QUERY;`, pero copia el estilo literal
     del método vecino).
   - Quitar el `ORDER BY Roll_Id` final si existe (`EndsWith(..., OrdinalIgnoreCase)` →
     `baseSql[..^orderBy.Length].TrimEnd()`), igual que el método vecino.
   - Añadir `AND Part_Number = @productId` **sin LIKE**: la consulta base termina en
     `... WHERE rn = 1`, así que el `AND` cuelga correctamente.
   - Parámetro `new SqlParameter("@productId", SqlDbType.NVarChar, 25) { Value = productId.Trim() }`.
   - Recompón `ORDER BY Roll_Id` al final.
   - Ejecuta con el `CargarTablaAsync(sql, false, parametros, "MastersProducto", true)` privado
     del propio servicio (línea 407).
   - `catch (SqlException ex)` → `ServiceErrors.Report(...)` con el mismo patrón de mensaje
     de los métodos vecinos y `return null;`.
   - Si `productId` es null/vacío en blanco: devolver `new DataTable("MastersProducto")`
     **sin tocar el servidor** (sin `await`), para que la pestaña muestre el estado vacío.

Verificación de la tarea: build 0/0 con la Global Constraint 1. No hay tests de este
servicio contra BD (los que existen usan conexión real); no crear ninguno nuevo.

## Task 2 — UI: pestaña «Inventario»

Ficheros: `Forms/FrmProductos.Designer.cs`, `Forms/FrmProductos.cs`,
`Ritrama2025.Tests/Stubs/InventarioServiceStub.cs` (nuevo),
`Ritrama2025.Tests/FrmProductosLayoutTests.cs` (solo constructores),
`Ritrama2025.Tests/FrmProductosFiltroCategoriaTests.cs` (solo constructores).

### A. Contrato de servicio (lo entrega la Task 1)

```csharp
Task<DataTable?> BuscarMastersDeProducto(string productId);   // en IInventarioService
```

Si esa tarea aún no ha terminado, la UI se escribe igual: solo depende de esa firma.

### B. Diseñador (`FrmProductos.Designer.cs`)

Cuatro controles nuevos, con el mismo trato que el resto del fichero (instantiación arriba,
`SuspendLayout`/`BeginInit`, bloque de configuración, `ResumeLayout`/`EndInit` y declaración
de campo al final):

| Control | Tipo | Nombre |
|---|---|---|
| Pestaña | `TabPage` | `tabInventarioProducto` |
| Contenedor | `TableLayoutPanel` | `tlpInventario` |
| Mensaje vacío | `Sunny.UI.UILabel` | `lblInventarioVacio` |
| Grid | `DataGridView` | `gridMasters` |

- `tabDetalle.Controls.Add(tabInventarioProducto);` justo después de la línea que añade
  `tabDetalleProducto` (línea 406).
- `tabInventarioProducto`: `BackColor = Color.White`, `Text = "Inventario"`,
  `Name = "tabInventarioProducto"`, `Location = new Point(0, 32)`,
  `Size = new Size(783, 583)`, `TabIndex = 1`, `Controls.Add(tlpInventario)`.
- `tlpInventario`: 1 columna, 2 filas (`Absolute 30F` para la etiqueta,
  `Percent 100F` para el grid), `Dock = DockStyle.Fill`, `Name = "tlpInventario"`,
  `Controls.Add(lblInventarioVacio, 0, 0)` y `Controls.Add(gridMasters, 0, 1)`.
- `lblInventarioVacio`: `Dock = DockStyle.Fill`,
  `TextAlign = ContentAlignment.MiddleCenter`, `Visible = false`,
  `Text = "Este producto no tiene masters en inventario"`, `Name = "lblInventarioVacio"`.
- `gridMasters` (copia el bloque de propiedades de `GridMaster` en
  `Frm_Inventarios.Designer.cs` líneas 227-239, más `Dock = DockStyle.Fill` y
  `AutoGenerateColumns = false`): `AllowUserToAddRows = false`,
  `AllowUserToDeleteRows = false`, `AllowUserToResizeRows = false`,
  `MultiSelect = false`, `ReadOnly = true`, `SelectionMode = CellSelect`,
  `RowHeadersWidth = 33`, `Name = "gridMasters"`, `TabIndex = 0`.
  **Sin** enganches de eventos en el diseñador: se cablean en `ConfigurarEventos()`.
- Declaraciones de campo al final, junto a `private TabPage tabDetalleProducto;`.

### C. Lógica (`FrmProductos.cs`)

Usings nuevos: `Ritrama2025.Services.InventarioService`, `Ritrama2025.Services.CommonService`,
`System.Data`.

1. Campo `private readonly IInventarioService _inventarioService;` y **último parámetro**
   del constructor `IInventarioService inventarioService` con
   `ArgumentNullException.ThrowIfNull(inventarioService);` y asignación (mismo patrón que los
   demás). XML doc del `<param>` en el constructor.
2. Campo `private string? _inventarioCargadoPara;` — código cuya carga ya está en el grid.
3. `ConfigurarGridMasters()`, llamado desde el constructor justo antes de
   `ConfigurarEventos();`, con **exactamente** estas 9 columnas, en este orden, vía
   `CommonService.ADD_COLUMN_GRID(nombre, ancho, cabecera, campo_bd, gridMasters)`:

   | nombre | ancho | cabecera | campo_bd |
   |---|---|---|---|
   | `roll_id` | 100 | `Rollid` | `roll_id` |
   | `width` | 80 | `Width` | `width` |
   | `length` | 80 | `Length` | `lenght` |
   | `length_consumido` | 90 | `Consumido` | `largo_consumido` |
   | `length_restante` | 90 | `Restante` | `largo_restante` |
   | `pct_disponible` | 120 | `% Disponible` | `pct_disponible` |
   | `tipo_mov` | 90 | `Origen` | `tipo_mov` |
   | `estado` | 150 | `Estado` | `estado` |
   | `documento_oc` | 140 | `Documento OC` | `documento_oc` |

   Primera línea: `gridMasters.AutoGenerateColumns = false;`.
   (`pct_disponible` no existe en el resultado SQL: es una columna visual pintada en
   `CellPainting`, igual que en `Frm_Inventarios.BindingMasterGrid`.)
4. En `ConfigurarEventos()` añadir:

```csharp
tabDetalle.SelectedIndexChanged += TabDetalle_SelectedIndexChanged;
gridMasters.CellFormatting += GridMasters_CellFormatting;
gridMasters.CellPainting += GridMasters_CellPainting;
```

5. `private async void TabDetalle_SelectedIndexChanged(object? sender, EventArgs e)`:
   si `tabDetalle.SelectedTab == tabInventarioProducto` → `await RefrescarInventarioAsync();`
   (los stubs de test devuelven `Task.FromResult`, así que el cuerpo termina de forma
   síncrona y las pruebas pueden afirmar justo después de cambiar de pestaña).
6. `private async Task RefrescarInventarioAsync()`:
   - Si la pestaña activa no es `tabInventarioProducto` → `return`.
   - `string? codigo = _idSeleccionado;` Si está vacío → `MostrarInventarioVacio(); _inventarioCargadoPara = null; return;`
     (sin llamar al servicio).
   - Si `_inventarioCargadoPara == codigo` → `return` (ya cargado, evita martillar la BD).
   - `DataTable? masters = await _inventarioService.BuscarMastersDeProducto(codigo);`
     dentro de `try { ... } catch (Exception ex) { ServiceErrors.Report("Error al cargar el inventario del producto: " + ex.Message); MostrarInventarioVacio(); }`
     (envuelve TODO el cuerpo: un `async void` con excepción sin capturar mata la app).
   - `_inventarioCargadoPara = codigo;` al salir del `try` (también en el fallo, para no
     reintentar en bucle).
   - `masters is null || masters.Rows.Count == 0` → `MostrarInventarioVacio(); return;`
   - `lblInventarioVacio.Visible = false; gridMasters.DataSource = masters.DefaultView;`
7. `private void MostrarInventarioVacio()`:
   `gridMasters.DataSource = null;` + `lblInventarioVacio.Text = "Este producto no tiene masters en inventario";`
   + `lblInventarioVacio.Visible = true;`. Con XML doc indicando que es el único mensaje
   de estado vacío (diseño aprobado: sin estados diferenciados).
8. En `RefrescarDetalleDeLaFilaActiva()`, tras `ActualizarBotonesBarra(producto);`:

```csharp
if (tabDetalle.SelectedTab == tabInventarioProducto)
{
    _inventarioCargadoPara = null;
    _ = RefrescarInventarioAsync();
}
```

   (pasa `_idSeleccionado` ya actualizado arriba; cubre también el caso sin producto,
   porque `MostrarDetalle(null)` → `LimpiarDetalle()`). El método no se vuelve `async`.
9. `GridMasters_CellFormatting`: versión de `Frm_Inventarios.GridMaster_CellFormatting`
   (líneas 665-694) sobre `gridMasters` y la columna `estado`, pero **sin** el `throw` del
   `catch` (usa `Convert.ToString(e.Value) ?? string.Empty`). Colores: `Agotado` y
   `Desperdicio` → rojo con letra blanca; `Completo` → verde con letra blanca;
   `Parcialmente Consumido` → naranja con letra blanca.
10. `GridMasters_CellPainting`: versión de `Frm_Inventarios.GridMaster_CellPainting`
    (líneas 700-722): guarda `e.RowIndex < 0 || e.ColumnIndex < 0`, solo columna
    `pct_disponible`, salta `fila.IsNewRow`, luego
    `double pct = ProgressBarRenderer.CalcularDesdeFila(fila, "length", "length_restante");`
    `ProgressBarRenderer.PaintCell(e.Graphics, e.CellBounds, e.State, pct, e.CellStyle, (e.State & DataGridViewElementStates.Selected) != 0);`
    `e.Handled = true;`.
11. Estilo: el `UIStyleManager` del constructor re-estiliza el formulario, así que aplica
    los colores/tipos de la pestaña en un método privado `EstilarPestanaInventario()`
    llamado desde el constructor junto a los demás `Estilar*` (letra
    `JetBrains Mono 9F` y `ForeColor = GrisTexto` en la etiqueta; grid con fondo blanco y
    cabecera con la misma familia que el resto). No dupliques propiedades que ya pone el
    diseñador si el estilo global las respeta: comprueba el resultado mirando el código de
    `EstilarCamposDetalle` y haz lo mínimo.

### D. Adaptación de los tests existentes (solo para que compile)

- Nuevo `Ritrama2025.Tests/Stubs/InventarioServiceStub.cs`: implementa
  `IInventarioService` entero (14 miembros). Los 13 miembros que la pestaña no usa
  hacen `throw new NotImplementedException();` (mismo estilo que
  `Stubs/ProductsImportServiceStub.cs` y el `ConsecutivosServiceStub` de los tests).
  Los dos miembros de apoyo:

```csharp
/// <summary>Tabla que devolvera la proxima busqueda.</summary>
public DataTable ResultadoMasters { get; set; } = new();

/// <summary>Codigos pedidos, en orden (para comprobar la carga perezosa).</summary>
public List<string> CodigosConsultados { get; } = [];

public Task<DataTable?> BuscarMastersDeProducto(string productId)
{
    CodigosConsultados.Add(productId);
    return Task.FromResult<DataTable?>(ResultadoMasters);
}
```

- `FrmProductosLayoutTests.FrmProductosSinDialogos` y
  `FrmProductosFiltroCategoriaTests.FrmProductosSinDialogos`: parámetro extra
  `IInventarioService inventario` pasado a `base(...)`.
- `CrearFormularioProbable` (Layout, línea 195) y `CrearFormulario` (Filtro, línea 195):
  pasan `new Stubs.InventarioServiceStub()`. En `FrmProductosLayoutTests` añade una
  **sobrecarga** `CrearFormularioProbable(catalogo, out servicio, out Stubs.InventarioServiceStub inventario)`
  para que los tests nuevos puedan programar el stub sin tocar los ~10 sitios existentes
  (la sobrecarga original delega en la nueva).

Verificación de la tarea: build 0/0 (Global Constraint 1) y la suite completa verde
(Global Constraint 2).

## Task 3 — Tests de la pestaña

Fichero: `Ritrama2025.Tests/FrmProductosLayoutTests.cs`.

Usa la sobrecarga con `out inventario` de la Task 2. Localiza los controles con
`form.Controls.Find("...", true)` (patrón ya usado en el fichero) y copia de los tests
existentes el patrón para cambiar la fila activa del grid de productos.

Tests nuevos (nombres exactos):

1. `LaPestañaDeInventarioExisteConSusNueveColumnas`
   - existen `tabInventarioProducto`, `tlpInventario`, `lblInventarioVacio` y `gridMasters`;
   - `gridMasters.Columns.Count == 9` y las cabeceras, en orden: `Rollid`, `Width`,
     `Length`, `Consumido`, `Restante`, `% Disponible`, `Origen`, `Estado`, `Documento OC`;
   - `ReadOnly == true`, `AllowUserToAddRows == false`, `AllowUserToDeleteRows == false`,
     `MultiSelect == false`.
2. `AlAbrirLaPestañaSeCarganLosMastersDelProductoSeleccionado`
   - catálogo con un producto; selecciona su fila; programa el stub con 2 filas
     (monta un `DataTable` con las columnas `roll_id`, `lenght`, `largo_consumido`,
     `largo_restante`, `tipo_mov`, `estado`, `documento_oc`, `part_number`);
   - `tabDetalle.SelectedIndex = 1`;
   - `inventario.CodigosConsultados` == `["<codigo>"]` (igualdad exacta, un solo código);
   - `gridMasters.Rows.Count == 2` y la primera fila muestra el `roll_id` esperado;
   - `lblInventarioVacio.Visible == false`.
3. `LaPestañaMuestraElMensajeCuandoNoHayMasters` — stub con tabla vacía →
   `gridMasters.DataSource` sin filas, `lblInventarioVacio.Visible == true` y
   `Text == "Este producto no tiene masters en inventario"`.
4. `SinProductoSeleccionadoNoSeConsultaElServicio` — con el catálogo vacío, cambiar a la
   pestaña: `CodigosConsultados` vacío y el mensaje visible.
5. `CambiarDeProductoConLaPestañaAbiertaRecargaElInventario` — dos productos; con la
   pestaña ya abierta y cargada, cambia la fila activa →
   `inventario.CodigosConsultados` contiene el código nuevo y el grid se repinta.
6. `LaPestañaNoSeVuelveASiSiElProductoNoCambia` — abrir la pestaña dos veces seguidas con
   la misma fila activa → `CodigosConsultados.Count == 1`.

Todo con stubs (sin BD). Verificación: build 0/0 y suite completa verde.

## Verificación final

- Build 0 errores / 0 advertencias.
- Suite completa en verde.
- Revisión final de todo el diff de las tres tareas.
