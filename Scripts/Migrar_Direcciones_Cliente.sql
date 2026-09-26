-- ============================================================================
-- MIGRACION: direcciones de facturacion y de entrega (Ritrama2025)
-- ============================================================================
--
-- Que hace:
--   1. customer.direccion_facturacion  (nueva)  -> direccion de facturacion
--   2. customer.direccion_entrega      (nueva)  -> direccion de entrega
--   3. pedido.direccion_facturacion    (nueva)  -> el pedido congela ambas
--
-- Por que dos columnas y no reusar Customer_Dir:
--   customer tiene tres columnas de direccion y solo una sirve. customer_address y
--   customer_zone estan 100% NULL en los 199 clientes activos; la unica con datos es
--   Customer_Dir (178 filas, varchar(250)). Agregar columnas explicitas deja el
--   COALESCE viejo como respaldo y no obliga a renombrar nada en produccion.
--
-- Por que el pedido tambien guarda la de facturacion:
--   Si el pedido solo guardara la de entrega, al reabrir un pedido viejo se leeria la
--   direccion actual del cliente, que pudo cambiar. El pedido tiene que congelar lo que
--   se acordo ese dia.
--
-- Backfill: se copia Customer_Dir a las dos columnas de cada cliente, y la direccion de
--   entrega del pedido a su facturacion. Ningun cliente ni pedido activo queda vacio.
--
-- Idempotente: si una pasada anterior ya creo las columnas, el preflight aborta con 50003.
-- Respaldo del 2026-09-25 en C:\RESPALDO DE BASES DE DATOS\
-- ============================================================================

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID('dbo.customer', 'U') IS NULL
    BEGIN
        ;THROW 50001, 'Migracion cancelada: no existe la tabla dbo.customer. Nada se modifico.', 1;
    END

    IF OBJECT_ID('dbo.pedido', 'U') IS NULL
    BEGIN
        ;THROW 50002, 'Migracion cancelada: no existe la tabla dbo.pedido. Nada se modifico.', 1;
    END

    -- Ya corrio en una pasada anterior.
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.customer') AND name = 'direccion_facturacion')
    BEGIN
        ;THROW 50003, 'Migracion cancelada: customer.direccion_facturacion ya existe, la migracion ya fue ejecutada. Nada se modifico.', 1;
    END

    -- ---- Columnas nuevas --------------------------------------------------
    -- nvarchar(400) como customer_address, para no truncar una direccion larga.
    -- NULL y sin DEFAULT: NULL significa "aun no la cargo el usuario".

    ALTER TABLE dbo.customer ADD direccion_facturacion nvarchar(400) NULL;
    ALTER TABLE dbo.customer ADD direccion_entrega     nvarchar(400) NULL;
    ALTER TABLE dbo.pedido  ADD direccion_facturacion nvarchar(400) NULL;

    -- ---- Backfill ---------------------------------------------------------
    --
    -- Van por sp_executesql a proposito. T-SQL compila el batch entero antes de
    -- ejecutarlo, asi que un UPDATE en texto plano que mencione las columnas nuevas no
    -- compila: el ALTER TABLE de arriba todavia no corrio cuando el compilador las resuelve.
    -- El SQL dinamico se compila en el momento de ejecutarse, cuando las columnas ya existen, y
    -- participa de la transaccion abierta salvo que se le pase una transaccion propia.
    --
    -- Solo desde Customer_Dir y solo cuando tenga algo: copiar un vacio a las columnas
    -- nuevas las marcaria como "definida pero en blanco", que no es lo mismo que
    -- "sin definir".

    EXEC sp_executesql N'
        UPDATE dbo.customer
        SET direccion_facturacion = Customer_Dir,
            direccion_entrega     = Customer_Dir
        WHERE NULLIF(LTRIM(RTRIM(Customer_Dir)), '''') IS NOT NULL;';

    -- El pedido historico solo tiene direccion_entrega. Se usa como base de la de
    -- facturacion para que el cuadro Bill To no aparezca vacio al revisar un pedido viejo.

    EXEC sp_executesql N'
        UPDATE dbo.pedido
        SET direccion_facturacion = direccion_entrega
        WHERE NULLIF(LTRIM(RTRIM(direccion_entrega)), '''') IS NOT NULL;';

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    -- @@TRANCOUNT con los dos signos arroba y ;THROW con punto y coma inicial: sin
    -- ellos el CATCH no compila y el batch entero no llega a ejecutarse.
    IF @@TRANCOUNT > 0
    BEGIN
        ROLLBACK TRANSACTION;
    END

    ;THROW;
END CATCH
GO

-- ============================================================================
-- VERIFICACION
-- ============================================================================
SET NOCOUNT ON;

SELECT 'clientes con facturacion cargada' AS revision, COUNT(*) AS informed
FROM dbo.customer WHERE direccion_facturacion IS NOT NULL;

SELECT 'clientes con entrega cargada' AS revision, COUNT(*) AS informed
FROM dbo.customer WHERE direccion_entrega IS NOT NULL;

SELECT 'clientes activos sin ninguna de las dos' AS revision, COUNT(*) AS sin_ninguna
FROM dbo.customer
WHERE anulado = 0
  AND direccion_facturacion IS NULL
  AND direccion_entrega IS NULL;

SELECT 'pedidos con facturacion heredada' AS revision, COUNT(*) AS informed
FROM dbo.pedido WHERE direccion_facturacion IS NOT NULL;
