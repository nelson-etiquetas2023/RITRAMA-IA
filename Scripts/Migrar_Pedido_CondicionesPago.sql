-- ============================================================================
-- MIGRACION: remapear pedido.condiciones_pago al vocabulario por plazo
-- ============================================================================
--
-- Que hace:
--   Los plazos dejan de escribirse "neto 30" y pasan a "30 dias", que es como
--   los ofrece la pantalla. Ademas reparte los valores que no son plazos:
--
--     neto 30    ->  30 dias
--     neto 45    ->  45 dias
--     neto 60    ->  60 dias
--     anticipo   ->  contado     (un anticipo se paga por adelantado)
--     contado    ->  sin cambio
--     credito    ->  sin cambio
--
-- Por que importa mas alla del nombre: PedidoCatalogos.CondicionesPago es la
-- unica lista que la pantalla ofrece, y AsignarComboOpcional no inventa una
-- seleccion si el valor no esta en ella. Sin este remapeo, 17 pedidos se
-- mostrarian con el combo vacio aunque la base si tenga el dato, y al volver
-- a guardarlos ese valor se perderia.
--
-- Rollback: es un UPDATE de texto sobre una sola columna, reversible con el
-- UPDATE inverso si se hace antes de que la aplicacion escriba filas nuevas.
-- Por eso conviene tomarlo con la aplicacion cerrada. Aun asi, el respaldo del
-- 2026-09-25 quedo tomado en C:\RESPALDO DE BASES DE DATOS\
-- ============================================================================

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

-- ============================================================================
-- 1) TRANSACCION: preflight + remapeo
--
-- El preflight aborta sin tocar nada si aparece un valor que este script no
-- conoce: seria un dato que se perderia en silencio al no tener a donde ir.
-- ============================================================================
BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID('dbo.pedido', 'U') IS NULL
    BEGIN
        ;THROW 50001, 'Migracion cancelada: no existe la tabla dbo.pedido. Nada se modifico.', 1;
    END

    IF EXISTS (
        SELECT 1
        FROM dbo.pedido
        WHERE condiciones_pago IS NOT NULL
          AND condiciones_pago COLLATE Latin1_General_BIN
              NOT IN ('contado', 'credito', 'neto 30', 'neto 45', 'neto 60', 'anticipo')
    )
    BEGIN
        ;THROW 50002, 'Migracion cancelada: hay condiciones de pago con un valor que este script no reconoce. Revisar la lista del paso 1 antes de correrlo. Nada se modifico.', 1;
    END

    -- Plazos: "neto N" pasa a "N dias".
    UPDATE dbo.pedido
    SET condiciones_pago = REPLACE(condiciones_pago, 'neto ', '') + ' dias'
    WHERE condiciones_pago LIKE 'neto %';

    -- Un anticipo es un pago por adelantado: es contado.
    UPDATE dbo.pedido
    SET condiciones_pago = 'contado'
    WHERE condiciones_pago = 'anticipo';

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    -- @@TRANCOUNT lleva los dos signos arroba y el THROW lleva punto y coma inicial:
    -- sin ninguno de los dos, el CATCH no compila y el batch entero no llega a ejecutarse.
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

SELECT ISNULL(condiciones_pago, '(null)') AS condicion, COUNT(*) AS pedidos
FROM dbo.pedido
GROUP BY condiciones_pago
ORDER BY pedidos DESC;

-- Debe dar 0. Si no, hay un valor fuera del catalogo y la pantalla lo
-- mostraria vacio.
SELECT 'valores fuera del catalogo' AS revision, COUNT(*) AS fuera
FROM dbo.pedido
WHERE condiciones_pago IS NOT NULL
  AND condiciones_pago NOT IN ('contado', '7 dias', '15 dias', '30 dias', '45 dias', '60 dias', 'credito');
