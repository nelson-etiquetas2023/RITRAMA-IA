-- ============================================================================
-- MIGRACION: agregar pedido.prioridad (Ritrama2025)
-- ============================================================================
--
-- Que hace:
--   Agrega la columna dbo.pedido.prioridad, nullable, para distinguir pedidos
--   urgentes de los normales. No reescribe filas existentes: los pedidos
--   actuales quedan con NULL, que la aplicacion interpreta como "normal".
--
-- Es segura de volver a correr: si la columna ya existe, no hace nada.
-- No necesita respaldo previo (agregar una columna nullable no toca datos),
-- pero el respaldo del 2026-09-25 igual quedo tomado por si acaso.
--
-- Nota: la columna va sin DEFAULT a proposito. Con DEFAULT, todo INSERT
-- futuro que la omita grabaria una fila y la aplicacion no podria distinguir
-- "el usuario no eligio" de "eligio normal". NULL significa "no informado".
-- ============================================================================

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

-- ============================================================================
-- 1) TRANSACCION: agregar la columna si no existe
-- ============================================================================
BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID('dbo.pedido', 'U') IS NULL
    BEGIN
        ;THROW 50001, 'Migracion cancelada: no existe la tabla dbo.pedido. Nada se modifico.', 1;
    END

    -- Agregar una columna es metadata pura: no reescribe filas ni bloquea
    -- lecturas, asi que alcanza con comprobar que no exista ya.
    IF NOT EXISTS (
        SELECT 1
        FROM sys.columns
        WHERE object_id = OBJECT_ID('dbo.pedido')
          AND name = 'prioridad'
    )
    BEGIN
        ALTER TABLE dbo.pedido ADD prioridad nvarchar(20) NULL;
    END

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    -- @@TRANCOUNT lleva los dos signos arroba: sin ellos T-SQL lo toma como
    -- nombre de columna y el batch no compila. El ; inicial de THROW tambien
    -- es obligatorio, porque la sentencia anterior es un END de bloque.
    IF @@TRANCOUNT > 0
    BEGIN
        ROLLBACK TRANSACTION;
    END

    ;THROW;
END CATCH
GO

-- ============================================================================
-- 2) VERIFICACION (fuera de la transaccion)
-- ============================================================================
SET NOCOUNT ON;

SELECT 'columna prioridad' AS revision,
       CASE WHEN EXISTS (
           SELECT 1 FROM sys.columns
           WHERE object_id = OBJECT_ID('dbo.pedido') AND name = 'prioridad'
       ) THEN 'OK' ELSE 'FALTA' END AS resultado;

SELECT 'pedidos con prioridad informada' AS revision, COUNT(*) AS informed
FROM dbo.pedido
WHERE prioridad IS NOT NULL;

SELECT 'pedidos con NULL (interpretado como normal)' AS revision, COUNT(*) AS sin_informar
FROM dbo.pedido
WHERE prioridad IS NULL;
