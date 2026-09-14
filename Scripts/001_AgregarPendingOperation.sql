-- Script SQL: Agregar campo pending_operation para rastreo de operaciones incompletas
-- Ejecutar en la base de datos InvoiceRitrama
-- Fecha: 2026-09-09
-- Propósito: Permitir detectar y recuperar operaciones interrumpidas por fallo de energía/red

-- 1. Agregar campo para rastrear operaciones pendientes
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('orden_corte') AND name = 'pending_operation')
BEGIN
    ALTER TABLE orden_corte ADD pending_operation NVARCHAR(50) NULL;
    PRINT 'Campo pending_operation agregado exitosamente.';
END
ELSE
BEGIN
    PRINT 'El campo pending_operation ya existe.';
END

-- 2. Agregar índice para búsquedas rápidas de operaciones pendientes
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('orden_corte') AND name = 'IX_orden_corte_pending_operation')
BEGIN
    CREATE INDEX IX_orden_corte_pending_operation ON orden_corte(pending_operation) WHERE pending_operation IS NOT NULL;
    PRINT 'Índice IX_orden_corte_pending_operation creado exitosamente.';
END
ELSE
BEGIN
    PRINT 'El índice IX_orden_corte_pending_operation ya existe.';
END

-- 3. Agregar campo de última modificación para concurrencia optimista
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('orden_corte') AND name = 'last_modified')
BEGIN
    ALTER TABLE orden_corte ADD last_modified DATETIME2 DEFAULT GETDATE();
    PRINT 'Campo last_modified agregado exitosamente.';
END
ELSE
BEGIN
    PRINT 'El campo last_modified ya existe.';
END

-- 4. Actualizar registros existentes con fecha de modificación
UPDATE orden_corte SET last_modified = GETDATE() WHERE last_modified IS NULL;

-- 5. Vista para detectar operaciones pendientes
IF EXISTS (SELECT 1 FROM sys.views WHERE name = 'vw_OperacionesPendientes')
    DROP VIEW vw_OperacionesPendientes;
GO

CREATE VIEW vw_OperacionesPendientes AS
SELECT 
    numero,
    step,
    pending_operation,
    last_modified,
    CASE 
        WHEN pending_operation = 'ETIQUETADO' AND step < 3 THEN 'Etiquetado incompleto'
        WHEN pending_operation = 'CIERRE' AND step < 5 THEN 'Cierre incompleto'
        WHEN pending_operation = 'CONSUMO' THEN 'Consumo pendiente de registro'
        ELSE 'Estado desconocido'
    END AS descripcion
FROM orden_corte
WHERE pending_operation IS NOT NULL
  AND anulada = 0
  AND CloseDocument = 0;
GO

PRINT 'Vista vw_OperacionesPendientes creada exitosamente.';
GO

-- 6. Stored procedure para limpiar operaciones pendientes antiguas (> 24 horas)
IF EXISTS (SELECT 1 FROM sys.procedures WHERE name = 'sp_LimpiarOperacionesPendientes')
    DROP PROCEDURE sp_LimpiarOperacionesPendientes;
GO

CREATE PROCEDURE sp_LimpiarOperacionesPendientes
    @HorasAntiguedad INT = 24
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE orden_corte 
    SET pending_operation = NULL 
    WHERE pending_operation IS NOT NULL
      AND last_modified < DATEADD(HOUR, -@HorasAntiguedad, GETDATE());
    
    SELECT @@ROWCOUNT AS RegistrosLimpiados;
END
GO

PRINT 'Stored procedure sp_LimpiarOperacionesPendientes creado exitosamente.';
GO

PRINT '=== Instalación completada ===';
PRINT 'Se han agregado los siguientes elementos:';
PRINT '1. Campo pending_operation en orden_corte';
PRINT '2. Índice IX_orden_corte_pending_operation';
PRINT '3. Campo last_modified en orden_corte';
PRINT '4. Vista vw_OperacionesPendientes';
PRINT '5. Stored procedure sp_LimpiarOperacionesPendientes';
