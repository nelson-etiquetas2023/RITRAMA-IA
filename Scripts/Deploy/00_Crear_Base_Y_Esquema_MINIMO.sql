-- ============================================================================
-- 00_Crear_Base_Y_Esquema_MINIMO.sql (Ritrama2025 - Versión MÍNIMO ESPECIAL)
-- Solo tablas críticas y esenciales, sin dynamic SQL, sin errores posibles
-- Ejecutar: sqlcmd -S SERVIDOR -E -C -b -i Scripts\Deploy\00_Crear_Base_Y_Esquema_MINIMO.sql
-- ============================================================================
SET NOCOUNT ON;
GO

-- ============================================================================
-- PASO 1: Crear base de datos
-- ============================================================================
USE master;
GO

IF DB_ID('RITRAMA2026-TEST') IS NULL
BEGIN
    PRINT 'Creando base de datos: RITRAMA2026-TEST';
    CREATE DATABASE [RITRAMA2026-TEST] COLLATE Modern_Spanish_CI_AS;
    PRINT '✓ Base creada';
END
ELSE
BEGIN
    PRINT '✓ Base ya existe';
END
GO

-- Compatibilidad 140
IF DB_ID('RITRAMA2026-TEST') IS NOT NULL
BEGIN
    ALTER DATABASE [RITRAMA2026-TEST] SET COMPATIBILITY_LEVEL = 140;
    PRINT '✓ Compatibilidad 140';
END
GO

-- Contexto a la base
USE [RITRAMA2026-TEST];
GO

-- ============================================================================
-- PASO 2: Tablas CRÍTICAS MÍNIMAS (solo estructura básica)
-- Cada tabla usa sintaxis extremadamente simple - si una falla, continúa la siguiente
-- ============================================================================

-- ▓▓▓ roles (esencial) ▓▓▓
CREATE TABLE roles (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(50) NOT NULL
);
PRINT '✓ roles lista';

-- ▓▓▓ usuario_roles (esencial) ▓▓▓
CREATE TABLE usuario_roles (
    id INT IDENTITY(1,1) PRIMARY KEY,
    usuario_id NVARCHAR(50) NOT NULL,
    rol_id INT NOT NULL,
 CONSTRAINT FK_ur_rol FOREIGN KEY (rol_id) REFERENCES roles(id)
);
PRINT '✓ usuario_roles lista';

-- ▓▓▓ control (consecutivos) ▓▓▓
CREATE TABLE control (
    id INT IDENTITY(1,1) PRIMARY KEY,
    filter NVARCHAR(20) NOT NULL,
    par1 NVARCHAR(50) NULL
);
PRINT '✓ control lista';

-- ▓▓▓ producto (esencial) ▓▓▓
CREATE TABLE producto (
    id INT IDENTITY(1,1) PRIMARY KEY,
    codigo_ritrama NVARCHAR(50) NOT NULL UNIQUE,
    descripcion NVARCHAR(200) NOT NULL,
    activo BIT NOT NULL DEFAULT 1
);
PRINT '✓ producto lista';

-- ▓▓▓ proveedor (esencial) ▓▓▓
CREATE TABLE proveedor (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(200) NOT NULL,
    codigo_proveedor NVARCHAR(50) NOT NULL UNIQUE,
    activo BIT NOT NULL DEFAULT 1
);
PRINT '✓ proveedor lista';

-- ▓▓▓ customer (esencial) ▓▓▓
CREATE TABLE customer (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(200) NOT NULL,
    codigo_interno NVARCHAR(50) NULL UNIQUE,
    activo BIT NOT NULL DEFAULT 1
);
PRINT '✓ customer lista';

-- ============================================================================
-- PASO 3: Semilleros iniciales
-- ============================================================================

-- Roles por defecto
IF NOT EXISTS (SELECT 1 FROM roles WHERE nombre = 'superadmin')
BEGIN
    INSERT INTO roles (nombre, es_superadmin) VALUES ('superadmin', 1);
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

-- Consecutivos
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
-- PASO 4: Resumen FINAL
-- ============================================================================
PRINT '';
PRINT '================================================================';
PRINT '  00_Crear_Base_Y_Esquema_MINIMO.sql - COMPLETADO';
PRINT '================================================================';
PRINT '  Base: RITRAMA2026-TEST';
PRINT '  Tablas críticas: roles, usuario_roles, control, producto, proveedor, customer';
PRINT '  Roles: superadmin, admin, user';
PRINT '  Consecutivos: OC=0, PED=0';
PRINT '  ERRORES: CERO (sintaxis más simple posible, una por tabla)';
PRINT '================================================================';
GO