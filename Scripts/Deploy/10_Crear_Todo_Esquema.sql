-- ============================================================================
-- 10_Crear_Todo_Esquema.sql (Ritrama2025)
-- Que hace: crea la base de datos si no existe + TODAS las tablas necesarias
--   para el funcionamiento completo del sistema.
-- Idempotente: si objetos ya existen, los deja sin cambios.
-- Ejecutar: sqlcmd -S RITRAMASRV01 -E -C -b -i Scripts\Deploy\10_Crear_Todo_Esquema.sql
-- ============================================================================

SET NOCOUNT ON;
GO

-- ============================================================================
-- PASO 1: Crear base de datos si no existe
-- ============================================================================
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

-- Modelo de recuperacion FULL para produccion.
IF EXISTS (SELECT 1 FROM sys.databases WHERE name = N'ritrama2026' AND recovery_model_desc <> N'FULL')
BEGIN
    ALTER DATABASE [ritrama2026] SET RECOVERY FULL;
    PRINT 'Recovery model = FULL.';
END
GO

-- ============================================================================
-- PASO 2: Usar la base de datos
-- ============================================================================
USE ritrama2026;
GO

-- ============================================================================
-- PASO 3: Tablas de Seguridad y Control
-- ============================================================================

-- Tabla: control (semilleros de consecutivos)
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'control' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE control (
        id                INT IDENTITY(1,1) PRIMARY KEY,
        filter            NVARCHAR(50)    NOT NULL,
        par1              NVARCHAR(50)    NULL,
        par2              NVARCHAR(50)    NULL,
        par3              NVARCHAR(50)    NULL,
        fecha_creacion    DATETIME        NOT NULL DEFAULT GETDATE()
    );
    PRINT 'Tabla control creada.';
END
ELSE
    PRINT 'Tabla control ya existe.';

-- Tabla: roles
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'roles' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE roles (
        id                INT IDENTITY(1,1) PRIMARY KEY,
        nombre            NVARCHAR(50)    NOT NULL UNIQUE,
        descripcion       NVARCHAR(200)   NULL,
        es_superadmin     BIT             NOT NULL DEFAULT 0,
        fecha_creacion    DATETIME        NOT NULL DEFAULT GETDATE()
    );
    PRINT 'Tabla roles creada.';
END
ELSE
    PRINT 'Tabla roles ya existe.';

-- Tabla: permisos
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'permisos' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE permisos (
        id                INT IDENTITY(1,1) PRIMARY KEY,
        rol_id            INT             NOT NULL,
        modulo            NVARCHAR(50)    NOT NULL,
        puede_leer        BIT             NOT NULL DEFAULT 1,
        puede_escribir    BIT             NOT NULL DEFAULT 1,
        puede_eliminar    BIT             NOT NULL DEFAULT 0,
        CONSTRAINT FK_perm_rol FOREIGN KEY (rol_id) REFERENCES roles(id)
    );
    PRINT 'Tabla permisos creada.';
END
ELSE
    PRINT 'Tabla permisos ya existe.';

-- Tabla: usuario_roles (asignacion de roles a usuarios)
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'usuario_roles' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE usuario_roles (
        id                INT IDENTITY(1,1) PRIMARY KEY,
        usuario_id        NVARCHAR(100)   NOT NULL,
        rol_id            INT             NOT NULL,
        fecha_asignacion  DATETIME        NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_ur_rol FOREIGN KEY (rol_id) REFERENCES roles(id)
    );
    PRINT 'Tabla usuario_roles creada.';
END
ELSE
    PRINT 'Tabla usuario_roles ya existe.';

-- ============================================================================
-- PASO 4: Tablas Maestras (Inventario/Products)
-- ============================================================================

-- Tabla: producto
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'producto' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE producto (
        id                INT IDENTITY(1,1) PRIMARY KEY,
        codigo_ritrama    NVARCHAR(50)    NOT NULL UNIQUE,   -- Código semillero Ritrama
        descripcion       NVARCHAR(200)   NOT NULL,
        categoria         NVARCHAR(100)   NULL,
        ancho             DECIMAL(18,2)   NULL,
        largo             DECIMAL(18,2)   NULL,
        peso_kg           DECIMAL(18,2)   NULL,
        costo_unitario    DECIMAL(18,2)   NULL,
        precio_venta      DECIMAL(18,2)   NULL,
        stock_actual      DECIMAL(18,2)   NOT NULL DEFAULT 0,
        stock_minimo      DECIMAL(18,2)   NOT NULL DEFAULT 10,
        unidad_medida     NVARCHAR(20)    NOT NULL,
        activo            BIT             NOT NULL DEFAULT 1,
        fecha_creacion    DATETIME        NOT NULL DEFAULT GETDATE()
    );
    PRINT 'Tabla producto creada.';
END
ELSE
    PRINT 'Tabla producto ya existe.';

-- Tabla: proveedor
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'proveedor' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE proveedor (
        id                INT IDENTITY(1,1) PRIMARY KEY,
        nombre            NVARCHAR(200)   NOT NULL,
        codigo_proveedor  NVARCHAR(50)    NOT NULL UNIQUE,
        direccion         NVARCHAR(300)   NULL,
        telefono          NVARCHAR(50)    NULL,
        email             NVARCHAR(100)   NULL,
        activo            BIT             NOT NULL DEFAULT 1,
        fecha_creacion    DATETIME        NOT NULL DEFAULT GETDATE()
    );
    PRINT 'Tabla proveedor creada.';
END
ELSE
    PRINT 'Tabla proveedor ya existe.';

-- Tabla: vendedor
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'vendedor' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE vendedor (
        id                INT IDENTITY(1,1) PRIMARY KEY,
        nombre            NVARCHAR(200)   NOT NULL,
        codigo_interno    NVARCHAR(50)    NULL UNIQUE,
        comision_pct      DECIMAL(5,2)    NOT NULL DEFAULT 0,
        activo            BIT             NOT NULL DEFAULT 1,
        fecha_creacion    DATETIME        NOT NULL DEFAULT GETDATE()
    );
    PRINT 'Tabla vendedor creada.';
END
ELSE
    PRINT 'Tabla vendedor ya existe.';

-- Tabla: customer
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'customer' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE customer (
        id                INT IDENTITY(1,1) PRIMARY KEY,
        nombre            NVARCHAR(200)   NOT NULL,
        codigo_interno    NVARCHAR(50)    NULL UNIQUE,
        direccion         NVARCHAR(300)   NULL,
        telefono          NVARCHAR(50)    NULL,
        email             NVARCHAR(100)   NULL,
        activo            BIT             NOT NULL DEFAULT 1,
        fecha_creacion    DATETIME        NOT NULL DEFAULT GETDATE()
    );
    PRINT 'Tabla customer creada.';
END
ELSE
    PRINT 'Tabla customer ya existe.';

-- ============================================================================
-- PASO 5: Tablas de Operaciones y Logs
-- ============================================================================

-- Tabla: operaciones_log
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'operaciones_log' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE operaciones_log (
        id                BIGINT IDENTITY(1,1) PRIMARY KEY,
        fecha_inicio      DATETIME2       NOT NULL DEFAULT GETDATE(),
        fecha_fin         DATETIME2       NULL,
        tipo_operacion    NVARCHAR(50)    NOT NULL,
        descripcion       NVARCHAR(500)   NOT NULL,
        usuario           NVARCHAR(100)   NOT NULL DEFAULT '',
        maquina           NVARCHAR(100)   NOT NULL DEFAULT '',
        ip_address        NVARCHAR(50)    NOT NULL DEFAULT '',
        exitoso           BIT             NOT NULL DEFAULT 0,
        resultado         NVARCHAR(1000)  NOT NULL DEFAULT '',
        detalle_error     NVARCHAR(2000)  NOT NULL DEFAULT '',
        created_at        DATETIME2       NOT NULL DEFAULT GETDATE()
    );
    PRINT 'Tabla operaciones_log creada.';

    -- Índices para operaciones_log
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
END
ELSE
    PRINT 'Tabla operaciones_log ya existe.';

-- Tabla: operaciones_log_detalle
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'operaciones_log_detalle' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE operaciones_log_detalle (
        id                BIGINT IDENTITY(1,1) PRIMARY KEY,
        operacion_id      BIGINT          NOT NULL,
        fecha             DATETIME2       NOT NULL DEFAULT GETDATE(),
        accion            NVARCHAR(100)   NOT NULL,
        entidad           NVARCHAR(100)   NOT NULL,
        valor_anterior    NVARCHAR(500)   NULL,
        valor_nuevo       NVARCHAR(500)   NULL,
        notas             NVARCHAR(1000)  NOT NULL DEFAULT '',
        CONSTRAINT FK_operaciones_log_detalle_operacion
            FOREIGN KEY (operacion_id) REFERENCES operaciones_log(id)
            ON DELETE CASCADE
    );
    PRINT 'Tabla operaciones_log_detalle creada.';

    -- Índice para operaciones_log_detalle
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('operaciones_log_detalle') AND name = 'IX_operaciones_log_detalle_operacion')
    BEGIN
        CREATE INDEX IX_operaciones_log_detalle_operacion ON operaciones_log_detalle(operacion_id);
        PRINT 'Índice IX_operaciones_log_detalle_operacion creado.';
    END
END
ELSE
    PRINT 'Tabla operaciones_log_detalle ya existe.';

-- ============================================================================
-- PASO 6: Tablas de Órdenes de Compra (Pedidos)
-- ============================================================================

-- Tabla: orden_compra (encabezado)
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'orden_compra' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE orden_compra (
        id                INT IDENTITY(1,1) PRIMARY KEY,
        numero            NVARCHAR(10)    NOT NULL,       -- "OC-00027"
        fecha             DATETIME        NOT NULL,
        proveedor_id      NVARCHAR(50)    NOT NULL,       -- string, no UNIQUEIDENTIFIER
        proveedor_name    NVARCHAR(200)   NULL,
        persona_contacto  NVARCHAR(100)   NULL,
        fecha_entrega     DATETIME        NULL,
        direccion_entrega NVARCHAR(200)   NULL,
        direccion_facturacion NVARCHAR(200) NULL,
        condiciones_pago  NVARCHAR(100)   NULL,
        prioridad         NVARCHAR(20)    NULL,
        estado            NVARCHAR(20)    NOT NULL DEFAULT 'creado',
        notas             NVARCHAR(MAX)   NULL,
        anulado           BIT             NOT NULL DEFAULT 0,
        subtotal          DECIMAL(18,2)   NULL,
        porc_itbis        DECIMAL(18,2)   NULL,
        itbis             DECIMAL(18,2)   NULL,
        total$            DECIMAL(18,2)   NULL,
        CONSTRAINT UQ_orden_compra_numero UNIQUE (numero)
    );
    PRINT 'Tabla orden_compra creada.';

    -- Índice
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_orden_compra_numero' AND object_id = OBJECT_ID('orden_compra'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_orden_compra_numero ON orden_compra(numero);
        PRINT 'Índice IX_orden_compra_numero creado.';
    END
END
ELSE
    PRINT 'Tabla orden_compra ya existe.';

-- Tabla: orden_compra_detalle (renglones)
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'orden_compra_detalle' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE orden_compra_detalle (
        id                INT IDENTITY(1,1) PRIMARY KEY,
        numero            NVARCHAR(10)    NOT NULL,       -- FK a orden_compra.numero
        product_id        NVARCHAR(25)    NOT NULL,
        product_name      NVARCHAR(200)   NULL,
        cant              DECIMAL(18,2)   NOT NULL DEFAULT 1,
        unidad            NVARCHAR(10)    NULL,
        width             DECIMAL(18,2)   NULL,
        lenght            DECIMAL(18,2)   NULL,           -- Nota: "lenght" en español sería "largo"
        msi               DECIMAL(18,2)   NULL,
        precio            DECIMAL(18,2)   NULL,
        total_renglon     DECIMAL(18,2)   NULL,
        notas             NVARCHAR(MAX)   NULL
    );
    PRINT 'Tabla orden_compra_detalle creada.';

    -- Índice
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_orden_compra_detalle_numero' AND object_id = OBJECT_ID('orden_compra_detalle'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_orden_compra_detalle_numero ON orden_compra_detalle(numero);
        PRINT 'Índice IX_orden_compra_detalle_numero creado.';
    END

    -- FK detalle -> header
    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_OC_DETALLE_OC' AND parent_object_id = OBJECT_ID('orden_compra_detalle'))
    BEGIN
        ALTER TABLE orden_compra_detalle
            ADD CONSTRAINT FK_OC_DETALLE_OC
            FOREIGN KEY (numero) REFERENCES orden_compra(numero);
        PRINT 'FK_OC_DETALLE_OC creada.';
    END
END
ELSE
    PRINT 'Tabla orden_compra_detalle ya existe.';

-- ============================================================================
-- PASO 7: Seguridad, roles y usuaries admin
-- ============================================================================

-- Insertar roles por defecto si no existen
IF NOT EXISTS (SELECT 1 FROM roles WHERE nombre = 'superadmin')
BEGIN
    INSERT INTO roles (nombre, descripcion, es_superadmin) VALUES ('superadmin', 'Administrador del sistema con todos los permisos', 1);
    PRINT 'Rol superadmin sembrado.';
END
ELSE
    PRINT 'Rol superadmin ya existe.';

IF NOT EXISTS (SELECT 1 FROM roles WHERE nombre = 'admin')
BEGIN
    INSERT INTO roles (nombre, descripcion, es_superadmin) VALUES ('admin', 'Administrador de operación', 1);
    PRINT 'Rol admin sembrado.';
END
ELSE
    PRINT 'Rol admin ya existe.';

IF NOT EXISTS (SELECT 1 FROM roles WHERE nombre = 'user')
BEGIN
    INSERT INTO roles (nombre, descripcion, es_superadmin) VALUES ('user', 'Usuario estándar', 0);
    PRINT 'Rol user sembrado.';
END
ELSE
    PRINT 'Rol user ya existe.';

-- Usuario admin: crear login y asignar rol
-- NOTA: El login se crea a nivel de servidor, pero el usuario en la BD se asigna aquí
-- Asumimos que el login 'ritrama_app' o 'admin' ya existe en el servidor o usamos auth integrada
-- Para SQL auth, descomenta la línea de abajo y configura la contraseña

-- Crear usuario admin en la base actual (solo si usando SQL auth)
-- IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'admin')
-- BEGIN
--     CREATE USER admin FOR LOGIN admin WITH DEFAULT_SCHEMA = dbo;
--     PRINT 'Usuario admin creado en la base de datos.';
-- END
-- ELSE
--     PRINT 'Usuario admin ya existe en la base de datos.';

-- Asignar rol superadmin al usuario admin
IF NOT EXISTS (SELECT 1 FROM usuario_roles WHERE usuario_id = 'admin' AND rol_id = (SELECT id FROM roles WHERE nombre = 'superadmin'))
BEGIN
    INSERT INTO usuario_roles (usuario_id, rol_id) VALUES ('admin', (SELECT id FROM roles WHERE nombre = 'superadmin'));
    PRINT 'Rol superadmin asignado al usuario admin.';
END
ELSE
    PRINT 'Rol superadmin ya asignado al usuario admin.';

-- ============================================================================
-- PASO 8: Semilleros iniciales de consecutivos
-- ============================================================================

-- Consecutivos iniciales para documentos (todos en 0 para iniciar)
-- Ejecutar solo la primera vez (el script es idempotente)

IF NOT EXISTS (SELECT 1 FROM control WHERE filter = 'OC')
BEGIN
    INSERT INTO control (filter, par1) VALUES ('OC', '0');
    PRINT 'Consecutivo OC sembrado en control (par1=0).';
END
ELSE
    PRINT 'Consecutivo OC ya existe en control (sin cambios).';

IF NOT EXISTS (SELECT 1 FROM control WHERE filter = 'PED')
BEGIN
    INSERT INTO control (filter, par1) VALUES ('PED', '0');
    PRINT 'Consecutivo PED sembrado en control (par1=0).';
END
ELSE
    PRINT 'Consecutivo PED ya existe en control (sin cambios).';

IF NOT EXISTS (SELECT 1 FROM control WHERE filter = 'CLI')
BEGIN
    INSERT INTO control (filter, par1) VALUES ('CLI', '0');
    PRINT 'Consecutivo CLI sembrado en control (par1=0).';
END
ELSE
    PRINT 'Consecutivo CLI ya existe en control (sin cambios).';

IF NOT EXISTS (SELECT 1 FROM control WHERE filter = 'PROV')
BEGIN
    INSERT INTO control (filter, par1) VALUES ('PROV', '0');
    PRINT 'Consecutivo PROV sembrado en control (par1=0).';
END
ELSE
    PRINT 'Consecutivo PROV ya existe en control (sin cambios).';

IF NOT EXISTS (SELECT 1 FROM control WHERE filter = 'VEND')
BEGIN
    INSERT INTO control (filter, par1) VALUES ('VEND', '0');
    PRINT 'Consecutivo VEND sembrado en control (par1=0).';
END
ELSE
    PRINT 'Consecutivo VEND ya existe en control (sin cambios).';

IF NOT EXISTS (SELECT 1 FROM control WHERE filter = 'COC')
BEGIN
    INSERT INTO control (filter, par1) VALUES ('COC', '0');
    PRINT 'Consecutivo COC sembrado en control (par1=0).';
END
ELSE
    PRINT 'Consecutivo COC ya existe en control (sin cambios).';

IF NOT EXISTS (SELECT 1 FROM control WHERE filter = 'UC')
BEGIN
    INSERT INTO control (filter, par1) VALUES ('UC', '0');
    PRINT 'Consecutivo UC sembrado en control (par1=0).';
END
ELSE
    PRINT 'Consecutivo UC ya existe en control (sin cambios).';

IF NOT EXISTS (SELECT 1 FROM control WHERE filter = 'CMP')
BEGIN
    INSERT INTO control (filter, par1) VALUES ('CMP', '0');
    PRINT 'Consecutivo CMP sembrado en control (par1=0).';
END
ELSE
    PRINT 'Consecutivo CMP ya existe en control (sin cambios).';

-- ============================================================================
-- PASO 9: Resumen final
-- ============================================================================
PRINT '';
PRINT '================================================================';
PRINT '  10_Crear_Todo_Esquema.sql completado exitosamente.';
PRINT '================================================================';
PRINT '  Base de datos: ritrama2026';
PRINT '  Compatibilidad: 140 (SQL Server 2017)';
PRINT '  Roles creados: superadmin, admin, user';
PRINT '  Usuario admin asignado con rol superadmin';
PRINT '  Consecutivos iniciales: OC, PED, CLI, PROV, VEND, COC, UC, CMP';
PRINT '  Tablas creadas/verificadas';
PRINT '';
PRINT '  Siguiente paso: Ejecutar 30_Verificar_Despliegue.sql para validar.';
PRINT '  Login: admin / Password: admin (o usar auth integrada con -E)';
PRINT '================================================================';
GO