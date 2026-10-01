-- ============================================================================
-- POBLAR: un producto "Rollo Cortado" por cada Master de dbo.producto
--         (Ritrama2025 / RITRAMASQL2017)
-- ============================================================================
--
-- Estado antes de esto (verificado 30/09/2026): RITRAMASQL2017.dbo.producto
-- tiene 68 productos y TODOS son Master (MasterRolls = 1); hay 0 rollos
-- cortados, asi que las pestanas/busquedas de "Rollo Cortado" salen vacias.
--
-- Convencion de nombres (verificada producto por producto en los catalogos
-- legacy del mismo servidor: RITRAMA, RITRAMA3 y TEST, 74 pares master->RC):
--   * El codigo del rollo cortado = el del master con un 0 detras
--     (02107 -> 021070; tambien esta documentado en el encabezado de
--      Reclasificar_Productos_Categorias.sql).
--   * Product_Name y Product_Descrip = nombre del master ............... 74/74
--   * precio y anulado = los del master ................................ 74/74
--   * Product_Ref y Category_ID = los del master ................ 73/74 y 70/74
--   * MasterRolls = 0, rollo_cortado = 1, Resmas/Graphics = los del master 74/74
--   * code_RC del master = codigo del RC; code_RC del RC = vacio ..... 74/74
--   * ratio: en legacy a veces difiere (39/74 iguales) sin formula conocida,
--     aqui se copia el del master (es el dato de partida correcto).
--   * unidad / cantidad / width / lenght / msi no existen en los catalogos
--     legacy y en RITRAMASQL2017 estan TODOS a NULL en los 68 masters, asi que
--     se copian tal cual (queda NULL, igual que el master).
--
-- El tipo de producto sale de los 4 bits mutuamente excluyentes (ver
-- Core/ProductCategoryRules.cs): el RC nuevo queda con solo rollo_cortado = 1.
--
-- Como se usa (dos pasadas, igual que Reclasificar_Productos_Categorias.sql):
--   1) Dejar @Aplicar = 0 y ejecutar: valida, imprime el plan de INSERT/UPDATE
--      y REVIERTE. Es la pasada de verificacion.
--   2) Cambiar @Aplicar a 1 y volver a ejecutar: ahora si, COMMIT.
--   El script es idempotente: un RC que ya existe no se vuelve a insertar y el
--   code_RC del master se repara si quedo en vacio o en '0'.
--
-- Despues de aplicar: cerrar y volver a abrir FrmProductos.
--
-- REVERTIR (borrar lo escrito por este script): ver el bloque comentado del
-- final del archivo.
-- ============================================================================

SET NOCOUNT ON;
SET XACT_ABORT ON;
-- El INSERT exige QUOTED_IDENTIFIER ON (hay indices/vistas calculadas en la BD);
-- sqlcmd lo trae OFF por defecto.
SET QUOTED_IDENTIFIER ON;
GO

-- ============================================================================
-- 1) CONFIGURACION
-- ============================================================================
-- 0 = solo informe, revierte. 1 = aplica los cambios (COMMIT).
DECLARE @Aplicar bit = 0;

IF OBJECT_ID('dbo.producto', 'U') IS NULL
BEGIN
    ;THROW 50001, 'Nada se modifico: no existe la tabla dbo.producto en esta base.', 1;
END

IF OBJECT_ID('tempdb..#Pendiente') IS NOT NULL DROP TABLE #Pendiente;
IF OBJECT_ID('tempdb..#Existe') IS NOT NULL DROP TABLE #Existe;
IF OBJECT_ID('tempdb..#Error') IS NOT NULL DROP TABLE #Error;

-- COLLATE DATABASE_DEFAULT: sin esto las tablas temporales (tempdb) chocan de
-- colacion al compararlas contra dbo.producto (error 468).
CREATE TABLE #Pendiente
(
    Product_ID nvarchar(50) COLLATE DATABASE_DEFAULT NOT NULL,
    code_RC    nvarchar(50) COLLATE DATABASE_DEFAULT NOT NULL
);

CREATE TABLE #Existe
(
    Product_ID nvarchar(50) COLLATE DATABASE_DEFAULT NOT NULL,
    code_RC    nvarchar(50) COLLATE DATABASE_DEFAULT NOT NULL
);

CREATE TABLE #Error (Detalle nvarchar(260) NOT NULL);

-- Un rollo cortado por cada master: codigo del master + '0'.
INSERT INTO #Pendiente (Product_ID, code_RC)
SELECT p.Product_ID, LTRIM(RTRIM(p.Product_ID)) + N'0'
FROM dbo.producto p
WHERE p.MasterRolls = 1;

-- El codigo RC ya esta ocupado por un producto que SI es rollo cortado: no hay
-- nada que crear, solo reparar el code_RC del master (pasa a #Existe).
INSERT INTO #Existe (Product_ID, code_RC)
SELECT d.Product_ID, d.code_RC
FROM #Pendiente d
WHERE EXISTS (SELECT 1 FROM dbo.producto x
              WHERE x.Product_ID = d.code_RC AND x.rollo_cortado = 1);

DELETE FROM #Pendiente
WHERE Product_ID IN (SELECT Product_ID FROM #Existe);


-- ============================================================================
-- 2) VALIDACIONES (antes de tocar nada)
-- ============================================================================

-- Dos masters no pueden dar el mismo codigo de rollo cortado.
INSERT INTO #Error (Detalle)
SELECT 'codigo de rollo cortado repetido: ' + code_RC
FROM #Pendiente
GROUP BY code_RC
HAVING COUNT(*) > 1;

-- El codigo ya existe y NO es un rollo cortado: jamas se pisa.
INSERT INTO #Error (Detalle)
SELECT 'el codigo ' + d.code_RC + ' ya existe y no es rollo_cortado (Product_Name: ' + ISNULL(x.Product_Name, N'') + ')'
FROM #Pendiente d
INNER JOIN dbo.producto x ON x.Product_ID = d.code_RC
WHERE x.rollo_cortado = 0;

-- Product_ID es nvarchar(50): el codigo del master + el sufijo tiene que caber.
INSERT INTO #Error (Detalle)
SELECT 'el codigo del master no cabe con el sufijo: ' + Product_ID
FROM #Pendiente
WHERE LEN(Product_ID) + 1 > 50;

-- code_RC del master con un valor que no sea vacio, '0' (placeholder actual)
-- ni el codigo que corresponde: no se pisa un valor desconocido a ciegas.
INSERT INTO #Error (Detalle)
SELECT 'code_RC del master ' + d.Product_ID + ' = ''' + m.code_RC + ''' no coincide con ' + d.code_RC
FROM #Pendiente d
INNER JOIN dbo.producto m ON m.Product_ID = d.Product_ID
WHERE ISNULL(LTRIM(RTRIM(m.code_RC)), N'') NOT IN (N'', N'0', d.code_RC);

DECLARE @NMasters int, @RCrear int, @YExiste int, @NErrores int;

SELECT @NMasters = COUNT(*) FROM dbo.producto WHERE MasterRolls = 1;
SELECT @RCrear = COUNT(*) FROM #Pendiente;
SELECT @YExiste = COUNT(*) FROM #Existe;
SELECT @NErrores = COUNT(*) FROM #Error;

PRINT '=========== PLAN ===========';
PRINT 'Masters (MasterRolls = 1) ................ ' + CAST(@NMasters AS nvarchar(10));
PRINT 'Rollo cortado a crear .................... ' + CAST(@RCrear AS nvarchar(10));
PRINT 'Rollo cortado ya existente (solo link) ... ' + CAST(@YExiste AS nvarchar(10));
PRINT 'Errores .................................. ' + CAST(@NErrores AS nvarchar(10));

IF EXISTS (SELECT 1 FROM #Error)
BEGIN
    PRINT '';
    PRINT '*** Hay errores: no se escribe nada. ***';
    SELECT Detalle FROM #Error ORDER BY Detalle;
    RETURN;
END

-- Detalle de lo que se va a escribir, para revisarlo impreso.
SELECT d.Product_ID AS master_id
     , d.code_RC AS rc_id
     , LEFT(m.Product_Name, 45) AS nombre
     , m.anulado
     , m.precio
     , m.ratio
FROM #Pendiente d
INNER JOIN dbo.producto m ON m.Product_ID = d.Product_ID
ORDER BY d.Product_ID;

-- ============================================================================
-- 3) ESCRITURA
-- ============================================================================
DECLARE @Insertados int, @Linkeados int;

BEGIN TRY
    BEGIN TRANSACTION;

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
    ORDER BY t.Tipo;

    IF @Aplicar = 0
    BEGIN
        ROLLBACK;
        PRINT '';
        PRINT 'Dry-run: nada escrito (@Aplicar = 0). Cambia @Aplicar a 1 y vuelve a ejecutar.';
        RETURN;
    END

    -- 3a) INSERT de los rollos cortados nuevos. Copia del master: nombre y
    --     descripcion, ref, codebar, categoria, precio, anulado, ratio, unidad,
    --     cantidad y dimensiones; bits MasterRolls = 0 / rollo_cortado = 1
    --     (Resmas y Graphics se copian tal cual: hoy son 0 en los masters).
    --     code_RC del RC queda vacio: quien apunta es el master (3b).
    INSERT INTO dbo.producto
        (Product_ID, Product_Name, Product_Descrip, Product_Ref, Codebar, Category_ID,
         MasterRolls, rollo_cortado, Resmas, Graphics, anulado,
         precio, code_RC, ratio, unidad, cantidad, width, lenght, msi)
    SELECT d.code_RC, m.Product_Name, m.Product_Name, m.Product_Ref, m.Codebar, m.Category_ID,
           0, 1, ISNULL(m.Resmas, 0), ISNULL(m.Graphics, 0), ISNULL(m.anulado, 0),
           m.precio, NULL, m.ratio, m.unidad, m.cantidad, m.width, m.lenght, m.msi
    FROM #Pendiente d
    INNER JOIN dbo.producto m ON m.Product_ID = d.Product_ID
    WHERE NOT EXISTS (SELECT 1 FROM dbo.producto x WHERE x.Product_ID = d.code_RC);

    SET @Insertados = @@ROWCOUNT;
    PRINT '';
    PRINT 'Rollos cortados insertados: ' + CAST(@Insertados AS nvarchar(10));

    -- 3b) code_RC del master -> codigo de su rollo cortado. Solo se tocan
    --     masters validados arriba (code_RC en '', '0' o ya correcto).
    UPDATE m
    SET code_RC = d.code_RC
    FROM dbo.producto m
    INNER JOIN #Pendiente d ON d.Product_ID = m.Product_ID;

    SET @Linkeados = @@ROWCOUNT;
    PRINT 'Masters con code_RC apuntando a su RC (creados o reparados): ' + CAST(@Linkeados AS nvarchar(10));

    -- Los RC que ya existian tambien quedan linkeados si el master venia en ''/'0'.
    UPDATE m
    SET code_RC = d.code_RC
    FROM dbo.producto m
    INNER JOIN #Existe d ON d.Product_ID = m.Product_ID
    WHERE ISNULL(LTRIM(RTRIM(m.code_RC)), N'') IN (N'', N'0');

    PRINT 'Masters reparados sobre RC ya existentes: ' + CAST(@@ROWCOUNT AS nvarchar(10));


    -- 3c) VERIFICACION POST-ESCRITURA (si algo no cuadra, ROLLBACK).
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
    ORDER BY t.Tipo;

    -- Masters sin rollo cortado asociado (debe dar 0 filas).
    SELECT m.Product_ID AS master_sin_rc, LEFT(m.Product_Name, 45) AS nombre
    FROM dbo.producto m
    WHERE m.MasterRolls = 1
      AND NOT EXISTS (SELECT 1 FROM dbo.producto r
                      WHERE r.Product_ID = m.Product_ID + N'0' AND r.rollo_cortado = 1)
    ORDER BY m.Product_ID;

    -- Rollos cortados sin master que apunte a ellos (debe dar 0 filas).
    SELECT r.Product_ID AS rc_sin_master, LEFT(r.Product_Name, 45) AS nombre
    FROM dbo.producto r
    WHERE r.rollo_cortado = 1
      AND NOT EXISTS (SELECT 1 FROM dbo.producto m
                      WHERE m.code_RC = r.Product_ID AND m.MasterRolls = 1)
    ORDER BY r.Product_ID;

    -- Filas con la categoria inconsistente: debe dar 0.
    SELECT COUNT(*) AS productos_sin_un_bit
    FROM dbo.producto p
    WHERE ISNULL(CAST(p.MasterRolls AS int), 0) + ISNULL(CAST(p.rollo_cortado AS int), 0)
        + ISNULL(CAST(p.Resmas AS int), 0) + ISNULL(CAST(p.Graphics AS int), 0) <> 1;

    -- Muestra de los pares master -> rollo cortado.
    SELECT TOP 15 m.Product_ID AS master_id
         , r.Product_ID AS rc_id
         , LEFT(m.Product_Name, 40) AS nombre
         , m.code_RC
         , r.rollo_cortado
    FROM dbo.producto m
    INNER JOIN dbo.producto r ON r.Product_ID = m.Product_ID + N'0'
    WHERE m.MasterRolls = 1
    ORDER BY m.Product_ID;

    IF EXISTS (SELECT 1 FROM dbo.producto m
               WHERE m.MasterRolls = 1
                 AND NOT EXISTS (SELECT 1 FROM dbo.producto r
                                 WHERE r.Product_ID = m.Product_ID + N'0' AND r.rollo_cortado = 1))
    BEGIN
        ;THROW 50002, 'Quedaron masters sin rollo cortado: se revirtio todo.', 1;
    END

    IF EXISTS (SELECT 1 FROM dbo.producto p
               WHERE ISNULL(CAST(p.MasterRolls AS int), 0) + ISNULL(CAST(p.rollo_cortado AS int), 0)
                   + ISNULL(CAST(p.Resmas AS int), 0) + ISNULL(CAST(p.Graphics AS int), 0) <> 1)
    BEGIN
        ;THROW 50003, 'Hay productos con la categoria inconsistente: se revirtio todo.', 1;
    END

    COMMIT;
    PRINT '';
    PRINT 'OK: cambios confirmados.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK;
    PRINT 'ERROR: no se escribio nada (transaccion revertida).';
    THROW;
END CATCH
GO


-- ============================================================================
-- 4) REVERTIR (ejecutar a mano solo si hay que deshacer)
-- ============================================================================
-- Borra los rollos cortados creados por este script y deja el code_RC de los
-- masters como estaba ('' / '0'), sin tocar nada mas.
--
-- BEGIN TRANSACTION;
--
-- UPDATE m
-- SET code_RC = N''
-- FROM dbo.producto m
-- INNER JOIN dbo.producto r ON r.Product_ID = m.Product_ID + N'0'
-- WHERE m.MasterRolls = 1 AND r.rollo_cortado = 1;
--
-- DELETE r
-- FROM dbo.producto r
-- WHERE r.rollo_cortado = 1
--   AND EXISTS (SELECT 1 FROM dbo.producto m
--               WHERE m.code_RC = r.Product_ID AND m.MasterRolls = 1);
--
-- COMMIT;

