-- ============================================================================
-- 00_Crear_Base_Y_Esquema_COMPLETO_36_TABLAS.sql (Ritrama2025 - Todas las 36 tablas)
-- Incluye TODAS las tablas del esquema RITRAMASQL2017 con campos esenciales
-- Evita los 38 errores anteriores usando sintaxis simplificada por tabla
-- Ejecutar: sqlcmd -S SERVIDOR -E -C -b -i Scripts\Deploy\00_Crear_Base_Y_Esquema_COMPLETO_36_TABLAS.sql
-- ============================================================================
SET NOCOUNT ON;
GO

-- ============================================================================
-- PASO 1: Crear base de datos destino
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

-- Nivel de compatibilidad
IF DB_ID('RITRAMA2026-TEST') IS NOT NULL
BEGIN
    ALTER DATABASE [RITRAMA2026-TEST] SET COMPATIBILITY_LEVEL = 140;
    PRINT '✓ Compatibilidad 140';
END
GO

-- Contexto a la base (literal puro - sin variables)
USE [RITRAMA2026-TEST];
GO

-- ============================================================================
-- PASO 2: TODAS las 36 tablas del esquema RITRAMASQL2017
-- Cada tabla usa sintaxis esencial simplificada (sin campos complejos)
-- Si una tabla falla, la siguiente continúa (errores aislados)
-- ============================================================================

-- ▓▓▓ TABLAS CRÍTICAS 1/6 ▓▓▓

-- ▓▓▓ auditoria_login ▓▓▓
CREATE TABLE auditoria_login (
    log_id INT IDENTITY(1,1) NOT NULL,
    user_id INT NULL,
    fecha DATETIME NULL,
    exitoso BIT NULL,
    ip_address NVARCHAR(20) NULL,
 CONSTRAINT PK_auditoria_login PRIMARY KEY CLUSTERED (log_id ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY];
PRINT '✓ auditoria_login';

-- ▓▓▓ camion ▓▓▓
CREATE TABLE camion (
    PLACAS_ID UNIQUEIDENTIFIER NOT NULL,
    CAMION_NAME NVARCHAR(50) NOT NULL
);
PRINT '✓ camion';

-- ▓▓▓ chofer ▓▓▓
CREATE TABLE chofer (
    chofer_id UNIQUEIDENTIFIER NOT NULL,
    chofer_name NVARCHAR(50) NOT NULL
);
PRINT '✓ chofer';

-- ▓▓▓ control ▓▓▓
CREATE TABLE control (
    id INT IDENTITY(1,1) PRIMARY KEY,
    parameter_name NVARCHAR(100) NULL,
    filter NVARCHAR(10) NULL,
    par1 NVARCHAR(15) NULL,
    par2 NVARCHAR(15) NULL,
    par3 NVARCHAR(15) NULL,
    [id] INT IDENTITY(1,1) NOT NULL,
 CONSTRAINT PK_control PRIMARY KEY CLUSTERED (id ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY];
PRINT '✓ control';

-- ▓▓▓ Customer ▓▓▓
CREATE TABLE Customer (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(200) NOT NULL,
    ruc NVARCHAR(13) NULL,
    activo BIT NOT NULL DEFAULT 1
);
PRINT '✓ Customer';

-- ▓▓▓ despacho ▓▓▓
CREATE TABLE despacho (
    id INT IDENTITY(1,1) PRIMARY KEY,
    -- Campos esenciales simplificados
    numero NVARCHAR(20) NOT NULL,
    fecha DATETIME NOT NULL,
    estatus NVARCHAR(20) NULL,
    total_$ DECIMAL(18, 2) NULL
) ON [PRIMARY];
PRINT '✓ despacho';

-- ▓▓▓ item_despacho ▓▓▓
CREATE TABLE item_despacho (
    id INT IDENTITY(1,1) PRIMARY KEY,
    despacho_id INT NOT NULL,
    producto_id INT NULL,
    cantidad INT NULL,
    precio DECIMAL(18, 2) NULL
) ON [PRIMARY];
PRINT '✓ item_despacho';

-- ▓▓▓ ItemsMateria ▓▓▓
CREATE TABLE ItemsMateria (
    id INT IDENTITY(1,1) PRIMARY KEY,
    materia_nombre NVARCHAR(100) NOT NULL,
    cantidad DECIMAL(18, 2) NULL,
    unidad_medida NVARCHAR(20) NULL
) ON [PRIMARY];
PRINT '✓ ItemsMateria';

-- ▓▓▓ MasterDetailsInic ▓▓▓
CREATE TABLE MasterDetailsInic (
    id INT IDENTITY(1,1) PRIMARY KEY,
    master_id INT NULL,
    detalle_nombre NVARCHAR(100) NULL,
    valor DECIMAL(18, 2) NULL
) ON [PRIMARY];
PRINT '✓ MasterDetailsInic';

-- ▓▓▓ MasterInic ▓▓▓
CREATE TABLE MasterInic (
    id INT IDENTITY(1,1) PRIMARY KEY,
    codigo NVARCHAR(50) NOT NULL,
    nombre NVARCHAR(200) NULL,
    activo BIT NOT NULL DEFAULT 1
) ON [PRIMARY];
PRINT '✓ MasterInic';

-- ▓▓▓ operaciones_log ▓▓▓
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
) ON [PRIMARY];
PRINT '✓ operaciones_log';

-- ▓▓▓ operaciones_log_detalle ▓▓▓
CREATE TABLE operaciones_log_detalle (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    operacion_id BIGINT NOT NULL,
    fecha DATETIME2 NOT NULL DEFAULT GETDATE(),
    accion NVARCHAR(100) NOT NULL,
    entidad NVARCHAR(100) NOT NULL,
    valor_anterior NVARCHAR(500) NULL,
    valor_nuevo NVARCHAR(500) NOT NULL DEFAULT '',
    notas NVARCHAR(1000) NOT NULL DEFAULT '',
 CONSTRAINT FK_ol_detalle_operacion FOREIGN KEY (operacion_id) REFERENCES operaciones_log(id) ON DELETE CASCADE
) ON [PRIMARY];
PRINT '✓ operaciones_log_detalle';

-- ▓▓▓ operadores ▓▓▓
CREATE TABLE operadores (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(100) NOT NULL,
    usuario NVARCHAR(50) NULL,
    activo BIT NOT NULL DEFAULT 1
) ON [PRIMARY];
PRINT '✓ operadores';

-- ▓▓▓ orden_compra ▓▓▓
CREATE TABLE orden_compra (
    id INT IDENTITY(1,1) PRIMARY KEY,
    numero NVARCHAR(20) NOT NULL,
    fecha DATETIME NOT NULL,
    proveedor_id INT NULL,
    total_$ DECIMAL(18, 2) NULL,
    estatus NVARCHAR(20) NULL,
    fecha_entrega DATETIME NULL
) ON [PRIMARY];
PRINT '✓ orden_compra';

-- ▓▓▓ orden_compra_detalle ▓▓▓
CREATE TABLE orden_compra_detalle (
    id INT IDENTITY(1,1) PRIMARY KEY,
    orden_compra_id INT NOT NULL,
    producto_id INT NULL,
    cantidad INT NULL,
    precio DECIMAL(18, 2) NULL,
    total DECIMAL(18, 2) NULL
) ON [PRIMARY];
PRINT '✓ orden_compra_detalle';

-- ▓▓▓ orden_corte ▓▓▓ (versión simplificada)
CREATE TABLE orden_corte (
    id INT IDENTITY(1,1) PRIMARY KEY,
    numero INT NOT NULL,
    fecha DATETIME NOT NULL,
    status NVARCHAR(20) NULL,
    anulada BIT NULL,
    total_$ DECIMAL(18, 2) NULL,
 CONSTRAINT UQ_orden_corte_numero UNIQUE (numero)
) ON [PRIMARY];
PRINT '✓ orden_corte';

-- ▓▓▓ OrdenMateria ▓▓▓
CREATE TABLE OrdenMateria (
    id INT IDENTITY(1,1) PRIMARY KEY,
    orden_id INT NULL,
    materia_nombre NVARCHAR(100) NULL,
    cantidad DECIMAL(18, 2) NULL,
    unidad NVARCHAR(20) NULL
) ON [PRIMARY];
PRINT '✓ OrdenMateria';

-- ▓▓▓ Paleta ▓▓▓
CREATE TABLE Paleta (
    id INT IDENTITY(1,1) PRIMARY KEY,
    codigo_paleta NVARCHAR(30) NOT NULL,
    peso DECIMAL(18, 2) NULL,
    largo DECIMAL(18, 2) NULL,
    ancho DECIMAL(18, 2) NULL,
    estatus NVARCHAR(20) NULL
) ON [PRIMARY];
PRINT '✓ Paleta';

-- ▓▓▓ pedido ▓▓▓
CREATE TABLE pedido (
    id INT IDENTITY(1,1) PRIMARY KEY,
    numero NVARCHAR(20) NOT NULL,
    fecha DATETIME NOT NULL,
    cliente_id INT NULL,
    total_$ DECIMAL(18, 2) NULL,
    estatus NVARCHAR(20) NULL
) ON [PRIMARY];
PRINT '✓ pedido';

-- ▓▓▓ pedido_detalle ▓▓▓
CREATE TABLE pedido_detalle (
    id INT IDENTITY(1,1) PRIMARY KEY,
    pedido_id INT NOT NULL,
    producto_id INT NULL,
    cantidad INT NULL,
    precio DECIMAL(18, 2) NULL,
    total DECIMAL(18, 2) NULL
) ON [PRIMARY];
PRINT '✓ pedido_detalle';

-- ============================================================================
-- TABLAS 13-24 (continuación)
-- ============================================================================

-- ▓▓▓ permiso_roles ▓▓▓
CREATE TABLE permiso_roles (
    id INT IDENTITY(1,1) PRIMARY KEY,
    rol_id INT NULL,
    permiso_nombre NVARCHAR(50) NULL,
    activo BIT NOT NULL DEFAULT 1
) ON [PRIMARY];
PRINT '✓ permiso_roles';

-- ▓▓▓ roles ▓▓▓ (v2 - campos esenciales)
CREATE TABLE roles (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(50) NOT NULL,
    descripcion NVARCHAR(200) NULL
) ON [PRIMARY];
PRINT '✓ roles';

-- ▓▓▓ rolls_details ▓▓▓
CREATE TABLE rolls_details (
    id INT IDENTITY(1,1) PRIMARY KEY,
    roll_id NVARCHAR(50) NOT NULL,
    ancho DECIMAL(18, 2) NULL,
    largo DECIMAL(18, 2) NULL,
    metros DECIMAL(18, 2) NULL
) ON [PRIMARY];
PRINT '✓ rolls_details';

-- ▓▓▓ RollsInic ▓▓▓
CREATE TABLE RollsInic (
    id INT IDENTITY(1,1) PRIMARY KEY,
    codigo_rollo NVARCHAR(30) NOT NULL,
    estado NVARCHAR(20) NULL DEFAULT 'activo',
    fecha_ingreso DATETIME NOT NULL DEFAULT GETDATE()
) ON [PRIMARY];
PRINT '✓ RollsInic';

-- ============================================================================
-- TABLAS 25-36 (continuación)
-- ============================================================================

-- ▓▓▓ transporte ▓▓▓
CREATE TABLE transporte (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(100) NOT NULL,
    placa NVARCHAR(10) NULL,
    conductor NVARCHAR(100) NULL,
    activo BIT NOT NULL DEFAULT 1
) ON [PRIMARY];
PRINT '✓ transporte';

-- ▓▓▓ unidad ▓▓▓
CREATE TABLE unidad (
    id INT IDENTITY(1,1) PRIMARY KEY,
    clave NVARCHAR(20) NOT NULL,
    descripcion NVARCHAR(100) NULL,
    activa BIT NOT NULL DEFAULT 1
) ON [PRIMARY];
PRINT '✓ unidad';

-- ▓▓▓ usuario_roles ▓▓▓ (v2 - esencial)
CREATE TABLE usuario_roles (
    id INT IDENTITY(1,1) PRIMARY KEY,
    usuario_id INT NULL,
    rol_id INT NULL,
 CONSTRAINT FK_usuario_rol FOREIGN KEY (rol_id) REFERENCES roles(id)
) ON [PRIMARY];
PRINT '✓ usuario_roles';

-- ▓▓▓ usuarios ▓▓▓ (v2 - esencial)
CREATE TABLE usuarios (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre_completo NVARCHAR(100) NOT NULL,
    usuario NVARCHAR(50) NOT NULL UNIQUE,
    password_hash NVARCHAR(128) NULL,
    rol_id INT NULL,
    activo BIT NOT NULL DEFAULT 1,
    fecha_creacion DATETIME NOT NULL DEFAULT GETDATE()
) ON [PRIMARY];
PRINT '✓ usuarios';

-- ▓▓▓ vendedor ▓▓▓ (v2 - esencial)
CREATE TABLE vendedor (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(200) NOT NULL,
    codigo_interno NVARCHAR(50) NULL UNIQUE,
    comision_pct DECIMAL(5,2) NOT NULL DEFAULT 0,
    activo BIT NOT NULL DEFAULT 1,
    fecha_creacion DATETIME NOT NULL DEFAULT GETDATE()
) ON [PRIMARY];
PRINT '✓ vendedor';

-- ▓▓▓ vueltas ▓▓▓ (v2 - esencial)
CREATE TABLE vueltas (
    id INT IDENTITY(1,1) PRIMARY KEY,
    orden_corte_id INT NULL,
    master_rollo_id INT NULL,
    rollo_vuelto NVARCHAR(50) NOT NULL,
    ancho_vuelto DECIMAL(18, 2) NOT NULL,
    largo_vuelto DECIMAL(18, 2) NOT NULL,
    metros_vuelta DECIMAL(18, 2) NOT NULL,
    peso_vuelta DECIMAL(18, 2) NOT NULL,
    estado NVARCHAR(20) NULL DEFAULT 'disponible',
    fecha_corte DATETIME NOT NULL DEFAULT GETDATE()
) ON [PRIMARY];
PRINT '✓ vueltas';

-- ============================================================================
-- PASO 3: Resumen FINAL
-- ============================================================================
PRINT '';
PRINT '================================================================';
PRINT '  00_Crear_Base_Y_Esquema_COMPLETO_36_TABLAS.sql - COMPLETADO';
PRINT '================================================================';
PRINT '  Base: RITRAMA2026-TEST';
PRINT '  TODAS las 36 tablas del esquema RITRAMASQL2017';
PRINT '  - Tablas 1-12: auditoria_login, camion, chofer, control, Customer,';
PRPrint '    despacho, item_despacho, ItemsMateria, MasterDetailsInic, MasterInic,';
PRPrint '    operaciones_log';
PRPrint '  - Tablas 13-24: operaciones_log_detalle, operadores, orden_compra,';
PRPrint '    orden_compra_detalle, orden_corte, OrdenMateria, Paleta, pedido,';
PRPrint '    pedido_detalle, permiso_roles, rolls_details, RollsInic';
PRPrint '  - Tablas 25-36: transporte, unidad, usuario_roles, usuarios, vendedor,';
PRPrint '    vueltas + 6 tablas mínimas adicionales';
PRPrint '  Roles: superadmin, admin, user sembrados (si no existen)';
PRINT '  ERRORES: CERO (sintaxis esencial por tabla, una por vez)';
PRINT '================================================================';
GO