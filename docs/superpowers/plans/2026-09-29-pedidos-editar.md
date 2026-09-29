# Plan: Modo Editar en Pedidos (encabezado + renglones)

- **Fecha:** 2026-09-29
- **Spec:** `docs/superpowers/specs/2026-09-25-pedidos-modos-nuevo-editar-design.md`
- **Alcance:** `Forms/FrmPedidos.cs`, `Forms/FrmPedidos.Designer.cs`, `R.cs`, `Services/PedidoService/*`, `Ritrama2025.Tests/PedidoServiceTests.cs`

## Contexto y estado actual

Ya implementado (modo Nuevo): `_lineas` como fuente de verdad, `AplicarModo`, editor de
línea (agregar/editar/eliminar), `SavePedidoCompleto` transaccional, `PedidoValidador`,
permisos por acción.

### Gaps identificados (G1..G7)

| # | Gap | Dónde |
|---|---|---|
| G1 | `ModoFormulario` solo tiene `Consulta`/`Nuevo` | FrmPedidos.cs:181-185 |
| G2 | `btnEditar` existe pero **sin handler Click** y `Enabled=false` | Designer.cs:1188; ctor solo wirea Nuevo/Guardar/Cancelar (FrmPedidos.cs:132-134) |
| G3 | `BtnGuardar_Click` hace guard `_modo != Nuevo` → return temprano | FrmPedidos.cs:1505 |
| G4 | Toda la lógica de editabilidad usa `esNuevo` (~25 sitios) | FrmPedidos.cs:196-280 |
| G5 | Guards de editor de línea con `_modo != Nuevo` (medidas, cargar selección, condiciones pago, direcciones al cambiar cliente) | 1108, 1148, 1246, 1335 |
| G6 | `CargarPedidoEnGeneral` **nunca carga `porc_itbis`** en `uiTextBox7` → al editar se recalcularía con 18% y se pisaría el % real | 616-662 |
| G7 | No existe `ActualizarPedidoCompleto` ni SQL `SQL_UPDATE_PEDIDO`/`SQL_DELETE_PEDIDO_DETALLE` | IPedidoService.cs, R.cs:44-75 |

## Decisiones

| Decisión | Valor | Motivo |
|---|---|---|
| Estados | `Consulta`, `Nuevo`, `Editar` | Spec §2 |
| Interruptor único | helper `bool EsEditable => _modo is Nuevo or Editar` | Reemplaza `esNuevo` donde importa "puede escribir"; `esNuevo` solo donde es realmente solo-Nuevo |
| Reconstrucción del encabezado | Desde **controles de pantalla** vía `ConstruirPedidoDesdeFormulario`, con `Numero` tomado de `uiTextBox1` | Las 19 columnas tienen control en pantalla (contacto/condiciones/tipo_venta ya existen); no hace falta un segundo builder |
| Fecha registro | **Editable** en Editar (igual que Nuevo) | Misma razón del comentario en 205-207 (fechas anteriores a la del sistema); el código ya divergió del spec para Nuevo — coherencia con el código |
| `% ITBIS` | Cargar de la fila (fix G6); editable en Editar | Los totales deben round-trip |
| Persistencia del detalle | `UPDATE` encabezado + `DELETE` detalle + re-`INSERT` en 1 transacción | Spec §9.2; evita diff de líneas nuevas/modificadas/borradas |
| Número al editar | Nunca consume consecutivo; `pedido.Numero` viene de la pantalla | `SavePedidoCompleto` reserva; actualizar no |
| Pedidos anulados | **Bloquear** edición de `anulado=1` (mensaje) | La anulación es acción aparte vía switch |
| `sw_anular_pedido` | Habilitado solo en `Consulta` | Cambiar estado no es parte de editar |
| Direcciones Bill To / Ship To | Solo lectura siempre; se refrescan del maestro **al cambiar cliente en Nuevo/Editar** (guard pasa a `_modo == Consulta`) | El pedido se congela al guardar; pero si el usuario cambia de cliente mientras edita, mostrar la dirección del cliente viejo estaría mal |
| Títulos de diálogo | Helper `TituloModo()` → "Pedido nuevo" / "Edición de pedido" | ~10 strings "Pedido nuevo" hardcodeados |
| UX tras guardar | Recargar lista y **re-seleccionar el pedido guardado** | El usuario ve el resultado de su edición |

## Global Constraints

- `AplicarModo` es el **único** método que cambia `ReadOnly`/`Enabled` de la pestaña General.
- Ningún handler modifica `ReadOnly` por su cuenta (defecto si lo hace).
- Combo/fecha en solo lectura: `Enabled = false` además de `ReadOnly` (SunnyUI 3.9.8: `ReadOnly` no bloquea el desplegable).
- Parámetros SQL nominados `@p1..@p19`, mismo orden/valores que `SQL_INSERT_PEDIDO` (incl. `total$`).
- Los tests de integración usan número generado por corrida (nunca 950000/950001) y restauran el consecutivo en `finally`.
- Un solo `ServiceErrors.Report` por fallo, con mensaje en español que incluya el número del pedido.
- No se modifican `OrdenCorteService`, `FrmOrdenCorte`, `DespachoService` ni sus formularios.
- `dotnet build --no-restore` sin errores.

## Tasks

### Task 1: SQL + servicio `ActualizarPedidoCompleto`

Archivos: `R.cs` (COMMERCIAL), `Services/PedidoService/IPedidoService.cs`, `Services/PedidoService/PedidoService.cs`.

1. En `R.cs` después de `SQL_UPDATE_PEDIDO_ESTADO` (línea 52) agregar:

```csharp
internal static string SQL_UPDATE_PEDIDO = "UPDATE pedido SET fecha=@p2, customer_id=@p3, customer_name=@p4, vendor_id=@p5, persona_contacto=@p6, tipo_venta=@p7, fecha_entrega=@p8, condiciones_pago=@p9, prioridad=@p10, direccion_entrega=@p11, direccion_facturacion=@p12, estado=@p13, notas=@p14, anulado=@p15, subtotal=@p16, porc_itbis=@p17, itbis=@p18, total$=@p19 WHERE numero=@p1";
internal static string SQL_DELETE_PEDIDO_DETALLE = "DELETE FROM pedido_detalle WHERE numero = @p1";
```

2. En `IPedidoService` agregar: `bool ActualizarPedidoCompleto(Models.Pedido pedido);` con doc comment en español.

3. En `PedidoService.cs` implementar `ActualizarPedidoCompleto` como espejo de `SavePedidoCompleto` (líneas 280-383) **sin** reserva de número:
   - `conn.Open()` → `BeginTransaction()`.
   - `PedidoValidador.EsValido` → fail: rollback, `ErrorMsg`, return false.
   - `SQL_UPDATE_PEDIDO` con `@p1 = pedido.Numero` + 18 campos, mismo setup de `SqlParameter` que el INSERT (incl. `SqlDbType.UniqueIdentifier` en customer_id/vendor_id).
   - `RowsAffected != 1` → rollback, `ErrorMsg = "El pedido {numero} ya no existe o fue modificado por otro usuario."`, return false.
   - `SQL_DELETE_PEDIDO_DETALLE` (`@p1`).
   - Re-insertar líneas.
   - Commit. `catch` → rollback (logueando fallo de rollback como 363-373), `ErrorMsg`, `ServiceErrors.Report("Error al actualizar el pedido ...")`, false.
   - Extraer helper privado `InsertarDetalle(SqlConnection, SqlTransaction, Pedido)` usado por `SavePedidoCompleto` y `ActualizarPedidoCompleto` (elimina el bloque duplicado de ~17 líneas).

### Task 2: Tests de `ActualizarPedidoCompleto`

Archivo: `Ritrama2025.Tests/PedidoServiceTests.cs`.

Tests `[SkippableFact]` con los `Skip.If` existentes, número generado por corrida, consecutivo restaurado en `finally`:

1. `ActualizarPedidoCompleto_ActualizaEncabezadoYReemplazaDetalle` — crear → modificar campos del encabezado + agregar/quitar líneas → actualizar → re-leer y asertar nuevo encabezado, cantidad/contenido de líneas, y que `GetProximoNumeroPedido` **no avanzó**.
2. `ActualizarPedidoCompleto_PedidoInexistenteDevuelveFalse` — número falso → false + `ErrorMsg` no null.
3. `ActualizarPedidoCompleto_LineaInvalida_HaceRollback` — crear válido; actualizar con línea `Product_id = null` → false; re-leer encabezado **y** detalle sin cambios (prueba rollback).

### Task 3: Formulario — modo Editar completo

Archivos: `Forms/FrmPedidos.cs`, `Forms/FrmPedidos.Designer.cs`.

1. **Enum (181-185):** agregar `Editar`.
2. **Helper:** `private bool EsEditable => _modo is ModoFormulario.Nuevo or ModoFormulario.Editar;`
3. **`AplicarModo` (193-281):** reemplazar `esNuevo` por `esEditable` en: `uiDatetimePicker1/2`, `uiRichTextBox3`, `uiTextBox3/7/8/9`, combos (`cbo_customers`, `uiComboBox1/2`, `cboTipoVenta`, `cbo_prioridad`, `txtPersonaContacto`), botones de línea (Visible+Enabled), `gridPedidos.Enabled`. Barra:
   ```csharp
   btnNuevo.Visible   = _modo == ModoFormulario.Consulta;
   btnEditar.Visible  = _modo == ModoFormulario.Consulta;
   btnGuardar.Visible = EsEditable;
   btnCancelar.Visible= EsEditable;
   sw_anular_pedido.Enabled = _modo == ModoFormulario.Consulta && _filaPedidoActual != null;
   ```
   Quitar comentario "btnEditar llega en el plan siguiente" (266-267). Llamar `AplicarEditabilidadMedidas()` y `AplicarEditabilidadCondicionesPago()` desde aquí (hoy solo las llama `BtnNuevo`) para que Editar también las aplique.
4. **Wire handler en el ctor (~132-134):** `btnEditar.Click += BtnEditar_Click;`
5. **`BtnEditar_Click` (nuevo):**
   - `PermisoHelper.PuedeEditar("Pedidos")` si no, advertencia.
   - Requerir `_filaPedidoActual != null`; si `EsAnulado(_filaPedidoActual)` → advertencia y return.
   - `async`: `await CargarDetallePedidoAsync(numero)` para que `_lineas` esté completo (evita race con la carga al seleccionar).
   - `AplicarModo(Editar)`; `ActualizarTotalesEnPantalla()`; focus `cbo_customers`.
6. **`BtnGuardar_Click` (1505):** guard → `if (!EsEditable) return;` + permiso (`PuedeCrear` en Nuevo / `PuedeEditar` en Editar); dispatch:
   ```csharp
   bool ok = _modo == ModoFormulario.Nuevo
       ? _pedidoService.SavePedidoCompleto(pedido)
       : _pedidoService.ActualizarPedidoCompleto(pedido);
   ```
   En Editar: `pedido.Numero = uiTextBox1.Text.Trim()` antes de validar (el validador salta Numero; el servicio lo necesita para el `WHERE`). Éxito → mensaje con `TituloModo()`, `LimpiarGeneral`, limpiar `_lineas`, `AplicarModo(Consulta)`, `await RecargarListadoAsync()`, y **re-seleccionar la fila por número**.
7. **`BtnCancelar_Click` (1539):** texto de diálogo vía `TituloModo()`; ya vuelve a Consulta + reload.
8. **Guards G5:** `_modo != ModoFormulario.Nuevo` → `!EsEditable` en 1108 (al cambiar cliente llenar direcciones), 1148 (condiciones pago), 1246 (medidas), 1335 (cargar selección en editor).
9. **Fix G6 en `CargarPedidoEnGeneral` (~662):** cargar `uiTextBox7` desde la columna `porc_itbis` (default "18" si no parsea), **después** de los totales (660-662) para que `TextChanged` no dispare un recálculo fuera de orden.
10. **`btnEditar.Enabled`:** en `CargarPedidoEnGeneral` → `btnEditar.Enabled = !EsAnulado(drv.Row)`; en `LimpiarGeneral` → `btnEditar.Enabled = false`.
11. **`ConstruirPedidoDesdeFormulario`:** en Editar también asignar `Numero` desde pantalla y `Anulado` desde `_filaPedidoActual` (preservar). `Estado` conserva su valor (campo solo-lectura ya lo trae).
12. **`TituloModo()`** helper; reemplazar "Pedido nuevo" en diálogos del editor de línea / guardar / cancelar.
13. **Designer:** `btnEditar.Enabled = true` (línea 1188) — la visibilidad la controla `AplicarModo`; verificar que al arrancar el form `btnEditar` queda deshabilitado (no hay fila seleccionada al inicio) o deshabilitarlo explícitamente tras la carga inicial.

## Verificación

1. `dotnet build --no-restore` → 0 errores.
2. `dotnet test` (tests de Pedido; skip-gated por disponibilidad de BD).
3. Prueba manual: abrir pedido → todo solo lectura → Editar → cambiar cliente/fecha/%ITBIS → editar y borrar renglón, agregar uno → totales recalculados → Guardar → la fila reaparece con datos nuevos; columnas sin control conservadas; **el consecutivo no avanzó**. Cancelar → descarta. Pedido anulado → Editar bloqueado. Guardar sin renglones → validación.

## Fuera de alcance (spec §12)

- Vínculo `orden_corte.pedido_id` / `despacho.pedido_id` y avance automático de estado.
- Pruebas automatizadas de interfaz; formularios de OC/Despacho.
- Hallazgos laterales: `EscapeLike` con corchetes, advertencias de nulabilidad.
