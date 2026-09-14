-- ============================================================================
-- SCRIPT DE INDICES PARA OPTIMIZACION DE RENDIMIENTO (Ritrama2025)
-- Ejecutar en la base de datos de produccion. Los indices se crean solo si
-- no existen. Revisar antes de aplicar en produccion.
-- ============================================================================

-- orden_corte: busqueda de master rolls. Indices CUBRIENTES para el CTE de consumo
-- del inventario/estado de master (evitan key lookups por fila en la agregacion de
-- consumos). Sustituyen a los anteriores (rollid_x) simples.
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_orden_corte_rollid_1' AND object_id = OBJECT_ID('orden_corte'))
    DROP INDEX IX_orden_corte_rollid_1 ON orden_corte;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_orden_corte_rollid_1' AND object_id = OBJECT_ID('orden_corte'))
CREATE NONCLUSTERED INDEX IX_orden_corte_rollid_1 ON orden_corte(rollid_1, anulada) INCLUDE (util1_real_lenght, desperdicio, lenght_1);

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_orden_corte_rollid_2' AND object_id = OBJECT_ID('orden_corte'))
    DROP INDEX IX_orden_corte_rollid_2 ON orden_corte;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_orden_corte_rollid_2' AND object_id = OBJECT_ID('orden_corte'))
CREATE NONCLUSTERED INDEX IX_orden_corte_rollid_2 ON orden_corte(rollid_2, anulada) INCLUDE (util2_real_lenght, desperdicio2, lenght_2);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_orden_corte_estado' AND object_id = OBJECT_ID('orden_corte'))
CREATE NONCLUSTERED INDEX IX_orden_corte_estado ON orden_corte(anulada, CloseDocument) INCLUDE (numero);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_orden_corte_producto' AND object_id = OBJECT_ID('orden_corte'))
CREATE NONCLUSTERED INDEX IX_orden_corte_producto ON orden_corte(product_id);

-- MasterInic: join con consumo_total por Roll_Id
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MasterInic_Roll_Id' AND object_id = OBJECT_ID('MasterInic'))
CREATE NONCLUSTERED INDEX IX_MasterInic_Roll_Id ON MasterInic(Roll_Id) INCLUDE (Part_Number, Lenght, msi, fecha_pro, fecha_reg, splice, core, Ubicacion);

-- ItemsMateria: join con consumo_total por rollid
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ItemsMateria_rollid' AND object_id = OBJECT_ID('ItemsMateria'))
CREATE NONCLUSTERED INDEX IX_ItemsMateria_rollid ON ItemsMateria(rollid) INCLUDE (product_id, Width, length, msi, fecha_produccion, fecha_llegada, splice, core, Ubicacion);

-- producto: filtro MasterRolls = 1 en el CTE de master rolls
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_producto_MasterRolls' AND object_id = OBJECT_ID('producto'))
CREATE NONCLUSTERED INDEX IX_producto_MasterRolls ON producto(product_id, MasterRolls);

-- rolls_details: relacionado con orden_corte por numero (carga inicial y reportes)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_rolls_details_numero' AND object_id = OBJECT_ID('rolls_details'))
CREATE NONCLUSTERED INDEX IX_rolls_details_numero ON rolls_details(numero);

-- cortes: relacionado con orden_corte por orden (carga inicial y reportes)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_cortes_orden' AND object_id = OBJECT_ID('cortes'))
CREATE NONCLUSTERED INDEX IX_cortes_orden ON cortes(orden);

-- ============================================================================
-- P1 (2026-09): indices adicionales detectados en el diagnostico de rendimiento
-- ============================================================================

-- rolls_details: busqueda por codigo unico (etiquetado) y update por numero+unique_code
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_rolls_details_unique_code' AND object_id = OBJECT_ID('rolls_details'))
CREATE NONCLUSTERED INDEX IX_rolls_details_unique_code ON rolls_details(unique_code);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_rolls_details_numero_unique_code' AND object_id = OBJECT_ID('rolls_details'))
CREATE NONCLUSTERED INDEX IX_rolls_details_numero_unique_code ON rolls_details(numero, unique_code);

-- MasterDetailsInic: consumos por rollid y union con orden_corte por orden
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MasterDetailsInic_rollid' AND object_id = OBJECT_ID('MasterDetailsInic'))
CREATE NONCLUSTERED INDEX IX_MasterDetailsInic_rollid ON MasterDetailsInic(rollid);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MasterDetailsInic_orden' AND object_id = OBJECT_ID('MasterDetailsInic'))
CREATE NONCLUSTERED INDEX IX_MasterDetailsInic_orden ON MasterDetailsInic(orden);

-- orden_corte: join con customer en el encabezado de OC
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_orden_corte_customer' AND object_id = OBJECT_ID('orden_corte'))
CREATE NONCLUSTERED INDEX IX_orden_corte_customer ON orden_corte(customer_id);

-- vueltas: configuracion de vueltas por orden
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_vueltas_orden' AND object_id = OBJECT_ID('vueltas'))
CREATE NONCLUSTERED INDEX IX_vueltas_orden ON vueltas(orden);

-- operadores: validacion del operador por defecto durante el etiquetado
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_operadores_operador_id' AND object_id = OBJECT_ID('operadores'))
CREATE NONCLUSTERED INDEX IX_operadores_operador_id ON operadores(operador_id);

-- ============================================================================
-- P2 (2026-09): PK CLUSTERED (IDENTITY) PARA TABLAS HEAP
-- Ejecutado en la BD de desarrollo. Las 13 tablas heap obtienen una columna
-- de identidad 'id' (si no existe) y una PRIMARY KEY clustered.
-- Idempotente: no duplica columnas ni PKs.
-- ============================================================================

DECLARE @tablas TABLE (nombre sysname NOT NULL);
INSERT INTO @tablas VALUES
    (N'rolls_details'), (N'orden_corte'), (N'cortes'), (N'MasterInic'),
    (N'vueltas'), (N'despacho'), (N'rcdespacho'), (N'item_despacho'),
    (N'Paleta'), (N'operadores'), (N'control'), (N'MasterDetailsInic'), (N'RollsInic');

DECLARE @nombre sysname;
DECLARE @sql nvarchar(500);
DECLARE cur CURSOR LOCAL FAST_FORWARD FOR SELECT nombre FROM @tablas;
OPEN cur;
FETCH NEXT FROM cur INTO @nombre;
WHILE @@FETCH_STATUS = 0
BEGIN
    DECLARE @pk sysname = N'PK_' + @nombre;
    DECLARE @obj int = OBJECT_ID(QUOTENAME(@nombre));

    IF @obj IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM sys.indexes i WHERE i.object_id = @obj AND i.index_id <> 0 AND i.is_primary_key = 1)
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM sys.columns c WHERE c.object_id = @obj AND c.name = N'id')
        BEGIN
            SET @sql = N'ALTER TABLE ' + QUOTENAME(@nombre) + N' ADD id INT IDENTITY(1,1) NOT NULL;';
            EXEC sp_executesql @sql;
        END
        SET @sql = N'ALTER TABLE ' + QUOTENAME(@nombre) + N' ADD CONSTRAINT ' + QUOTENAME(@pk) + N' PRIMARY KEY (id);';
        EXEC sp_executesql @sql;
        PRINT 'PK clustered agregada a: ' + @nombre;
    END
    ELSE
    BEGIN
        PRINT 'Sin cambios (no existe o ya tiene PK): ' + ISNULL(@nombre, N'');
    END

    FETCH NEXT FROM cur INTO @nombre;
END
CLOSE cur;
DEALLOCATE cur;

-- ============================================================================
-- ADVISORY (NO se ejecutan automaticamente): historico del diagnostico.
-- Las siguientes tablas eran HEAP y YA recibieron PK clustered (id IDENTITY)
-- en la seccion P2 anterior:
--   rolls_details, orden_corte, cortes, MasterInic, vueltas, despacho,
--   rcdespacho, item_despacho, Paleta, operadores, control,
--   MasterDetailsInic, RollsInic
-- ============================================================================
