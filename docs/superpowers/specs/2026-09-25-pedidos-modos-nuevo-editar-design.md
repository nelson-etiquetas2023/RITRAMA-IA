# Diseño: modos Nuevo/Editar/Anular en el módulo de Pedidos

- **Fecha:** 2026-09-25
- **Módulo:** `Ritrama2025` (WinForms .NET 10)
- **Alcance:** `Forms/FrmPedidos`, `Services/PedidoService` y sus pruebas
- **Fuera de alcance:** Orden de Corte y Despacho (el vínculo automático de estado se difiere)

## 1. Problema

Hoy `FrmPedidos` es un formulario de solo consulta. Los botones `btnNuevo`, `btnEditar`
(barra de herramientas), `btnEditarProducto` y `btnEliminarProducto` existen en el
diseñador pero no tienen handler, y los métodos de escritura del servicio
(`GetNewNumeroPedido`, `SavePedidoCompleto`, `AnularPedido`, `ActualizarEstadoPedido`)
no se invocan desde ningún punto de producción: solo desde las pruebas.

Además, el detalle tiene dos defectos que impiden construir el flujo de escritura:

- `BtnAddProducto_Click` (`Forms/FrmPedidos.cs:450`) inserta los valores en columnas
  equivocadas: escribe `"ROLLO"` en la columna de nombre de producto y la descripción
  real en la columna de unidad, deja el total vacío y no conserva el `product_id`.
- `LlenarGridDetalle` (`Forms/FrmPedidos.cs:267-283`) proyecta la fila de la base a
  celdas del grid y descarta el `product_id`, que es `NOT NULL` en
  `pedido_detalle` (`Scripts/Setup_Pedidos.sql:56`). Cualquier guardado de esas
  líneas fallaría por violación de la columna.

## 2. Decisiones tomadas

| Decisión | Valor | Motivo |
|---|---|---|
| Comportamiento por defecto | **Todos** los controles de la pestaña General en solo lectura, declarado en el diseñador | En esta pantalla no se hacen cambios: solo lectura es el estado base y la escritura exige entrar explícitamente a `Nuevo` o `Editar` (ver sección 5) |
| Estados de la UI | `Consulta`, `Nuevo`, `Editar` | Un modo único por formulario, sin condicionales dispersos |
| Gestión del estado del pedido | Automática por OC/Despacho, **diferida** | `orden_corte.pedido_id` y `despacho.pedido_id` existen en la base de datos pero ningún código los escribe; la OC solo tiene el texto libre `SellOrder` |
| Anulación | Permitida siempre, solo marca `anulado = 1` | Requisito del usuario |
| Edición de líneas | Botones + editor en la propia pestaña | Coherente con los botones ya diseñada; evita edición por celda |
| Totales | Recalculados en el cliente, %ITBIS editable (18 por defecto) | Requisito del usuario |
| Numeración | El consecutivo se consume al pulsar Nuevo | Requisito del usuario; puede dejar huecos si se cancela |
| Persistencia | Dos métodos de servicio explícitos | Contrato claro, sin parámetros booleanos |

## 3. Alcance

Archivos a modificar:

- `Forms/FrmPedidos.cs`
- `Forms/FrmPedidos.Designer.cs` (controles nuevos de %ITBIS, precio y notas de línea, y botones de la barra)
- `Services/PedidoService/IPedidoService.cs`
- `Services/PedidoService/PedidoService.cs`
- `Ritrama2025.Tests/PedidoServiceTests.cs`

No se modifican `OrdenCorteService`, `FrmOrdenCorte`, `DespachoService` ni sus formularios.

## 4. Arquitectura

### 4.1 Modo del formulario

`FrmPedidos` incorpora un enumerado privado y un método único que aplica el modo:

```csharp
private enum ModoFormulario { Consulta, Nuevo, Editar }

private ModoFormulario _modo = ModoFormulario.Consulta;

private void AplicarModo(ModoFormulario modo)
```

`AplicarModo` es el único lugar que decide qué campos se habilitan, qué botones se
muestran y si el grid de la izquierda admite selección. Ningún handler modifica
`ReadOnly` por su cuenta.

El modo inicial es `Consulta` y `AplicarModo(ModoFormulario.Consulta)` se invoca al
terminar la carga del formulario, de modo que el formulario arranque y termine siempre
en solo lectura completa. Los valores por defecto de solo lectura ya viven en el
diseñador (sección 5), por lo que un fallo en esta llamada no dejaría el formulario
editable.

### 4.2 Fuente de verdad del detalle

Se agrega `private readonly List<PedidoDetalle> _lineas = new();` como fuente de verdad:

- En `Consulta`, `_lineas` se llena al cargar el detalle del pedido seleccionado,
  **incluyendo `product_id`**.
- `LlenarGridDetalle` pasa a ser una proyección de `_lineas`; cada
  `DataGridViewRow` recibe `_lineas[i]` en `Tag` para identificar la línea sin
  depender de índices visibles.
- Agregar, editar y eliminar-lines operan sobre `_lineas` y vuelven a proyectar.
- Al guardar se construye el `Pedido` a partir de `_lineas`.

Esto elimina la pérdida de `product_id` y elimina el acoplamiento actual entre
posición de fila y línea de negocio.

### 4.3 Reconstrucción del encabezado

Para editar no se agrega un método de carga nueva: el `DataRowView` seleccionado de
`_dtPedidos` ya trae las 17 columnas de `SQL_SELECT_PEDIDOS` (`R.cs:46`), incluidas
`persona_contacto`, `condiciones_pago` y `tipo_venta`, que hoy no tienen control en
pantalla. Un método privado `ConstruirPedidoDesdeFila(DataRowView)` arma el `Pedido`
completo y evita que la edición borre esas columnas.

## 5. Solo lectura por defecto (requisito principal)

**En la pestaña General no se pueden hacer cambios.** El estado por defecto del
formulario es solo lectura para *todos* los controles, sin excepción, y ese estado se
declara en el diseñador, no depende de que el código lo aplique correctamente:

1. `FrmPedidos.Designer.cs` fija `ReadOnly = true` en los 17 controles de entrada de
   `tabGeneral` (los 14 preexistentes más `uiTextBox7`, `uiTextBox8` y `uiTextBox9`, que
   este plan agrega), y `Enabled = false` en los cuatro botones del editor de línea
   (`btnAddProducto`, `btnEditarProducto`, `btnEliminarProducto`, `btnBuscarProducto`).
   Esto vale desde el diseño, antes de que corra una sola línea de código.
2. `uiDataGridView1` conserva `ReadOnly = true` y `AllowUserToAddRows = false`.
3. `AplicarModo(ModoFormulario.Consulta)` se invoca en la carga del formulario, de modo
   que el estado de solo lectura también queda garantizado en ejecución.
4. `AplicarModo` es el **único** método que cambia `ReadOnly` o `Enabled`, y siempre
   devuelve el formulario a solo lectura completa al salir de `Nuevo` o `Editar`
   (vía `Guardar` o `Cancelar`).
5. Ningún handler modifica `ReadOnly` por su cuenta. Si un control se habilita fuera
   de `AplicarModo`, es un defecto.

### 5.1 Inventario de controles de `tabGeneral`

| Control | Campo | Consulta | Nuevo | Editar |
|---|---|---|---|---|
| `uiTextBox1` | Número de pedido | Solo lectura | Solo lectura (asignado) | Solo lectura |
| `uiDatetimePicker1` | Fecha de registro | Deshabilitado | Deshabilitado | Deshabilitado |
| `uiDatetimePicker2` | Fecha de entrega | Deshabilitado | Editable | Editable |
| `cbo_customers` | Cliente | Deshabilitado | Editable | Editable |
| `uiComboBox1` | Vendedor | Deshabilitado | Editable | Editable |
| `uiTextBox2` | Estado | Solo lectura | Solo lectura | Solo lectura |
| `uiRichTextBox1` | Dirección de entrega | Solo lectura | Editable | Editable |
| `uiRichTextBox2` | Ship To (derivado) | Solo lectura | Solo lectura | Solo lectura |
| `uiRichTextBox3` | Notas / Comentario | Solo lectura | Editable | Editable |
| `uiTextBox4` | Subtotal | Solo lectura | Solo lectura (calculado) | Solo lectura (calculado) |
| `uiTextBox5` | Monto ITBIS | Solo lectura | Solo lectura (calculado) | Solo lectura (calculado) |
| `uiTextBox6` | Total | Solo lectura | Solo lectura (calculado) | Solo lectura (calculado) |
| `uiComboBox2` | Producto de la línea | Deshabilitado | Habilitado | Habilitado |
| `uiTextBox3` | Cantidad de la línea | Deshabilitado | Habilitado | Habilitado |
| `uiTextBox7` *(nuevo)* | % ITBIS | Solo lectura | Editable (18 por defecto) | Editable |
| `uiTextBox8` *(nuevo)* | Precio de la línea | Deshabilitado | Habilitado | Habilitado |
| `uiTextBox9` *(nuevo)* | Notas de la línea | Deshabilitado | Habilitado | Habilitado |
| `btnBuscarProducto` | Buscar producto | Oculto | Visible | Visible |
| `btnEditarProducto` | Editar línea | Oculto | Visible | Visible |
| `btnEliminarProducto` | Eliminar línea | Oculto | Visible | Visible |
| `uiDataGridView1` | Detalle | Solo lectura | Solo lectura (proyección) | Solo lectura (proyección) |

`persona_contacto`, `condiciones_pago` y `tipo_venta` no tienen control en pantalla:
se conservan tal cual, nunca se editan desde esta pantalla.

### 5.2 Comportamiento por modo

| | Consulta | Nuevo | Editar |
|---|---|---|---|
| Encabezado | Todos solo lectura | Editable salvo número y fecha de registro | Editable salvo número y fecha de registro |
| Estado | Solo lectura | Solo lectura | Solo lectura |
| Subtotal / ITBIS / Total | Solo lectura | Solo lectura (calculado) | Solo lectura (calculado) |
| Editor de línea | Deshabilitado | Habilitado | Habilitado |
| Botones de línea | Ocultos | Agregar, Editar, Eliminar | Agregar, Editar, Eliminar |
| Botones de la barra | Nuevo, Editar, Anular | Guardar, Cancelar | Guardar, Cancelar |
| Selección en el grid izquierdo | Activa | Bloqueada | Bloqueada |

Mientras el formulario está en `Nuevo` o `Editar`, el grid de pedidos de la izquierda
no cambia de selección, para que un clic en otro pedido no descarte el borrador en
silencio. `Cancelar` vuelve a `Consulta` y recarga la lista.

"Ship To" es derivado: al elegir cliente se muestra el `customer_name` del cliente
seleccionado, y al visualizar un pedido se mantiene el texto que hoy compone
`customer_name` más `persona_contacto` (`FrmPedidos.cs:197-201`). No se agrega
`customer_contact` a la consulta de clientes porque no hace falta para esta iteración.
En un pedido nuevo, `persona_contacto`, `condiciones_pago` y `tipo_venta` quedan
vacíos porque la pantalla no ofrece control para ellos.

## 6. Controles nuevos

En `FrmPedidos.Designer.cs`, siguiendo el patrón del diseñador actual:

- `uiLabel13` + `uiTextBox7` para el porcentaje de ITBIS, junto a los campos de
  importes.
- `uiLabel14` + `uiTextBox8` para el precio de la línea, y `uiTextBox9` para las notas de
  la línea, en la zona del editor de línea.
- `btnGuardar` y `btnCancelar` como `ToolStripButton` en `barraHerramientas`.
- `btnAnular` como `ToolStripButton` en `barraHerramientas`.

Los controles de texto usan `ReadOnly`, que en SunnyUI 3.9.8 sí bloquea la escritura
(verificado en el IL: `UITextBox.ReadOnly` y `UIRichTextBox.ReadOnly` escriben en el
`TextBoxBase` interno). Los combos y los selectores de fecha necesitan además
`Enabled = false` en solo lectura: `UIDropControl.ReadOnly` solo escribe en el
`TextBoxBase` interno, mientras `UIComboBox.ListBox_Click` y `Edit_KeyDown` siguen
cambiando el valor y el desplegable se sigue abriendo con el ratón, de modo que
`ReadOnly` por sí solo no impide elegir otro cliente. Los botones se ocultan o se
deshabilitan según el modo.

## 7. Editor de línea

- **Agregar:** toma el producto del combo, la cantidad, el precio y las notas; valida
  que el producto esté elegido y que cantidad sea mayor que cero; agrega la línea a
  `_lineas`, recalcula totales y refresca el grid.
- **Editar:** requiere fila seleccionada; carga sus valores en los controles del
  editor y, al pulsar de nuevo, reemplaza la línea identificada por su `Tag`.
- **Eliminar:** requiere fila seleccionada y confirmación; la quita de `_lineas`.
- El precio por defecto es el del catálogo (`producto.precio`); el usuario puede
  pisarlo, en cuyo caso se guarda el valor modificado.

## 8. Validación y cálculos

Al agregar, editar o eliminar una línea, y antes de guardar:

```
Subtotal     = Σ (Cant × Precio)
Monto_Itbis  = Subtotal × Porc_Itbis / 100
Total        = Subtotal + Monto_Itbis
Total_Renglon= Cant × Precio
```

Validación mínima antes de guardar (formulario y servicio):

- `Numero > 0`.
- `Customer_Id != Guid.Empty`.
- Al menos una línea en el detalle.
- Cada línea: `Product_id` no vacío, `Cant > 0`, `Precio >= 0`.
- `Porc_Itbis >= 0`, `Subtotal`, `Monto_Itbis` y `Total` no negativos.
- `Estado` dentro de los valores de `PedidoEstado`.

## 9. Servicio

### 9.1 Contrato

```csharp
string? ErrorMsg { get; }
Task<int> GetNewNumeroPedido(CancellationToken cancellationToken = default);
bool SavePedidoCompleto(Pedido pedido);
bool ActualizarPedidoCompleto(Pedido pedido);
bool AnularPedido(int numero);
bool ActualizarEstadoPedido(int numero, string estado);
```

Se conserva la firma actual de los métodos existentes. Se agrega
`ActualizarPedidoCompleto` y se expone `ErrorMsg` en el contrato, porque la pantalla
necesita leer el motivo del rechazo sin conocer la implementación concreta del servicio.
`ErrorMsg` es `string?` y vale `null` en el constructor.

### 9.2 `ActualizarPedidoCompleto`

Una sola transacción:

1. `UPDATE pedido SET ... WHERE numero = @p1` con **todos** los campos del
   encabezado. Los que no tienen control en pantalla conservan el valor cargado
   desde la fila, porque el `Pedido` se reconstruye completo. Si `RowsAffected != 1`
   se hace rollback y se informa que el pedido no existe.
2. `DELETE FROM pedido_detalle WHERE numero = @p1`.
3. Reinserción de las líneas de `_lineas`.
4. `Commit`; ante cualquier excepción, `Rollback`, se registra la causa y se
   devuelve `false`.

El reemplazo completo del detalle evita tener que distinguir líneas nuevas,
modificadas y eliminadas.

Las dos sentencias nuevas siguen la convención del repositorio y se agregan en
`R.QUERY.COMMERCIAL`: `SQL_UPDATE_PEDIDO` y `SQL_DELETE_PEDIDO_DETALLE`. Los
parámetros se nominan `@p1`, `@p2`, … como en el resto de las consultas de
pedidos.

### 9.3 Validación compartida

Un método privado `Validar(Pedido pedido, out string error)` se invoca desde
`SavePedidoCompleto` y `ActualizarPedidoCompleto`. Devuelve `false` con un mensaje en
español orientado al usuario. Elimina la situación actual en la que un
`product_id` nulo llega a la base y revienta la transacción por `NOT NULL`.

### 9.4 Anulación

`AnularPedido` se mantiene: `UPDATE pedido SET anulado = 1 WHERE numero = @p1`. No
verifica órdenes de corte ni despachos. Como `SQL_SELECT_PEDIDOS` filtra
`anulado = 0` (`R.cs:46`), el pedido desaparece de la lista tras anular; la interfaz
informa que la operación se realizó. Este comportamiento es aceptado de forma
consciente.

### 9.5 Estado

`ActualizarEstadoPedido` se conserva sin cambios de firma, disponible para que el
vínculo con OC y Despacho lo invoque en el futuro. En esta iteración el estado se
fija en `PedidoEstado.Creado` al crear y se conserva el valor actual al editar.

## 10. Errores y permisos

- Un único `ServiceErrors.Report` por fallo, con mensaje en español que incluya el
  número de pedido y la causa, en lugar de exponer `ex.Message` crudo.
- `SavePedidoCompleto` deja de silenciar el rollback con `catch { }` vacío y registra
  la causa real.
- Permisos por acción usando `Helpers/PermisoHelper.cs`:
  - `btnNuevo` → `Pedidos:Crear`
  - `btnEditar` → `Pedidos:Editar`
  - `btnAnular` → `Pedidos:Eliminar`
  - `btnGuardar` → `Pedidos:Crear` en `Nuevo`, `Pedidos:Editar` en `Editar`

  `Main.cs:342` ya exige `Pedidos:Ver` para abrir el formulario; esta iteración
  añade el detalle por acción.

## 11. Pruebas

Pruebas de integración nuevas en `Ritrama2025.Tests/PedidoServiceTests.cs`:

- `ActualizarPedidoCompleto_ActualizaEncabezadoYReemplazaDetalle`
- `ActualizarPedidoCompleto_PedidoInexistenteDevuelveFalse`
- `ActualizarPedidoCompleto_LineaInvalida_HaceRollback`
- `SavePedidoCompleto_DetalleConProductIdNulo_RechazadoConMensaje`

Todas usan un número de pedido generado por corrida, nunca los valores fijos
`950000` y `950001` que hoy pueden borrar pedidos reales, y restauran el consecutivo
en un bloque `finally`.

No se agregan pruebas de UI: el proyecto no tiene un harness de WinForms y
`FrmPedidos` depende de `Sunny.UI`, que requiere un handle de ventana.

## 12. Fuera de alcance

- Vínculo `orden_corte.pedido_id` / `despacho.pedido_id` y avance automático de estado.
- Script de migración para poblar `pedido_id` en las OC existentes.
- Pruebas automatizadas de la interfaz.
- Corrección del resto de hallazgos de la revisión (paralelismo del fixture,
  `EscapeLike` con corchetes, advertencias de nulabilidad, `GetNewNumeroPedido`
  sin `CancellationToken`).

## 13. Verificación

1. `dotnet build Ritrama2025.sln` sin errores.
2. Ejecución manual del flujo: Nuevo → agregar 2 líneas → verificar totales → Guardar →
   el pedido aparece en el listado.
3. Editar el pedido guardado → cambiar cantidad de una línea → Guardar → verificar que
   el detalle se reemplazó y que las columnas sin control en pantalla conservaron su
   valor.
4. Anular → el pedido desaparece del listado.
5. Abrir un pedido existente sin entrar en modo edición → todos los controles en solo
   lectura.
6. Las pruebas de `PedidoServiceTests` solo se ejecutan con confirmación de que la
   base de datos de pruebas es segura, porque escriben en la base configurada.
