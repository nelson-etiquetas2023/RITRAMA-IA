-- ============================================================
-- Productos: codigo ritrama (codigo unico de la compania,
-- lo introduce el usuario) + semillero del consecutivo.
-- Base de desarrollo: ritrama2026 (server 192.168.10.10).
-- Re-ejecutable (idempotente).
-- ============================================================
USE ritrama2026;
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

-- 2. Semillero del consecutivo de productos: par1=0 y el servicio
--    entrega par1+1 (OUTPUT INSERTED), asi la primera alta toma el 1.
IF NOT EXISTS (SELECT 1 FROM dbo.control WHERE filter = 'PROD')
    INSERT INTO dbo.control (filter, par1) VALUES ('PROD', 0);
ELSE
    UPDATE dbo.control SET par1 = 0 WHERE filter = 'PROD';
GO

PRINT 'producto.codigo_ritrama + consecutivo PROD listos.';
GO
