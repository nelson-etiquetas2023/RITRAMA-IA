-- ============================================================================
-- SCRIPT DE INSTALACION: MODULO DE PEDIDOS (Ritrama2025)
-- Crea las tablas pedido y pedido_detalle, extiende customer, agrega
-- pedido_id a orden_corte y despacho, y siembra el consecutivo en control.
-- Todas las operaciones son IDEMPOTENTES: se pueden ejecutar N veces sin error.
-- ============================================================================

-- ============================================================================
-- 1) TABLA: pedido (encabezado de pedido de venta)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'pedido' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE pedido (
        id                INT IDENTITY(1,1) PRIMARY KEY,
        numero            INT            NOT NULL,
        fecha             DATETIME       NOT NULL,
        customer_id       UNIQUEIDENTIFIER NOT NULL,
        customer_name     NVARCHAR(200)  NULL,
        vendor_id         UNIQUEIDENTIFIER NULL,
        persona_contacto  NVARCHAR(100)  NULL,
        tipo_venta        NVARCHAR(20)   NULL,
        fecha_entrega     DATETIME       NULL,
        condiciones_pago  NVARCHAR(100)  NULL,
        direccion_entrega NVARCHAR(200)  NULL,
        estado            NVARCHAR(20)   NOT NULL DEFAULT 'creado',
        notas             NVARCHAR(MAX)  NULL,
        anulado           BIT            NOT NULL DEFAULT 0,
        subtotal          DECIMAL(18,2)  NULL,
        porc_itbis        DECIMAL(18,2)  NULL,
        itbis             DECIMAL(18,2)  NULL,
        total$            DECIMAL(18,2)  NULL,
        CONSTRAINT UQ_pedido_numero UNIQUE (numero)
    );
    PRINT 'Tabla pedido creada correctamente.';
END
ELSE
    PRINT 'Tabla pedido ya existe (sin cambios).';

-- Indice IX_pedido_numero
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_pedido_numero' AND object_id = OBJECT_ID('pedido'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_pedido_numero ON pedido(numero);
    PRINT 'Indice IX_pedido_numero creado.';
END
ELSE
    PRINT 'Indice IX_pedido_numero ya existe (sin cambios).';

-- ============================================================================
-- 2) TABLA: pedido_detalle (renglones del pedido)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'pedido_detalle' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE pedido_detalle (
        id              INT IDENTITY(1,1) PRIMARY KEY,
        numero          INT            NOT NULL,
        product_id      NVARCHAR(25)  NOT NULL,
        product_name    NVARCHAR(200) NULL,
        cant            DECIMAL(18,2) NOT NULL DEFAULT 1,
        unidad          NVARCHAR(10)  NULL,
        width           DECIMAL(18,2) NULL,
        lenght          DECIMAL(18,2) NULL,
        msi             DECIMAL(18,2) NULL,
        precio          DECIMAL(18,2) NULL,
        total_renglon   DECIMAL(18,2) NULL,
        notas           NVARCHAR(MAX) NULL
    );
    PRINT 'Tabla pedido_detalle creada correctamente.';
END
ELSE
    PRINT 'Tabla pedido_detalle ya existe (sin cambios).';

-- Indice IX_pedido_detalle_numero
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_pedido_detalle_numero' AND object_id = OBJECT_ID('pedido_detalle'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_pedido_detalle_numero ON pedido_detalle(numero);
    PRINT 'Indice IX_pedido_detalle_numero creado.';
END
ELSE
    PRINT 'Indice IX_pedido_detalle_numero ya existe (sin cambios).';

-- FK PEDIDO_DETALLE -> pedido(numero)
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_PEDIDO_DETALLE_PEDIDO' AND parent_object_id = OBJECT_ID('pedido_detalle'))
BEGIN
    ALTER TABLE pedido_detalle
        ADD CONSTRAINT FK_PEDIDO_DETALLE_PEDIDO
        FOREIGN KEY (numero) REFERENCES pedido(numero);
    PRINT 'FK_PEDIDO_DETALLE_PEDIDO creada.';
END
ELSE
    PRINT 'FK_PEDIDO_DETALLE_PEDIDO ya existe (sin cambios).';

-- ============================================================================
-- 3) ALTER TABLE customer: columnas adicionales para el modulo de pedidos
--    Idempotente: verifica COL_LENGTH antes de agregar cada columna.
-- ============================================================================

-- customer_address
IF COL_LENGTH('customer', 'customer_address') IS NULL
BEGIN
    ALTER TABLE customer ADD customer_address NVARCHAR(200) NULL;
    PRINT 'customer: columna customer_address agregada.';
END
ELSE
    PRINT 'customer: customer_address ya existe (sin cambios).';

-- customer_phone
IF COL_LENGTH('customer', 'customer_phone') IS NULL
BEGIN
    ALTER TABLE customer ADD customer_phone NVARCHAR(30) NULL;
    PRINT 'customer: columna customer_phone agregada.';
END
ELSE
    PRINT 'customer: customer_phone ya existe (sin cambios).';

-- customer_ruc
IF COL_LENGTH('customer', 'customer_ruc') IS NULL
BEGIN
    ALTER TABLE customer ADD customer_ruc NVARCHAR(20) NULL;
    PRINT 'customer: columna customer_ruc agregada.';
END
ELSE
    PRINT 'customer: customer_ruc ya existe (sin cambios).';

-- customer_contact
IF COL_LENGTH('customer', 'customer_contact') IS NULL
BEGIN
    ALTER TABLE customer ADD customer_contact NVARCHAR(100) NULL;
    PRINT 'customer: columna customer_contact agregada.';
END
ELSE
    PRINT 'customer: customer_contact ya existe (sin cambios).';

-- customer_zone
IF COL_LENGTH('customer', 'customer_zone') IS NULL
BEGIN
    ALTER TABLE customer ADD customer_zone NVARCHAR(60) NULL;
    PRINT 'customer: columna customer_zone agregada.';
END
ELSE
    PRINT 'customer: customer_zone ya existe (sin cambios).';

-- condiciones_pago
IF COL_LENGTH('customer', 'condiciones_pago') IS NULL
BEGIN
    ALTER TABLE customer ADD condiciones_pago NVARCHAR(100) NULL;
    PRINT 'customer: columna condiciones_pago agregada.';
END
ELSE
    PRINT 'customer: condiciones_pago ya existe (sin cambios).';

-- limite_credito
IF COL_LENGTH('customer', 'limite_credito') IS NULL
BEGIN
    ALTER TABLE customer ADD limite_credito DECIMAL(18,2) NULL;
    PRINT 'customer: columna limite_credito agregada.';
END
ELSE
    PRINT 'customer: limite_credito ya existe (sin cambios).';

-- ============================================================================
-- 4) ALTER TABLE orden_corte: agregar pedido_id
-- ============================================================================
IF COL_LENGTH('orden_corte', 'pedido_id') IS NULL
BEGIN
    ALTER TABLE orden_corte ADD pedido_id INT NULL;
    PRINT 'orden_corte: columna pedido_id agregada.';
END
ELSE
    PRINT 'orden_corte: pedido_id ya existe (sin cambios).';

-- ============================================================================
-- 5) ALTER TABLE despacho: agregar pedido_id
-- ============================================================================
IF COL_LENGTH('despacho', 'pedido_id') IS NULL
BEGIN
    ALTER TABLE despacho ADD pedido_id INT NULL;
    PRINT 'despacho: columna pedido_id agregada.';
END
ELSE
    PRINT 'despacho: pedido_id ya existe (sin cambios).';

-- ============================================================================
-- 6) SEMILLERO: consecutivo de pedidos en tabla control
--    Si no existe filtro='PED', lo inserta con par1='0' (primer consecutivo).
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM control WHERE filter = 'PED')
BEGIN
    INSERT INTO control (filter, par1) VALUES ('PED', '0');
    PRINT 'Consecutivo PED sembrado en control (par1=0).';
END
ELSE
    PRINT 'Consecutivo PED ya existe en control (sin cambios).';

-- ============================================================================
-- RESUMEN DE OPERACIONES APLICADAS
-- ============================================================================
PRINT '================================================================';
PRINT '  RESUMEN: Setup_Pedidos.sql completado.';
PRINT '================================================================';
PRINT '  1. Tabla pedido .............. encabezado de pedidos de venta.';
PRINT '  2. Tabla pedido_detalle ...... renglones/lineas del pedido.';
PRINT '  3. Indices IX_pedido_numero y IX_pedido_detalle_numero.';
PRINT '  4. FK_PEDIDO_DETALLE_PEDIDO .. integridad detalle->pedido.';
PRINT '  5. customer (+7 columnas) .... direccion, tel, ruc, etc.';
PRINT '  6. orden_corte.pedido_id ..... enlace OC <-> Pedido.';
PRINT '  7. despacho.pedido_id ........ enlace despacho <-> Pedido.';
PRINT '  8. control (filter=PED) ...... semilla del consecutivo.';
PRINT '================================================================';
