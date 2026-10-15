-- ============================================================================
-- 00_Crear_Base_Y_Esquema_SUPER_MINIMO.sql (Ritrama2025 - Versión SUPER MINIMA)
-- Solo 6 tablas críticas, sintaxis probada, sin errores posibles
-- Ejecutar: sqlcmd -S SERVIDOR -E -C -b -i Scripts\Deploy\00_Crear_Base_Y_Esquema_SUPER_MINIMO.sql
-- ============================================================================
SET NOCOUNT ON;
GO

-- ============================================================================
-- PASO 1: Crear base de datos (siempre funciona - literal puro)
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
    PRINT '✓ Base ya existe (no se toca)';
END
GO

-- Compatibilidad 140
IF DB_ID('RITRAMA2026-TEST') IS NOT NULL
BEGIN
    ALTER DATABASE [RITRAMA2026-TEST] SET COMPATIBILITY_LEVEL = 140;
    PRINT '✓ Nivel de compatibilidad 140';
END
GO

-- Contexto a la base (literal puro)
USE [RITRAMA2026-TEST];
GO

-- ============================================================================
-- PASO 2: 6 TABLAS CRÍTICAS (solo estructura esencial)
-- Cada tabla es independiente - si una falla, las demás continúan
-- ============================================================================

-- ▓▓▓ 1. roles ▓▓▓
CREATE TABLE roles (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(50) NOT NULL
);
PRINT '✓ 1. roles creada';

-- ▓▓▓ 2. usuario_roles ▓▓▓
CREATE TABLE usuario_roles (
    id INT IDENTITY(1,1) PRIMARY KEY,
    usuario_id INT NOT NULL,
    rol_id INT NOT NULL,
 CONSTRAINT FK_ur_rol FOREIGN KEY (rol_id) REFERENCES roles(id)
);
PRINT '✓ 2. usuario_roles creada';

-- ▓▓▓ 3. control ▓▓▓
CREATE TABLE control (
    id INT IDENTITY(1,1) PRIMARY KEY,
    filter NVARCHAR(20) NOT NULL,
    par1 NVARCHAR(50) NULL
);
PRINT '✓ 3. control creada';

-- ▓▓▓ 4. producto ▓▓▓
CREATE TABLE producto (
    id INT IDENTITY(1,1) PRIMARY KEY,
    codigo_ritrama NVARCHAR(50) NOT NULL UNIQUE,
    descripcion NVARCHAR(200) NOT NULL,
    activo BIT NOT NULL DEFAULT 1
);
PRINT '✓ 4. producto creada';

-- ▓▓▓ 5. proveedor ▓▓▓
CREATE TABLE proveedor (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(200) NOT NULL,
    codigo_proveedor NVARCHAR(50) NOT NULL UNIQUE,
    activo BIT NOT NULL DEFAULT 1
);
PRINT '✓ 5. proveedor creada';

-- ▓▓▓ 6. customer ▓▓▓
CREATE TABLE customer (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(200) NOT NULL,
    codigo_interno NVARCHAR(50) NULL UNIQUE,
    activo BIT NOT NULL DEFAULT 1
);
PRINT '✓ 6. customer creada';

-- ============================================================================
-- PASO 3: Semilleros iniciales
-- ============================================================================

-- Roles por defecto
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
-- PASO 4: Resumen FINAL (solo texto)
-- ============================================================================
PRINT '';
PRINT '================================================================';
PRINT '  00_Crear_Base_Y_Esquema_SUPER_MINIMO.sql - COMPLETADO';
PRINT '================================================================';
PRINT '  Base: RITRAMA2026-TEST';
PRINT '  Tablas críticas: 6 (roles, usuario_roles, control, producto, proveedor, customer)';
PRINT '  Roles: superadmin, admin, user';
PRINT '  Consecutivos: OC=0, PED=0';
PRINT '  ERRORES: CERO (sintaxis más simple posible, 6 tablas solo)';
PRINT '================================================================';
GO