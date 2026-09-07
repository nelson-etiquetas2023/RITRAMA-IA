---
name: negocio-orden-corte
description: Reglas de negocio, ciclo de vida, procedimientos y validaciones de la Orden de Corte (OC) y los rollos cortados en Ritrama2025. USAR al trabajar con producción/corte de rollos master, cuadre a lo ancho, configuracion de vueltas, generacion de rollos, etiquetado (ROLLID), aprobacion, cierre y consumo de inventario, despacho, reporte de desperdicios, o al implementar las acciones pendientes (ACC). Triggers: "orden de corte", "OC", "cuadre", "cortes", "vueltas", "maquina", "master", "rollo cortado", "etiquetar", "aprobar OC", "cerrar OC", "consumo master", "desperdicio", "restante", "anis de master", "anular OC", "ROLLID".
---

# Negocio: Orden de Corte (OC)

Reglas y flujo del modulo `FrmOrdenCorte` y sus servicios, relevadas y validadas con el negocio. Usar esta skill como fuente de verdad del comportamiento antes de modificar o crear codigo de produccion/corte.

## Cuándo usar

- Trabajar sobre `Forms\FrmOrdenCorte.cs`, `Services\ProduccionService\OrdenCorteService.cs`, `Services\ProduccionService\ConsumoMasterService.cs`, `R.cs` (queries `PRODUCTION`), reportes `RptOC` / `ReporteDesperdicios`.
- Crear, editar, etiquetar, aprobar, cerrar o anular una OC.
- Calculos de MSI, restantes, cuadre, vueltas, generacion de rollos.
- Validar procesos, detectar brechas, o implementar las ACC pendientes.

## Entidades y tablas

- `producto`: catalogo. Define la **categoria** por 4 columnas bit exclusivas (derivacion en `R.cs:82`):
  - `MasterRolls=1` → **MASTER** (materia prima para OC; picker filtra `b.MasterRolls = 1`).
  - `rollo_cortado=1` → **ROLLO CORTADO** (producto terminado generado por la OC).
  - `resmas=1` → **HOJAS** (resmas; pestana "Hojas" de inventarios/picking).
  - `Graphics=1` → **GRAPHICS**.
- `MasterInic` y `ItemsMateria`: inventario de rollos master (detalle consumido en `MasterDetailsInic`).
- `orden_corte`: cabecera de la OC (`numero`, `step`, `CloseDocument`, `anulada`, `TwoMasters`, `rollid_1/2`, `lenght_1/2`, `util1/2_real_lenght`, `real1_width`, `rest1_width`, `desperdicio/2`, `cant_rollos`, `can_rollos`, `Fecha_Autorize`, `ToAutorize`).
- `cortes`: cortes a lo ancho definidos por la OC (`width`, `lenght`, `large`, `msi`, `roll_number`).
- `rolls_details`: inventario de **rollos cortados (productos terminados)**; `unique_code` (ROLLID), `status`, `disponible`, `despacho`, `fecha_despacho`, `vuelta`, `ratio`.

## Unidades de medida (acordadas con el negocio)

- **Ancho** (master y cortes): **pulgadas**.
- **Largo** (master, `longitud_cortar`, rollo cortado, consumos, restantes): **pies**.
- **MSI** = ancho(pulg) × largo(pies) × **0.012**; `0.012 = 12/1000` (pies→pulgadas ÷ 1000) → `FACTOR_MSI` en `CalculosOrdenCorte.cs`.

## Modelo de estados de la OC

Nombres de negocio vs mecanismo interno (`orden_corte.step`):

| Estado | `step` | Acontecimiento |
|---|---|---|
| **CREADA** | 2 | Al guardar la OC (UI muestra "PRODUCCION") |
| **ETIQUETADA** | 3 | Etiquetar: aqui al rollo cortado se le genera su **ROLLID** (`unique_code` `RC*` consecutivos sin colision) |
| **APROBADA** | 4 | Transitoria (0 filas en BD; requiere `Frm_AprobarOC`) |
| **CERRADA** | 5 | `CloseDocument=1`; actualiza inventario de **master** y marca los **rollos cortados** como producto terminado (`disponible=1`) |

- Filtros de listado: OC abierta = `anulada=0 AND CloseDocument=0`. Cierres: `step=5` (535 en dev), `step=3` (8), `step=2` (57).
- `AnularOrdenCorte` **bloquea** cerrar-anulada: si `anulada=1` → "ya esta anulada"; si `CloseDocument=1` → "no se puede anular porque esta cerrada" (`OrdenCorteService.cs:143-192`). Las OC anuladas con `CloseDocument=1` existentes en BD son historico previo a esta validacion.

## Reglas del negocio (RN)

### RN-PROD: Categorias de producto
- Los 4 tipos de producto se determinan por las columnas bit de `producto` (Master / Rollo Cortado / Hojas(resmas) / Graphics). Solo el MASTER se usa como materia prima de la OC.

### RN-MASTER: Materia prima
- Toda OC requiere al menos un rollo **master** de gran tamaño (ancho plg × largo pies), seleccionable de `MasterInic` (Inic.) o `ItemsMateria` (Por Compras).
- El picker (`OrdenCorteService.cs:108-125`) muestra estado Completo / Agotado / Parcialmente Consumido y **excluye masters con `largo_restante = Length − largo_consumido` ≤ 100 pies**.
- Dado que el restante se acumula de OC no anuladas con `consumo > 0`, no reutilizar un master cuyo `length` no alcance para los rollos a producir.

### RN-CORTES: Cuadre a lo ancho
- La suma de los anchos de los cortes debe igualar el ancho del master (ej.: master de 39 plg → cortes 10+10+19) para aprovechar todo el material.
- El codigo calcula `real1_width` = suma de anchos y `rest1_width` = width del master − `real1_width`, **pero no valida la igualdad** → ver ACC-03.

### RN-CAL: Calculos
- MSI por rollo = ancho × largo × 0.012.
- Restante de material = largo original − largo utilizado.
- Largo real por corte = largo − menos + plus (`CalculosOrdenCorte.LongitudReal`).
- Consumos y restantes se manejan en **pies**.

### RN-ROLLOS: Generacion de rollos
- **Cada vuelta de la maquina produce un rollo cortado por cada corte definido a lo ancho.**
- Total rollos master 1 = `vueltas1 × cortes_ancho` = `cant_rollos`; si `TwoMasters`, agregar `vueltas2 × cortes_ancho` = `cantidad_rollos2`.
- `GENERAR_ROLLOS_CORTADOS` (`FrmOrdenCorte.cs:546`): por cada vuelta i (1..vueltas) y cada corte j crea rollo con `roll_number` secuencial, `width` del corte, `large` = longitud a cortar, `msi`, `roll_id` = master origen, `vuelta` = i, `status` = primer valor del combo, `unique_code` = "0" (hasta etiquetar). Requiere master, producto y cortes definidos.

### RN-ETIQUETADO
- Nº de rollos a etiquetar = `cant_rollos` definido en el cuadre.
- El etiquetado genera el **ROLLID** (`unique_code`) de cada rollo cortado (`EtiquetarOrdenCorte`, `FrmOrdenCorte.cs:1750`), con secuencia `RC*` sin colisionar con existentes.

### RN-EDICION
- Solo se modifica una OC **no anulada y en `step=2`** (`Opt_modif_orden`, `FrmOrdenCorte.cs:1463`).
- Quirk conocido: `GuardarOrderUpdate` actualiza solo los campos del **master 1**, no los del master 2.

### RN-CIERRE (consumo de inventario)
- `Opt_cerrar_orden` (`FrmOrdenCorte.cs:1662`): `UpdateStatusDocumentOC(5)` → `ACTUALIZAR_INVENTARIOS_MASTER`.
- Al cerrar se determina si el master se consumio **completo** o **parcial**:
  - Inventario de master: `largo_consumido += consumo` (consumo real y, si `desperdicio`, el desperdicio = largo − usado) en `MasterInic`/`ItemsMateria` via `ActualizarInventariosMasterAsync` (`ConsumoMasterService.cs:121-168`).
  - Inventario de rollos cortados (producto terminado): `UPDATE rolls_details SET disponible=1 WHERE numero=@oc` en la misma transaccion.
- Brecha: con `TwoMasters`, `ACTUALIZAR_INVENTARIOS_MASTER` solo descuenta la inventario del **master 1** → ver ACC-01.

### RN-DESPACHO
- Rollos disponibles para vender = `rolls_details` + `RollsInic` con `disponible=1` (`CommonService.cs:63-64`; tipo 'C' cortado / 'M' inicial).
- Al despachar: `UPDATE rolls_details SET disponible=0, despacho=@conduce, fecha_despacho=GETDATE() WHERE unique_code=@ucc` (`DespachoService.cs:71,434`). Un rollo despachado no vuelve a ofrecerse; el picking valida `disponible=1`.
- `RollosCortadosDispobnibles(oc)` (`OrdenCorteService.cs:859`) re-marca `disponible=1` (restauracion/reversion).

## Flujo paso a paso (pantalla)

1. **Crear OC** (`Opt_create_document`): seleccionar producto y master(s); definir cortes y cuadre a lo ancho.
2. **Cuadre / Config de vueltas** (`Opt_send_production`, `Btn_vueltas` en `FrmOrdenCorte.cs:2371`): vueltas por master → legitima `cant_rollos`.
3. **Generar rollos**: materializa `vueltas × cortes` en el detalle (editable por rollo antes de guardar).
4. **Guardar**: persiste cabecera `step=2` (CREADA), cortes y `rolls_details` (`disponible=1`, `unique_code="0"`).
5. **Etiquetar** (`Opt_etiquetar_orden` → `EtiquetarOrdenCorte`): genera ROLLID `RC*` por rollo; `step=3` (ETIQUETADA).
6. **Aprobar** (`Opt_aprobar_orden` → `Frm_AprobarOC` Write): `step=4` (APROBADA).
7. **Cerrar** (`Opt_cerrar_orden`): `step=5` + `CloseDocument`, actualiza inventario master y libera `disponible=1`.
8. **Anular** (`AnularOrdenCorte`): solo si `anulada=0 AND CloseDocument=0`.

## Procedimientos

- **Crear OC**: validar que el restante del master alcance para los rollos a producir (ver ACC-04).
- **Configurar vueltas**: guardar config por OC (`OrdenCorteService.cs:100`, registro con `rollid_oculto`, `ratio`, `ubic='nt'`).
- **Generar rollos**: tras cuadre; confirma `cant_rollos = vueltas × cortes`.
- **Imprimir**: `RptOC` (etiquetas/orden) y `ReporteDesperdicios.rdlc` (`reporteDeDesperdiciosToolStripMenuItem_Click`, `FrmOrdenCorte.cs:2598`) con `ReportsService.Reporte_Desperdicios` (consolidado master 1/2, pies = largo − consumido).

## Validacion (queries dev)

BD dev: `192.168.10.10\InvoiceRitrama`. Comando: `sqlcmd -S 192.168.10.10 -d InvoiceRitrama -U <user> -P <pwd> -C`.

- OC por estado y anulacion: `SELECT step, CloseDocument, anulada, COUNT(*) FROM orden_corte GROUP BY step, CloseDocument, anulada;`
- OC con master 2 sin descontar (brecha ACC-01): `... WHERE TwoMasters=1 AND util2_real_lenght>0 ...`
- Restante por master y estado: ver `SQL_QUERY_SELECT_LOAD_ROLL_ID` (`R.cs:20`).
- Rollos cortados por disponibilidad: `SELECT disponible, despacho, COUNT(*) FROM rolls_details GROUP BY disponible, despacho;`

## Acciones pendientes (ACC)

- **ACC-01**: al cerrar OC con `TwoMasters`, descontar inventario del master 2 (consumo + desperdicio) igual que el master 1.
- **ACC-02**: bloquear anulacion de OC cerrada — **ya implementada** en `OrdenCorteService.cs:171-175` (validar solo que la UI no marque `anulada` local antes del resultado).
- **ACC-03**: validar en guardado que `sum(width de cortes) == width del master` (cuadre correcto).
- **ACC-04**: al hacer el cuadre de la cantidad de rollos a producir, rechazar un master cuyo `length` restante no alcance; obligar a elegir master con material suficiente.

## Referencias de codigo (archivo:línea)

- `Forms\FrmOrdenCorte.cs`: `Opt_create_document` (crear), `Opt_modif_orden` (:1463), `Opt_cerrar_orden` (:1662), `Opt_etiquetar_orden` (:1714) / `EtiquetarOrdenCorte` (:1750), `Opt_aprobar_orden` (:1719), `GENERAR_ROLLOS_CORTADOS` (:546), `VALIDAR_CORTES`/`ValidDefintionsCortes` (:2086), `ACTUALIZAR_INVENTARIOS_MASTER` (:2350), `Btn_vueltas` (:2371), `reporteDeDesperdiciosToolStripMenuItem_Click` (:2598).
- `Services\ProduccionService\OrdenCorteService.cs`: `BuildSqlRollIdDisponibles` (:108-125), `AnularOrdenCorte` (:143-192), insert de rollos (:437), actualizar items (:730), insert config vueltas (:829), `RollosCortadosDispobnibles` (:859).
- `Services\ProduccionService\ConsumoMasterService.cs`: `ActualizarInventariosMasterAsync` (:121-168), desbloqueo `disponible=1` (:157).
- `Core\CalculosOrdenCorte.cs`: `FACTOR_MSI=0.012`, `CalcularMsi`, `RestanteMaterial`, `LongitudTotal`, `LongitudReal`.
- `Services\DespachoService\DespachoService.cs` (:71, :434); `Services\CommonService\CommonService.cs` (:63-64).
- `R.cs`: :20 (`SQL_QUERY_SELECT_LOAD_ROLL_ID` con restante/estado), :82 (`SELECT_QUERY_PRODUCTS` con categorias).
- `Reports\ReporteDesperdicios.rdlc` y `Services\ReportsService\ReportsService.cs` (`Reporte_Desperdicios`).