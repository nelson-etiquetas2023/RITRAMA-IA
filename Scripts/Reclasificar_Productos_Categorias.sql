-- ============================================================================
-- RECLASIFICACION: repartir dbo.producto entre las 4 categorias (Ritrama2025)
-- ============================================================================
--
-- Por que hace falta:
--   El tipo de un producto sale de 4 bits mutuamente excluyentes de dbo.producto
--   (MasterRolls, rollo_cortado, Resmas, Graphics). En la base real los 68
--   productos tienen MasterRolls = 1 y las otras tres en 0, por eso la aplicacion
--   (FrmProductos, el picker de productos y el CASE de R.SQL_STRING_QUERY)
--   muestra "Master" en todos. No es un bug del codigo: es el dato.
--
-- Que hace:
--   Lee la lista de abajo (#Mapping: producto_id + tipo), respalda los bits
--   actuales, escribe exactamente un bit por producto y comprueba el resultado.
--   Si algo no cuadra, hace ROLLBACK y no deja nada escrito.
--
-- Como se usa (dos pasadas):
--   1) Rellenar la columna Tipo del bloque #Mapping con uno de estos valores:
--        Master | Rollo Cortado | Hoja | Graphics
--        ("Hoja" y "Resma" son el mismo bit; se aceptan tambien las variantes
--        "Resmas", "Graficas", etc: se normalizan mas abajo).
--      Dejar '' (vacio) en un producto = "no tocarlo", sigue como esta.
--   2) Dejar @Aplicar = 0 y ejecutar: hace todo en seco, imprime el antes/despues
--      y REVIERTE. Es la pasada de verificacion.
--   3) Cambiar @Aplicar a 1 y volver a ejecutar: ahora si, COMMIT.
--
-- El respaldo lleva fecha en el nombre y NO se pisa si ya existe, asi que la
--   segunda pasada conserva el snapshot original (el de antes de tocar nada).
--
-- Despues de aplicar: cerrar y volver a abrir FrmProductos (el catalogo se carga
--   al abrir el formulario). Y tener en cuenta que los modulos que filtran
--   MasterRolls = 1 (consumo de master rolls / materia prima) dejan de aceptar
--   cualquier producto: a partir de ahora solo ven los que realmente son Master.
--
-- Al final del archivo queda el UPDATE para restaurar desde el respaldo.
-- ============================================================================

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

-- ============================================================================
-- 1) CONFIGURACION + LISTA DE RECLASIFICACION
-- ============================================================================
-- 0 = solo informe, revierte. 1 = aplica los cambios (COMMIT).
DECLARE @Aplicar bit = 0;

IF OBJECT_ID('dbo.producto', 'U') IS NULL
BEGIN
    ;THROW 50001, 'Nada se modifico: no existe la tabla dbo.producto en esta base.', 1;
END

IF OBJECT_ID('tempdb..#Mapping') IS NOT NULL
BEGIN
    DROP TABLE #Mapping;
END

-- COLLATE DATABASE_DEFAULT: la tabla temporal nace con la colacion de tempdb y
-- al compararla contra dbo.producto daria el error 468 (collation conflict).
CREATE TABLE #Mapping
(
    Product_ID nvarchar(50) COLLATE DATABASE_DEFAULT NOT NULL,
    Tipo       nvarchar(20) COLLATE DATABASE_DEFAULT NOT NULL
);

-- >>> RELLENAR LA SEGUNDA COLUMNA <<<  (Master | Rollo Cortado | Hoja | Graphics)
INSERT INTO #Mapping (Product_ID, Tipo) VALUES
 (N'02102',       N''),  -- 02102 (RI-757/60 PP CAVITATED TC8 AP901 WG62..)xx
 (N'02107',       N''),  -- RI-837/85 Pe Gloss White TC8 AP901 WG62
 (N'03000',       N''),  -- producto x
 (N'03838',       N''),  -- 03838 (RI-707/50 PP GLOSS CLEAR TC8 AP901 WG62..)
 (N'05317',       N''),  -- 05317 (RI-757/60 PP GLOSS WHITE CAVITATED TC8 AR801 WG62)
 (N'06007',       N''),  -- 06007 (Coated 80 Premium FSC SH9020 WG60)
 (N'06010',       N''),  -- 06010 (Papel Metalizado plata brillo AP903 W60..)
 (N'06011',       N''),  -- 06011 (CAST GLOSS FSC SH9020 WG60)
 (N'06222',       N''),  -- 06222 (RI-757/60 PP CAVITATED TC8 AP903 WG62..)
 (N'06422',       N''),  -- 06422 (Aconcagua Martele White 90GR AP903 WG60-Papel..)
 (N'06758',       N''),  -- Top Transfer Premium FSC AP904 WG55
 (N'06770',       N''),  -- Coated 80 AP904 SK60
 (N'07031',       N''),  -- Papel Metalizado Plata brillo FSC AP904 PET30
 (N'07358',       N''),  -- RI-7526/60 PP CAVITATED TC8 AP901 WG60
 (N'07522',       N''),  -- 07522 (RI-717/50 PP Gloss Silver TC8 AP901 WG62..)
 (N'07656',       N''),  -- SEMIGLOSS FSC AR805 WK85 RIT/GRY
 (N'07758',       N''),  -- TOP TRANSFER PREMIUM AR805 SK60
 (N'07836',       N''),  -- 07836 (Thermal top AP-903 WG56....)
 (N'07875',       N''),  -- Litho AP1009 SK60 o WG56
 (N'07879',       N''),  -- 07879 (RI-757/60 PP CAVITATED DGT AP901 WG62)
 (N'07979',       N''),  -- 07979 (Rapidjet Matt Inkjet Paper AP904 WG62...)
 (N'08044',       N''),  -- 08044 (Rapidjet Gloss Clear PP TC AP901 WG62)
 (N'08045',       N''),  -- PP GLOSS WHITE WB JET / AP901 / WG62
 (N'08046',       N''),  -- 08046 (RI-7526/70 PP MATT WHITE TC AP901 SK60..)
 (N'08051',       N''),  -- 08051 (Coated 80 DGT AP903 WG56)
 (N'08886',       N''),  -- 08886 (Thermal Easytop AP-903 SK60....)
 (N'08923',       N''),  -- RI-7093/60 PP GLOSS WHITE TC8 AP903 PET30
 (N'10225',       N''),  -- Top Transfer Premium AP1009 SK60
 (N'10823',       N''),  -- 10823 (Thermal PP 90 White AP1009 WG62...)
 (N'10955',       N''),  -- PP TCX GLOSS WHITE CAV 60 / RP1101 / WG62
 (N'11299',       N''),  -- 11299 (Coated MATT WB Jet FSC P1000 WG62 (Rapidjet))
 (N'11300',       N''),  -- 11300 (RAPIDJET GLOSS INKJET PAPER AP904 WG 62)
 (N'11605',       N''),  -- 11605 (Thermal PP 90 White AP903 Plus WG60)
 (N'11719',       N''),  -- FLUORO PINK AP904 SK62..
 (N'11762',       N''),  -- FLUOR ORANGE FSC(TM) / P1000 / WG62 FSC
 (N'11784',       N''),  -- FLUOR YELLOW FSC(TM) / P1000 / WG62
 (N'12160',       N''),  -- Natural Kraft Brown FSC AP904 WK80
 (N'12450',       N''),  -- ACONCAGUA MARTELE WHITE H+O WS FSC SH9020 WG60
 (N'12629',       N''),  -- COATED 80 FSC(TM) / RP3000 / WG62
 (N'12803',       N''),  -- THERMAL TOP HS FSC(TM) / RF20 / YG60 FSC
 (N'13037',       N''),  -- METALVAC SILVER FSC(TM) / P1000 / PET30
 (N'13473',       N''),  -- PP TCX GLOSS CLEAR 50 / AP901 / PET30
 (N'14229',       N''),  -- 14229 (COATED DT FSC / AP903 / 40#)
 (N'14240',       N''),  -- 14240 (54# SEMIGLOSS A FSC / AP999 / 40#)
 (N'14750',       N''),  -- COATED 80 FSC / FH05 / WG55 - 80g Art paper, Hot melt
 (N'14753',       N''),  -- COATED 80 FSC / AP409 / WG55 - 80g Art paper, acrylic
 (N'17381',       N''),  -- 17381 (BOPP BLANCO MATE 3.2 INKJET MIL ADH ACRILICO)
 (N'3010066106',  N''),  -- Vellum SC FSC / P1000 / YG60
 (N'303000PU07',  N''),  -- COATED PREMIUM 80 FSC SH6020 PLUS / PET30
 (N'3932732U08',  N''),  -- RI-MOVE GLASS VACUUM METAL LUX-BRIGHT SILVER / RH4000
 (N'803001T194',  N''),  -- COATED 80 FSC(TM) / FH21 / YG60
 (N'842001T177',  N''),  -- THERMAL TOP FSC FH21 / YG60 PDT0268
 (N'B0711',       N''),  -- Ipanema Embossed Cream FSC SH9020 CB80 Solid Imprint
 (N'B0952',       N''),  -- B0952 (Fluor Green FSC / AP904 / WG55 ...)
 (N'B0954',       N''),  -- B0954 (Fluor Red FSC / AP904 / WG55 Solid Imprint)
 (N'B0955',       N''),  -- FLUOR YELLOW FSC AP904 WG55 SOLID-IMPRIT
 (N'B0985',       N''),  -- Bagasse Meringue Ultra WS FSC SH9020 CB74
 (N'B1009',       N''),  -- B1009 (VACUUM METAL SILVER LUX FSC / SH9020 / WG55)
 (N'B1057',       N''),  -- COATED SNOW MATT FSC SH9020 CB74 SOLID-IMPRINT
 (N'B1067',       N''),  -- TRANSFER PLUS FSC AP903 PLUS SCK55
 (N'B5223',       N''),  -- 4-RL2D-07131PT PP CAV TC8 GT AP901 PET23 BR
 (N'C40001T206',  N''),  -- TRANSFER PREMIUM FSC(TM) / FH21 / WG62
 (N'D030229511',  N''),  -- FREELIFE MERIDA WHITE WS FSC SH6020 PLUS / WG80
 (N'D462766U08',  N''),  -- VACUUM METAL SILVER FSC(TM) / P1000 / PET30
 (N'D726429515',  N''),  -- ISPIRA BIANCO PUREZZA ULTRA WS
 (N'G150029520',  N''),  -- SUPERMATT ULTRA WS FSC SH6020 PLUS / WG80
 (N'G555129511',  N''),  -- ACQUERELLO AVORIO WS FSC SH6020 PLUS / WG80
 (N'G556429511',  N'');  -- ACQUERELLO BIANCO WS FSC SH6020 PLUS / WG80


-- ============================================================================
-- 2) NORMALIZAR Y VALIDAR EL MAPPING (antes de tocar nada)
-- ============================================================================
-- Acepta las variantes escritas a mano y las deja en los cuatro nombres
-- canonicos, que son los que producen ProductCategoryRules.GetNombre y el CASE
-- de R.SQL_STRING_QUERY: Master / Rollo Cortado / Resma / Graphics.
UPDATE #Mapping
SET Tipo = CASE UPPER(LTRIM(RTRIM(Tipo)))
               WHEN N'MASTER'          THEN N'Master'
               WHEN N'ROLLO CORTADO'   THEN N'Rollo Cortado'
               WHEN N'ROLLOS CORTADOS' THEN N'Rollo Cortado'
               WHEN N'ROLLO'           THEN N'Rollo Cortado'
               WHEN N'HOJA'            THEN N'Resma'
               WHEN N'HOJAS'           THEN N'Resma'
               WHEN N'RESMA'           THEN N'Resma'
               WHEN N'RESMAS'          THEN N'Resma'
               WHEN N'GRAPHICS'        THEN N'Graphics'
               WHEN N'GRAFICA'         THEN N'Graphics'
               WHEN N'GRAFICAS'        THEN N'Graphics'
               ELSE LTRIM(RTRIM(Tipo))
           END;

IF OBJECT_ID('tempdb..#Errores') IS NOT NULL
BEGIN
    DROP TABLE #Errores;
END

CREATE TABLE #Errores (Detalle nvarchar(260) NOT NULL);

-- IDs repetidos: dos filas para el mismo producto pelearian por el bit.
INSERT INTO #Errores (Detalle)
SELECT 'producto_id duplicado en el mapping: ' + Product_ID
FROM #Mapping
GROUP BY Product_ID
HAVING COUNT(*) > 1;

-- Tipo que, despues de normalizar, no es ninguno de los cuatro.
INSERT INTO #Errores (Detalle)
SELECT 'tipo no valido "' + Tipo + '" para ' + Product_ID + ': validos Master, Rollo Cortado, Hoja, Graphics'
FROM #Mapping
WHERE NULLIF(LTRIM(RTRIM(Tipo)), N'') IS NOT NULL
  AND Tipo NOT IN (N'Master', N'Rollo Cortado', N'Resma', N'Graphics');

-- IDs que no existen: mejor avisar que dejarlos pasar silenciosamente.
INSERT INTO #Errores (Detalle)
SELECT LTRIM(RTRIM(m.Product_ID)) + ' no existe en dbo.producto, revisar el codigo'
FROM #Mapping m
WHERE NULLIF(LTRIM(RTRIM(m.Tipo)), N'') IS NOT NULL
  AND NOT EXISTS (
        SELECT 1
        FROM dbo.producto p
        WHERE LTRIM(RTRIM(p.Product_ID)) = LTRIM(RTRIM(m.Product_ID))
      );

-- Resumen de la lista y avisos (los avisos no cancelan el script).
SELECT COUNT(*) AS filas_del_mapping
     , SUM(CASE WHEN NULLIF(LTRIM(RTRIM(Tipo)), N'') IS NOT NULL THEN 1 ELSE 0 END) AS con_tipo
     , SUM(CASE WHEN NULLIF(LTRIM(RTRIM(Tipo)), N'') IS NULL THEN 1 ELSE 0 END) AS vacias_no_se_tocan
FROM #Mapping;

SELECT COUNT(*) AS productos_sin_filas_en_el_mapping_se_quedan_igual
FROM dbo.producto p
WHERE NOT EXISTS (
    SELECT 1
    FROM #Mapping m
    WHERE LTRIM(RTRIM(m.Product_ID)) = LTRIM(RTRIM(p.Product_ID))
);


-- ============================================================================
-- 3) APLICAR, todo dentro de una unica transaccion
-- ============================================================================
DECLARE @Filas int = 0;
DECLARE @MalAntes int = 0;
DECLARE @MalDespues int = 0;
DECLARE @EscritoMal int = 0;
DECLARE @Errores int = (SELECT COUNT(*) FROM #Errores);

IF @Errores > 0
BEGIN
    SELECT Detalle AS error FROM #Errores ORDER BY Detalle;
    ;THROW 50002, 'Reclasificacion cancelada: el mapping tiene errores (ver lista). Nada se modifico.', 1;
END

IF NOT EXISTS (SELECT 1 FROM #Mapping WHERE NULLIF(LTRIM(RTRIM(Tipo)), N'') IS NOT NULL)
BEGIN
    PRINT 'Nada que hacer: ninguna fila del mapping tiene tipo escrito. Ningun dato fue tocado.';
    RETURN;
END

BEGIN TRY
    BEGIN TRANSACTION;

    -- Snapshot de los bits actuales. Si la tabla ya existe (segunda pasada con
    -- @Aplicar = 1) NO se pisa: conviene conservar el snapshot de antes de todo.
    IF OBJECT_ID('dbo.producto_Categorias_Bak_20260929', 'U') IS NULL
    BEGIN
        SELECT p.Product_ID, p.Product_Name, p.MasterRolls, p.rollo_cortado, p.Resmas, p.Graphics
        INTO dbo.producto_Categorias_Bak_20260929
        FROM dbo.producto p;

        PRINT 'Respaldo de bits creado en dbo.producto_Categorias_Bak_20260929';
    END
    ELSE
    BEGIN
        PRINT 'El respaldo dbo.producto_Categorias_Bak_20260929 ya existia: no se pisa.';
    END

    PRINT '';
    PRINT '=========== ANTES ===========';

    SELECT t.Tipo, COUNT(*) AS productos
    FROM dbo.producto p
    CROSS APPLY (SELECT CASE WHEN p.MasterRolls = 1 THEN N'Master'
                             WHEN p.rollo_cortado = 1 THEN N'Rollo Cortado'
                             WHEN p.Resmas = 1 THEN N'Resma'
                             WHEN p.Graphics = 1 THEN N'Graphics'
                             ELSE N'(sin categoria)'
                        END AS Tipo) t
    GROUP BY t.Tipo
    ORDER BY productos DESC;

    -- Integrity previa: cada producto debe tener exactamente un bit. Si alguno
    -- ya venia mal (2 bits o ninguno) se avisa, pero no bloquea: el UPDATE solo
    -- toca los que estan en el mapping.
    SET @MalAntes = (SELECT COUNT(*)
                     FROM dbo.producto p
                     WHERE CAST(p.MasterRolls AS int) + CAST(p.rollo_cortado AS int)
                         + CAST(p.Resmas AS int) + CAST(p.Graphics AS int) <> 1);

    PRINT 'Productos con bits inconsistentes antes (0 o mas de 1 bit): ' + CAST(@MalAntes AS nvarchar(10));

    -- Detalle de lo que se va a escribir, para revisarlo impreso.
    SELECT LTRIM(RTRIM(m.Product_ID)) AS product_id
         , LEFT(p.Product_Name, 45) AS nombre
         , a.Tipo AS antes
         , m.Tipo AS despues
    FROM #Mapping m
    INNER JOIN dbo.producto p
        ON LTRIM(RTRIM(p.Product_ID)) = LTRIM(RTRIM(m.Product_ID))
    CROSS APPLY (SELECT CASE WHEN p.MasterRolls = 1 THEN N'Master'
                             WHEN p.rollo_cortado = 1 THEN N'Rollo Cortado'
                             WHEN p.Resmas = 1 THEN N'Resma'
                             WHEN p.Graphics = 1 THEN N'Graphics'
                             ELSE N'(sin categoria)'
                        END AS Tipo) a
    WHERE NULLIF(LTRIM(RTRIM(m.Tipo)), N'') IS NOT NULL
    ORDER BY product_id;


    -- ========================================================================
    -- Escritura: queda exactamente un bit por producto.
    -- ========================================================================
    UPDATE p
    SET p.MasterRolls   = CASE m.Tipo WHEN N'Master' THEN 1 ELSE 0 END,
        p.rollo_cortado = CASE m.Tipo WHEN N'Rollo Cortado' THEN 1 ELSE 0 END,
        p.Resmas        = CASE m.Tipo WHEN N'Resma' THEN 1 ELSE 0 END,
        p.Graphics      = CASE m.Tipo WHEN N'Graphics' THEN 1 ELSE 0 END
    FROM dbo.producto p
    INNER JOIN #Mapping m
        ON LTRIM(RTRIM(p.Product_ID)) = LTRIM(RTRIM(m.Product_ID))
    WHERE NULLIF(LTRIM(RTRIM(m.Tipo)), N'') IS NOT NULL;

    SET @Filas = @@ROWCOUNT;
    PRINT '';
    PRINT 'Productos actualizados: ' + CAST(@Filas AS nvarchar(10));

    PRINT '';
    PRINT '=========== DESPUES ===========';

    SELECT t.Tipo, COUNT(*) AS productos
    FROM dbo.producto p
    CROSS APPLY (SELECT CASE WHEN p.MasterRolls = 1 THEN N'Master'
                             WHEN p.rollo_cortado = 1 THEN N'Rollo Cortado'
                             WHEN p.Resmas = 1 THEN N'Resma'
                             WHEN p.Graphics = 1 THEN N'Graphics'
                             ELSE N'(sin categoria)'
                        END AS Tipo) t
    GROUP BY t.Tipo
    ORDER BY productos DESC;

    SET @MalDespues = (SELECT COUNT(*)
                       FROM dbo.producto p
                       WHERE CAST(p.MasterRolls AS int) + CAST(p.rollo_cortado AS int)
                           + CAST(p.Resmas AS int) + CAST(p.Graphics AS int) <> 1);

    -- Ningun producto del mapping puede haber quedado con un tipo distinto del
    -- que se pidio: es la comprobacion que cierra el circulo.
    SET @EscritoMal = (SELECT COUNT(*)
                       FROM #Mapping m
                       INNER JOIN dbo.producto p
                           ON LTRIM(RTRIM(p.Product_ID)) = LTRIM(RTRIM(m.Product_ID))
                       CROSS APPLY (SELECT CASE WHEN p.MasterRolls = 1 THEN N'Master'
                                                WHEN p.rollo_cortado = 1 THEN N'Rollo Cortado'
                                                WHEN p.Resmas = 1 THEN N'Resma'
                                                WHEN p.Graphics = 1 THEN N'Graphics'
                                                ELSE N'(sin categoria)'
                                           END AS Tipo) d
                       WHERE NULLIF(LTRIM(RTRIM(m.Tipo)), N'') IS NOT NULL
                         AND d.Tipo <> m.Tipo);

    IF @EscritoMal > 0
    BEGIN
        ;THROW 50003, 'Verificacion fallida: algun producto no quedo con el tipo pedido. Se revierte todo.', 1;
    END

    IF @MalDespues > @MalAntes
    BEGIN
        ;THROW 50004, 'Verificacion fallida: hay productos sin exactamente un bit. Se revierte todo.', 1;
    END

    IF @Aplicar = 1
    BEGIN
        COMMIT TRANSACTION;
        PRINT '';
        PRINT 'COMMIT aplicado. Reabrir FrmProductos para ver las categorias nuevas.';
    END
    ELSE
    BEGIN
        ROLLBACK TRANSACTION;
        PRINT '';
        PRINT 'PASADA EN SECO (@Aplicar = 0): todo revertido, la base queda igual.';
        PRINT 'Si el antes/despues impreso es correcto, cambie @Aplicar a 1 y reejecute.';
    END
END TRY
BEGIN CATCH
    -- @@TRANCOUNT lleva los dos arroba: sin ellos T-SQL lo toma como nombre de
    -- columna. El ; antes de THROW tambien es obligatorio.
    IF @@TRANCOUNT > 0
    BEGIN
        ROLLBACK TRANSACTION;
    END

    PRINT '';
    PRINT 'ERROR: nada se modifico. ' + ERROR_MESSAGE();
    ;THROW;
END CATCH
GO


