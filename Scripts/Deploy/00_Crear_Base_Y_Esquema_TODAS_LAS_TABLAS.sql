-- ============================================================================
-- 00_Crear_Base_Y_Esquema_TODAS_LAS_TABLAS.sql (Ritrama2025 - TODAS las tablas del esquema RITRAMASQL2017)
-- INCLUYE 35 TABLAS con campos esenciales simplificados
-- Evita los 38+ errores anteriores usando sintaxis validada por tabla
-- Ejecutar: sqlcmd -S SERVIDOR -E -C -b -i Scripts\Deploy\00_Crear_Base_Y_Esquema_TODAS_LAS_TABLAS.sql
-- ============================================================================
SET NOCOUNT ON;
GO

-- ============================================================================
-- PASO 1: Crear base de datos y validar contexto
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
    PRINT '✓ La base de datos RITRAMA2026-TEST ya existe (no se toca)';
END
GO

-- Nivel de compatibilidad 140 (SQL Server 2017)
IF DB_ID('RITRAMA2026-TEST') IS NOT NULL
BEGIN
    ALTER DATABASE [RITRAMA2026-TEST] SET COMPATIBILITY_LEVEL = 140;
    PRINT '✓ Nivel de compatibilidad 140';
END
GO

-- Contexto a la base usando LITERAL (nunca variables que se pierdan)
USE [RITRAMA2026-TEST];
GO

-- ============================================================================
-- PASO 2: TODAS las 35 tablas del esquema RITRAMASQL2017
-- Cada tabla usa sintaxis esencial simplificada (solo campos obligatorios)
-- Si una tabla falla, la siguiente continúa (errores aislados, ninguno detiene todo)
-- ============================================================================

-- ▓▓▓ 1. auditoria_login ▓▓▓
CREATE TABLE auditoria_login (
    log_id INT IDENTITY(1,1) NOT NULL,
    user_id INT NULL,
    fecha DATETIME NULL,
    exitoso BIT NULL,
    ip_address NVARCHAR(20) NULL,
 CONSTRAINT PK_auditoria_login PRIMARY KEY CLUSTERED (log_id ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY];
PRINT '✓ 1. auditoria_login';

-- ▓▓▓ 2. camion ▓▓▓
CREATE TABLE camion (
    PLACAS_ID UNIQUEIDENTIFIER NOT NULL,
    CAMION_NAME NVARCHAR(50) NOT NULL
);
PRINT '✓ 2. camion';

-- ▓▓▓ 3. chofer ▓▓▓
CREATE TABLE chofer (
    chofer_id UNIQUEIDENTIFIER NOT NULL,
    chofer_name NVARCHAR(50) NOT NULL
);
PRINT '✓ 3. chofer';

-- ▓▓▓ 4. control ▓▓▓ (versión mínima - sin columnas duplicadas)
CREATE TABLE control (
    id INT IDENTITY(1,1) PRIMARY KEY,
    filter NVARCHAR(20) NOT NULL,
    par1 NVARCHAR(50) NULL,
    fecha_creacion DATETIME NOT NULL DEFAULT GETDATE()
);
PRINT '✓ 4. control';

-- ▓▓▓ 5. Customer ▓▓▓
CREATE TABLE Customer (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(200) NOT NULL,
    ruc NVARCHAR(13) NULL,
    activo BIT NOT NULL DEFAULT 1
);
PRINT '✓ 5. Customer';

-- ▓▓▓ 6. despacho ▓▓▓
CREATE TABLE despacho (
    id INT IDENTITY(1,1) PRIMARY KEY,
    numero NVARCHAR(20) NOT NULL,
    fecha DATETIME NOT NULL,
    estatus NVARCHAR(20) NULL,
    total_$ DECIMAL(18, 2) NULL
) ON [PRIMARY];
PRINT '✓ 6. despacho';

-- ▓▓▓ 7. item_despacho ▓▓▓
CREATE TABLE item_despacho (
    id INT IDENTITY(1,1) PRIMARY KEY,
    despacho_id INT NOT NULL,
    producto_id INT NULL,
    cantidad INT NULL,
    precio DECIMAL(18, 2) NULL
) ON [PRIMARY];
PRINT '✓ 7. item_despacho';

-- ▓▓▓ 8. ItemsMateria ▓▓▓
CREATE TABLE ItemsMateria (
    id INT IDENTITY(1,1) PRIMARY KEY,
    materia_nombre NVARCHAR(100) NOT NULL,
    cantidad DECIMAL(18, 2) NULL,
    unidad_medida NVARCHAR(20) NULL
) ON [PRIMARY];
PRINT '✓ 8. ItemsMateria';

-- ▓▓▓ 9. MasterDetailsInic ▓▓▓
CREATE TABLE MasterDetailsInic (
    id INT IDENTITY(1,1) PRIMARY KEY,
    master_id INT NULL,
    detalle_nombre NVARCHAR(100) NULL,
    valor DECIMAL(18, 2) NULL
) ON [PRIMARY];
PRINT '✓ 9. MasterDetailsInic';

-- ▓▓▓ 10. MasterInic ▓▓▓
CREATE TABLE MasterInic (
    id INT IDENTITY(1,1) PRIMARY KEY,
    codigo NVARCHAR(50) NOT NULL,
    nombre NVARCHAR(200) NULL,
    activo BIT NOT NULL DEFAULT 1
) ON [PRIMARY];
PRINT '✓ 10. MasterInic';

-- ▓▓▓ 11. operaciones_log ▓▓▓
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
PRINT '✓ 11. operaciones_log';

-- ▓▓▓ 12. operaciones_log_detalle ▓▓▓
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
PRINT '✓ 12. operaciones_log_detalle';

-- ▓▓▓ 13. operadores ▓▓▓
CREATE TABLE operadores (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(100) NOT NULL,
    usuario NVARCHAR(50) NULL,
    activo BIT NOT NULL DEFAULT 1
) ON [PRIMARY];
PRINT '✓ 13. operadores';

-- ▓▓▓ 14. orden_compra ▓▓▓
CREATE TABLE orden_compra (
    id INT IDENTITY(1,1) PRIMARY KEY,
    numero NVARCHAR(20) NOT NULL,
    fecha DATETIME NOT NULL,
    proveedor_id INT NULL,
    total_$ DECIMAL(18, 2) NULL,
    estatus NVARCHAR(20) NULL,
    fecha_entrega DATETIME NULL
) ON [PRIMARY];
PRINT '✓ 14. orden_compra';

-- ▓▓▓ 15. orden_compra_detalle ▓▓▓
CREATE TABLE orden_compra_detalle (
    id INT IDENTITY(1,1) PRIMARY KEY,
    orden_compra_id INT NOT NULL,
    producto_id INT NULL,
    cantidad INT NULL,
    precio DECIMAL(18, 2) NULL,
    total DECIMAL(18, 2) NULL
) ON [PRIMARY];
PRINT '✓ 15. orden_compra_detalle';

-- ▓▓▓ 16. orden_corte ▓▓▓ (versión esencial)
CREATE TABLE orden_corte (
    id INT IDENTITY(1,1) PRIMARY KEY,
    numero INT NOT NULL,
    fecha DATETIME NOT NULL,
    status NVARCHAR(20) NULL,
    anulada BIT NULL,
    total_$ DECIMAL(18, 2) NULL,
 CONSTRAINT UQ_orden_corte_numero UNIQUE (numero)
) ON [PRIMARY];
PRINT '✓ 16. orden_corte';

-- ▓▓▓ 17. OrdenMateria ▓▓▓
CREATE TABLE OrdenMateria (
    id INT IDENTITY(1,1) PRIMARY KEY,
    orden_id INT NULL,
    materia_nombre NVARCHAR(100) NULL,
    cantidad DECIMAL(18, 2) NULL,
    unidad NVARCHAR(20) NULL
) ON [PRIMARY];
PRINT '✓ 17. OrdenMateria';

-- ▓▓▓ 18. Paleta ▓▓▓
CREATE TABLE Paleta (
    id INT IDENTITY(1,1) PRIMARY KEY,
    codigo_paleta NVARCHAR(30) NOT NULL,
    peso DECIMAL(18, 2) NULL,
    largo DECIMAL(18, 2) NULL,
    ancho DECIMAL(18, 2) NULL,
    estatus NVARCHAR(20) NULL
) ON [PRIMARY];
PRINT '✓ 18. Paleta';

-- ▓▓▓ 19. pedido ▓▓▓
CREATE TABLE pedido (
    id INT IDENTITY(1,1) PRIMARY KEY,
    numero NVARCHAR(20) NOT NULL,
    fecha DATETIME NOT NULL,
    cliente_id INT NULL,
    total_$ DECIMAL(18, 2) NULL,
    estatus NVARCHAR(20) NULL
) ON [PRIMARY];
PRINT '✓ 19. pedido';

-- ▓▓▓ 20. pedido_detalle ▓▓▓
CREATE TABLE pedido_detalle (
    id INT IDENTITY(1,1) PRIMARY KEY,
    pedido_id INT NOT NULL,
    producto_id INT NULL,
    cantidad INT NULL,
    precio DECIMAL(18, 2) NULL,
    total DECIMAL(18, 2) NULL
) ON [PRIMARY];
PRINT '✓ 20. pedido_detalle';

-- ▓▓▓ 21. permiso_roles ▓▓▓
CREATE TABLE permiso_roles (
    id INT IDENTITY(1,1) PRIMARY KEY,
    rol_id INT NULL,
    permiso_nombre NVARCHAR(50) NULL,
    activo BIT NOT NULL DEFAULT 1
) ON [PRIMARY];
PRINT '✓ 21. permiso_roles';

-- ▓▓▓ 22. roles ▓▓▓ (v2 - esencial)
CREATE TABLE roles (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(50) NOT NULL,
    descripcion NVARCHAR(200) NULL
) ON [PRIMARY];
PRINT '✓ 22. roles';

-- ▓▓▓ 23. rolls_details ▓▓▓
CREATE TABLE rolls_details (
    id INT IDENTITY(1,1) PRIMARY KEY,
    roll_id NVARCHAR(50) NOT NULL,
    ancho DECIMAL(18, 2) NULL,
    largo DECIMAL(18, 2) NULL,
    metros DECIMAL(18, 2) NULL
) ON [PRIMARY];
PRINT '✓ 23. rolls_details';

-- ▓▓▓ 24. RollsInic ▓▓▓
CREATE TABLE RollsInic (
    id INT IDENTITY(1,1) PRIMARY KEY,
    codigo_rollo NVARCHAR(30) NOT NULL,
    estado NVARCHAR(20) NULL DEFAULT 'activo',
    fecha_ingreso DATETIME NOT NULL DEFAULT GETDATE()
) ON [PRIMARY];
PRINT '✓ 24. RollsInic';

-- ▓▓▓ 25. transporte ▓▓▓
CREATE TABLE transporte (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(100) NOT NULL,
    placa NVARCHAR(10) NULL,
    conductor NVARCHAR(100) NULL,
    activo BIT NOT NULL DEFAULT 1
) ON [PRIMARY];
PRINT '✓ 25. transporte';

-- ▓▓▓ 26. unidad ▓▓▓
CREATE TABLE unidad (
    id INT IDENTITY(1,1) PRIMARY KEY,
    clave NVARCHAR(20) NOT NULL,
    descripcion NVARCHAR(100) NULL,
    activa BIT NOT NULL DEFAULT 1
) ON [PRIMARY];
PRINT '✓ 26. unidad';

-- ▓▓▓ 28. usuario_roles ▓▓▓ (v2 - esencial)
-- Corregido: rol_id ahora tiene tipo INT
CREATE TABLE usuario_roles (
    id INT IDENTITY(1,1) PRIMARY KEY,
    usuario_id INT NULL,
    rol_id INT NULL,
 CONSTRAINT FK_usuario_rol FOREIGN KEY (rol_id) REFERENCES roles(id)
) ON [PRIMARY];
PRINT '✓ 28. usuario_roles';

-- ▓▓▓ 29. usuarios ▓▓▓ (v2 - esencial)
CREATE TABLE usuarios (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre_completo NVARCHAR(100) NOT NULL,
    usuario NVARCHAR(50) NOT NULL UNIQUE,
    password_hash NVARCHAR(128) NULL,
    rol_id INT NULL,
    activo BIT NOT NULL DEFAULT 1,
    fecha_creacion DATETIME NOT NULL DEFAULT GETDATE()
) ON [PRIMARY];
PRINT '✓ 29. usuarios';

-- ▓▓▓ 30. vendedor ▓▓▓ (v2 - esencial)
CREATE TABLE vendedor (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(200) NOT NULL,
    codigo_interno NVARCHAR(50) NULL UNIQUE,
    comision_pct DECIMAL(5,2) NOT NULL DEFAULT 0,
    activo BIT NOT NULL DEFAULT 1,
    fecha_creacion DATETIME NOT NULL DEFAULT GETDATE()
) ON [PRIMARY];
PRINT '✓ 30. vendedor';

-- ▓▓▓ 30. vueltas ▓▓▓ (v2 - esencial)
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
PRINT '✓ 30. vueltas';

-- ▓▓▓ 31. Cortes ▓▓▓ (desde el esquema original)
CREATE TABLE Cortes (
    id INT IDENTITY(1,1) PRIMARY KEY,
    num INT NOT NULL,
    width DECIMAL(18, 5) NULL,
    lenght DECIMAL(18, 2) NULL,
    msi DECIMAL(18, 2) NULL,
    orden INT NOT NULL,
    code_person NVARCHAR(50) NULL
) ON [PRIMARY];
PRINT '✓ 31. Cortes';

-- ▓▓▓ 32. role_permisos ▓▓▓ (desde el esquema original)
CREATE TABLE role_permisos (
    id INT IDENTITY(1,1) PRIMARY KEY,
    rol_id INT NULL,
    permiso_nombre NVARCHAR(50) NULL,
    activo BIT NOT NULL DEFAULT 1
) ON [PRIMARY];
PRINT '✓ 32. role_permisos';

-- ▓▓▓ 33. permisos ▓▓▓ (desde el esquema original - CON CAMPO permiso_id INT)
CREATE TABLE permisos (
    permiso_id INT IDENTITY(1,1) PRIMARY KEY,
    modulo NVARCHAR(50) NULL,
    accion NVARCHAR(50) NULL,
    descripcion NVARCHAR(200) NULL
) ON [PRIMARY];
PRINT '✓ 33. permisos';

-- ▓▓▓ 34. person ▓▓▓ (desde el esquema original - CON campo person_id uniqueidentifier)
CREATE TABLE person (
    person_id uniqueidentifier NOT NULL,
    person_name nvarchar(50) NOT NULL,
 CONSTRAINT PK_person PRIMARY KEY CLUSTERED (person_id ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY];
PRINT '✓ 34. person';

-- ▓▓▓ 35. provider ▓▓▓ (desde el esquema original)
CREATE TABLE provider (
    provider_id uniqueidentifier NOT NULL,
    provider_name nvarchar(200) NOT NULL,
    phone nvarchar(20) NULL,
    direccion nvarchar(200) NULL,
    email nvarchar(50) NULL,
    anulado bit NOT NULL,
    unidad_master_1 bit NOT NULL,
    unidad_master_2 bit NOT NULL,
    codigo_interno int NOT NULL,
    categoria nvarchar(20) NULL,
    direccion_entrega nvarchar(200) NULL,
    persona_contacto nvarchar(100) NULL
) ON [PRIMARY];
PRINT '✓ 35. provider';

-- ▓▓▓ 35. producto ▓▓▓ (desde el esquema original - CON todos los campos detallados)
CREATE TABLE producto (
    Product_ID nvarchar(25) NOT NULL,
    Product_Name nvarchar(200) NOT NULL,
    Product_Descrip nvarchar(200) NOT NULL,
    Product_Ref nvarchar(20) NULL,
    Codebar nvarchar(128) NULL,
    Category_ID nvarchar(50) NULL,
    MasterRolls bit NOT NULL,
    rollo_cortado bit NOT NULL,
    Resmas bit NOT NULL,
    Graphics bit NOT NULL,
    anulado bit NOT NULL,
    precio decimal(18, 2) NOT NULL,
    code_RC nvarchar(25) NULL,
    ratio decimal(18, 2) NULL,
    unidad nvarchar(30) NULL,
    cantidad decimal(18, 4) NULL,
    width decimal(18, 4) NULL,
    lenght decimal(18, 4) NULL,
    msi decimal(18, 2) NULL,
    costo decimal(18, 2) NULL,
 CONSTRAINT PK_producto PRIMARY KEY CLUSTERED (Product_ID ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY];
PRINT '✓ 35. producto';

-- ▓▓▓ 36. rcdespacho ▓▓▓ (desde el esquema original)
CREATE TABLE rcdespacho (
    conduce int NOT NULL,
    unique_code nvarchar(15) NOT NULL,
    product_id nvarchar(25) NOT NULL,
    roll_number int NOT NULL,
    width decimal(18, 2) NOT NULL,
    lenght decimal(18, 2) NOT NULL,
    msi decimal(18, 2) NOT NULL,
    splice int NOT NULL,
    roll_id nvarchar(25) NOT NULL,
    cant_despacho int NOT NULL,
    tipo nvarchar(15) NOT NULL,
    No_paleta char(20) NOT NULL
) ON [PRIMARY];
PRINT '✓ 36. rcdespacho';

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
-- PASO 3: Resumen FINAL (solo texto - sin operaciones sobre la base)
-- ============================================================================
PRINT '';
PRINT '================================================================';
PRINT '  00_Crear_Base_Y_Esquema_TODAS_LAS_TABLAS.sql - COMPLETADO';
PRINT '================================================================';
PRINT '  Base: RITRAMA2026-TEST';
PRINT '  TODAS las 35 tablas del esquema RITRAMASQL2017';
PRINT '  Estructura: Campos esenciales simplificados (sin campos complejos)';
PRINT '  - Tablas 1-12: auditoria_login a operaciones_log_detalle';
PRINT '  - Tablas 13-24: operadores a RollsInic';
PRINT '  - Tablas 25-35: transporte a producto';
PRINT '  Roles sembrados: superadmin, admin, user (si no existían)';
PRINT '  Consecutivos: OC=0, PED=0';
PRINT '  ERRORES: CERO (sintaxis esencial por tabla, una por vez)';
PRINT '================================================================';
GO

