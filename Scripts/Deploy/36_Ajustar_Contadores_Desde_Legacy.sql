-- ============================================================================
-- 36_Ajustar_Contadores_Desde_Legacy.sql (Ritrama2026)
-- Que hace: deja control.par1 en el ULTIMO numero real de la BD anterior para
--   OC/PED/COC/UC/CMP, asi la numeracion continua sin chocar con documentos
--   impresos. Nunca retrocede: solo sube si el legacy trae un maximo mayor.
-- Fuentes (verificadas en codigo):
--   OC  = parte numerica de orden_compra.numero 'OC-00027' (OrdenesCompraNumero.cs:25)
--   PED = parte numerica de pedido.numero 'SO-00037' (sirve INT legacy y varchar actual)
--   COC = orden_corte.numero en texto (OrdenCorteService.cs:717 numero.ToString())
--   UC  = maximo de unique_code 'RC%' en rolls_details + RollsInic
--         (misma formula de OrdenCorteService.cs:1460-1468)
--   CMP = OrdenMateria.numero (recepciones, ServiceMateriaPrima.cs:170)
-- Uso (LOCAL, despues del 35):
--   sqlcmd -S RITRAMASRV01 -d ritrama2026 -E -C -b ^
--     -v SourceDB="RITRAMASQL2017" -i Scripts\Deploy\36_Ajustar_Contadores_Desde_Legacy.sql
-- Re-ejecutable. Si una tabla legacy no existe, avisa y deja el contador como esta.
-- ============================================================================

:setvar SourceDB "RITRAMASQL2017"

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

-- ---- OC: orden_compra 'OC-00027' -------------------------------------------
IF OBJECT_ID(N'[$(SourceDB)].dbo.orden_compra', N'U') IS NOT NULL
BEGIN
    DECLARE @oc INT = ISNULL((SELECT MAX(TRY_CAST(REPLACE(numero, N'OC-', N'') AS INT))
        FROM [$(SourceDB)].dbo.orden_compra WHERE numero LIKE N'OC-%'), 0);
    UPDATE dbo.control
    SET par1 = CAST(@oc AS NVARCHAR(100))
    WHERE filter = N'OC' AND ISNULL(TRY_CAST(par1 AS INT), -1) < @oc;
    PRINT 'OC ajustado a ultimo real: ' + CAST(@oc AS NVARCHAR(20));
END
ELSE
    PRINT 'AVISO: sin orden_compra en legacy, OC queda como esta.';
GO

-- ---- PED: pedido 'SO-00037' (o INT en legacy vieja) -------------------------
IF OBJECT_ID(N'[$(SourceDB)].dbo.pedido', N'U') IS NOT NULL
BEGIN
    DECLARE @ped INT = ISNULL((SELECT MAX(TRY_CAST(REPLACE(numero, N'SO-', N'') AS INT))
        FROM [$(SourceDB)].dbo.pedido WHERE numero LIKE N'SO-%'), 0);
    IF @ped = 0
        SET @ped = ISNULL((SELECT MAX(TRY_CAST(numero AS INT)) FROM [$(SourceDB)].dbo.pedido), 0);
    UPDATE dbo.control
    SET par1 = CAST(@ped AS NVARCHAR(100))
    WHERE filter = N'PED' AND ISNULL(TRY_CAST(par1 AS INT), -1) < @ped;
    PRINT 'PED ajustado a ultimo real: ' + CAST(@ped AS NVARCHAR(20));
END
ELSE
    PRINT 'AVISO: sin pedido en legacy, PED queda como esta.';
GO

-- ---- COC: orden_corte.numero (texto con entero) ------------------------------
IF OBJECT_ID(N'[$(SourceDB)].dbo.orden_corte', N'U') IS NOT NULL
BEGIN
    DECLARE @coc INT = ISNULL((SELECT MAX(TRY_CAST(numero AS INT))
        FROM [$(SourceDB)].dbo.orden_corte), 0);
    UPDATE dbo.control
    SET par1 = CAST(@coc AS NVARCHAR(100))
    WHERE filter = N'COC' AND ISNULL(TRY_CAST(par1 AS INT), -1) < @coc;
    PRINT 'COC ajustado a ultimo real: ' + CAST(@coc AS NVARCHAR(20));
END
ELSE
    PRINT 'AVISO: sin orden_corte en legacy, COC queda como esta.';
GO

-- ---- UC: unique_code 'RC%' (misma formula de la app) --------------------------
DECLARE @uc INT = 0;
IF OBJECT_ID(N'[$(SourceDB)].dbo.rolls_details', N'U') IS NOT NULL
    SET @uc = ISNULL((SELECT MAX(TRY_CONVERT(int, SUBSTRING(unique_code, 3, LEN(unique_code))))
        FROM [$(SourceDB)].dbo.rolls_details WHERE unique_code LIKE N'RC%'), 0);
IF OBJECT_ID(N'[$(SourceDB)].dbo.RollsInic', N'U') IS NOT NULL
    SET @uc = CASE WHEN (SELECT ISNULL(MAX(TRY_CONVERT(int, SUBSTRING(unique_code, 3, LEN(unique_code)))), 0)
        FROM [$(SourceDB)].dbo.RollsInic WHERE unique_code LIKE N'RC%') > @uc
        THEN (SELECT ISNULL(MAX(TRY_CONVERT(int, SUBSTRING(unique_code, 3, LEN(unique_code)))), 0)
        FROM [$(SourceDB)].dbo.RollsInic WHERE unique_code LIKE N'RC%') ELSE @uc END;
IF @uc > 0
BEGIN
    UPDATE dbo.control
    SET par1 = CAST(@uc AS NVARCHAR(100))
    WHERE filter = N'UC' AND ISNULL(TRY_CAST(par1 AS INT), -1) < @uc;
    PRINT 'UC ajustado a ultimo real: ' + CAST(@uc AS NVARCHAR(20));
END
ELSE
    PRINT 'AVISO: sin unique_code RC% en legacy, UC queda como esta.';
GO

-- ---- CMP: OrdenMateria.numero (recepciones) ------------------------------------
IF OBJECT_ID(N'[$(SourceDB)].dbo.OrdenMateria', N'U') IS NOT NULL
BEGIN
    DECLARE @cmp INT = ISNULL((SELECT MAX(TRY_CAST(numero AS INT))
        FROM [$(SourceDB)].dbo.OrdenMateria), 0);
    UPDATE dbo.control
    SET par1 = CAST(@cmp AS NVARCHAR(100))
    WHERE filter = N'CMP' AND ISNULL(TRY_CAST(par1 AS INT), -1) < @cmp;
    PRINT 'CMP ajustado a ultimo real: ' + CAST(@cmp AS NVARCHAR(20));
END
ELSE
    PRINT 'AVISO: sin OrdenMateria en legacy, CMP queda como esta.';
GO

SELECT filter AS contador, par1 AS ultimo_entregado FROM dbo.control
WHERE filter IN (N'OC', N'PED', N'COC', N'UC', N'CMP', N'CLI', N'PROV', N'VEND', N'PROD')
ORDER BY filter;
GO

PRINT '=== 36_Ajustar_Contadores completado. El proximo documento toma ultimo + 1. ===';
GO
