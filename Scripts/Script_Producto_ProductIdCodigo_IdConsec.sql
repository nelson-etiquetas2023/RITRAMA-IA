/*  Inversion de convencion del modulo Productos - ritrama2026 (Desarrollo)
    ======================================================================
    ANTES:  product_id  = consecutivo del sistema (1, 2, 3...) y
            codigo_ritrama = codigo de empresa tecleado por el usuario.
    AHORA:  product_id  = codigo de empresa (como en Produccion, para que
            las consultas de la app -inventario, OC, reportes- no cambien) y
            IdConsec    = consecutivo del sistema, solo referencial.

    El catalogo de ritrama2026 trae ademas un duplicado: el producto 00753
    existe dos veces (una fila creada por el alta automatica de "productos no
    encontrados" al importar los iniciales, sin precio ni costo, y otra con el
    consecutivo 1). Se conserva la fila que trae precio/costo/codigo de barra
    y se elimina la otra antes de pasarle el codigo.

    El semillero control.PROD queda en el maximo IdConsec, de modo que la
    proxima alta entrega max + 1.

    Precondicion comprobada antes de ejecutar: ninguna tabla hija (MasterInic,
    orden_corte, rolls_details, pedido_detalle, ...) referencia todavia los
    product_id numericos, asi que renombrarlos no deja filas huerfanas.
*/
SET NOCOUNT ON;
GO

-- 1) Columna nueva, todavia nullable para poder poblarla.
IF COL_LENGTH('dbo.producto', 'IdConsec') IS NULL
    ALTER TABLE dbo.producto ADD IdConsec INT NULL;
GO

-- 2) El duplicado creado por el alta automatica ("productos no encontrados"
--    al importar los iniciales) se identifica porque no trae codigo de empresa:
--    no hay que darle consecutivo. OJO: no se detecta por "product_id no
--    numerico", porque un codigo como 00753 SI se convierte a entero (753) y
--    habia robado el consecutivo en la primera ejecucion de este script.
DELETE FROM dbo.producto WHERE codigo_ritrama IS NULL;
GO

-- 3) El consecutivo vigente es el product_id numerico que traia la fila.
UPDATE dbo.producto
SET IdConsec = TRY_CAST(product_id AS INT)
WHERE IdConsec IS NULL
  AND TRY_CAST(product_id AS INT) IS NOT NULL;
GO

-- 4) El codigo de empresa (antes codigo_ritrama) pasa a ser el product_id.
UPDATE dbo.producto
SET product_id = codigo_ritrama
WHERE codigo_ritrama IS NOT NULL
  AND product_id <> codigo_ritrama;
GO

-- 5) IdConsec pasa a ser obligatorio y unico.
ALTER TABLE dbo.producto ALTER COLUMN IdConsec INT NOT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = 'UQ_producto_idconsec' AND object_id = OBJECT_ID('dbo.producto'))
    CREATE UNIQUE INDEX UQ_producto_idconsec ON dbo.producto (IdConsec);
GO

-- 6) El codigo ya vive en product_id: la columna derivada y su indice sobran.
IF EXISTS (SELECT 1 FROM sys.indexes
           WHERE name = 'UX_producto_codigo_ritrama' AND object_id = OBJECT_ID('dbo.producto'))
    DROP INDEX UX_producto_codigo_ritrama ON dbo.producto;
GO

IF COL_LENGTH('dbo.producto', 'codigo_ritrama') IS NOT NULL
    ALTER TABLE dbo.producto DROP COLUMN codigo_ritrama;
GO

-- 7) Semillero: la proxima alta entrega el maximo consecutivo actual + 1.
UPDATE dbo.control
SET par1 = CONVERT(NVARCHAR(20), x.maximo)
FROM (SELECT MAX(IdConsec) AS maximo FROM dbo.producto) x
WHERE filter = 'PROD'
  AND ISNULL(CONVERT(INT, par1), 0) < ISNULL(x.maximo, 0);
GO

-- Verificacion final.
SELECT product_id, IdConsec, product_name FROM dbo.producto ORDER BY IdConsec;
SELECT filter, par1 FROM dbo.control WHERE filter = 'PROD';
PRINT 'ritrama2026: product_id = codigo de empresa + IdConsec consecutivo. Listo.';
GO
