/*  Inversion de convencion del modulo Productos - RITRAMA2025-TEST
    ===============================================================
    Igual que Script_Producto_ProductIdCodigo_IdConsec.sql pero para la base
    de pruebas, cuyo product_id ya ES el codigo de empresa (00439, 00504, ...)
    y cuya columna codigo_ritrama (añadida en la migracion anterior) esta
    totalmente vacia: aqui solo hace falta crear y poblar IdConsec, quitar la
    columna derivada y recolocar el semillero.
*/
SET NOCOUNT ON;
GO

-- 1) Columna nueva, todavia nullable.
IF COL_LENGTH('dbo.producto', 'IdConsec') IS NULL
    ALTER TABLE dbo.producto ADD IdConsec INT NULL;
GO

-- 2) Consecutivo correlativo 1..N (el product_id ya es el codigo, no sirve
--    como consecutivo).
UPDATE p
SET IdConsec = (SELECT COUNT(*) FROM dbo.producto b WHERE b.product_id <= p.product_id)
FROM dbo.producto p
WHERE p.IdConsec IS NULL;
GO

-- 3) Obligatorio y unico.
ALTER TABLE dbo.producto ALTER COLUMN IdConsec INT NOT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = 'UQ_producto_idconsec' AND object_id = OBJECT_ID('dbo.producto'))
    CREATE UNIQUE INDEX UQ_producto_idconsec ON dbo.producto (IdConsec);
GO

-- 4) La columna codigo_ritrama (vacia) y su indice se eliminan: el codigo ya
--    vive en product_id.
IF EXISTS (SELECT 1 FROM sys.indexes
           WHERE name = 'UX_producto_codigo_ritrama' AND object_id = OBJECT_ID('dbo.producto'))
    DROP INDEX UX_producto_codigo_ritrama ON dbo.producto;
GO

IF COL_LENGTH('dbo.producto', 'codigo_ritrama') IS NOT NULL
    ALTER TABLE dbo.producto DROP COLUMN codigo_ritrama;
GO

-- 5) Semillero PROD (estaba en 108950 por la convencion anterior): la proxima
--    alta entrega el maximo consecutivo actual + 1.
UPDATE dbo.control
SET par1 = CONVERT(NVARCHAR(20), x.maximo)
FROM (SELECT MAX(IdConsec) AS maximo FROM dbo.producto) x
WHERE filter = 'PROD'
  AND ISNULL(CONVERT(INT, par1), 0) < ISNULL(x.maximo, 0);
GO

-- Verificacion final.
SELECT COUNT(*) AS productos, MIN(IdConsec) AS min_idconsec, MAX(IdConsec) AS max_idconsec FROM dbo.producto;
SELECT filter, par1 FROM dbo.control WHERE filter = 'PROD';
PRINT 'RITRAMA2025-TEST: product_id = codigo de empresa + IdConsec consecutivo. Listo.';
GO
