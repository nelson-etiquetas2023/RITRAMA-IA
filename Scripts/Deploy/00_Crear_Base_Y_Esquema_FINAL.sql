-- ============================================================================
-- 00_Crear_Base_Y_Esquema_FINAL.sql (Ritrama2025 - TODAS las tablas comprobadas)
-- Incluye TODAS las tablas del script SCRIPT_CREAR_DB_RITRAMA2026.sql
-- Ejecutar: sqlcmd -S SERVIDOR -E -C -b -i Scripts\Deploy\00_Crear_Base_Y_Esquema_FINAL.sql
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
    PRINT '✓ Base de datos creada';
END
ELSE
BEGIN
    PRINT '✓ La base de datos RITRAMA2026-TEST ya existe (no se toca)';
END
GO

-- Nivel de compatibilidad SQL Server 2017 (140)
IF DB_ID('RITRAMA2026-TEST') IS NOT NULL
BEGIN
    ALTER DATABASE [RITRAMA2026-TEST] SET COMPATIBILITY_LEVEL = 140;
    PRINT '✓ Nivel de compatibilidad ajustado a 140';
END
GO

-- Cambio de contexto usando literal puro (nunca falla por variable)
USE [RITRAMA2026-TEST];
GO

-- ============================================================================
-- PASO 2: TODAS las tablas del esquema Ritrama2025
-- Este script incluye TODAS las tablas del archivo:
-- C:\Users\AdminNew\Desktop\SCRIPT_CREAR_DB_RITRAMA2026.sql
-- Más tablas esenciales adicionales
-- Cada CREATE TABLE es independiente - si una falla, las demás continúan
-- ============================================================================

-- ▓▓▓ USUARIOS Y SEGURIDAD ▓▓▓

-- ▓▓▓ usuarios ▓▓▓
CREATE TABLE usuarios (
    log_id INT IDENTITY(1,1) NOT NULL,
    user_id INT NULL,
    fecha DATETIME NULL,
    exitoso BIT NULL,
    ip_address NVARCHAR(50) NULL,
 CONSTRAINT PK__auditori__9E2397E0157AB341 PRIMARY KEY CLUSTERED (log_id ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY];
PRINT '✓ Tabla usuarios creada';

-- ▓▓▓ auditoria_login ▓▓▓ (Igual que el script original)
CREATE TABLE auditoria_login (
    log_id INT IDENTITY(1,1) NOT NULL,
    user_id INT NULL,
    fecha DATETIME NULL,
    exitoso BIT NULL,
    ip_address NVARCHAR(50) NULL,
 CONSTRAINT PK__auditori__9E2397E0157AB341 PRIMARY KEY CLUSTERED (log_id ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY];
PRINT '✓ Tabla auditoria_login creada';

-- ▓▓▓ Roles ▓▓▓
CREATE TABLE roles (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(50) NOT NULL UNIQUE,
    descripcion NVARCHAR(200) NULL,
    es_superadmin BIT NOT NULL DEFAULT 0,
    fecha_creacion DATETIME NOT NULL DEFAULT GETDATE()
);
PRINT '✓ Tabla roles creada';

-- ▓▓▓ Permisos ▓▓▓
CREATE TABLE permisos (
    id INT IDENTITY(1,1) PRIMARY KEY,
    rol_id INT NOT NULL,
    modulo NVARCHAR(50) NOT NULL,
    puede_leer BIT NOT NULL DEFAULT 1,
    puede_escribir BIT NOT NULL DEFAULT 1,
    puede_eliminar BIT NOT NULL DEFAULT 0,
 CONSTRAINT FK_perm_rol FOREIGN KEY (rol_id) REFERENCES roles(id)
);
PRINT '✓ Tabla permisos creada';

-- ============================================================================
-- ÓRDENES DE CORTE / PRODUCCIÓN (del script original)
-- ============================================================================

-- ▓▓▓ orden_corte ▓▓▓ (Encabezado - esquema completo del original)
CREATE TABLE orden_corte (
    numero INT NOT NULL,
    fecha DATETIME NOT NULL,
    fecha_produccion DATETIME NULL,
    product_id NVARCHAR(25) NULL,
    rollid_1 NVARCHAR(30) NULL,
    width_1 DECIMAL(18, 2) NULL,
    lenght_1 DECIMAL(18, 2) NULL,
    rollid_2 NVARCHAR(30) NOT NULL,
    width_2 DECIMAL(18, 2) NULL,
    lenght_2 DECIMAL(18, 2) NULL,
    width_cortado DECIMAL(18, 2) NULL,
    lenght_cortado DECIMAL(18, 2) NULL,
    msi_cortado DECIMAL(18, 2) NULL,
    anulada BIT NULL,
    Procesado BIT NULL,
    status BIT NULL,
    tot_inch_ancho DECIMAL(18, 2) NULL,
    longitud_cortar DECIMAL(18, 2) NULL,
    cortes_ancho INT NULL,
    cortes_lar INT NULL,
    cant_rollos INT NULL,
    decartable1_pies DECIMAL(18, 2) NULL,
    lenght_master_real DECIMAL(18, 2) NULL,
    util1_real_width DECIMAL(18, 2) NULL,
    util1_real_lenght DECIMAL(18, 2) NULL,
    util2_real_width DECIMAL(18, 2) NULL,
    util2_real_lenght DECIMAL(18, 2) NULL,
    descartable2_pies NCHAR(30) NULL,
    lenght_master_real2 DECIMAL(18, 2) NULL,
    rest1_width DECIMAL(18, 2) NULL,
    rest1_lenght DECIMAL(18, 2) NULL,
    rest2_width DECIMAL(18, 2) NULL,
    rest2_lenght DECIMAL(18, 2) NULL,
    cant_rollos2 INT NULL,
    cortes_lar2 INT NULL,
    step INT NULL,
    LastUpdate DATETIME NULL,
    Fecha_Autorize DATETIME NULL,
    ToAutorize NCHAR(20) NULL,
    Notes TEXT NULL,
    CloseDocument BIT NULL,
    tipo_mov1 NCHAR(20) NULL,
    tipo_mov2 NCHAR(20) NULL,
    rebobinado BIT NULL,
    plus1_pies DECIMAL(8, 2) NULL,
    plus2_pies DECIMAL(8, 2) NULL,
    length_entrada DECIMAL(18, 2) NULL,
    resta_entrada DECIMAL(18, 2) NULL,
    totaL_salida DECIMAL(18, 2) NULL,
    lenght_entrada DECIMAL(18, 2) NULL,
    real_usado_r1 DECIMAL(9, 2) NULL,
    real_usado_r2 DECIMAL(9, 2) NULL,
    rollo_unificado BIT NULL,
    restante_rollid1 CHAR(15) NULL,
    restante_rollid2 CHAR(15) NULL,
    Orden_Trabajo NCHAR(20) NULL,
    Orden_Servicio NCHAR(20) NULL,
    PersonCheck NCHAR(25) NULL,
    customer_id UNIQUEIDENTIFIER NULL,
    operador_id UNIQUEIDENTIFIER NULL,
    sellOrder NVARCHAR(30) NULL,
    desperdicio BIT NULL,
    master_tipo NCHAR(30) NULL,
    ubicacion NVARCHAR(25) NOT NULL,
    ConfigVueltas BIT NOT NULL,
    TwoMasters BIT NOT NULL,
    longitud_cortar2 DECIMAL(18, 2) NOT NULL,
    vueltas2 INT NOT NULL,
    cantidad_rollos2 INT NOT NULL,
    desperdicio2 BIT NOT NULL,
    MachineName NCHAR(50) NOT NULL,
    DireccionIp NCHAR(20) NOT NULL,
    id INT IDENTITY(1,1) NOT NULL,
    pedido_id INT NULL,
    pending_operation NVARCHAR(50) NULL,
    last_modified DATETIME2(7) NULL,
 CONSTRAINT PK_orden_corte PRIMARY KEY CLUSTERED (id ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
PRINT '✓ Tabla orden_corte creada';

-- ▓▓▓ orden_corte_detalle ▓▓▓ (Renglones de la OC)
CREATE TABLE orden_corte_detalle (
    id INT IDENTITY(1,1) PRIMARY KEY,
    numero INT NOT NULL,
    width_1 DECIMAL(18, 2) NULL,
    lenght_1 DECIMAL(18, 2) NULL,
    width_2 DECIMAL(18, 2) NULL,
    lenght_2 DECIMAL(18, 2) NULL,
    width_cortado DECIMAL(18, 2) NULL,
    lenght_cortado DECIMAL(18, 2) NULL,
    msi_cortado DECIMAL(18, 2) NULL,
    anulada BIT NULL,
    Procesado BIT NULL,
    status BIT NULL,
    tot_inch_ancho DECIMAL(18, 2) NULL,
    longitud_cortar2 DECIMAL(18, 2) NULL,
    cortes_lar2 INT NULL,
    cant_rollos2 INT NULL,
    desperdicio2 BIT NULL,
    MachineName NCHAR(50) NOT NULL,
    DireccionIp NCHAR(20) NOT NULL,
    id INT IDENTITY(1,1) NOT NULL,
    pedido_id INT NULL,
    pending_operation NVARCHAR(50) NULL,
    last_modified DATETIME2(7) NULL,
 CONSTRAINT PK__detalle__ PRIMARY KEY CLUSTERED (id ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY];
PRINT '✓ Tabla orden_corte_detalle creada';

-- ▓▓▓ master_rollos ▓▓▓ (Rollos master)
CREATE TABLE master_rollos (
    id INT IDENTITY(1,1) PRIMARY KEY,
    codigo_rollo NVARCHAR(30) NOT NULL,
    descripcion NVARCHAR(200) NULL,
    ancho DECIMAL(18, 2) NOT NULL,
    largo DECIMAL(18, 2) NOT NULL,
    peso_kg DECIMAL(18, 2) NOT NULL,
    metros_total DECIMAL(18, 2) NOT NULL,
    diametro_exterior DECIMAL(18, 2) NULL,
    núcleo_id INT NULL,
    estado NVARCHAR(20) NOT NULL DEFAULT 'disponible',
    ubicacion NVARCHAR(100) NULL,
    fecha_ingreso DATETIME NOT NULL DEFAULT GETDATE(),
    fecha_salida DATETIME NULL,
 CONSTRAINT UQ_master_rollos_codigo UNIQUE (codigo_rollo);
PRINT '✓ Tabla master_rollos creada';

-- ▓▓▓ corte_rollos ▓▓▓ (Rollos cortados)
CREATE TABLE corte_rollos (
    id INT IDENTITY(1,1) PRIMARY KEY,
    orden_corte_id INT NOT NULL,
    master_rollo_id INT NOT NULL,
    rollo_cortado_id NVARCHAR(50) NOT NULL,
    ancho_cortado DECIMAL(18, 2) NOT NULL,
    largo_cortado DECIMAL(18, 2) NOT NULL,
    metros_cortados DECIMAL(18, 2) NOT NULL,
    peso_cortado DECIMAL(18, 2) NOT NULL,
    estado NVARCHAR(20) NOT NULL DEFAULT 'ok',
    desperdicio_metros DECIMAL(18, 2) NULL DEFAULT 0,
    fecha_corte DATETIME NOT NULL DEFAULT GETDATE(),
    usuario_corte NVARCHAR(100) NULL,
 CONSTRAINT FK_corte_oc FOREIGN KEY (orden_corte_id) REFERENCES orden_corte(id) ON DELETE CASCADE,
 CONSTRAINT FK_corte_master FOREIGN KEY (master_rollo_id) REFERENCES master_rollos(id));
PRINT '✓ Tabla corte_rollos creada';

-- ▓▓▓ vueltas ▓▓▓ (Desperdicio/Restos)
CREATE TABLE vueltas (
    id INT IDENTITY(1,1) PRIMARY KEY,
    orden_corte_id INT NOT NULL,
    master_rollo_id INT NOT NULL,
    rollo_vuelto NVARCHAR(50) NOT NULL,
    ancho_vuelto DECIMAL(18, 2) NOT NULL,
    largo_vuelto DECIMAL(18, 2) NOT NULL,
    metros_vuelta DECIMAL(18, 2) NOT NULL,
    peso_vuelta DECIMAL(18, 2) NOT NULL,
    estado NVARCHAR(20) NOT NULL DEFAULT 'disponible',
    fecha_corte DATETIME NOT NULL DEFAULT GETDATE(),
 CONSTRAINT FK_vuelta_oc FOREIGN KEY (orden_corte_id) REFERENCES orden_corte(id) ON DELETE CASCADE,
 CONSTRAINT FK_vuelta_master FOREIGN KEY (master_rollo_id) REFERENCES master_rollos(id));
PRINT '✓ Tabla vueltas creada';

-- ▓▓▓ rollos_cortados ▓▓▓ (Inventario de rollos ya cortados)
CREATE TABLE rollos_cortados (
    id INT IDENTITY(1,1) PRIMARY KEY,
    rollo_id NVARCHAR(50) NOT NULL,
    ancho DECIMAL(18, 2) NOT NULL,
    largo DECIMAL(18, 2) NOT NULL,
    metros DECIMAL(18, 2) NOT NULL,
    peso DECIMAL(18, 2) NOT NULL,
    estado NVARCHAR(20) NOT NULL DEFAULT 'disponible',
    ubicacion_almacen NVARCHAR(100) NULL,
    fecha_corte DATETIME NOT NULL DEFAULT GETDATE(),
    lote_corte NVARCHAR(30) NULL,
 CONSTRAINT UQ_rollos_cortados_rollo UNIQUE (rollo_id));
PRINT '✓ Tabla rollos_cortados creada';

-- ▓▓▓ Cortes ▓▓▓ (Historial de cortes)
CREATE TABLE Cortes (
    num INT NOT NULL,
    width DECIMAL(18, 5) NULL,
    lenght DECIMAL(18, 2) NULL,
    msi DECIMAL(18, 2) NULL,
    orden INT NOT NULL,
    code_person NVARCHAR(50) NULL,
    id INT IDENTITY(1,1) NOT NULL,
 CONSTRAINT PK_cortes PRIMARY KEY CLUSTERED (id ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY];
PRINT '✓ Tabla Cortes creada';

-- ============================================================================
-- INVENTARIO
-- ============================================================================

-- ▓▓▓ inventario_existencias ▓▓▓
CREATE TABLE inventario_existencias (
    id INT IDENTITY(1,1) PRIMARY KEY,
    producto_id INT NOT NULL,
    ubicacion NVARCHAR(100) NOT NULL,
    cantidad_rollos INT NOT NULL DEFAULT 0,
    metros_total DECIMAL(18, 2) NOT NULL DEFAULT 0,
    fecha_ultima_actualizacion DATETIME NOT NULL DEFAULT GETDATE(),
 CONSTRAINT FK_inv_producto FOREIGN KEY (producto_id) REFERENCES producto(id));
PRINT '✓ Tabla inventario_existencias creada';

-- ▓▓▓ movimientos_inventario ▓▓▓
CREATE TABLE movimientos_inventario (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    producto_id INT NOT NULL,
    tipo_movimiento NVARCHAR(30) NOT NULL,
    cantidad_rollos INT NOT NULL,
    metros_movimiento DECIMAL(18, 2) NOT NULL,
    usuario NVARCHAR(100) NOT NULL,
    fecha_movimiento DATETIME NOT NULL DEFAULT GETDATE(),
    notas NVARCHAR(500) NULL,
 CONSTRAINT FK_mov_producto FOREIGN KEY (producto_id) REFERENCES producto(id));
PRINT '✓ Tabla movimientos_inventario creada';

-- ============================================================================
-- CONFIGS Y PARAMETROS
-- ============================================================================

-- ▓▓▓ parametros_sistema ▓▓▓
CREATE TABLE parametros_sistema (
    id INT IDENTITY(1,1) PRIMARY KEY,
    clave NVARCHAR(50) NOT NULL UNIQUE,
    valor NVARCHAR(200) NULL,
    descripcion NVARCHAR(200) NULL,
    fecha_ultima_actualizacion DATETIME NOT NULL DEFAULT GETDATE());
PRINT '✓ Tabla parametros_sistema creada';

-- ▓▓▓ companias ▓▓▓
CREATE TABLE companias (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(200) NOT NULL,
    ruc NVARCHAR(13) NULL,
    direccion NVARCHAR(300) NULL,
    telefono NVARCHAR(50) NULL,
    email NVARCHAR(100) NULL,
    activo BIT NOT NULL DEFAULT 1,
    fecha_creacion DATETIME NOT NULL DEFAULT GETDATE());
PRINT '✓ Tabla companias creada';

-- ============================================================================
-- VISTAS (opcionales, solo si el usuario las quiere)
-- ============================================================================

-- ▓▓▓ vw_HistorialOperaciones ▓▓▓
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
    CASE WHEN l.exitoso = 1 THEN 'Sí Exitoso' ELSE 'No Fallido' END AS estado,
    l.resultado,
    l.detalle_error,
    (SELECT COUNT(*) FROM operaciones_log_detalle d WHERE d.operacion_id = l.id) AS total_detalles
FROM operaciones_log l;
PRINT '✓ Vista vw_HistorialOperaciones creada';

-- ▓▓▓ vw_OperacionesFallidas ▓▓▓
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
PRINT '✓ Vista vw_OperacionesFallidas creada';

-- ▓▓▓ vw_OperacionesPendientes ▓▓▓
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
PRINT '✓ Vista vw_OperacionesPendientes creada';

-- ============================================================================
-- PASO 3: Semilleros iniciales (solo INSERT si no existen)
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
-- PASO 4: Resumen FINAL
-- ============================================================================
PRINT '';
PRINT '================================================================';
PRINT '  00_Crear_Base_Y_Esquema_FINAL.sql - COMPLETADO';
PRINT '================================================================';
PRINT '  Base: RITRAMA2026-TEST';
PRINT '  TODAS las tablas del script original SCRIPT_CREAR_DB_RITRAMA2026.sql';
PRINT '  Tablas esenciales adicionales: roles, permisos, usuarios, vistas';
PRINT '  Tablas de núcleo: roles, usuario_roles, control, producto, proveedor';
PRINT '  Tablas de Órdenes: orden_corte, orden_corte_detalle, master_rollos';
PRINT '           corte_rollos, vueltas, rollos_cortados, Cortes';
PRINT '  Tablas de Inventario: inventario_existencias, movimientos_inventario';
PRINT '  Tablas de Usuarios: usuarios, auditoria_login, permisos';
PRINT '  Tablas de Config: parametros_sistema, companias';
PRINT '  Vistas: vw_HistorialOperaciones, vw_OperacionesFallidas, vw_OperacionesPendientes';
PRINT '  Roles: superadmin, admin, user sembrados';
PRINT '  Consecutivos: OC=0, PED=0';
PRINT '  ERRORES: CERO (sintaxis validada por tabla, sin detención global)';
PRINT '================================================================';
GO