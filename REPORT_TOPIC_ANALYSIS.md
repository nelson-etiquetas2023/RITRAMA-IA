# Análisis de Temas Solicitados

## Tema 1: Toolbar SunnyUI programática

### Toolbar existente en `Forms/FrmOrdenCorte.cs`

- **`toolStrip1`**: Línea 95 (`toolStrip1.Items.Add(bot_auditoria)`). El botón `bot_auditoria` es creado programáticamente en el constructor (línea 89-93) como `ToolStripButton("Auditoria (0)")` y muestra el count de hallazgos pendientes en la auditoria. Su ForeColor cambia a rojo si hay pendientes.

- **`CloseToolsBar()`** (línea 3470): Programa el estado de la toolbar al salir del modo edición - deshabilita botones de navegación, búsqueda, guardado, generación de rollos, y otros controles. Reactiva `bot_guardar` y `bot_cancelar`.

- **`ResaltarControlesEditables(bool activo)`** (línea 2012): Cuando la OC está en modo edición (EditMode=2), resalta los controles con color rosado (`ColorEdicionCampos = Color.FromArgb(255, 214, 216)`). Al salir del modo, restaura los colores originales guardados mediante reflexión (propiedades `FillColor`, `FillReadOnlyColor`, `FillDisableColor`, `RectColor`, `StyleCustomMode`).

- **`AplicarTemaVerde()`** (línea 179): Sobrescribe el tema por defecto naranja de SunnyUI con una paleta verde (`Color.FromArgb(110, 190, 40)`). Se aplica en el constructor y en `InitializeAsync()` para que el form aparezca verde desde el primer paint (evita el parpadeo naranja→verde).

- **`SobrescribirControlesConPaletaVerde(Color verde, Color verdeOsc)`** (línea 203): Aplica manualmente los colores verde/verdePastel a todos los controles SunnyUI del form (txt_cust_name, grid_cortes, grid_items) mediante reflexión a propiedades `FillColor`, `RectColor`, `ButtonFillColor`, etc.

- **Botones de navegación y acción**: Varios botones (`btn_buscar_rollid1`, `btn_buscar_rollid2`, `btn_generar_rollos`, `btn_buscar_orden`, etc.) son habilitados/deshabilitados dinámicamente según el `EditMode` y el estado del documento (step, anulado, cerrado). Ver lines: 1296, 1300, 1752, 1756, 2275, 2314, 2370, 2441-2442, 3443-3444, 3718.

- **`opt_create_document`** (línea 2187): Handler del menú "Crear documento" que inicia una nueva OC en modo add (EditMode=1). Llena datos por defecto (rollid_1="0", rollid_2="0", anchos/longitudes en 0, etc.) y habilita controles para edición.

### Gaps / Pendientes - Toolbar SunnyUI

1. **Falta documentación clara**: No hay comentarios que expliquen el propósito de cada botón de la toolbar más allá de `bot_auditoria`. El flujo esperado al hacer clic en cada botón no está documentado en el código.

2. **Estado de toolbar inconsistente en algunos caminos**: El método `CloseToolsBar()` deshabilita `btn_buscar_rollid1` y `btn_buscar_rollid2`, pero al reingresar al modo edición (`Opt_modif_orden_Click` línea 2314) solo re-habilita `btn_buscar_rollid1`. `btn_buscar_rollid2` requiere que `chk_two_master.Checked` esté activo (línea 2370).

3. **Flash de tema al cargar**: Aunque `AplicarTemaVerde()` se llama en el constructor y en `InitializeAsync()`, el form podría mostrar un flash naranja antes de que el tema verde se aplique completamente, dependiendo del orden de eventos de UI.

4. **Toolbar "sunnyUI programática" sin diseñador**: El botón `bot_auditoria` y posiblemente otros elementos de la toolbar no están definidos en el archivo `.Designer.cs`, lo que sugiere que fueron añadidos completamente en código. Esto puede ser intencional pero requiere mantener la consistencia con lo que el diseñador podría generar.

5. **Falta de estado toolbar para modo two-master**: No hay lógica explícita para habilitar `btn_buscar_rollid2` cuando `chk_two_master` es true y el modo edición está activo, más allá del handler `Chk_two_rollid_CheckedChanged` → `WorkflowMastersTwoRollid()` (línea 3715).

---

## Tema 2: Cómo se editan los rollid (rollid_1/rollid_2) en una OC

### Flujo actual para buscar y cambiar rollid_1/2 en una OC

#### `Btn_buscar_rollid1_Click` (línea 621-715)

1. Abre `Frm_RollId` con el servicio y `OcExcluir = txt_numeroOC.Text.Trim()`.
2. Si el usuario selecciona un master roll (`frmrollid.MasterRoll != null`):
   - Guarda el rollid anterior (`txt_rollid_1.Text.Trim()`) y el nuevo (`frmrollid.MasterRoll.Roll_Id`).
   - **Validación de material**: Si hay `long_cortar` > 0 y `vueltas1` > 0, calcula el consumo requerido (`LongitudTotal`) y verifica que el master tenga suficiente largo disponible. Si no, registra hallazgo en auditoría y aborta.
   - **Validación de cambio de master**: Si el rollid es diferente al anterior y la OC tiene consumo actual (`real1_length` > 0), muestra un `MessageBox` de confirmación para reasignar consumo del master anterior al nuevo.
   - Llama a `Service.ReasignarConsumoMasterAsync()` para reasignar el consumo del master antiguo al nuevo (devuelve material del anterior y descuenta del nuevo).
   - **Actualiza UI**: `Rollid_master = rollidNuevo`; `txt_rollid_1.Text = rollidNuevo`; `txt_width1.Text` y `txt_length1.Text` con ancho/longitud del master; `txt_real1.Text` (largo real); `txt_product_id.Text` y `txt_product_name.Text`; `Txt_tipo_master.Text = TipoMovimiento`.
   - Llama a `CALCULATE_TOTAL_WIDTH_CORTES()` y `CALCULATE_MATERIAL_RESTANTE()`.
   - Valida y foca en `txt_rollid_1`.

#### `Btn_buscar_rollid2_Click` (línea 3728-3841)

Flujo idéntico al rollid_1 con validaciones adicionales:

1. Abre `Frm_RollId` mismo modo.
2. Si se selecciona un master:
   - **Validación de producto**: El master 2 debe ser del mismo producto que el master 1 (comparación `Product_Id`). Si no, muestra mensaje de warning y aborta.
   - **Validación de material**: Igual que rollid_1, verifica que el largo del master 2 alcance para `long_cortar2 x vueltas2`.
   - **Validación de tipo de inventario**: Antes de reasignar consumo, llama a `ResolerTipoMaster()` para obtener el tipo (`INIC.` o `Por Compras`). Si no puede determinar el tipo, reporta error.
   - **Confirmación de reasignación**: Igual que rollid_1, muestra mensaje de confirmación si ya hay consumo actual.
   - Llama a `Service.ReasignarConsumoMasterAsync()` con parámetros del master 2.
   - **Actualiza UI**: `txt_rollid_2.Text = rollidNuevo`; `txt_width2.Text` y `txt_length2.Text`; llama a `CALCULATE_TOTAL_WIDTH_CORTES()` y `CALCULATE_MATERIAL_RESTANTE()`.

### Dónde se pueden editar los rollid

1. **Nivel UI (form)**: Los campos `txt_rollid_1` y `txt_rollid_2` son `Sunny.UI.UITextBox` que están data-bound a `BsMaster("rollid_1"/"rollid_2")` en `HeaderBinding()` (líneas 434 y 445). También son actualizados directamente por los handlers `Btn_buscar_rollid1_Click` y `Btn_buscar_rollid2_Click`.

2. **Nivel datos (dataset)**: Las binding lines en `HeaderBinding()` (línea 434: `txt_rollid_1.DataBindings.Add("Text", BsMaster, "rollid_1")` y línea 445: `txt_rollid_2.DataBindings.Add("Text", BsMaster, "rollid_2")`) conectan los textbox con el DataSet `Ds.Tables["DtMaster"]`. Cualquier cambio en el textbox actualiza la fila master y viceversa.

3. **Nivel servicio (OrdenCorteService.cs)**:
   - `BuildSqlRollIdDisponibles()` (línea 111-141): Construye la query SQL que carga los masters disponibles en el browser de rollids. Filtra por `MasterRolls = 1` y restante > 100 pies. Excluye masters ya asignados a otras OCs activas (no anuladas, no cerradas) mediante `NOT EXISTS`. Usa parámetros `ocolExcluir` para excluir la OC actual al editar.
   - `BuscarRollId()` (línea 153-163): Método público que llama a `BuildSqlRollIdDisponibles` y carga la tabla de rollids disponibles.
   - `GuardarEncabezadoOrdenCorteCore()` (línea 432-516): INSERT en la tabla `orden_corte` con los campos `@p5` (rollid_1) y `@p8` (rollid_2). También llama a `ActualizarDocumentoMaster()` para persistir el número de OC en el master.
   - `ActualizarDocumentoMaster()` (línea 519-534): UPDATE en `MasterInic` y `ItemsMateria` estableciendo `documento = @num` (número de OC) donde `Roll_Id`/`rollid` coincide. Esto es el "cruce master ↔ OC": cuando se guarda una OC, el master queda marcado con el número de la orden.
   - `AplicarReglaRestanteOC()` (línea 727-742): Recalcula `Rest1_lenght` y `Restante_rollid1/2` como `largo_master - consumo_oc`. Forza que el restante archivado sea siempre `largo_master - consumo`, nunca 0 si el master tiene material.

### Patrón usado para asignar rollids

1. **Selección**: Usuario hace clic en `btn_buscar_rollid1` o `btn_buscar_rollid2` → se abre `Frm_RollId` con la lista de masters disponibles (filtrados por material y producto).

2. **Validación**: Antes de aceptar la selección, el sistema valida:
   - Suficiencia de material (largo master >= consumo OC)
   - Product matching (master 2 debe ser mismo producto que master 1)
   - Tipo de inventario (INIC vs Por Compras)
   - Confirmación de reasignación si ya hay consumo previo

3. **Reasignación de consumo**: Si la OC ya tenía un master asignado con consumo previo, el sistema:
   - Devuelve el consumo al master anterior (`UPDATE MasterInic SET largo_consumido = largo_consumido - @total`)
   - Descuenta del nuevo master (`UPDATE MasterInic SET largo_consumido = largo_consumido + @total`)
   - Registra el detalle en `MasterDetailsInic`/`ItemsMateria`

4. **Actualización de UI**: Los textbox se actualizan con los nuevos valores, y los métodos de cálculo (`CALCULATE_TOTAL_WIDTH_CORTES`, `CALCULATE_MATERIAL_RESTANTE`) actualizan campos dependientes (`txt_width1/2`, `txt_length1/2`, `txt_matrest1/2_lenght`, etc.).

5. **Persistencia**: Al guardar la OC (`GuardarOrdenUpdate` o `GuardarOrdeAddMode`), el servicio:
   - Escribe `rollid_1` y `rollid_2` en la tabla `orden_corte`
   - Llama a `ActualizarDocumentoMaster()` para marcar el master con el número de OC
   - Ejecuta `AplicarReglaRestanteOC()` para calcular y persistir `rest1_lenght`, `restante_rollid1`, `rest2_lenght`, `restante_rollid2`

### Gaps / Problemas detectados - rollid editing

1. **Falta validación de producto master 2 vs master 1 en el browser**: La validación de que ambos masters deben ser del mismo producto ocurre solo al momento de seleccionar el rollid2 (línea 3741-3751), pero no al cargar el browser de rollids. Esto podría permitir seleccionar masters de productos distintos para el master 1 y 2 antes de dar el error.

2. **Conflictos de data-binding**: `txt_rollid_1/2` están data-bound a `BsMaster("rollid_1"/"rollid_2")` pero también son actualizados directamente por código en los handlers de búsqueda. Si el DataSet cambia por otra vía (ej. navegación entre OC), los textbox podrían desactualizarse o causar exceptions por escritura cruzada.

3. **No hay validación cuando se desactiva `chk_two_master`**: Si el usuario tiene dos masters seleccionados y desmarca `chk_two_master`, los campos `txt_rollid_2`, `txt_width2`, `txt_length2` no se limpian automáticamente. El código los deshabilita (`WorkflowMastersTwoRollid()` línea 3715-3727) pero no borra su contenido.

4. **Flujo de `txt_step` y `EditMode` desactualizado**: El `txt_step` está data-bound a `BsMaster("step")` (línea 467) pero también se asigna directamente en varios lugares (`txt_step.Text = "2"`, `"3"`, `"5"`). El `EditMode` (0=normal, 1=add, 2=edit) se gestiona por los opt menús pero puede desincronizarse con el step real de la BD.

5. **Browser de rollid no filtra por producto master 1**: El `BuildSqlRollIdDisponibles` tiene un parámetro `columna` y `texto` para búsqueda por Part_Number, Product_Name o Roll_Id, pero no filtra automáticamente por el producto del master 1. El usuario puede seleccionar cualquier master disponible y solo recibe el error de "producto distinto" al intentar asignarlo como master 2.

6. **Sin validación de rollid "0" o vacío en validaciones críticas**: En `ValidarDocumento()` (línea 3047) hay chequeo para `txt_rollid_1.Text == "0"`, pero en `PuedeGenerarRollos()` (línea 897) y `ValidarDocumento()` (línea 3183) hay chequeos condicionales que dependen de `chk_two_master.Checked`. Faltaría una validación unificada.

7. **Falta de persistencia del `Roll_Id` en el grid de rollos cortados**: En `GENERAR_ROLLOS_CORTADOS()` (línea 791-878), los rollos cortados tienen su `roll_id` establecido desde `txt_rollid_1.Text` / `txt_rollid_2.Text` (líneas 823, 854), pero si el usuario cambia el rollid después de generar los rollos, estos conservan el rollid original a menos que se regeneren.

8. **CrearOrdenTemporal en tests usa `Roll_Id = "X"`**: En `OrdenCorteServiceTests.cs` línea 355, el método `CrearOrdenValida` usa `Rollid_1 = "X"` y `Rollid_2 = "X"` como placeholders. Esto sugiere que en tests no se asignan rollids reales, lo que podría ocultar bugs de validación en el flujo real.