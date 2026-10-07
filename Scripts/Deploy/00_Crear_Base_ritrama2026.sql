-- ============================================================================
-- 00_Crear_Base_ritrama2026.sql (Ritrama2025)
-- Que hace: crea la base vacia ritrama2026 si no existe y la deja en
--   COMPATIBILITY_LEVEL 140 (SQL Server 2017), lista para recibir el baseline
--   + los scripts de Scripts/*.sql. Es la base de PRODUCCION.
-- Como se usa (LOCAL en el server, no por WAN):
--   sqlcmd -S RITRAMASRV01 -E -C -b -i Scripts\Deploy\00_Crear_Base_ritrama2026.sql
-- Idempotente: si la base ya existe no la borra, solo ajusta compatibilidad.
-- NOTA: Scripts/*.sql NO crean las tablas legacy (orden_corte, customer,
--   producto, MasterInic...). Esas vienen del baseline: restore .bak (solo si
--   el origen es 2017) o bacpac/scripts generados donde vive hoy ritrama2026.
--   Este script solo prepara el contenedor. 30_Verificar te avisa que falta.
-- ============================================================================
SET NOCOUNT ON;
GO

USE master;
GO

IF DB_ID(N'ritrama2026') IS NULL
BEGIN
    PRINT 'Creando base ritrama2026 con paths por defecto del servidor...';
    CREATE DATABASE [ritrama2026]
        COLLATE Modern_Spanish_CI_AS;
    PRINT 'Base creada.';
END
ELSE
BEGIN
    PRINT 'La base ritrama2026 ya existe (no se borra nada).';
END
GO

-- Nivel de compatibilidad 140 = SQL Server 2017. Obligatorio para el T20.
IF EXISTS (SELECT 1 FROM sys.databases WHERE name = N'ritrama2026' AND compatibility_level <> 140)
BEGIN
    PRINT 'Ajustando COMPATIBILITY_LEVEL a 140...';
    ALTER DATABASE [ritrama2026] SET COMPATIBILITY_LEVEL = 140;
END
GO

-- Modelo de recuperacion FULL para produccion (permite point-in-time restore).
-- Si el T20 no tiene plan de backups log, cambiar a SIMPLE a mano.
IF EXISTS (SELECT 1 FROM sys.databases WHERE name = N'ritrama2026' AND recovery_model_desc <> N'FULL')
BEGIN
    ALTER DATABASE [ritrama2026] SET RECOVERY FULL;
    PRINT 'Recovery model = FULL.';
END
GO

SELECT
    name AS base,
    compatibility_level AS compat,
    CASE WHEN compatibility_level = 140 THEN 'OK' ELSE 'FALTA: debe ser 140' END AS revision,
    collation_name AS collation,
    recovery_model_desc AS recovery,
    state_desc AS estado
FROM sys.databases
WHERE name = N'ritrama2026';
GO

PRINT '00_Crear_Base completado. Siguiente: restaurar baseline y correr 10_Desplegar_Esquema.ps1.';
GO
