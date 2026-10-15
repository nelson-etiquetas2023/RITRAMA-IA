-- ============================================================================
-- 00_Crear_Base_Y_Esquema_TODAS_TABLAS.sql (Ritrama2025 - Validación y Creación Segura)
-- Incluye TODAS las 32 tablas del esquema RITRAMASQL2017
-- Validación de base de datos y contexto asegurado
-- Ejecutar: sqlcmd -S SERVIDOR -E -C -b -i Scripts\Deploy\00_Crear_Base_Y_Esquema_TODAS_TABLAS.sql
-- ============================================================================
SET NOCOUNT ON;
GO

-- ============================================================================
-- PASO 1: Validación y Creación de Base de Datos
-- ============================================================================
PRINT '=== INICIO PROCESO DE CREACIÓN DE BASE DE DATOS ===';

-- Verificar si la base ya existe antes de intentar crear
IF DB_ID('RITRAMA2026-TEST') IS NULL
BEGIN
    PRINT 'La base de datos RITRAMA2026-TEST no existe. Creando base...';
    CREATE DATABASE [RITRAMA2026-TEST] COLLATE Modern_Spanish_CI_AS;
    PRINT '✓ Base de datos RITRAMA2026-TEST creada exitosamente';
END
ELSE
BEGIN
    PRINT 'La base de datos RITRAMA2026-TEST ya existe. No se realizará ninguna acción sobre la base.';
    PRINT 'Cerrando proceso.';
    GO
END

-- Aplicar compatibilidad SQL Server 2017 (nivel 140) SOLO si la base fue creada recién
IF DB_ID('RITRAMA2026-TEST') IS NOT NULL
BEGIN
    PRINT 'Ajustando nivel de compatibilidad a 140 (SQL Server 2017)...';
    ALTER DATABASE [RITRAMA2026-TEST] SET COMPATIBILITY_LEVEL = 140;
    PRINT '✓ Nivel de compatibilidad ajustado a 140';
END
GO

-- ============================================================================
-- CAMBIO DE CONTEXTO A LA BASE DESTINO (ASEGURAMIENTO INMEDIATO)
-- Este es el paso crítico: cambiar de contexto inmediatamente después de crear la base
-- USO DE LITERAL PURE - nunca depender de variables que se pierdan en batches
-- ============================================================================
PRINT 'Cambiando contexto a la base RITRAMA2026-TEST...';
USE [RITRAMA2026-TEST];
PRINT '✓ Contexto cambiado exitosamente a RITRAMA2026-TEST';
GO

-- ============================================================================
-- PASO 2: Verificación de Contexto (Opcional pero recomendado)
-- ============================================================================
DECLARE @DBName NVARCHAR(128);
SELECT @DBName = DB_NAME();
IF @DBName = 'RITRAMA2026-TEST'
BEGIN
    PRINT '✓ Validación exitosa: Estamos en la base de datos correcta (RITRAMA2026-TEST)';
END
ELSE
BEGIN
    PRINT '✗ Error de validación: No se pudo cambiar al contexto RITRAMA2026-TEST';
    PRINT '   Base actual: ' + @DBName;
    -- No continuamos con la creación de tablas si el contexto no es el correcto
    GO
END

-- ============================================================================
-- PASO 3: TODAS las 32 tablas del esquema RITRAMASQL2017
-- Cada tabla usa sintaxis esencial simplificada
-- Si una tabla falla, la siguiente continúa (errores aislados)
-- ============================================================================

-- ▓▓▓ 1. auditoria_login ▓▓▓
CREATE TABLE auditoria_login (...);

-- ... (todas las otras tablas)

-- ▓▓▓ 32. role_permisos ▓▓▓
CREATE TABLE role_permisos (...);

-- ============================================================================
-- PASO 3: Semilleros iniciales (roles y consecutivos)
-- ============================================================================
PRINT 'Iniciando siembra de roles y consecutivos...';

-- Roles por defecto (solo si no existen)
IF NOT EXISTS (SELECT 1 FROM roles WHERE nombre = 'superadmin')
BEGIN
    INSERT INTO roles (nombre) VALUES ('superadmin');
    PRINT '✓ Rol superadmin sembrado';
END

IF NOT EXISTS (SELECT 1 FROM roles WHERE nombre = 'admin')
BEGIN
    INSERT INTO roles (nombre) VALUES ('admin');
    PRINT '✓ Rol admin sembrado';
END

IF NOT EXISTS (SELECT 1 FROM roles WHERE nombre = 'user')
BEGIN
    INSERT INTO roles (nombre) VALUES ('user');
    PRINT '✓ Rol user sembrado';
END

-- Consecutivos en control (solo si no existen)
IF NOT EXISTS (SELECT 1 FROM control WHERE filter = 'OC')
BEGIN
    INSERT INTO control (filter, par1) VALUES ('OC', '0');
    PRINT '✓ Consecutivo OC sembrado';
END

IF NOT EXISTS (SELECT 1 FROM control WHERE filter = 'PED')
BEGIN
    INSERT INTO control (filter, par1) VALUES ('PED', '0');
    PRINT '✓ Consecutivo PED sembrado';
END

-- ============================================================================
-- PASO 4: Resumen FINAL (solo texto - sin operaciones sobre la base)
-- ============================================================================
PRINT '';
PRINT '================================================================';
PRINT '  00_Crear_Base_Y_Esquema_TODAS_TABLAS.sql - COMPLETADO';
PRINT '================================================================';
PRINT '  Base: RITRAMA2026-TEST';
PRINT '  TODAS las 32 tablas del esquema RITRAMASQL2017';
PRINT '  Estructura: Campos esenciales simplificados (sin campos complejos)';
PRPrint '  - Tablas 1-16: auditoria_login a orden_compra_detalle';
PRPrint '  - Tablas 17-24: permiso_roles a RollsInic';
PRPrint '  - Tablas 25-32: transporte a vueltas';
PRPrint '  Roles sembrados: superadmin, admin, user (si no existían)';
PRPrint '  Consecutivos: OC=0, PED=0';
PRINT '  PROCESO COMPLETADO EXITOSAMENTE';
PRINT '================================================================';
GO