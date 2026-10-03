-- ============================================================================
-- SCRIPT DE INSTALACION: MODULO DE ORDENES DE COMPRA (Ritrama2025)
-- Crea las tablas orden_compra y orden_compra_detalle, y siembra el
-- consecutivo de OC en la tabla control.
-- Todas las operaciones son IDEMPOTENTES: se pueden ejecutar N veces sin error.
-- ============================================================================

-- ============================================================================
-- 1) TABLA: orden_compra (encabezado de orden de compra)
--    `numero` es NVARCHAR(10) para almacenar "OC-00027", string para que el
--    prefijo viaje con el dato (mismo patron que el modulo Pedidos).
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'orden_compra' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE orden_compra (
        id                INT IDENTITY(1,1) PRIMARY KEY,
        numero            NVARCHAR(10)    NOT NULL,  -- "OC-00027"
        fecha             DATETIME        NOT NULL,
        proveedor_id      NVARCHAR(50)    NOT NULL,  -- string, no UNIQUEIDENTIFIER
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
    PRINT 'Tabla orden_compra creada correctamente.';
END
ELSE
    PRINT 'Tabla orden_compra ya existe (sin cambios).';

-- Indice IX_orden_compra_numero
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_orden_compra_numero' AND object_id = OBJECT_ID('orden_compra'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_orden_compra_numero ON orden_compra(numero);
    PRINT 'Indice IX_orden_compra_numero creado.';
END
ELSE
    PRINT 'Indice IX_orden_compra_numero ya existe (sin cambios).';

-- ============================================================================
-- 2) TABLA: orden_compra_detalle (renglones de la orden de compra)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'orden_compra_detalle' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE orden_compra_detalle (
        id              INT IDENTITY(1,1) PRIMARY KEY,
        numero          NVARCHAR(10)     NOT NULL,  -- FK a orden_compra.numero
        product_id      NVARCHAR(25)     NOT NULL,
        product_name    NVARCHAR(200)    NULL,
        cant            DECIMAL(18,2)    NOT NULL DEFAULT 1,
        unidad          NVARCHAR(10)     NULL,
        width           DECIMAL(18,2)    NULL,
        lenght          DECIMAL(18,2)    NULL,
        msi             DECIMAL(18,2)    NULL,
        precio          DECIMAL(18,2)    NULL,
        total_renglon   DECIMAL(18,2)    NULL,
        notas           NVARCHAR(MAX)    NULL
    );
    PRINT 'Tabla orden_compra_detalle creada correctamente.';
END
ELSE
    PRINT 'Tabla orden_compra_detalle ya existe (sin cambios).';

-- Indice IX_orden_compra_detalle_numero
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_orden_compra_detalle_numero' AND object_id = OBJECT_ID('orden_compra_detalle'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_orden_compra_detalle_numero ON orden_compra_detalle(numero);
    PRINT 'Indice IX_orden_compra_detalle_numero creado.';
END
ELSE
    PRINT 'Indice IX_orden_compra_detalle_numero ya existe (sin cambios).';

-- FK orden_compra_detalle -> orden_compra(numero)
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_OC_DETALLE_OC' AND parent_object_id = OBJECT_ID('orden_compra_detalle'))
BEGIN
    ALTER TABLE orden_compra_detalle
        ADD CONSTRAINT FK_OC_DETALLE_OC
        FOREIGN KEY (numero) REFERENCES orden_compra(numero);
    PRINT 'FK_OC_DETALLE_OC creada.';
END
ELSE
    PRINT 'FK_OC_DETALLE_OC ya existe (sin cambios).';

-- ============================================================================
-- 3) SEMILLERO: consecutivo de ordenes de compra en tabla control
--    Si no existe filtro='OC', lo inserta con par1='0' (primer consecutivo).
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM control WHERE filter = 'OC')
BEGIN
    INSERT INTO control (filter, par1) VALUES ('OC', '0');
    PRINT 'Consecutivo OC sembrado en control (par1=0).';
END
ELSE
    PRINT 'Consecutivo OC ya existe en control (sin cambios).';

-- ============================================================================
-- RESUMEN DE OPERACIONES APLICADAS
-- ============================================================================
PRINT '================================================================';
PRINT '  RESUMEN: 004_CrearTablas_OrdenesCompra.sql completado.';
PRINT '================================================================';
PRINT '  1. Tabla orden_compra ............. encabezado de ordenes de compra.';
PRINT '  2. Tabla orden_compra_detalle ..... renglones/lineas de la OC.';
PRINT '  3. Indice IX_orden_compra_numero . acceso rapido por numero.';
PRINT '  4. Indice IX_orden_compra_detalle_numero . detalle por OC.';
PRINT '  5. FK_OC_DETALLE_OC .............. integridad detalle->header.';
PRINT '  6. control (filter=OC) ........... semilla del consecutivo.';
PRINT '================================================================';
