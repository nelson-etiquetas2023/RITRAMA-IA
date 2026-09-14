-- Script SQL: Tablas de Log de Operaciones
-- Ejecutar en la base de datos InvoiceRitrama
-- Fecha: 2026-09-09
-- Propósito: Registrar todas las operaciones importantes para auditoría y troubleshooting

-- 1. Tabla principal de logs de operaciones
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'operaciones_log')
BEGIN
    CREATE TABLE operaciones_log (
        id BIGINT IDENTITY(1,1) PRIMARY KEY,
        fecha_inicio DATETIME2 NOT NULL DEFAULT GETDATE(),
        fecha_fin DATETIME2 NULL,
        tipo_operacion NVARCHAR(50) NOT NULL,
        descripcion NVARCHAR(500) NOT NULL,
        usuario NVARCHAR(100) NOT NULL DEFAULT '',
        maquina NVARCHAR(100) NOT NULL DEFAULT '',
        ip_address NVARCHAR(50) NOT NULL DEFAULT '',
        exitoso BIT NOT NULL DEFAULT 0,
        resultado NVARCHAR(1000) NOT NULL DEFAULT '',
        detalle_error NVARCHAR(2000) NOT NULL DEFAULT '',
        created_at DATETIME2 NOT NULL DEFAULT GETDATE()
    );

    PRINT 'Tabla operaciones_log creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'La tabla operaciones_log ya existe.';
END

-- 2. Tabla de detalles de operaciones
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'operaciones_log_detalle')
BEGIN
    CREATE TABLE operaciones_log_detalle (
        id BIGINT IDENTITY(1,1) PRIMARY KEY,
        operacion_id BIGINT NOT NULL,
        fecha DATETIME2 NOT NULL DEFAULT GETDATE(),
        accion NVARCHAR(100) NOT NULL,
        entidad NVARCHAR(100) NOT NULL,
        valor_anterior NVARCHAR(500) NULL,
        valor_nuevo NVARCHAR(500) NULL,
        notas NVARCHAR(1000) NOT NULL DEFAULT '',
        CONSTRAINT FK_operaciones_log_detalle_operacion 
            FOREIGN KEY (operacion_id) REFERENCES operaciones_log(id)
            ON DELETE CASCADE
    );

    PRINT 'Tabla operaciones_log_detalle creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'La tabla operaciones_log_detalle ya existe.';
END

-- 3. Índices para búsquedas rápidas
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('operaciones_log') AND name = 'IX_operaciones_log_fecha')
BEGIN
    CREATE INDEX IX_operaciones_log_fecha ON operaciones_log(fecha_inicio);
    PRINT 'Índice IX_operaciones_log_fecha creado.';
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('operaciones_log') AND name = 'IX_operaciones_log_tipo')
BEGIN
    CREATE INDEX IX_operaciones_log_tipo ON operaciones_log(tipo_operacion);
    PRINT 'Índice IX_operaciones_log_tipo creado.';
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('operaciones_log') AND name = 'IX_operaciones_log_estado')
BEGIN
    CREATE INDEX IX_operaciones_log_estado ON operaciones_log(exitoso);
    PRINT 'Índice IX_operaciones_log_estado creado.';
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('operaciones_log_detalle') AND name = 'IX_operaciones_log_detalle_operacion')
BEGIN
    CREATE INDEX IX_operaciones_log_detalle_operacion ON operaciones_log_detalle(operacion_id);
    PRINT 'Índice IX_operaciones_log_detalle_operacion creado.';
END

-- 4. Vista para consultas frecuentes
IF EXISTS (SELECT 1 FROM sys.views WHERE name = 'vw_HistorialOperaciones')
    DROP VIEW vw_HistorialOperaciones;
GO

CREATE VIEW vw_HistorialOperaciones AS
SELECT 
    l.id,
    l.fecha_inicio,
    l.fecha_fin,
    DATEDIFF(SECOND, l.fecha_inicio, ISNULL(l.fecha_fin, GETDATE())) AS duracion_segundos,
    l.tipo_operacion,
    l.descripcion,
    l.usuario,
    l.maquina,
    l.ip_address,
    CASE WHEN l.exitoso = 1 THEN '✓ Exitoso' ELSE '✗ Fallido' END AS estado,
    l.resultado,
    l.detalle_error,
    (SELECT COUNT(*) FROM operaciones_log_detalle d WHERE d.operacion_id = l.id) AS total_detalles
FROM operaciones_log l;
GO

PRINT 'Vista vw_HistorialOperaciones creada exitosamente.';
GO

-- 5. Vista para operaciones fallidas (monitoreo)
IF EXISTS (SELECT 1 FROM sys.views WHERE name = 'vw_OperacionesFallidas')
    DROP VIEW vw_OperacionesFallidas;
GO

CREATE VIEW vw_OperacionesFallidas AS
SELECT 
    id,
    fecha_inicio,
    tipo_operacion,
    descripcion,
    usuario,
    maquina,
    detalle_error,
    CASE 
        WHEN tipo_operacion = 'ETIQUETADO' THEN 'Verificar RC codes y consumo'
        WHEN tipo_operacion = 'CIERRE' THEN 'Verificar inventario masters'
        WHEN tipo_operacion = 'RECONCILIACION' THEN 'Revisar inconsistencias'
        WHEN tipo_operacion = 'ANULACION' THEN 'Verificar estado OC'
        ELSE 'Revisar logs manualmente'
    END AS accion_sugerida
FROM operaciones_log
WHERE exitoso = 0
  AND fecha_inicio > DATEADD(DAY, -7, GETDATE());
GO

PRINT 'Vista vw_OperacionesFallidas creada exitosamente.';
GO

-- 6. Stored procedure para limpiar logs antiguos
IF EXISTS (SELECT 1 FROM sys.procedures WHERE name = 'sp_LimpiarLogsAntiguos')
    DROP PROCEDURE sp_LimpiarLogsAntiguos;
GO

CREATE PROCEDURE sp_LimpiarLogsAntiguos
    @DiasRetencion INT = 90
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @FechaCorte DATETIME2 = DATEADD(DAY, -@DiasRetencion, GETDATE());
    DECLARE @Eliminados INT = 0;
    
    -- Eliminar detalles de operaciones antiguas
    DELETE FROM operaciones_log_detalle 
    WHERE operacion_id IN (
        SELECT id FROM operaciones_log WHERE fecha_inicio < @FechaCorte
    );
    
    SET @Eliminados = @@ROWCOUNT;
    
    -- Eliminar operaciones antiguas
    DELETE FROM operaciones_log WHERE fecha_inicio < @FechaCorte;
    SET @Eliminados = @Eliminados + @@ROWCOUNT;
    
    SELECT @Eliminados AS RegistrosEliminados, @FechaCorte AS FechaCorte;
END
GO

PRINT 'Stored procedure sp_LimpiarLogsAntiguos creado exitosamente.';
GO

-- 7. Tipos de operaciones predefinidas (referencia)
/*
TIPOS DE OPERACIONES SOPORTADOS:
- CREAR_OC: Creación de nueva orden de corte
- ETIQUETADO: Asignación de ROLLID a rollos
- CIERRE: Cierre de OC y actualización inventario
- ANULACION: Anulación de orden de corte
- RECONCILIACION: Detección y corrección de inconsistencias
- EDICION: Modificación de OC existente
- DESPACHO: Despacho de rollos
- INVENTARIO: Operaciones de inventario
- SISTEMA: Operaciones del sistema (inicio, cierre, etc.)
*/

PRINT '=== Instalación del Sistema de Logs completada ===';
PRINT '';
PRINT 'Tablas creadas:';
PRINT '  1. operaciones_log - Registro principal de operaciones';
PRINT '  2. operaciones_log_detalle - Detalle de acciones';
PRINT '';
PRINT 'Vistas creadas:';
PRINT '  1. vw_HistorialOperaciones - Consulta general';
PRINT '  2. vw_OperacionesFallidas - Monitoreo de errores';
PRINT '';
PRINT 'Procedimientos:';
PRINT '  1. sp_LimpiarLogsAntiguos - Mantenimiento de datos';
PRINT '';
PRINT 'Tipos de operaciones soportados:';
PRINT '  CREAR_OC, ETIQUETADO, CIERRE, ANULACION, RECONCILIACION,';
PRINT '  EDICION, DESPACHO, INVENTARIO, SISTEMA';
