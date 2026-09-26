-- ============================================================================
-- MIGRACION: numero de pedido de SO-#### a SO-##### (Ritrama2025)
-- ============================================================================
--
-- Que hace:
--   Amplia la parte numerica de 4 a 5 digitos. "SO-0037" pasa a "SO-00037".
--   Se aplica identica en pedido y pedido_detalle para que la FK siga emparejando.
--   La columna ya es varchar(20), asi que no hace falta cambiar el tipo: 8 caben de sobra.
--
-- Por que:
--   Con 4 digitos el contador se agota en el pedido 10000. PedidoNumero.Formatear lanza
--   excepcion a partir de ahi, asi que el aplicativo se caeria. Con 5 digitos hay margen
--   hasta 99999.
--
-- Idempotente en cuanto al resultado: si una pasada anterior ya lo hizo, el preflight
-- aborta con 50003 en vez de tocar datos.
--
-- IMPORTANTE: con la aplicacion cerrada. Development y Produccion apuntan a la misma
-- base real, asi que esto afecta datos de produccion. Respaldo del 2026-09-25 en
-- C:\RESPALDO DE BASES DE DATOS\RITRAMASQL2017_pre_so5digitos_20260925.bak
-- ============================================================================

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

-- ============================================================================
-- 1) TRANSACCION: preflight + conversion + recreacion de la FK
-- ============================================================================
BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID('dbo.pedido', 'U') IS NULL
    BEGIN
        ;THROW 50001, 'Migracion cancelada: no existe la tabla dbo.pedido. Nada se modifico.', 1;
    END

    IF OBJECT_ID('dbo.pedido_detalle', 'U') IS NULL
    BEGIN
        ;THROW 50002, 'Migracion cancelada: no existe la tabla dbo.pedido_detalle. Nada se modifico.', 1;
    END

    -- La migracion ya corrio: los numeros ya tienen cinco digitos.
    IF EXISTS (
        SELECT 1
        FROM dbo.pedido
        WHERE numero NOT LIKE 'SO-[0-9][0-9][0-9][0-9][0-9]'
          AND numero NOT LIKE 'SO-[0-9][0-9][0-9][0-9]'
    )
    BEGIN
        ;THROW 50003, 'Migracion cancelada: hay numeros de pedido que no son ni de 4 ni de 5 digitos. Revisar antes de correr. Nada se modifico.', 1;
    END

    -- Lineas de detalle sin pedido: la FK no las dejaria convertir.
    IF EXISTS (
        SELECT 1
        FROM dbo.pedido_detalle AS d
        WHERE NOT EXISTS (SELECT 1 FROM dbo.pedido AS p WHERE p.numero = d.numero)
    )
    BEGIN
        ;THROW 50005, 'Migracion cancelada: pedido_detalle tiene lineas sin pedido correspondiente. Nada se modifico.', 1;
    END

    -- ---- Conversion: SO-#### -> SO-##### -----------------------------------
    --
    -- RIGHT('00000' + '0037', 5) = '00037'. El prefijo se reconstruye en vez de
    -- recortar la cadena, para que el resultado no dependa de como vino el dato.

    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_PEDIDO_DETALLE_PEDIDO')
    BEGIN
        ALTER TABLE dbo.pedido_detalle DROP CONSTRAINT FK_PEDIDO_DETALLE_PEDIDO;
    END

    UPDATE dbo.pedido
    SET numero = 'SO-' + RIGHT('00000' + REPLACE(numero, 'SO-', ''), 5)
    WHERE numero LIKE 'SO-[0-9][0-9][0-9][0-9]';

    UPDATE dbo.pedido_detalle
    SET numero = 'SO-' + RIGHT('00000' + REPLACE(numero, 'SO-', ''), 5)
    WHERE numero LIKE 'SO-[0-9][0-9][0-9][0-9]';

    -- ---- Recrear la FK, validando los datos -------------------------------
    --
    -- WITH CHECK obliga a SQL Server a comprobar que no quedo ninguna linea
    -- apuntando a un pedido inexistente.

    ALTER TABLE dbo.pedido_detalle WITH CHECK
        ADD CONSTRAINT FK_PEDIDO_DETALLE_PEDIDO
        FOREIGN KEY (numero) REFERENCES dbo.pedido (numero);

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    -- @@TRANCOUNT lleva los dos signos arroba y el THROW el punto y coma inicial:
    -- sin ellos el CATCH no compila y el batch entero no llega a ejecutarse.
    IF @@TRANCOUNT > 0
    BEGIN
        ROLLBACK TRANSACTION;
    END

    ;THROW;
END CATCH
GO

-- ============================================================================
-- 2) VERIFICACION (fuera de la transaccion)
--    Las tres primeras deben salir en 0.
-- ============================================================================
SET NOCOUNT ON;

SELECT 'pedidos que NO son de 5 digitos' AS revision, COUNT(*) AS fuera
FROM dbo.pedido
WHERE numero NOT LIKE 'SO-[0-9][0-9][0-9][0-9][0-9]';

SELECT 'detalle que NO es de 5 digitos' AS revision, COUNT(*) AS fuera
FROM dbo.pedido_detalle
WHERE numero NOT LIKE 'SO-[0-9][0-9][0-9][0-9][0-9]';

SELECT 'lineas huerfanas despues del cambio' AS revision, COUNT(*) AS huerfanas
FROM dbo.pedido_detalle AS d
WHERE NOT EXISTS (SELECT 1 FROM dbo.pedido AS p WHERE p.numero = d.numero);

SELECT 'numeros duplicados' AS revision, COUNT(*) AS duplicados
FROM (SELECT numero FROM dbo.pedido GROUP BY numero HAVING COUNT(*) > 1) AS d;

SELECT numero, fecha, estado, customer_name
FROM dbo.pedido
ORDER BY numero;

-- El contador debe quedar consistente: control.par1 es el ultimo numero entregado.
SELECT 'control par1 (proximo a entregar)' AS revision, par1 AS valor
FROM dbo.control
WHERE filter = 'PED';
