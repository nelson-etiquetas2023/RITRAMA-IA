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
- **REGLA DE PRODUCCIÓN (negocio): un master se puede consumir entre 1 o VARIAS OC según sea la necesidad.** No es exclusivo de una sola OC.
- El picker (`BuildSqlRollIdDisponibles`, `OrdenCorteService.cs:108-132`) **sí ofrece masters con consumo previo (sobrante)** aunque estén asignados como `rollid_1`/`rollid_2` de otra OC abierta: la exclusión aplica `AND (ISNULL(ct.largo_consumido, 0) > 0 OR NOT EXISTS (...))`, es decir, solo quedan **fuera** los masters asignados a otra OC activa que **aún no han consumido nada** (para no comprometer el material completo a dos OC al mismo tiempo).
- La columna **"Documento OC"** del inventario de masters (`R.cs:20`, `documento_oc`) lista los **números de las OC no anuladas** que usaron el master (p. ej. `4611,4612`); si no tiene OC lo muestra vacío.
- Al **etiquetar**, `GuardarEtiquetado` registra el consumo del master por OC (idempotente por `(rollid,orden,desperdicio)`), de modo que cada OC que consume deja su parte descontada y el restante queda disponible para la siguiente OC.
- **Etiquetado con múltiples OC (ACC-05, implementada):** `ValidarConsumoDisponibleMaster` (`OrdenCorteService.cs:726`) reemplaza la vieja prohibición (`ValidarMasterDisponible`). Al etiquetar NO se bloquea un master por estar asignado a otra OC abierta: se valida que el consumo a registrar de esta OC (consumo + desperdicio) **no exceda el material disponible real** = `largo del master − Σ consumo ya registrado por otras OC no anuladas`. Si sobra, etiqueta (master 1 y 2, y tipo Inic./Por Compras); si no alcanza, aborta con mensaje de pies disponibles. Permite el caso legítimo de reutilizar un master con sobrante (p. ej. `1251209453` en OC `4611,4612`) sin permitir que dos OC comprometan el material completo.
- Dado que el restante se acumula de OC no anuladas con `consumo > 0`, no reutilizar un master cuyo `length` no alcance para los rollos a producir.
- Estado `Desperdicio` (solo módulo Inventario, `SQL_QUERY_SELECT_LOAD_ROLL_ID_INVENTARIO`): master con restante ≤ 100 pies se muestra en rojo; el OC picker y reportes siguen con `Agotado`.

### RN-CORTES: Cuadre a lo ancho
- La suma de los anchos de los cortes **no debe exceder** el ancho del master (ej.: master de 60 plg → cortes 20+40=60 OK, o suma < 60 OK). El master es la materia prima: no se pueden producir cortes más anchos de lo que permite el ancho del rollo master.
- El codigo calcula `real1_width` = suma de anchos y `rest1_width` = width del master − `real1_width`. La validacion usa `CalculosOrdenCorte.SumaCortesNoExcedeMaster` (suma ≤ ancho del master con tolerancia) → ver ACC-03.
- **Regla visual interactiva (PintarValidacionGrid):** el grid de cortes usa el color base del modo actual (verde al crear `ColorCreacionGrid`, rosa al editar `ColorEdicionGrid`, blanco en solo lectura). Si se viola la regla de cuadre a lo ancho, **TODO el grid cambia a `ColorValidacionRojo`** (`FrmOrdenCorte.PintarValidacionGrid`, reglas en `HayViolacionAncho`/`HayViolacionLargoMaster1`/`HayViolacionLargoMaster2`). **Si la violación es de largo** (longitud a cortar × vueltas > largo del master), además del grid se pintan **en rojo los textboxes de longitud a cortar y vueltas** del master que falla (`PintarTextboxesPorLargo`, `AplicarOVerificarTextboxLargo`); al cumplirse vuelven al color del modo (verde crear `ColorCreacionCampos` / rosa editar `ColorEdicionCampos` / original en solo lectura). Ayudantes Core: `CalculosOrdenCorte.CortesExcedenAnchoMaster`, `CalculosOrdenCorte.ConsumoExcedeLargoDisponible`. Se reevalúa al: CellEndEdit del grid, agregar/fila corte, montar master 1 y 2, cambiar vueltas1/2, cambiar longitud a cortar 1/2. **Al guardar correctamente**, `ActivarModoSoloLectura` → `ResaltarControlesEditables(Ninguno)` → `PintarValidacionGrid` restaura todo: textboxes a `RestaurarFillColor`, grid a blanco, botón verde deshabilitado.

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
- El etiquetado genera el **ROLLID** (`unique_code`) de cada rollo cortado (`EtiquetarOrdenCorte`, `FrmOrdenCorte.cs:1750`), con secuencia `RC*`.
- **SIN SALTOS en el consecutivo RC**: el bloque [RC(primero)..RC(ultimo)] se reserva de forma **atomica** con `UPDATE control (filter='UC') ... OUTPUT` (`GuardarEtiquetado`, `OrdenCorteService.cs`), auto-alineando el contador contra el **máximo RC real** de `rolls_details` + `RollsInic` (sanea contadores atrasados/adelantados). Toda la escritura (códigos + `step=3` + contador) va en **una sola transacción** que revierte si falla, evitando huecos o duplicados por carreras o fallos parciales. No se debe tocar el contador `UC` manualmente ni saltarse el flujo de etiquetado.
- **Registro de consumo al etiquetar**: en la misma transacción de `GuardarEtiquetado` se registra el consumo de inventario del/los master (consumo real `util*_real_lenght` + desperdicio, y detalle en `MasterDetailsInic`), de modo que **una OC etiquetada siempre deja su consumo registrado** y su master queda descontado. Escritura **idempotente** por `(rollid,orden,desperdicio)` (si el detalle ya existe no vuelve a descontar). Si el consumo no se puede registrar, la transacción revierte y la OC NO queda etiquetada.

### RN-CONSUMO-SUM: Suma de consumos por master (invariante de inventario)
- Invariante ESTRICTO: **SUM(consumo de TODAS las OC `anulada=0` que comparten el mismo rollid) ≤ lenght del master** en TODO momento (crear Y editar). Violarlo deja el inventario del master en **negativo** y esta PROHIBIDO. Caso real generado: OC 4616 (1000×11=11000) + OC 4617 (1017×10=10170) sobre master 225017001 (largo 20177) → 21170 > 20177 → restante del picker = **−993**.
- Compromiso de cada OC: el **mayor** entre su consumo PLANIFICADO (`longitud_cortar × cortes_largo` / `longitud_cortar2 × vueltas2`) y su consumo REAL (`util1/2_real_lenght` + desperdicio). NUNCA subestimar un compromiso (una OC en edicion puede re-planificar y el real puede superar el plan).
- **UI (crear y editar):** `FrmOrdenCorte.ValidarDocumento` consulta `Service.ConsumoComprometidoOtrosOC(rollid, ocExcluir)` (suma el compromiso de las OC de OTRAS ordenes excluyendo la OC actual) y bloquea si `LongitudTotal(longitud_cortar, vueltas) > txt_lengthN − comprometidoOtros + 0.01`. Así se impide MONTAR (crear) una segunda OC cuando la OC(s) ya montada(s) comprometen todo el master — el hueco original que dejó pasar a 4616+4617.
- **Backstop al editar:** `Update_Header_Documnet_OC` → `ValidarConsumoTotalizadoMaster` (`OrdenCorteService.cs`) dentro de la transaccion, antes de persistir: la OC en edicion compromete `MAX(nuevo consumo, util1_real_lenght)` y se suma al compromiso de las otras OC (planificado **o** real, el mayor, `anulada=0`, sin la OC editada). Si excede el lenght, lanza `InvalidOperationException`, rollback y **no se guarda**. Permite corregir REDUCIENDO (ej.: 11000+1017×9=20153 ≤ 20177) — la regla es estricta pero corregible.
- **UX obligatoria (no romper la app):** la validacion NO usa modales, con UNA excepcion: **RN-CONSUMO-SUM** (`FrmOrdenCorte.ValidarDocumento` codigos `RN_CONSUMO_SUM_M1/M2` y backstop `GuardarOrderUpdate`/`RN_CONSUMO_SUM`) muestra mensaje modal "Validacion Material" y **bloquea el guardado** — el sistema no permite guardar si la sumatoria excede el master. El resto de hallazgos va a `IAuditoriaStore` (origen `ValidacionDocumento`, `CUADRE_ANCHO`, etc.) y el badge "Auditoria (N)" avisa; la auditoria se revisa bajo demanda en `FrmAuditoriaInconsistencias`. `FrmOrdenCorte.Bot_guardar_Click` solo pasa los controles a solo lectura si el guardado devolvio `true` (`GuardarOrderNew`/`GuardarOrderUpdate` devuelven `bool`). Si la validacion rechaza, la OC queda **editable** para que el operador corrija longitud a cortar/vueltas y reintente. No debe crashear ni dejar el form bloqueado.
- Complementa a ACC-04 (que solo compara esa OC contra el largo **completo** del master) y a la validacion por-OC del etiquetado (`ValidarConsumoDisponibleMaster`, `OrdenCorteService.cs:726`). Aplica tambien el filtro `anulada=0` (mismo criterio que el etiquetado).
- Quirk de edicion: `Update_Header_Documnet_OC` persiste solo los campos del **master 1**; la validacion totalizada corre para `Rollid_1`.

### RN-CONSUMO-RESTANTE: el inventario nunca queda en negativo
- Regla: **NINGUN registro de consumo puede dejar el restante de un master/material en negativo** (`restante = largo − total consumido`). Violarlo deja inventario negativo (caso OC 4616+4617 sobre master 225017001 → restante del picker **−993**).
- Implementada en el registro **físico/contable** del consumo `ActualizarInventariosMasterAsync` (`ConsumoMasterService.cs:121`, usado por el cierre `ACTUALIZAR_INVENTARIOS_MASTER` y botones de inventario): antes de descontar stock, si se escribira un detalle NUEVO (excluye lo ya registrado por idempotencia), exige `cantidad a escribir ≤ restante` donde `restante = largo master − SUM(consumo en MasterDetailsInic)`. Si no cabe, fija `ErrorMsg` (motivo RN-CONSUMO-RESTANTE) y devuelve `false`; `FrmOrdenCorte` registra el motivo como hallazgo de `Cierre` en la auditoria (sin modal).
- Capas que ya impedían el negativo: **RN-CONSUMO-SUM** (crear/editar, compromiso planificado vs largo), **`ValidarConsumoDisponibleMaster`** (etiquetado, consumo real vs largo real restante) y el **picker** que solo ofrece masters con `restante > 100`. Esta regla cierra el último hueco: la escritura contable del consumo.
- Reasignacion (`ReasignarConsumoMasterAsync`) NO se bloquea a propósito: traslada un consumo ya registrado entre masters (no añade consumo neto al sistema) para no romper flujos/tests.

### RN-EDICION
- Solo se modifica una OC **no anulada y en `step=2`** (`Opt_modif_orden`, `FrmOrdenCorte.cs:1463`).
- Al guardar la edicion se valida **RN-CONSUMO-SUM** (suma de consumos de las OC con el mismo rollid ≤ lenght del master); si no pasa, mensaje + bloqueo hasta corregir parametros.
- Quirk conocido: `GuardarOrderUpdate` actualiza solo los campos del **master 1**, no los del master 2.

### RN-CIERRE (consumo de inventario)
- `Opt_cerrar_orden` (`FrmOrdenCorte.cs:1662`): `UpdateStatusDocumentOC(5)` → `ACTUALIZAR_INVENTARIOS_MASTER`.
- El consumo de inventario de los masters se registra **al etiquetar** (ver RN-ETIQUETADO); al cerrar `ACTUALIZAR_INVENTARIOS_MASTER` es una red de seguridad mantenida para OC históricas y es **idempotente**: si ya existe el detalle `(rollid,orden,desperdicio)` en `MasterDetailsInic` (etiquetado previo, cierre repetido o corrida anterior) no vuelve a descontar stock ni duplica el detalle; solo libera `disponible=1`.
  - Inventario de master: `largo_consumido += consumo` (consumo real y, si `desperdicio`, el desperdicio = largo − usado) en `MasterInic`/`ItemsMateria` via `ActualizarInventariosMasterAsync` (`ConsumoMasterService.cs:121-168`), que consulta `SQL_QUERY_CONSUMO_OC_DETALLE_EXISTE` (`R.cs:40`) con `ConsumoDetalleExisteAsync` antes de escribir.
  - Inventario de rollos cortados (producto terminado): `UPDATE rolls_details SET disponible=1 WHERE numero=@oc` en la misma transaccion.
- Con `TwoMasters` el cierre descuenta **ambos masters**: `ACTUALIZAR_INVENTARIOS_MASTER` (`FrmOrdenCorte.cs:2863`) llama 2 veces al service (master 1 + master 2 con su tipo resuelto). No es necesario un paso adicional.

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

- **ACC-01**: al cerrar OC con `TwoMasters`, descontar inventario del master 2 (consumo + desperdicio) igual que el master 1. **Cubierto**: el cierre llama `ActualizarInventariosMasterAsync` para ambos masters; **idempotencia** (no doble descuento cierre/etiquetado) implementada con `ConsumoDetalleExisteAsync` + `SQL_QUERY_CONSUMO_OC_DETALLE_EXISTE`.
- **ACC-02**: bloquear anulacion de OC cerrada — **ya implementada** en `OrdenCorteService.cs:171-175` (validar solo que la UI no marque `anulada` local antes del resultado).
- **ACC-03**: validar en guardado que `sum(width de cortes) <= width del master` (los cortes no pueden exceder el ancho de la materia prima) — **implementada** con `SumaCortesNoExcedeMaster`, calculando la suma desde `grid_cortes` (no desde `total_salida` de la BD, que historico quedaba en 0 por desalineacion de parametros ya corregida).
- **ACC-04**: rechazar un master cuyo `lenght` no alcance para el consumo de la orden = `longitud a cortar × vueltas` (ej.: 1500 pies × 13 vueltas = 19.500 pies; cada vuelta produce tantos rollos como cortes). **Implementada** en el flujo de **modificar OC**:
  - Al **montar** (seleccionar) el master: `Btn_buscar_rollid1_Click` / `Btn_buscar_rollid2_Click` (FrmOrdenCorte.cs) validan ANTES de reasignar consumos si `MasterTieneMaterialSuficiente(LongitudTotal(longitud, vueltas), master.Length)`; si no alcanza, no se monta y se sugiere ajustar cortes/longitud/vueltas al material disponible.
  - Al **guardar**: `ValidarDocumento` (FrmOrdenCorte.cs:2522) valida el master 1 y (two-master) el 2 contra `txt_length1/2` (mismos helpers).
- **ACC-05**: alinear el etiquetado con la regla de múltiples OC (RN-MASTER) — **implementada** con `ValidarConsumoDisponibleMaster` (`OrdenCorteService.cs:726`): reemplazó a `ValidarMasterDisponible`; permite etiquetar si el consumo de esta OC cabe en `largo del master − consumo ya registrado por otras OC` (sobrante real), cubriendo master 1/2 y tipo Inic./Por Compras. Build 0/0, 28/28 tests.
- **ACC-06**: impedir que DOS O MÁS OC montadas sobre el mismo rollid superen el lenght del master (RN-CONSUMO-SUM) — **implementada**: al **crear/editar** `FrmOrdenCorte.ValidarDocumento` bloquea via `Service.ConsumoComprometidoOtrosOC` (compromiso de las otras OC, max(planificado, real)) registrando hallazgo `RN_CONSUMO_SUM_M1/M2` en la auditoria (sin modal) y al **editar** `ValidarConsumoTotalizadoMaster` (`OrdenCorteService.cs`, dentro de `Update_Header_Documnet_OC`) lanza `InvalidOperationException`, rollback y `GuardarOrderUpdate` registra el hallazgo y **bloquea el guardado** hasta corregir longitud a cortar/vueltas.

## Referencias de codigo (archivo:línea)

- `Forms\FrmOrdenCorte.cs`: `Opt_create_document` (crear), `Opt_modif_orden` (:1463), `Opt_cerrar_orden` (:1662), `Opt_etiquetar_orden` (:1714) / `EtiquetarOrdenCorte` (:1750), `Opt_aprobar_orden` (:1719), `GENERAR_ROLLOS_CORTADOS` (:546), `VALIDAR_CORTES`/`ValidDefintionsCortes` (:2086), `ACTUALIZAR_INVENTARIOS_MASTER` (:2350), `Btn_vueltas` (:2371), `reporteDeDesperdiciosToolStripMenuItem_Click` (:2598).
- `Services\ProduccionService\OrdenCorteService.cs`: `BuildSqlRollIdDisponibles` (:108-132, picker que muestra masters con sobrante aunque estén en otra OC activa y excluye solo los asignados sin consumo previo, `ocExcluir` = OC actual), `AnularOrdenCorte` (:143-192), `GuardarEtiquetado`/`ReservarRangoUniqueCodeConsec`/`ValidarConsumoDisponibleMaster` (:726, múltiples OC por material disponible)/`RegistrarConsumoMaster` (etiquetado sin saltos + consumo atómico, tx única), insert de rollos (:437), actualizar items (:730), insert config vueltas (:829), `RollosCortadosDispobnibles` (:859).
- `Services\ProduccionService\ConsumoMasterService.cs`: `ActualizarInventariosMasterAsync` (:121-168), desbloqueo `disponible=1` (:157).
- `Core\CalculosOrdenCorte.cs`: `FACTOR_MSI=0.012`, `CalcularMsi`, `RestanteMaterial`, `LongitudTotal`, `LongitudReal`.
- `Services\DespachoService\DespachoService.cs` (:71, :434); `Services\CommonService\CommonService.cs` (:63-64).
- `R.cs`: :20 (`SQL_QUERY_SELECT_LOAD_ROLL_ID` con restante/estado y `documento_oc` = lista de OC no anuladas que usaron el master), :25 (`SQL_QUERY_SELECT_LOAD_ROLL_ID_INVENTARIO` variante Desperdicio), :40 (`SQL_QUERY_CONSUMO_OC_DETALLE_EXISTE` para idempotencia), :82 (`SELECT_QUERY_PRODUCTS` con categorias).
- `Reports\ReporteDesperdicios.rdlc` y `Services\ReportsService\ReportsService.cs` (`Reporte_Desperdicios`).