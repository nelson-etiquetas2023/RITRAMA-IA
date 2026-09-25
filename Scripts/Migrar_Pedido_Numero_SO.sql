-- ============================================================================
-- MIGRACION: pedido.numero de INT a texto con formato SO-#### (Ritrama2025)
-- ============================================================================
--
-- IMPORTANTE: este script NO debe ejecutarse sin respaldo previo y con la
-- aplicacion cerrada. Convers Development y Produccion apuntan a la misma
-- base real, asi que un doble clic aqui afecta datos de produccion.
--
-- Que hace, en orden:
--   1. Valida que se pueda correr (preflight). Cualquier fallo aborta sin tocar nada.
--   2. Convierte pedido_detalle.numero a varchar(20) para poder soltar la FK.
--   3. Convierte pedido.numero a varchar(20).
--   4. Rellena ambos con 'SO-' + 4 digitos, de forma identica en las dos tablas
--      para que la FK siga emparejando.
--   5. Recrea la FK.
--   6. Muestra el resultado para verificacion visual.
--
-- Rollback: la conversion a texto es irreversible en el sentido de que SQL Server
-- no recuerda de que tipo era la columna. Si algo sale mal, restaurar el respaldo.
-- ============================================================================

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

-- ============================================================================
-- 1) TRANSACCION: preflight + conversion + backfill + FK (pasos 1 a 5)
--    Paso 1 y 2-5: todo dentro de una transaccion. Si el preflight falla o algo
--    revienta en medio, el ROLLBACK del CATCH deja la base exactamente como estaba.
--    Los THROW del preflight usan THROW y no RAISERROR, asi que siempre caen en
--    el CATCH de abajo: la transaccion se revierte y sale el numero 50001..50006.
-- ============================================================================
BEGIN TRY
    BEGIN TRANSACTION;

    -- ---- Preflight ------------------------------------------------------
    -- Ojo: T-SQL no tiene llaves. Los bloques van con BEGIN ... END.

    IF OBJECT_ID('dbo.pedido', 'U') IS NULL
    BEGIN
        THROW 50001, 'Migracion cancelada: no existe la tabla dbo.pedido. Nada se modifico.', 1;
    END

    IF OBJECT_ID('dbo.pedido_detalle', 'U') IS NULL
    BEGIN
        THROW 50002, 'Migracion cancelada: no existe la tabla dbo.pedido_detalle. Nada se modifico.', 1;
    END

    -- La columna ya no es INT: el script ya corrio en una pasada anterior.
    IF EXISTS (
        SELECT 1
        FROM sys.columns AS c
        INNER JOIN sys.types AS t ON c.user_type_id = t.user_type_id
        WHERE c.object_id = OBJECT_ID('dbo.pedido')
          AND c.name = 'numero'
          AND t.name <> 'int'
    )
    BEGIN
        THROW 50003, 'Migracion cancelada: pedido.numero ya no es INT, la migracion ya fue ejecutada. Nada se modifico.', 1;
    END

    -- El relleno a 4 digitos trunaria un numero de 5 o mas.
    IF EXISTS (SELECT 1 FROM dbo.pedido WHERE numero > 9999)
    BEGIN
        THROW 50004, 'Migracion cancelada: hay un pedido con numero mayor que 9999 y el relleno lo truncaria. Nada se modifico.', 1;
    END

    -- Un numero 0 o negativo se volveria SO-0000, que es un punto fijo del backfill:
    -- pasaria la verificacion LIKE, quedaria confirmado, y PedidoNumero.Formatear(0)
    -- lanza ArgumentOutOfRangeException. Se aborta antes de crear ese dato.
    IF EXISTS (SELECT 1 FROM dbo.pedido WHERE numero < 1)
    BEGIN
        THROW 50006, 'Migracion cancelada: hay un pedido con numero menor que 1 y el relleno produciria SO-0000, que la aplicacion no puede volver a formatear. Nada se modifico.', 1;
    END

    -- Lineas de detalle sin pedido: la FK no las dejaria convertir.
    IF EXISTS (
        SELECT 1
        FROM dbo.pedido_detalle AS d
        WHERE NOT EXISTS (SELECT 1 FROM dbo.pedido AS p WHERE p.numero = d.numero)
    )
    BEGIN
        THROW 50005, 'Migracion cancelada: pedido_detalle tiene lineas sin pedido correspondiente. Nada se modifico.', 1;
    END

    -- ---- Paso 2: convertir la hija para poder soltar la FK --------------
    --
    -- ALTER COLUMN falla (error 5074/4922) si algun indice sigue apoiado en la
    -- columna. Por eso se sueltan la FK y los indices ANTES de convertir, y se
    -- vuelven a crear en el paso 5 con las mismas columnas y opciones.

    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_PEDIDO_DETALLE_PEDIDO')
    BEGIN
        ALTER TABLE dbo.pedido_detalle DROP CONSTRAINT FK_PEDIDO_DETALLE_PEDIDO;
    END

    IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_pedido_detalle_numero' AND object_id = OBJECT_ID('dbo.pedido_detalle'))
    BEGIN
        DROP INDEX IX_pedido_detalle_numero ON dbo.pedido_detalle;
    END

    ALTER TABLE dbo.pedido_detalle ALTER COLUMN numero varchar(20) NOT NULL;

    -- ---- Paso 3: convertir la tabla padre -------------------------------
    --
    -- La padre tiene dos objetos sobre numero: el indice no unico
    -- IX_pedido_numero y la restriccion unica UQ_pedido_numero. Se sueltan los
    -- dos, incluido el UNIQUE, que tambien bloquea el ALTER.

    IF EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = 'UQ_pedido_numero')
    BEGIN
        ALTER TABLE dbo.pedido DROP CONSTRAINT UQ_pedido_numero;
    END

    IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_pedido_numero' AND object_id = OBJECT_ID('dbo.pedido'))
    BEGIN
        DROP INDEX IX_pedido_numero ON dbo.pedido;
    END

    ALTER TABLE dbo.pedido ALTER COLUMN numero varchar(20) NOT NULL;

    -- ---- Paso 4: backfill identico en las dos tablas --------------------

    UPDATE dbo.pedido
    SET numero = 'SO-' + RIGHT('0000' + CAST(numero AS varchar(10)), 4);

    UPDATE dbo.pedido_detalle
    SET numero = 'SO-' + RIGHT('0000' + CAST(numero AS varchar(10)), 4);

    -- ---- Paso 5: recrear UNIQUE, FK e indices ---------------------------
    --
    -- El orden importa: una clave externa necesita que la columna referenciada
    -- tenga PK o UNIQUE. Por eso UQ_pedido_numero se recrea ANTES que la FK;
    -- al reves, SQL Server responde 1776 "no hay claves principales ni
    -- candidatas en la tabla a la que se hace referencia".
    -- La FK se crea WITH CHECK para que valide que ningun numero del detalle
    -- quedo sin su pedido. Los indices no unicos se recrean al final, igual que
    -- estaban: una sola columna numero, ascendente, sin filtro ni incluidas.

    IF NOT EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = 'UQ_pedido_numero')
    BEGIN
        ALTER TABLE dbo.pedido ADD CONSTRAINT UQ_pedido_numero UNIQUE (numero);
    END

    ALTER TABLE dbo.pedido_detalle WITH CHECK
        ADD CONSTRAINT FK_PEDIDO_DETALLE_PEDIDO
        FOREIGN KEY (numero) REFERENCES dbo.pedido (numero);

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_pedido_numero' AND object_id = OBJECT_ID('dbo.pedido'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_pedido_numero ON dbo.pedido (numero ASC);
    END

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_pedido_detalle_numero' AND object_id = OBJECT_ID('dbo.pedido_detalle'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_pedido_detalle_numero ON dbo.pedido_detalle (numero ASC);
    END

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    -- @@TRANCOUNT: los dos signs arroba son obligatorios. Sin ellos T-SQL lo
    -- interpreta como un nombre de columna y el batch entero no compila.
    IF @@TRANCOUNT > 0
    BEGIN
        ROLLBACK TRANSACTION;
    END

    -- El THROW del preflight y los errores del CATCH salen por aqui.
    -- El punto y coma inicial es obligatorio: la sentencia anterior es el END
    -- del bloque IF, que no cuenta como sentencia terminada. Sin el, T-SQL
    -- responde "Sintaxis incorrecta cerca de 'THROW'" y el CATCH no compila.
    ;THROW;
END CATCH
GO

-- ============================================================================
-- 6) VERIFICACION: consultas de control, fuera de la transaccion (paso 6)
--    Las cuatro primeras deben salir en 0. Solo tienen sentido si el bloque
--    anterior llego al COMMIT: si ahi habia error, se reporto y estos SELECT
--    estan midiendo una base que no se migro.
-- ============================================================================
SET NOCOUNT ON;

SELECT 'pedidos sin formato SO-####' AS revision, COUNT(*) AS fuera_de_rango
FROM dbo.pedido
WHERE numero NOT LIKE 'SO-[0-9][0-9][0-9][0-9]'
    OR numero IS NULL;

SELECT 'detalle sin formato SO-####' AS revision, COUNT(*) AS fuera_de_rango
FROM dbo.pedido_detalle
WHERE numero NOT LIKE 'SO-[0-9][0-9][0-9][0-9]'
    OR numero IS NULL;

SELECT 'lineas huerfanas despues del backfill' AS revision, COUNT(*) AS huerfanas
FROM dbo.pedido_detalle AS d
WHERE NOT EXISTS (SELECT 1 FROM dbo.pedido AS p WHERE p.numero = d.numero);

SELECT 'numeros duplicados' AS revision, COUNT(*) AS duplicados
FROM (
    SELECT numero FROM dbo.pedido GROUP BY numero HAVING COUNT(*) > 1
) AS d;

SELECT numero, fecha, estado, customer_name
FROM dbo.pedido
ORDER BY numero;

-- ============================================================================
-- NOTA SOBRE OTRAS TABLAS
-- orden_corte.pedido_id y despacho.pedido_id son INT NULL y hoy estan vacias.
-- NO se convierten en este script. Cuando se conecten a Pedido, harvested bajo
-- un ticket aparte con su propia FK.
-- ============================================================================
