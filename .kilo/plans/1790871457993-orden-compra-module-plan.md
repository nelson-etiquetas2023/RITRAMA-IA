# Plan: Módulo Órdenes de Compra

## Objetivo

Crear un módulo `Órdenes de Compra` que replica los patrones arquitectónicos del módulo
`Pedidos de Ventas` (Sales Orders), adaptados a la compra (nosotros → proveedor). El módulo
incluye: tabla SQL idempotente, modelo, clases puras (número, cálculos, validación, mapper),
servicio, formulario WinForms con 30/70 + toolbar/edit modes, botón de sidebar, registro en DI,
queries SQL en `R.cs`, y pruebas unitarias + de layout + de integración.

## Contexto del Código Base (verificado)

- **Stack**: .NET 10 WinForms + **SunnyUI** (UIStyle.Green) + Microsoft.Data.SqlClient
- **DI**: `Microsoft.Extensions.DependencyInjection` + `IHost` (Generic Host) en `Program.cs`
- **Namespace**: `Ritrama2025` (root), forms en `Ritrama2025.Forms`, services en
  `Ritrama2025.Services.*`, models en `Ritrama2025.Models`
- **`[assembly: InternalsVisibleTo("Ritrama2025.Tests")]`** en `R.cs:1` — tests pueden
  acceder a internals
- **Tests**: xUnit v3, `FluentAssertions`, `[Trait("Categoria", "Unit")]` para pruebas sin DB,
  `[Trait("Categoria", "Integracion")]` para pruebas contra SQL Server de pruebas.
  `SkippableFact`/`Skip.If` para tests que requieren datos en la base.
- **Tema**: `LightGreenTheme` define `Primary`, `PrimaryDark`, `Background`, `AlternateRow`
- **UI**: `IAsyncFormLoad` para forms async (llamado por FormManager antes de mostrar),
  `IFormTemaClaro` para forms con tema propio (verde)
- **`FormManager.ShowForm<T>()`**: resuelve el form por DI, muestra `FrmLoading` mientras
  `InitializeAsync()` corre, luego crea una tab en `tabContent` y embebe el form
- **`PermisoHelper`**: `PuedeVer("Modulo")`, `PuedeCrear("Modulo")`, `PuedeEditar("Modulo")`,
  basados en `SesionActual.Permisos` con formato `"modulo:accion"` (ej. `"Pedidos:Ver"`)
- **`ConexionResolver.Resolver(config)`**: resuelve connection string de
  `config["Ambiente"]` + `config["ConnectionStringsEnvironment:" + ambiente]`
  (namespace interno `Ritrama2025.Services.ProduccionService`)
- **`ServiceErrors`** (`Services/ServiceErrors.cs`): `Report(string msg)` + `Notify` (MessageBox)
- **`ServiceLogger`** (`Services/ServiceLogger.cs`): `Log(string msg)` → archivo local o `Sink`
- **`CommonService.ADD_COLUMN_GRID(name, size, title, field_bd, grid)`**: helper para columnas

## Patrón Pedidos (referencia directa)

### Modelo (`Pedido.cs`)
- `Numero` es **`string`** con formato `"SO-#####"` (NO int en la base de datos).
  Comentario: "Se persiste como texto para que el prefijo viaje con el dato."
- `Customer_Id` es `Guid` (cliente usa GUID)
- `Vendor_Id` es `Guid?` (vendedor usa GUID)
- Campos: `Fecha`, `Customer_Id`, `Customer_Name`, `Vendor_Id`, `Persona_Contacto`,
  `Tipo_venta`, `Fecha_entrega`, `Condiciones_pago`, `Prioridad`, `Direccion_entrega`,
  `Direccion_facturacion`, `Estado`, `Notas`, `Anulado`, `SubTotal`, `Porc_Itbis`,
  `Monto_Itbis`, `Total`, `Detalle` (List<PedidoDetalle>)

### PedidoNumero (`PedidoNumero.cs`) — lógica pura
- `const string Prefijo = "SO-"`, `const int LargoNumero = 5`, `const int MaximoNumero = 99999`
- `Formatear(int)` → `"SO-00027"`
- `EsValido(string?)` → bool
- `ParteNumerica(string?)` → int (devuelve 0 si inválido)

### PedidoEstado (`PedidoEstado.cs`) — constantes
- `Creado = "creado"`, `EnProduccion = "en producción"`, `Pickeado = "pickeado"`,
  `Despachado = "despachado"`, `Devuelto = "devuelto"`

### PedidoCatalogos (`PedidoCatalogos.cs`) — vocabularios cerrados
- `Contado = "contado"`, `TipoVenta = {contado, credito}`,
  `CondicionesPago = {contado, "7 dias", "15 dias", "30 dias", "45 dias", "60 dias", credito}`,
  `Prioridad = {normal, urgente}`
- `EsContado(string)`, `CondicionesPagoEditable(string tipoVenta, bool esNuevo)`

### PedidoCalculos (`PedidoCalculos.cs`) — lógica pura
- `TotalRenglon(decimal cant, decimal? precio)` → `Math.Round(cant * precio, 2, AwayFromZero)`
- `TotalCantidad(IEnumerable<PedidoDetalle>)` → suma exacta (decimal)
- `Calcular(IEnumerable<PedidoDetalle>, decimal porcItbis)` → `(SubTotal, MontoItbis, Total)`

### PedidoDetalleMapper (`PedidoDetalleMapper.cs`) — lógica pura
- `Mapear(DataTable)` → `List<PedidoDetalle>`
- Helpers internos: `Texto(row, col)`, `Decimal(row, col)`, `DecimalNulo(row, col)`,
  `TieneColumna(row, col)`

### PedidoValidador (`PedidoValidador.cs`) — lógica pura
- `EsValido(Pedido?, out string error)` → valida: no null, Customer_Id != Guid.Empty,
  Porc_Itbis >= 0, montos >= 0, estado válido, detalle no vacío, cada línea: Product_id
  no vacío, Cant > 0, Precio >= 0
- `EsEstadoValido(string?)` → bool

### ConsecutivoCliente (`ConsecutivoCliente.cs`) — lógica pura
- Formatea el consecutivo del cliente/vendedor a 4 dígitos (`"D4"`)
- `Formatear(DataRow?)`, `FormatearValor(object?)`, `FormatearPorLlave(DataTable?, col, val)`,
  `BuscarFila(DataTable?, col, val)`, `BuscarFilaPorTexto(DataTable?, col, texto)`
- **Importante**: `BuscarFila` compara con `Equals` sobre el valor crudo (Guid contra Guid)

### PedidoDetalle (`PedidoDetalle.cs`)
- `Product_id`, `Product_name`, `Cant`, `Unidad`, `Width`, `Lenght`, `Msi`, `Precio?`,
  `Total_Renglon?`, `Notas`

### ClienteDatos (`ClienteDatos.cs`) — sealed
- `Consecutivo` (string, 4 dígitos), `DireccionFacturacion` (string),
  `DireccionEntrega` (string)

### PedidoMedidas (`PedidoMedidas.cs`) — lógica pura
- `TipoRolloCortado = "Rollo Cortado"`
- `PideMedidas(string? tipoProducto)` → true solo para Rollo Cortado
- `LeerMedida(string?, out decimal)` → parsea con es-ES luego invariant
- `Validar(string?, decimal ancho, decimal largo, out string? error)`

### IPedidoService / PedidoService (`PedidoService.cs`)
Constructor: `PedidoService(IConfiguration config)` — resuelve conn vía `ConexionResolver.Resolver(config)`

Métodos:
- `LoadDataPedidos(CancellationToken)` → DataTable (lista de pedidos)
- `LoadDataCustomers(CancellationToken)` → DataTable (combo de clientes)
- `LoadDataVendors(CancellationToken)` → DataTable (combo de vendedores)
- `LoadDataPedidoDetalle(string numero, CancellationToken)` → DataTable
- `BuscarClienteAsync(Guid customerId, CancellationToken)` → `ClienteDatos?`
- `GetProximoNumeroPedido(CancellationToken)` → Task<string> (previsualiza número)
- `SavePedidoCompleto(Pedido)` → bool (valida, reserva número con UPDLOCK, INSERT header+detalle)
- `ActualizarPedidoCompleto(Pedido)` → bool (valida, UPDATE header + DELETE+INSERT detalle)
- `AnularPedido(string numero)` → bool (UPDATE anulado=1)
- `RestaurarPedido(string numero)` → bool (UPDATE anulado=0)
- `ActualizarEstadoPedido(string numero, string estado)` → bool (valida estado, UPDATE)

### SQL Queries (`R.cs:44-76`, sección `R.QUERY.COMMERCIAL`)
- `SQL_SELECT_PEDIDOS` — SELECT completo con `numero` como string
- `SQL_SELECT_PEDIDO_DETALLE` — WHERE numero = @p1 (string)
- `SQL_INSERT_PEDIDO` — INSERT con @p1 = string numero (formato "SO-#####")
- `SQL_INSERT_PEDIDO_DETALLE` — INSERT, @p1 = string numero
- `SQL_UPDATE_PEDIDO` — UPDATE WHERE numero = @p1 (string)
- `SQL_DELETE_PEDIDO_DETALLE` — DELETE WHERE numero = @p1
- `SQL_ANULAR_PEDIDO` — UPDATE anulado=1 WHERE numero=@p1
- `SQL_RESTAURAR_PEDIDO` — UPDATE anulado=0 WHERE numero=@p1
- `SQL_UPDATE_PEDIDO_ESTADO` — UPDATE estado=@p2 WHERE numero=@p1
- `SQL_QUERY_CONSUMO_PEDIDO_CONSECUTIVO` — UPDLOCK reservation:
  `UPDATE control WITH (UPDLOCK, HOLDLOCK) SET par1 = par1 + 1 OUTPUT DELETED.par1 WHERE filter='PED'`
- `SQL_SELECT_PEDIDO_PROXIMO` — SELECT par1 FROM control WHERE filter='PED'
- `SQL_SELECT_LOAD_CUSTOMER_COMBO` — customer combo with direcciones
- `SQL_SELECT_LOAD_VENDOR_COMBO` — vendor combo (ROW_NUMBER for consecutivo)
- `SQL_SELECT_CLIENTE_POR_ID` — single customer query (consecutivo, direcciones)

### Base de datos (`Scripts/Setup_Pedidos.sql`)
**IMPORTANTE / CORRECCIÓN**: El script `Setup_Pedidos.sql` declara `numero INT NOT NULL`,
pero el código real (modelo Pedido, servicio, y tests) usa `numero` como **string**
(ej. "SO-00027"). Las pruebas en `PedidoServiceTests.cs` insertan y consultan `numero`
directamente como string: `INSERT INTO pedido (numero,...) VALUES (@p1,...)` con
`@p1 = numero` (string), y `SELECT COUNT(*) FROM pedido WHERE numero = @p1` con string.

**Conclusión**: La columna real `pedido.numero` en la base de datos es NVARCHAR/VARCHAR,
NO INT. El script `Setup_Pedidos.sql` es inconsistente/anticuado. La tabla `control`
almacena el contador como string en `par1`.

**Recomendación para OC**: `orden_compra.numero` debe ser `NVARCHAR(10)` para almacenar
"OC-00027", siguiendo el patrón real del módulo Pedidos, NO el script SQL incorrecto.

### Tablas
- `pedido` (header): `numero` (string), `fecha`, `customer_id` (Guid), `customer_name`,
  `vendor_id` (Guid), `persona_contacto`, `tipo_venta`, `fecha_entrega`, `condiciones_pago`,
  `direccion_entrega`, `direccion_facturacion`, `estado`, `notas`, `anulado` (bit),
  `subtotal`, `porc_itbis`, `itbis`, `total$`
- `pedido_detalle`: `numero` (FK a pedido.numero), `product_id`, `product_name`,
  `cant`, `unidad`, `width`, `lenght`, `msi`, `precio`, `total_renglon`, `notas`
- `control`: `filter` ('PED'), `par1` (string contador)

### Tabla `provider` (verificado desde R.cs y ProveedorService)
Columnas: `Proveedor_Id` (string, ej. "P-001"), `Proveedor_Name`, `phone`, `direccion`,
`email`, `anulado` (bit), `unidad_master_1` (bit), `unidad_master_2` (bit)
**Importante**: `Proveedor_Id` es STRING, no Guid. No hay direcciones separadas de
facturación/entrega — solo `direccion`.

### Proveedores service (`ProveedorService.cs`)
- Constructor: `ProveedorService(IConfiguration configuration)` — resuelve conn con
  `ConexionResolver.Resolver(config)` o `ResolveConnectionString(config)` (que verifica
  cadena no vacía, lanza `InvalidOperationException`)
- `LoadListadoAsync(CancellationToken)` → DataTable con columnas tipadas como string/bool
- `IProveedorService` solo tiene `LoadListadoAsync` por ahora (servicio en construcción)

### FrmProveedores (patrón 30/70 — base para OC)
- Hereda `UIForm`, implementa `IAsyncFormLoad, IFormTemaClaro`
- Constructor: `FrmProveedores(IProveedorService proveedoresService)` — DI inyecta el servicio
- Layout: `tlpRoot` (TableLayoutPanel, 2 columnas 30%/70%), `panelIzq`, `panelDer`,
  `gridProveedores` (UIDataGridView), `pnlBuscador` (txtBuscar + lblTitulo), `pnlResumen` (lblResumen),
  `tabDetalle` (UITabControl), `tabDetalleProveedor` (TabPage), `tlpDetalle` (caption|valor grid),
  `lblDetalleTitulo`, varios `txtValor*` / `lblCap*` pares
- Estilo: `UIStyleManager(components) { Style = UIStyle.Green, ... }`, `AplicarTemaVerde()`
- Búsqueda: `txtBuscar.TextChanged` → RowFilter sobre DefaultView
- Grid: `AutoGenerateColumns = false`, 3 columnas, `SelectionMode = FullRowSelect`
- Resumen: `lblResumen.Text = "Total: N proveedores"` o `"Mostrando X de Y proveedores"`
- Detalle: `RefrescarDetalle()` desde `SelectionChanged` / `CurrentCellChanged`

### FrmPedidos (patrón edit/save/switch — funcionalidad para OC)
- Hereda `UIForm`, implementa `IAsyncFormLoad, IFormTemaClaro`
- Constructor: `FrmPedidos(IPedidoService, IProductsService, IConfiguration)`
- Layout: sidebar izquierdo (gridPedidos + buscador) + tabs a la derecha (tabGeneral)
- Toolbar (`ToolStrip`): btnNuevo, btnEditar, btnGuardar, btnCancelar
- `sw_anular_pedido` (UISwitch) — para anularizar; **`Style = UIStyle.Custom`** (evita que
  SetChildUIStyle lo rebasa); colores: `ActiveColor = LightGreenTheme.PrimaryDark`,
  `InActiveColor = Color.Firebrick` (rojo)
- `ModoFormulario`: Consulta (default), Nuevo, Editar
- `AplicarModo()`: único método que cambia ReadOnly/Enabled del todo
- `ConstruirPedidoDesdeFormulario()`: arma el Pedido desde los controles, con preservación
  de valores en edición (ClienteDatos respaldo, etc.)
- `Guardar`: valida → previsualiza número → `SavePedidoCompleto` (Nuevo) /
  `ActualizarPedidoCompleto` (Editar) → mensaje de confirmación con nota si número cambió
- `CargarDetallePedidoAsync()`: async con CancellationToken, `BeginInvoke` para thread safety
- Combos: `ConfigurarComboFiltroIncremental()` (ShowFilter=true, FilterIgnoreCase, TrimFilter,
  FilterMaxCount=5000)
- `AsignarCombo()` registra valores en `_valoresSel` porque SunnyUI no aplica SelectedValue con filtro
- Fechas: `DateFormat = "dd/MM/yyyy HH:mm"`
- Permisos: `PermisoHelper.PuedeCrear("Pedidos")`, `PermisoHelper.PuedeEditar("Pedidos")`
- `gridPedidos`: checkbox columna de selección + columnas numero/customer_name/estado
- Lineas detalle: `uiDataGridView1` con columnas (Id.Pro, descripción, unidad, qty, notas,
  precio, width, lenght, subtotal) + editor (combo producto, qty, precio, notas, width, length)
- Row anulada: `CellFormatting` pinta fila de rojo (Firebrick background, blanco foreground)
- `TxtBuscarPedido.TextChanged`: RowFilter con `CONVERT(numero, 'System.String')` para buscar
  string numero en la columna (aunque venga como string de la base)

### Sidebar (`Main.cs` + `Main.Designer.cs`)
- Botones declarados en `Main.Designer.cs`: `button1` (Clientes), `button2` (Vendedores),
  `button3` (Proveedores), `button4` (Reportes → "Auditoria"), `bot_pedidos` (Pedidos),
  y buttons de producción/inventario/despacho/etc.
- `Main.cs` arma el sidebar en código: `CrearGrupoVentas()` crea acordeón "Ventas" con
  sub-panel `pnlVentas` (3 bots: Pedidos, Clientes, Vendedores). `button3` (Proveedores)
  y `button2` quedan como botones raíz.
- `menuButtons` array y `titulosModulo` array en `InicializarSidebarColapsable()` definen
  el orden y tooltips. `ordenVisual` / `nuevoOrden` arrays controlan posición via
  `SetChildIndex`.
- Handlers: cada botón → `VerificarPermiso("Modulo")` → `_formManager.ShowForm<FrmX>()`
- `VerificarPermiso`: admin siempre pasa; `PermisoHelper.PuedeVer("Modulo")`; MessageBox si denegado
- Botones raíz (no en accordion): `button3` (Proveedores), `button2` (Vendedores), `button4`
  (Auditoría), `OPC_MENU_LABELS` (Etiquetas)

### Program.cs (DI registration)
- Services: `builder.Services.AddTransient<IServicioService, ServicioService>()`
- Forms: `builder.Services.AddTransient<FrmX>()`
- `Main` también registrado: `AddTransient<Main>()`
- `FormManager` como singleton: `AddSingleton<FormManager>()`

## Diferencias Clave: Pedido de Venta vs Orden de Compra

| Aspecto | Pedido de Venta (SO) | Orden de Compra (OC) |
|---|---|---|
| Entidad maestra | `customer` (Guid `customer_id`) | `provider` (string `Proveedor_Id`) |
| Formato número | `SO-#####` | `OC-#####` |
| Filtro `control` | `filter='PED'` | `filter='OC'` |
| Tipo de venta | contado / credito (combo) | **NO aplica** (eliminar `Tipo_venta`) |
| Vendedor | `vendor_id` (Guid, opcional) | **NO aplica** (eliminar `Vendor_Id`) |
| Condiciones pago | contado, 7/15/30/45/60, credito | contado, 7/15/30/45/60, credito (reutilizar vocabulario) |
| Prioridad | normal / urgente | normal / urgente (reutilizar) |
| Dirección facturación | del cliente (frozen) | [DECISIÓN ABIERTA] |
| Dirección entrega | del cliente (frozen) | del proveedor (`provider.direccion`) |
| ITBIS | sí (recaudado) | [DECISIÓN ABIERTA: ¿recuperable? usar 0 o 18%?] |
| Estados | creado, en producción, pickeado, despachado, devuelto | creado, ordenado, parcial, recibido, cancelado |
| Enviar al dispositivo | Sí (picking) | No (es entrada, no salida) |

## Decisiones de Diseño (validadas contra el código)

1. **Número `OC-#####`**: idéntico a `PedidoNumero` — prefijo "OC-", 5 dígitos, max 99999,
   zero-padded. Reutilizar el patrón UPDLOCK/HOLDLOCK en `control` con `filter='OC'`.
2. **Modelo**: `OrdenCompra` con `Proveedor_Id` como **string** (no Guid), sin
   `Tipo_venta`, sin `Vendor_Id`. Reutilizar `Direccion_facturacion`/`Direccion_entrega`
   como campos del header (aunque el proveedor solo tenga una dirección, la OC necesita
   capturar Ship To / Bill To al momento de la orden).
3. **Estados**: `creado`, `ordenado`, `parcial`, `recibido`, `cancelado`
   (paralelo a PedidoEstado).
4. **Layout del formulario**: Combinar 30/70 (`FrmProveedores`) + edit/save/switch
   (`FrmPedidos`). Panel izquierdo con grid + buscador + resumen; panel derecho con
   tabs (tabGeneral: toolbar, header campos, detalle grid, editor de línea, totales,
   switch de anulación).
5. **`Proveedor_Id` tipo string**: el combo de proveedores usará `Proveedor_Id` (string)
   como ValueMember, igual que `ProveedorService` ya retorna. `BuscarProveedorAsync`
   tomará `string proveedorId` (no Guid).
6. **Tabla SQL**: `orden_compra.numero` como **NVARCHAR(10)** (no INT), consistente con
   el módulo Pedidos real. `orden_compra_detalle.numero` también NVARCHAR.
7. **Test infrastructure**: `DatabaseFixture` + `TestConfiguration` para tests de
   integración. Stubs para tests unitarios de lógica y layout.

## Archivos a Crear/Modificar

### Nuevos archivos (Models)
- `Models/OrdenCompra.cs` — header model (string Numero, string Proveedor_Id, etc.)
- `Models/OrdenCompraDetalle.cs` — línea model
- `Models/OrdenCompraEstado.cs` — constantes de estados
- `Models/OrdenCompraCatalogos.cs` — vocabularios (CondicionesPago, Prioridad)
- `Models/ProveedorDatos.cs` — datos de proveedor para combo (Consecutivo, DireccionEntrega)

### Nuevos archivos (Services/OrdenesCompras)
- `Services/OrdenesCompras/IOrdenesComprasService.cs` — interfaz del servicio
- `Services/OrdenesCompras/OrdenesComprasService.cs` — implementación
- `Services/OrdenesCompras/OrdenesCompraNumero.cs` — formato `OC-#####`
- `Services/OrdenesCompras/OrdenesCompraValidador.cs` — validación de OC
- `Services/OrdenesCompras/OrdenesCompraCalculos.cs` — cálculos (subtotal, ITBIS, total)
- `Services/OrdenesCompras/OrdenesCompraDetalleMapper.cs` — DataTable → List<OrdenCompraDetalle>
- `Services/OrdenesCompras/ConsecutivoProveedor.cs` — formato consecutivo proveedor (4 dígitos)

### Nuevos archivos (Forms)
- `Forms/FrmOrdenesCompra.cs` — formulario con lógica (30/70 + edit/save/switch)
- `Forms/FrmOrdenesCompra.Designer.cs` — layout generado

### Nuevos archivos (Scripts)
- `Scripts/004_CrearTablas_OrdenesCompra.sql` — DDL idempotente + seed `control`

### Archivos a modificar
- `R.cs` — agregar queries: `SQL_SELECT_OC`, `SQL_SELECT_OC_DETALLE`, `SQL_INSERT_OC`,
  `SQL_INSERT_OC_DETALLE`, `SQL_UPDATE_OC`, `SQL_DELETE_OC_DETALLE`, `SQL_ANULAR_OC`,
  `SQL_RESTAURAR_OC`, `SQL_UPDATE_OC_ESTADO`, `SQL_QUERY_CONSUMO_OC_CONSECUTIVO`,
  `SQL_SELECT_OC_PROXIMO`, `SQL_SELECT_LOAD_PROVEEDOR_COMBO`, `SQL_SELECT_PROVEEDOR_POR_ID`
- `Program.cs` — registrar `IOrdenesComprasService` → `OrdenesComprasService` + `FrmOrdenesCompra`
- `Main.cs` — botón sidebar + handler `Bot_ordenes_compra_Click`
- `Main.Designer.cs` — declarar `bot_ordenes_compra` button + wiring en `panel1`
- `Scripts/Setup_Pedidos.sql` — **[FLAG]**: corregir `numero` de INT a NVARCHAR (inconsistencia hallada)

### Nuevos archivos (Tests)
- `Ritrama2025.Tests/OrdenesCompraNumeroTests.cs` — unit tests (formato, validación, parte numérica)
- `Ritrama2025.Tests/OrdenesCompraCalculosTests.cs` — unit tests (subtotal, ITBIS, total, qty)
- `Ritrama2025.Tests/OrdenesCompraValidadorTests.cs` — unit tests (validación header + líneas)
- `Ritrama2025.Tests/ConsecutivoProveedorTests.cs` — unit tests (formato 4 dígitos)
- `Ritrama2025.Tests/FrmOrdenesCompraLayoutTests.cs` — layout tests (30/70, grid, campos, switch, toolbar)
- `Ritrama2025.Tests/OrdenesComprasServiceTests.cs` — integration tests (save/update/anular contra BD)

## Especificaciones Técnicas Detalladas

### Modelo `OrdenCompra`
```csharp
public class OrdenCompra
{
    public string Numero { get; set; } = string.Empty;  // "OC-00027"
    public DateTime Fecha { get; set; }
    public string? Proveedor_Id { get; set; }  // string, no Guid
    public string? Proveedor_Name { get; set; }
    public string? Persona_Contacto { get; set; }
    public DateTime? Fecha_entrega { get; set; }  // expected delivery date
    public string? Condiciones_pago { get; set; }  // reutilizar vocabulario
    public string? Prioridad { get; set; }  // normal / urgente
    public string? Direccion_entrega { get; set; }  // recibida del proveedor
    public string? Direccion_facturacion { get; set; }  // [DECISION ABIERTA]
    public string? Estado { get; set; }  // creado, ordenado, parcial, recibido, cancelado
    public string? Notas { get; set; }
    public bool Anulado { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Porc_Itbis { get; set; }
    public decimal Monto_Itbis { get; set; }
    public decimal Total { get; set; }
    public List<OrdenCompraDetalle> Detalle { get; set; } = new();
}
```

### SQL: `Scripts/004_CrearTablas_OrdenesCompra.sql`
```sql
-- idempotent: IF NOT EXISTS + COL_LENGTH guards
CREATE TABLE orden_compra (
    id              INT IDENTITY(1,1) PRIMARY KEY,
    numero          NVARCHAR(10) NOT NULL,  -- "OC-00027", string para viajar con el prefijo
    fecha           DATETIME NOT NULL,
    proveedor_id    NVARCHAR(50) NOT NULL,    -- string, no UNIQUEIDENTIFIER
    proveedor_name  NVARCHAR(200) NULL,
    persona_contacto NVARCHAR(100) NULL,
    fecha_entrega   DATETIME NULL,
    direccion_entrega NVARCHAR(200) NULL,
    direccion_facturacion NVARCHAR(200) NULL,
    condiciones_pago NVARCHAR(100) NULL,
    prioridad       NVARCHAR(20) NULL,
    estado          NVARCHAR(20) NOT NULL DEFAULT 'creado',
    notas           NVARCHAR(MAX) NULL,
    anulado         BIT NOT NULL DEFAULT 0,
    subtotal        DECIMAL(18,2) NULL,
    porc_itbis      DECIMAL(18,2) NULL,
    itbis           DECIMAL(18,2) NULL,
    total$          DECIMAL(18,2) NULL,
    CONSTRAINT UQ_orden_compra_numero UNIQUE (numero)
);

CREATE TABLE orden_compra_detalle (
    id            INT IDENTITY(1,1) PRIMARY KEY,
    numero        NVARCHAR(10) NOT NULL,  -- FK a orden_compra.numero
    product_id    NVARCHAR(25) NOT NULL,
    product_name  NVARCHAR(200) NULL,
    cant          DECIMAL(18,2) NOT NULL DEFAULT 1,
    unidad        NVARCHAR(10) NULL,
    width         DECIMAL(18,2) NULL,
    lenght        DECIMAL(18,2) NULL,
    msi           DECIMAL(18,2) NULL,
    precio        DECIMAL(18,2) NULL,
    total_renglon DECIMAL(18,2) NULL,
    notas         NVARCHAR(MAX) NULL
);

-- FK detalle -> header
ALTER TABLE orden_compra_detalle
    ADD CONSTRAINT FK_OC_DETALLE_OC
    FOREIGN KEY (numero) REFERENCES orden_compra(numero);

-- Indices
CREATE NONCLUSTERED INDEX IX_orden_compra_numero ON orden_compra(numero);
CREATE NONCLUSTERED INDEX IX_orden_compra_detalle_numero ON orden_compra_detalle(numero);

-- Semilla del consecutivo
IF NOT EXISTS (SELECT 1 FROM control WHERE filter = 'OC')
    INSERT INTO control (filter, par1) VALUES ('OC', '0');
```

### Queries SQL (`R.cs`, nueva sección `R.QUERY.PURCHASE`)
```csharp
internal static string SQL_SELECT_OC = "SELECT numero,fecha,proveedor_id,proveedor_name,persona_contacto,fecha_entrega,direccion_entrega,direccion_facturacion,condiciones_pago,prioridad,estado,notas,anulado,subtotal,porc_itbis,itbis,total$ FROM orden_compra ORDER BY numero DESC";
internal static string SQL_SELECT_OC_DETALLE = "SELECT numero,product_id,product_name,cant,unidad,width,lenght,msi,precio,total_renglon,notas FROM orden_compra_detalle WHERE numero = @p1 ORDER BY id";
internal static string SQL_INSERT_OC = "INSERT INTO orden_compra (numero,fecha,proveedor_id,proveedor_name,persona_contacto,fecha_entrega,direccion_entrega,direccion_facturacion,condiciones_pago,prioridad,estado,notas,anulado,subtotal,porc_itbis,itbis,total$) VALUES (@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14,@p15,@p16,@p17,@p18)";
internal static string SQL_INSERT_OC_DETALLE = "INSERT INTO orden_compra_detalle (numero,product_id,product_name,cant,unidad,width,lenght,msi,precio,total_renglon,notas) VALUES (@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11)";
internal static string SQL_UPDATE_OC = "UPDATE orden_compra SET fecha=@p2,proveedor_id=@p3,proveedor_name=@p4,persona_contacto=@p5,fecha_entrega=@p6,direccion_entrega=@p7,direccion_facturacion=@p8,condiciones_pago=@p9,prioridad=@p10,estado=@p11,notas=@p12,anulado=@p13,subtotal=@p14,porc_itbis=@p15,itbis=@p16,total$=@p17 WHERE numero=@p1";
internal static string SQL_DELETE_OC_DETALLE = "DELETE FROM orden_compra_detalle WHERE numero = @p1";
internal static string SQL_ANULAR_OC = "UPDATE orden_compra SET anulado = 1 WHERE numero = @p1";
internal static string SQL_RESTAURAR_OC = "UPDATE orden_compra SET anulado = 0 WHERE numero = @p1";
internal static string SQL_UPDATE_OC_ESTADO = "UPDATE orden_compra SET estado = @p2 WHERE numero = @p1";
internal static string SQL_QUERY_CONSUMO_OC_CONSECUTIVO = "UPDATE control WITH (UPDLOCK, HOLDLOCK) SET par1 = par1 + 1 OUTPUT DELETED.par1 WHERE filter='OC'";
internal static string SQL_SELECT_OC_PROXIMO = "SELECT par1 FROM control WHERE filter='OC'";
internal static string SQL_SELECT_LOAD_PROVEEDOR_COMBO = "SELECT Proveedor_Id,Proveedor_Name,direccion FROM provider WHERE anulado = 0 ORDER BY Proveedor_Name";
internal static string SQL_SELECT_PROVEEDOR_POR_ID = "SELECT direccion FROM provider WHERE Proveedor_Id = @id AND anulado = 0";
```

### Service Interface `IOrdenesComprasService`
```csharp
Task<DataTable> LoadDataOrdenesCompra(CancellationToken ct = default);
Task<DataTable> LoadDataProveedores(CancellationToken ct = default);
Task<ProveedorDatos?> BuscarProveedorAsync(string proveedorId, CancellationToken ct = default);
Task<DataTable> LoadDataOrdenCompraDetalle(string numero, CancellationToken ct = default);
Task<string> GetProximoNumeroOrdenCompra(CancellationToken ct = default);
bool SaveOrdenCompraCompleto(OrdenCompra oc);
bool ActualizarOrdenCompraCompleto(OrdenCompra oc);
string? ErrorMsg { get; }
bool AnularOrdenCompra(string numero);
bool RestaurarOrdenCompra(string numero);
bool ActualizarEstadoOrdenCompra(string numero, string estado);
```

### Sidebar (Main.Designer.cs + Main.cs)
- Nuevo button `bot_ordenes_compra` declarado en Designer, añadido a `panel1.Controls`
- Handler `Bot_ordenes_compra_Click` en `Main.cs`:
  `VerificarPermiso("OrdenesCompra")` → `_formManager.ShowForm<FrmOrdenesCompra>()`
- Añadir a `menuButtons[]` y `titulosModulo[]` arrays
- Posicion: después de `bot_pedidos` (o como root button independiente)
- [DECISION ABIERTA]: botón como root independiente (compras ≠ ventas) con texto
  "Orden de Compra", tooltip "COMPRAS - ORDEN DE COMPRA", icono `add_file_32px.png`

### Program.cs
```csharp
builder.Services.AddTransient<IOrdenesComprasService, OrdenesComprasService>();
builder.Services.AddTransient<FrmOrdenesCompra>();
```

## Plan de Implementación (orden secuencial)

1. **BD + Queries**
   - Escribir `Scripts/004_CrearTablas_OrdenesCompra.sql` (DDL + seed control)
   - Agregar queries en `R.cs` → nueva sección `R.QUERY.PURCHASE`
   - [FLAG] Documentar inconsistencia en `Setup_Pedidos.sql` (`numero INT` debería ser NVARCHAR)

2. **Modelos + lógica pura**
   - `Models/OrdenCompra.cs`, `OrdenCompraDetalle.cs`, `OrdenCompraEstado.cs`,
     `OrdenCompraCatalogos.cs`, `ProveedorDatos.cs`
   - `Services/OrdenesCompras/OrdenesCompraNumero.cs`, `OrdenesCompraCalculos.cs`,
     `OrdenesCompraValidador.cs`, `OrdenesCompraDetalleMapper.cs`,
     `ConsecutivoProveedor.cs`

3. **Service layer**
   - `Services/OrdenesCompras/IOrdenesComprasService.cs`,
     `Services/OrdenesCompras/OrdenesComprasService.cs`

4. **Formulario**
   - `Forms/FrmOrdenesCompra.Designer.cs` — layout 30/70 + toolbar + switch + grid detalle
   - `Forms/FrmOrdenesCompra.cs` — lógica: modos, eventos, guardar/anular

5. **Sidebar + DI**
   - `Main.Designer.cs` — nuevo button
   - `Main.cs` — handler + arrays
   - `Program.cs` — registro DI

6. **Tests**
   - Unit: `OrdenesCompraNumeroTests`, `OrdenesCompraCalculosTests`,
     `OrdenesCompraValidadorTests`, `ConsecutivoProveedorTests`
   - Layout: `FrmOrdenesCompraLayoutTests` (stub service, Controls.Find pattern)
   - Integration: `OrdenesComprasServiceTests` (DatabaseFixture, cleanup/restore par1)

7. **Build + Verify**
   - `dotnet build`
   - `dotnet test --filter "Categoria=Unit"`
   - `dotnet test --filter "Categoria=Integracion"` (requiere BD de pruebas)

## Riesgos y Consideraciones

- **`numero` tipo en DB**: El script `Setup_Pedidos.sql` dice INT pero el código usa string.
  OC usará NVARCHAR(10) para ser consistente con el código real. **Flag**: corregir el script
  Pedidos.
- **`Proveedor_Id` es string**: Diferente de `customer_id` (Guid). Todo el combo/service debe
  manejar strings, no Guids.
- **ITBIS en compras**: En RD, el ITBIS de compras puede ser recuperable/facturable.
  [DECISION ABIERTA] Usar 0 por defecto o 18%?
- **Direcciones del proveedor**: La tabla `provider` solo tiene `direccion` (una). Para la OC
  se capturan `direccion_facturacion` y `direccion_entrega` como campos del header, precargados
  de `provider.direccion` al elegir.
- **Switch de anulación**: Necesita `UIStyle.Custom` para preservar `InActiveColor` (rojo)
  después del render de SunnyUI — patrón documentado en `FrmPedidosAnulacionTests`.
- **`Tipo_venta` no aplica**: OC no tiene tipo de venta. Eliminar `cboTipoVenta` y la lógica
  asociada (`CondicionesPagoEditable`, etc.). La condición de pago es directa.
- **Filtrado de búsqueda**: El RowFilter de `TxtBuscarPedido` usa
  `CONVERT(numero, 'System.String')` porque numero viene como string de la BD; para OC,
  si `numero` es NVARCHAR el filtro es `numero LIKE '%{filtro}%'`.
- **No hay "Enviar al dispositivo"**: OC no usa `btnEnviarDispositivo` (es entrada, no salida).

## Preguntas Abiertas (resolver con el usuario)

1. ¿El número de OC usa el mismo formato `OC-#####` (5 dígitos) o diferente?
2. ¿Qué estados definitivos tendrá la OC? (propuesta: creado, ordenado, parcial, recibido, cancelado)
3. ¿La OC incluye dirección de facturación o solo dirección de entrega/recepción?
4. ¿Cómo se calcula el ITBIS en la OC? (¿0%, 18%, o configurable?)
5. ¿El botón de OC va como root en el sidebar (grupo "Compras") o como sub-boton de Ventas?
6. ¿La OC debe relacionarse con Recepciones de Materia Prima (campo `orden_compra` en `OrdenMateria`)?
