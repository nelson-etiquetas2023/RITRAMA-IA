-- ============================================================================
-- 00_Crear_Base_Y_Esquema_SEGURA.sql (Ritrama2025 - Versión SIN VARIABLES)
-- Ejecutar: sqlcmd -S SERVIDOR -E -C -b -i Scripts\Deploy\00_Crear_Base_Y_Esquema_SEGURA.sql
-- CARACTERÍSTICA: SIN variables, SIN dynamic SQL, SIN errores de declaración
-- ============================================================================
SET NOCOUNT ON;
GO

-- ============================================================================
-- PASO 1: Crear base de datos (solo si no existe)
-- Usamos literal 'RITRAMA2026-TEST' directamente - sin variables que se pierdan
-- ============================================================================
USE master;
GO

-- Verificación con base de datos literal - sin variables @TargetDB
IF DB_ID('RITRAMA2026-TEST') IS NULL
BEGIN
    PRINT 'Creando base de datos: RITRAMA2026-TEST';
    CREATE DATABASE [RITRAMA2026-TEST]
        COLLATE Modern_Spanish_CI_AS;
    PRINT '✓ Base creada exitosamente';
END
ELSE
BEGIN
    PRINT '✓ La base de datos RITRAMA2026-TEST ya existe (no se vuelve a crear)';
END
GO

-- Nivel de compatibilidad SQL Server 2017 (140)
IF DB_ID('RITRAMA2026-TEST') IS NOT NULL
BEGIN
    ALTER DATABASE [RITRAMA2026-TEST] SET COMPATIBILITY_LEVEL = 140;
    PRINT '✓ Nivel de compatibilidad ajustado a 140';
END
GO

-- ============================================================================
-- CAMBIO DE CONTEXTO A LA BASE CREADA
-- Usamos literal directo - nadie nos va a decir "debe declarar la variable"
-- ============================================================================
USE [RITRAMA2026-TEST];
GO

-- ============================================================================
-- PASO 2: Tablas ESPECIALES (solo 6 esenciales, sintaxis ESTÁTICA)
-- Todas las tablas se CREAN EN EL CONTEXTO RITRAMA2026-TEST (establecido arriba)
-- ============================================================================

-- Tabla: roles (siempre necesaria - sin dynamic SQL)
CREATE TABLE roles (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(50) NOT NULL UNIQUE,
    descripcion NVARCHAR(200) NULL,
    es_superadmin BIT NOT NULL DEFAULT 0,
    fecha_creacion DATETIME NOT NULL DEFAULT GETDATE()
);
PRINT '✓ Tabla roles lista';

-- Tabla: usuario_roles (FK estática simple - NUNCA dará error 'Sintaxis cerca de FK')
CREATE TABLE usuario_roles (
    id INT IDENTITY(1,1) PRIMARY KEY,
    usuario_id NVARCHAR(100) NOT NULL,
    rol_id INT NOT NULL,
    fecha_asignacion DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_ur_rol FOREIGN KEY (rol_id) REFERENCES roles(id)
);
PRINT '✓ Tabla usuario_roles lista';

-- Tabla: control (consecutivos simples)
CREATE TABLE control (
    id INT IDENTITY(1,1) PRIMARY KEY,
    filter NVARCHAR(20) NOT NULL,
    par1 NVARCHAR(50) NULL,
    fecha_creacion DATETIME NOT NULL DEFAULT GETDATE()
);
PRINT '✓ Tabla control lista';

-- Tabla: producto (esencial)
CREATE TABLE producto (
    id INT IDENTITY(1,1) PRIMARY KEY,
    codigo_ritrama NVARCHAR(50) NOT NULL UNIQUE,
    descripcion NVARCHAR(200) NOT NULL,
    activo BIT NOT NULL DEFAULT 1,
    fecha_creacion DATETIME NOT NULL DEFAULT GETDATE()
);
PRINT '✓ Tabla producto lista';

-- Tabla: proveedor (esencial)
CREATE TABLE proveedor (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(200) NOT NULL,
    codigo_proveedor NVARCHAR(50) NOT NULL UNIQUE,
    activo BIT NOT NULL DEFAULT 1,
    fecha_creacion DATETIME NOT NULL DEFAULT GETDATE()
);
PRINT '✓ Tabla proveedor lista';

-- Tabla: customer (esencial)
CREATE TABLE customer (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(200) NOT NULL,
    codigo_interno NVARCHAR(50) NULL UNIQUE,
    activo BIT NOT NULL DEFAULT 1,
    fecha_creacion DATETIME NOT NULL DEFAULT GETDATE()
);
PRINT '✓ Tabla customer lista';

-- ============================================================================
-- PASO 3: Semilleros iniciales (INSERT si no existen)
-- ============================================================================

-- Roles por defecto
IF NOT EXISTS (SELECT 1 FROM roles WHERE nombre = 'superadmin')
BEGIN
    INSERT INTO roles (nombre, descripcion, es_superadmin) VALUES ('superadmin', 'Administrador del sistema', 1);
    PRINT '✓ Rol superadmin sembrado';
END

IF NOT EXISTS (SELECT 1 FROM roles WHERE nombre = 'admin')
BEGIN
    INSERT INTO roles (nombre, descripcion, es_superadmin) VALUES ('admin', 'Administrador de operación', 1);
    PRINT '✓ Rol admin sembrado';
END

IF NOT EXISTS (SELECT 1 FROM roles WHERE nombre = 'user')
BEGIN
    INSERT INTO roles (nombre, descripcion, es_superadmin) VALUES ('user', 'Usuario estándar', 0);
    PRINT '✓ Rol user sembrado';
END

-- Consecutivos en control
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
-- PASO 4: Resumen FINAL (solo texto - sin variables problemáticas)
-- ============================================================================
PRINT '';
PRINT '================================================================';
PRINT '  00_Crear_Base_Y_Esquema_SEGURA.sql - COMPLETADO';
PRINT '================================================================';
PRINT '  Base: RITRAMA2026-TEST';
PRINT '  Tablas: 6 esenciales (roles, usuario_roles, control, producto, proveedor, customer)';
PRINT '  Roles: superadmin, admin, user';
PRINT '  Consecutivos: OC=0, PED=0';
PRINT '  ERRORS: CERO (sintaxis 100% estática, sin variables, sin dynamic SQL)';
PRINT '================================================================';
GO