# Diseño: numero de pedido con formato SO-#### y datos de direccion del cliente

Fecha: 2026-09-25
Estado: aprobado para implementar
Alcance: `FrmPedidos` (flujo de crear pedido), `PedidoService`, `R.cs`, modelo `Pedido`,
script de migracion de esquema y pruebas.

## 1. Objetivo

Tres cambios en el flujo de crear pedido nuevo:

1. El numero del pedido se persiste con el formato `SO-####` (`SO` = SALES ORDERS) en
   lugar de un entero pelado.
2. Tres cajas de texto de solo lectura muestran el **ID Cliente**, el **ID Vendedor** y la
   **Direccion del cliente**.
3. Al seleccionar el cliente se traen su direccion y se prellena la direccion de entrega
   del pedido.

## 2. Decisiones tomadas y su motivo

| Decision | Motivo |
|---|---|
| `SO-####` se guarda como texto, no solo se muestra | Requisito explicito. Obliga a migrar el tipo de la columna. |
| Relleno con ceros a 4 digitos (`SO-0027`) | Es el formato pedido y hace que `ORDER BY numero DESC` siga siendo correcto hasta 9999. Sin relleno, `SO-25` ordenaria despues de `SO-100`. |
| `pedido_detalle.numero` se migra tambien | Tiene FK a `pedido(numero)`; si no, la FK se rompe. |
| `orden_corte.pedido_id` y `despacho.pedido_id` se migran | Nadie los lee ni escribe todavia, pero dejarlos en `INT` con `pedido.numero` en texto obliga a otra migracion el dia que se enlace la OC o el despacho. |
| `control.par1` sigue en `INT` | El consecutivo no cambia de naturaleza. El prefijo y el relleno se componen en C#. |
| Los ids se muestran como GUID crudo | `customer_id` y `vendor_id` son `uniqueidentifier`. `customer.Identificacion` es el RNC, no el id: meterlo en una caja rotulada "ID Cliente" seria enganoso. |
| ITBIS sigue fijo en 18 | Decidido explicitamente. `customer.impuesto` (179 de 199 filas) queda para otro trabajo. |
| La direccion del cliente se resuelve con `COALESCE` en SQL | Reutiliza la precedencia que ya establecio `Backfill_Pedidos_Completos.sql`: `customer_address`, luego `Customer_Dir`, luego `customer_zone`. |

## 3. Estado real de la base (verificado con consultas de solo lectura)

Servidor `192.168.10.10`, base `RITRAMASQL2017`. Ojo: `Desarrollo` y `Produccion` apuntan a
**la misma** base.

- `pedido`: 20 filas, `numero` maximo 24. `control.par1` para el filtro `PED` = 26, asi que
  el proximo numero sera `SO-0027`.
- Unica FK que referencia a pedido: `FK_PEDIDO_DETALLE_PEDIDO : pedido_detalle(numero) -> pedido`.
- `customer.Customer_Id` es `uniqueidentifier`.
- Direcciones en `customer` de 199 filas: `Customer_Dir` 178 informadas, `customer_address`
  **0**, `customer_zone` 0 informadas en la muestra.
- **0** pedidos apuntan a un cliente anulado, asi que resolver la direccion por
  `customer_id` siempre encuentra fila.
- `Identificacion` informada en 161 de 199 (no se usa en este trabajo).

## 4. Migracion de esquema

Script nuevo: `Scripts/Migrar_Pedido_Numero_SO.sql`. Idempotente, envuelto en transaccion,
con validaciones previas que abortan sin tocar nada si el estado no es el esperado.

Orden de ejecucion:

1. Preflight, y abortar sin tocar nada si se cumple cualquiera de estas condiciones:
   - la tabla `pedido` no existe;
   - `MAX(numero) > 9999`, porque el relleno a 4 digitos truncaria el valor;
   - alguna fila ya empieza con `SO-`, o sea que el script ya se corrio;
   - existe alguna fila de `pedido_detalle` cuyo `numero` no tenga pedido correspondiente
     (huerfanos). No se comparan conteos: `pedido_detalle` tiene mas filas que `pedido`
     porque un pedido lleva varias lineas.
2. Caer `FK_PEDIDO_DETALLE_PEDIDO`.
3. `ALTER TABLE pedido_detalle ALTER COLUMN numero VARCHAR(20) NOT NULL`.
4. `ALTER TABLE pedido ALTER COLUMN numero VARCHAR(20) NOT NULL`.
5. `ALTER TABLE orden_corte ALTER COLUMN pedido_id VARCHAR(20) NULL`.
6. `ALTER TABLE despacho ALTER COLUMN pedido_id VARCHAR(20) NULL`.
7. `UPDATE` de los datos: `'SO-' + RIGHT('0000' + CAST(numero AS varchar(4)), 4)`.
8. Recrear la FK `FK_PEDIDO_DETALLE_PEDIDO`.
9. Verificacion: cero filas sin prefijo, conteo de filas igual al previo, y recorrido de
   `pedido_detalle` para confirmar que el detalle sigue apuntando a su pedido.

Los indices `UQ_pedido_numero`, `IX_pedido_numero` e `IX_pedido_detalle_numero` sobreviven
al cambio de tipo: SQL Server los reconstruye con el nuevo tipo, no hay que recrearlos.

**Este script no lo ejecuta el agente.** Toca 20 pedidos reales en la base que es a la vez
Desarrollo y Produccion. Requiere respaldo previo y la aplicacion cerrada. El agente solo
lo escribe y lo deja listo.

## 5. `PedidoNumero`: helper unico del formato

`Services/PedidoService/PedidoNumero.cs`, estatico, sin dependencias y totalmente testeable.

```csharp
public const string Prefijo = "SO-";
public const int LargoNumero = 4;   // digitos despues del prefijo

public static string Formatear(int numero);       // 27 -> "SO-0027"
public static bool EsValido(string? numero);      // null, "", "SO-", "SO-abc", "SO-12" -> false
public static int ParteNumerica(string? numero);  // "SO-0027" -> 27; invalido -> 0
```

La clase es `public` y no `internal`: el proyecto de pruebas no tiene
`InternalsVisibleTo`, asi que un tipo `internal` seria invisible para las pruebas.

`EsValido` exige que el total sea `Prefijo.Length + LargoNumero` caracteres, que el
prefijo case exactamente y que el resto sean digitos. `ParteNumerica` nunca lanza: ante
un valor invalido devuelve 0, para que un dato corrupto no tumbe la pantalla.

`Formatear` lanza `ArgumentOutOfRangeException` si `numero <= 0`, porque un consecutivo no
puede ser cero ni negativo y conviene fallar en el origen, no mostrar `SO-0000`.

## 6. Cambios de codigo

### 6.1 Modelo

`Models/Pedido.cs`: `public int Numero` pasa a `public string Numero { get; set; } = string.Empty;`

### 6.2 `R.cs`

- `SQL_SELECT_PEDIDO` y `SQL_INSERT_PEDIDO`: sin cambios de texto. `numero` se sigue
  pasando como parametro; solo cambia el tipo del valor.
- `SQL_SELECT_LOAD_CUSTOMER_COMBO` pasa a:

```sql
SELECT customer_id, customer_name,
       COALESCE(customer_address, Customer_Dir, customer_zone, 'Sin especificar') AS direccion_cliente
FROM customer WHERE anulado = 0 ORDER BY customer_name
```

- `SQL_QUERY_CONSUMO_PEDIDO_CONSECUTIVO`: sin cambios.

### 6.3 `IPedidoService` / `PedidoService`

| Firma | Antes | Despues |
|---|---|---|
| `GetNewNumeroPedido` | `Task<int>` | `Task<string>` (consume `par1` y devuelve `SO-0027`) |
| `LoadDataPedidoDetalle` | `(int numero, ct)` | `(string numero, ct)` |
| `AnularPedido` | `(int numero)` | `(string numero)` |
| `ActualizarEstadoPedido` | `(int numero, string estado)` | `(string numero, string estado)` |

El nombre `GetNewNumeroPedido` se conserva aunque cambie el tipo de retorno: ya es el
nombre que usan la pantalla y las pruebas, y renombrarlo solo agrega ruido en el diff.

### 6.4 `PedidoValidador`

Se reemplaza `if (pedido.Numero <= 0)` por `if (!PedidoNumero.EsValido(pedido.Numero))`. El
mensaje de error pasa a "El numero del pedido no tiene el formato SO-####.".

### 6.5 Formulario

**Controles nuevos** (tres `uiTextBox`, todos con `ReadOnly = true` declarado en el
diseniador, como ya se hace con los demas):

| Control | Rotulo | Ubicacion | Origen del valor |
|---|---|---|---|
| `uiTextBox12` | Direccion del cliente | region libre `x=690, y=147, 210x113`, a la derecha de `uiRichTextBox2` | columna `direccion_cliente` del combo |
| `uiTextBox10` | ID Cliente | region libre `x=758, y=470, 145x29`. Se sube desde `y=488` porque `uiLabel13 "Itbis % :"` ocupa `x 762..842, y 511..534` y lo solapaba | GUID del cliente elegido |
| `uiTextBox11` | ID Vendedor | region libre `x=758, y=546, 145x29`. `uiLabel12 "Total :"` esta en `x 383..497`, sin choque horizontal | GUID del vendedor elegido |

Las tres regiones libres son las unicas que quedan: la banda de `y=270` ya esta ocupada
por el editor de productos de `x=151` a `x=660`, y los totales ocupan `x=504` a `x=752` en
`y=466`, `y=505` y `y=544`. Anyadir controles a la izquierda del resto obligaria a
rediseñar la pestana General, que queda fuera de alcance.

**Consecuencia asumida:** un GUID son 36 caracteres y las cajas de ID miden 145 px de ancho,
asi que se ve el comienzo del valor, no el GUID completo. Se mitiga con un `TipText` con
el valor íntegro y con que la caja es de solo lectura y seleccionable, de modo que el
operador puede copiar el valor completo. Si en la prueba manual resulta ilegible, la
salida es rediseñar la columna de totales para ganar ancho, y eso se decide aparte.

Los tres son de solo lectura **tambien en modo Nuevo**: un id editable que se desincronice
del combo es un defecto de integridad, no una conveniencia.

`AplicarModo` los incluye en su lista de solo lectura, para respetar la regla ya
establecida de que ese metodo es el unico que cambia `ReadOnly` o `Enabled`.

**Prellenado.** Al cambiar la seleccion del combo de cliente:

1. `uiTextBox10` = GUID del cliente.
2. `uiTextBox12` = `direccion_cliente` de la fila del combo.
3. `uiRichTextBox1` (direccion de entrega, que ya existe y ya se guarda) = el mismo
   `direccion_cliente`, editable. Si el cliente cambia, se sobrescribe: la direccion
   pertenece al cliente, no al pedido.

**Modo consulta.** Al seleccionar un pedido del listado, la direccion del cliente se
resuelve por `customer_id` contra la `DataTable` del combo, igual que `AsignarCombo` ya
hace con el nombre. Como ningun pedido referencia un cliente anulado, siempre resuelve.

**Filtro del listado.** `TxtBuscarPedido_TextChanged` usa hoy
`CONVERT(numero, 'System.String')`, que era necesario porque `numero` era entero. Con
`numero` en `VARCHAR` la conversion sobra y se deja solo `numero LIKE '%filtro%'`.

## 7. Pruebas

- `Ritrama2025.Tests/PedidoNumeroTests.cs`, nuevo: `Formatear` con relleno y con numero
  grande, `EsValido` sobre una tabla de entradas validas e invalidas, y `ParteNumerica`
  con valor valido, vacio, nulo y con formato roto.
- `PedidoValidadorTests`: los 16 casos existentes se adaptan; el caso de numero invalido
  pasa a verificar el formato.
- `PedidoServiceValidacionTests`: los 4 casos usan numeros con el formato nuevo.
- `PedidoServiceTests` (integracion): genera el numero con el formato nuevo. **No se
  ejecuta** sin autorizacion de base de datos.

Meta: build con `0 Errores` y `0 Advertencia(s)`, y la suite unitaria en verde.

## 8. Fuera de alcance

- Tomar el ITBIS de `cliente.impuesto`.
- Enlazar `orden_corte.pedido_id` o `despacho.pedido_id`; solo se migran los tipos.
- Modo `Editar` del pedido.
- Mostrar `Identificacion` (RNC) del cliente.
- Backfill de los 20 pedidos existentes: el script solo cambia el formato del numero, no
  rellena datos.
- `Backfill_Pedidos_Completos.sql` no se toca: es un script historico de una sola vez y no
  filtra ni actualiza por `numero`.
