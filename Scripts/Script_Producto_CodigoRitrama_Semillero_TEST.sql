-- ============================================================
-- Productos: codigo ritrama + semillero del consecutivo.
-- Base de TESTS: RITRAMA2025-TEST (server 192.168.10.10).
-- Re-ejecutable (idempotente).
--
-- Igual que Script_Producto_CodigoRitrama_Semillero.sql pero
-- SIN poner par1 a 0: esta base ya tiene productos con datos,
-- y el consecutivo empieza en 1 solo en una base vacia. Aqui el
-- semillero se lleva al maximo Product_id para que la siguiente
-- alta no choque con la clave primaria.
-- ============================================================
USE [RITRAMA2025-TEST];
GO

-- 1. Columna: texto libre tecleado por el usuario, NULL en filas
--    legadas. El indice UNICO filtrado permite varios NULL (legados)
--    pero nunca dos codigos iguales con valor.
IF COL_LENGTH('dbo.producto', 'codigo_ritrama') IS NULL
    ALTER TABLE dbo.producto ADD codigo_ritrama nvarchar(50) NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_producto_codigo_ritrama')
    CREATE UNIQUE INDEX UX_producto_codigo_ritrama
        ON dbo.producto (codigo_ritrama)
        WHERE codigo_ritrama IS NOT NULL;
GO

-- 2. Semillero del consecutivo de productos: nunca por debajo del
--    mayor Product_id numerico existente.
DECLARE @maximo int =
    (SELECT ISNULL(MAX(TRY_CAST(Product_id AS INT)), 0) FROM dbo.producto);

IF NOT EXISTS (SELECT 1 FROM dbo.control WHERE filter = 'PROD')
    INSERT INTO dbo.control (filter, par1) VALUES ('PROD', @maximo);
ELSE IF TRY_CAST((SELECT par1 FROM dbo.control WHERE filter = 'PROD') AS int) < @maximo
    UPDATE dbo.control SET par1 = @maximo WHERE filter = 'PROD';
GO

PRINT 'RITRAMA2025-TEST: producto.codigo_ritrama + semillero PROD listos.';
GO
