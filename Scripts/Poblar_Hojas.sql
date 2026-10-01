-- ============================================================================
-- POBLAR: productos tipo Hoja (Resmas) en dbo.producto
--         (Ritrama2025 / RITRAMASQL2017)
-- ============================================================================
--
-- Estado antes de esto (verificado 30/09/2026 contra la propia base):
--   RITRAMASQL2017.dbo.producto tiene 210 productos = 68 Master + 68 Rollo
--   Cortado (Poblar_Rollos_Cortados.sql) + 74 Graphics (Poblar_Graphics.sql),
--   y CERO Resmas, asi que la columna/pestana "Hojas" sale vacia. Con esto la
--   tabla pasa a 235 = 68 Master + 68 Rollo Cortado + 74 Graphics + 25 Hoja.
--
-- Fuente (verificada 30/09/2026, mismo servidor 192.168.10.10):
--   RITRAMA.dbo.producto WHERE Resmas = 1  ->  25 filas. Es el catalogo legacy
--   mas completo: RITRAMA tiene 443 productos con 25 Resmas; TEST 424/25;
--   RITRAMA3 401/12 y RITRAMA2025-TEST 383/12 son copias atrasadas (subconjunto).
--   Se copia de RITRAMA porque es el unico con los 25.
--
-- Convencion (que queda como queda, y por que):
--   * Product_ID / Product_Name / Product_Descrip = los del legacy, TAL CUAL.
--     No se limpian los espacios dobles del legacy (p.ej. "WK135  SPL38  Sheet"),
--     para que el dato sea contrastable 1:1 contra RITRAMA.
--   * Product_ID es nvarchar(25) y los 25 miden 5: entra todo sin recortar.
--   * Product_Descrip es nvarchar(200) y el mas largo mide 83: entra todo.
--   * Product_Ref llega vacio o a '0' en el legacy: se copia igual (no se inventa).
--   * Codebar llega vacio salvo 3 (00753, 01874, 02033) y a '0' en 2 mas: se copia
--     igual, sin normalizar.
--   * Category_ID llega vacio en los 25: se copia NULL.
--   * precio y ratio = los del legacy (precio .00 en los 25; ratio 7.44 en 17 y
--     .00 en 8). anulado = 0 en los 25.
--   * costo: RITRAMA NO tiene esa columna (se anadio despues en Ritrama2025), y
--     las 210 filas actuales la tienen a NULL, asi que se deja fuera del INSERT
--     y queda NULL, igual que los 68 Master y los 74 Graphics.
--   * unidad / cantidad / width / lenght / msi: RITRAMA tampoco las tiene y en la
--     tabla destino estan a NULL en las 210 filas, asi que quedan NULL.
--   * code_RC = NULL (en el legacy los 25 lo traen vacio: son hojas, no rollos
--     cortados, asi que no tienen rollo asociado).
--   * Tipo: Resmas = 1 y los otros tres bits a 0 (son mutuamente excluyentes:
--     ver Core/ProductCategoryRules.cs y el CASE de R.SELECT_QUERY_PRODUCTS).
--
-- Colisiones (verificadas): los 25 Product_ID NO existen todavia en
-- RITRAMASQL2017, y tampoco hay un Master con el mismo codigo en RITRAMA, asi
-- que no se pisa ni se confunde ningun producto existente. Aun asi, el script
-- aborta si alguno aparece con otra categoria.
--
-- Ojo con lo que NO se hace: no se crea un "Master" para cada hoja ni se
-- relacionan entre si. En el legacy estas 25 filas son productos Hoja
-- independientes, y asi se cargan.
--
-- Como se usa (dos pasadas, igual que Poblar_Rollos_Cortados.sql y
-- Poblar_Graphics.sql):
--   1) Dejar @Aplicar = 0 y ejecutar: valida, imprime el plan de INSERT y
--      REVIERTE. Es la pasada de verificacion.
--   2) Cambiar @Aplicar a 1 y volver a ejecutar: ahora si, COMMIT.
--   Idempotente: una Hoja ya cargada no se vuelve a insertar; si el codigo
--   existe con OTRA categoria, es error y no se escribe nada.
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
-- 1) CONFIGURACION + CATALOGO DE HOJAS
-- ============================================================================
-- 0 = solo informe, revierte. 1 = aplica los cambios (COMMIT).
DECLARE @Aplicar bit = 0;

IF OBJECT_ID('dbo.producto', 'U') IS NULL
BEGIN
    ;THROW 50001, 'Nada se modifico: no existe la tabla dbo.producto en esta base.', 1;
END

IF OBJECT_ID('tempdb..#Catalogo') IS NOT NULL DROP TABLE #Catalogo;
IF OBJECT_ID('tempdb..#Existe') IS NOT NULL DROP TABLE #Existe;
IF OBJECT_ID('tempdb..#Error') IS NOT NULL DROP TABLE #Error;

-- COLLATE DATABASE_DEFAULT: sin esto las tablas temporales (tempdb) chocan de
-- colacion al compararlas contra dbo.producto (error 468).
CREATE TABLE #Catalogo
(
    Product_ID      nvarchar(50)  COLLATE DATABASE_DEFAULT NOT NULL,
    Product_Name    nvarchar(400) COLLATE DATABASE_DEFAULT NOT NULL,
    Product_Descrip nvarchar(400) COLLATE DATABASE_DEFAULT NULL,
    Product_Ref     nvarchar(50)  COLLATE DATABASE_DEFAULT NULL,
    Codebar         nvarchar(150) COLLATE DATABASE_DEFAULT NULL,
    Category_ID     nvarchar(50)  COLLATE DATABASE_DEFAULT NULL,
    precio          decimal(18,2) NOT NULL,
    ratio           decimal(18,2) NOT NULL
);

CREATE TABLE #Existe (Product_ID nvarchar(50) COLLATE DATABASE_DEFAULT NOT NULL);
CREATE TABLE #Error (Detalle nvarchar(260) NOT NULL);

-- Las 25 hojas de RITRAMA.dbo.producto (Resmas = 1), generadas con un SELECT
-- sobre esa base el 30/09/2026 y copiadas aqui tal cual para que el script no
-- dependa de que RITRAMA siga existiendo. Product_Ref / Codebar vacios se
-- guardan como NULL; Category_ID vacio tambien.
INSERT INTO #Catalogo
    (Product_ID, Product_Name, Product_Descrip, Product_Ref, Codebar, Category_ID, precio, ratio)
VALUES
  (N'00753', N'00753-500 (RI-705/60 PP Gloss Clear TC8 AP900 WK135  SPL38  Sheet 500 mm  x 700 mm)', N'00753-500 (RI-705/60 PP Gloss Clear TC8 AP900 WK135  SPL38  Sheet 500 mm  x 700 mm)', NULL, N'00753', NULL, 0.00, 7.44),
  (N'01874', N'01874 (SEMI GLOSS AP904 WK85 RIT/GRY  Sheet 700 x 1000)', N'01874 (SEMI GLOSS AP904 WK85 RIT/GRY  Sheet 700 x 1000)', NULL, N'01874', NULL, 0.00, 7.44),
  (N'02033', N'02033-500 (RI-205/80 PVC GLOSS CLEAR AP900 WK135 RIT/GRY (Sheet 500mm  x 700mm))', N'02033-500 (RI-205/80 PVC GLOSS CLEAR AP900 WK135 RIT/GRY (Sheet 500mm  x 700mm))', NULL, N'02033', NULL, 0.00, 7.44),
  (N'02207', N'02207-500 (LITHO AP-904 WK85 RIT/GRY SPL38-Sheet 500 x 700)', N'02207-500 (LITHO AP-904 WK85 RIT/GRY SPL38-Sheet 500 x 700)', NULL, NULL, NULL, 0.00, 7.44),
  (N'02211', N'02211-500 (Matt Coated AP-904 WK85 RIT/GRY SPL38  Sheet 500 mm  x 700 mm)', N'02211-500 (Matt Coated AP-904 WK85 RIT/GRY SPL38  Sheet 500 mm  x 700 mm)', NULL, NULL, NULL, 0.00, 7.44),
  (N'02216', N'02216-432 (CAST GLOSS AP-904 WK85 RIT/GRY SPL38  Sheet 432 mm  x 560 mm)', N'02216-432 (CAST GLOSS AP-904 WK85 RIT/GRY SPL38  Sheet 432 mm  x 560 mm)', NULL, NULL, NULL, 0.00, 7.44),
  (N'02221', N'SEMIGLOSS AP-904 WK85 RIT/GRY SPL38  Sheet', N'SEMIGLOSS AP-904 WK85 RIT/GRY SPL38  Sheet', NULL, NULL, NULL, 0.00, 7.44),
  (N'03641', N'03641-500 (RI-145/80 PVC GLOSS WHITE AP900 WK135 RIT/GRY (Sheet 500mm  x 700mm))', N'03641-500 (RI-145/80 PVC GLOSS WHITE AP900 WK135 RIT/GRY (Sheet 500mm  x 700mm))', NULL, NULL, NULL, 0.00, 7.44),
  (N'10032', N'10032-500 (RI-705/60 PP Gloss Clear TC8 AP900 WK135  SPL50  Sheet 500 mm  x 700 mm)', N'10032-500 (RI-705/60 PP Gloss Clear TC8 AP900 WK135  SPL50  Sheet 500 mm  x 700 mm)', NULL, NULL, NULL, 0.00, 7.44),
  (N'10033', N'10033-500 (Glo Pink  AP-904 WK85 RIT/GRY SPL38  Sheet 500 mm  x 700 mm)', N'10033-500 (Glo Pink  AP-904 WK85 RIT/GRY SPL38  Sheet 500 mm  x 700 mm)', NULL, NULL, NULL, 0.00, 7.44),
  (N'10245', N'RI-765/65 PP Matt White TTR AP901 WK135', N'RI-765/65 PP Matt White TTR AP901 WK135', NULL, NULL, NULL, 0.00, 0.00),
  (N'10586', N'10586-432 (Semigloss 80 AP904 WK135 RIT/GRY SPL38 (432 mm x 560 mm))', N'10586-432 (Semigloss 80 AP904 WK135 RIT/GRY SPL38 (432 mm x 560 mm))', NULL, NULL, NULL, 0.00, 7.44),
  (N'10952', N'10952-500 (LITHO AP999 WK85 RIT/GRY SPL38 (seguridad)  Sheet 500 x 700)', N'10952-500 (LITHO AP999 WK85 RIT/GRY SPL38 (seguridad)  Sheet 500 x 700)', NULL, NULL, NULL, 0.00, 7.44),
  (N'11418', N'Coated Gloss FSC P10P KB85 Split', N'Coated Gloss FSC P10P KB85 Split', N'0', N'0', NULL, 0.00, 0.00),
  (N'11423', N'Coated Gloss FSC P10O KB85 Split Imprint', N'Coated Gloss FSC P10O KB85 Split Imprint', NULL, NULL, NULL, 0.00, 0.00),
  (N'11508', N'PP FD Ultra Matt 65 PF2 MCK135', N'PP FD Ultra Matt 65 PF2 MCK135', NULL, NULL, NULL, 0.00, 0.00),
  (N'12838', N'Laser PP White Matt Perm - Ritrama', N'Laser PP White Matt Perm - Ritrama', NULL, NULL, NULL, 0.00, 0.00),
  (N'12839', N'Laser PP Metallized Gloss Perm - Ritrama', N'Laser PP Metallized Gloss Perm - Ritrama', NULL, NULL, NULL, 0.00, 0.00),
  (N'12840', N'DTS PP Clear Gloss Perm - Adhoc', N'DTS PP Clear Gloss Perm - Adhoc', NULL, NULL, NULL, 0.00, 0.00),
  (N'12841', N'DTS PP White Gloss Perm - Adhoc', N'DTS PP White Gloss Perm - Adhoc', NULL, NULL, NULL, 0.00, 0.00),
  (N'12842', N'DTS PP White Matt Perm - Adhoc', N'DTS PP White Matt Perm - Adhoc', NULL, NULL, NULL, 0.00, 0.00),
  (N'12843', N'DTS PP Metallized Gloss Perm - Adhoc', N'DTS PP Metallized Gloss Perm - Adhoc', NULL, NULL, NULL, 0.00, 0.00),
  (N'12844', N'Laser PP Clear Gloss Perm - Ritrama', N'Laser PP Clear Gloss Perm - Ritrama', NULL, NULL, NULL, 0.00, 0.00),
  (N'12845', N'Laser PP White Gloss Perm - Ritrama', N'Laser PP White Gloss Perm - Ritrama', NULL, NULL, NULL, 0.00, 0.00),
  (N'13754', N'Ri-705/50 PP Gloss Clear TC8 AP901 WK135 RIT/GRY SPL 38', N'Ri-705/50 PP Gloss Clear TC8 AP901 WK135 RIT/GRY SPL 38', N'0', N'0', NULL, 0.00, 0.00);


-- ============================================================================
-- 2) VALIDACIONES + PLAN (antes de tocar nada)
-- ============================================================================

-- Codigo repetido dentro del catalogo del script.
INSERT INTO #Error (Detalle)
SELECT 'codigo duplicado en el catalogo: ' + Product_ID
FROM #Catalogo
GROUP BY Product_ID
HAVING COUNT(*) > 1;

-- Longitudes contra las columnas REALES de la tabla destino:
-- Product_ID nvarchar(25), Product_Name/Product_Descrip nvarchar(200),
-- Product_Ref nvarchar(20), Codebar nvarchar(128), Category_ID nvarchar(50).
INSERT INTO #Error (Detalle)
SELECT 'codigo demasiado largo para Product_ID (25): ' + Product_ID
FROM #Catalogo
WHERE LEN(Product_ID) > 25;

INSERT INTO #Error (Detalle)
SELECT 'nombre demasiado largo para Product_Name (200): ' + Product_ID
FROM #Catalogo
WHERE LEN(Product_Name) > 200;

INSERT INTO #Error (Detalle)
SELECT 'descripcion demasiado larga para Product_Descrip (200): ' + Product_ID
FROM #Catalogo
WHERE LEN(Product_Descrip) > 200;

INSERT INTO #Error (Detalle)
SELECT 'referencia demasiado larga para Product_Ref (20): ' + Product_ID
FROM #Catalogo
WHERE LEN(Product_Ref) > 20;

INSERT INTO #Error (Detalle)
SELECT 'codigo de barra demasiado largo para Codebar (128): ' + Product_ID
FROM #Catalogo
WHERE LEN(Codebar) > 128;

INSERT INTO #Error (Detalle)
SELECT 'categoria demasiado larga para Category_ID (50): ' + Product_ID
FROM #Catalogo
WHERE LEN(Category_ID) > 50;

-- El ID ya existe en la tabla con OTRA categoria: no se pisa nunca. Esto es lo que
-- protege de convertir sin querer un Master o un Rollo Cortado en Hoja.
INSERT INTO #Error (Detalle)
SELECT 'el codigo ' + c.Product_ID + ' ya existe y NO es Hoja (Product_Name: ' + ISNULL(x.Product_Name, N'') + ')'
FROM #Catalogo c
INNER JOIN dbo.producto x ON x.Product_ID = c.Product_ID
WHERE x.Resmas = 0;

-- Los que ya estan cargados como Hoja no se reinsertan (idempotencia).
INSERT INTO #Existe (Product_ID)
SELECT c.Product_ID
FROM #Catalogo c
INNER JOIN dbo.producto x ON x.Product_ID = c.Product_ID AND x.Resmas = 1;

DECLARE @NTotal int, @NCrear int, @NExiste int, @NErrores int;

SELECT @NTotal = COUNT(*) FROM #Catalogo;
-- Si hay errores el script aborta antes de escribir, asi que "a crear" es
-- simplemente lo que no esta ya cargado como Hoja.
SELECT @NCrear = COUNT(*) FROM #Catalogo
WHERE Product_ID NOT IN (SELECT Product_ID FROM #Existe);
SELECT @NExiste = COUNT(*) FROM #Existe;
SELECT @NErrores = COUNT(*) FROM #Error;

PRINT '=========== PLAN ===========';
PRINT 'Hojas en el catalogo (legacy RITRAMA) ... ' + CAST(@NTotal AS nvarchar(10));
PRINT 'Hojas a crear .......................... ' + CAST(@NCrear AS nvarchar(10));
PRINT 'Hojas ya existentes (se omiten) ........ ' + CAST(@NExiste AS nvarchar(10));
PRINT 'Errores ................................ ' + CAST(@NErrores AS nvarchar(10));

IF EXISTS (SELECT 1 FROM #Error)
BEGIN
    PRINT '';
    PRINT '*** Hay errores: no se escribe nada. ***';
    SELECT Detalle FROM #Error ORDER BY Detalle;
    RETURN;
END

-- Detalle de lo que se va a escribir, para revisarlo impreso.
SELECT c.Product_ID
     , c.Product_Name
     , c.Product_Ref
     , c.precio
     , c.ratio
     , CASE WHEN e.Product_ID IS NULL THEN 'A CREAR' ELSE 'YA EXISTE' END AS accion
FROM #Catalogo c
LEFT JOIN #Existe e ON e.Product_ID = c.Product_ID
ORDER BY c.Product_ID;

-- ============================================================================
-- 3) ESCRITURA
-- ============================================================================
DECLARE @Insertados int;

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

    -- 3a) INSERT de las Hojas nuevas. Las que ya existian como Hoja no se tocan
    --     (#Existe); las que chocan con otra categoria ni llegan aqui (lo aborta
    --     la validacion de la seccion 2).
    --
    --     La lista de columnas se arma EN DINAMICO porque las dos bases de este
    --     proyecto no tienen el mismo esquema: RITRAMASQL2017.dbo.producto tiene
    --     unidad / cantidad / width / lenght / msi y RITRAMA2025-TEST.dbo.producto
    --     NO (error 207 "el nombre de columna no es valido"). Como las cinco van a
    --     NULL en las 25, se escriben solo las que existan de verdad en la base donde
    --     se este ejecutando, en vez de una lista fija que revienta en una de las dos.
    DECLARE @Opcionales TABLE (Posicion int IDENTITY(1,1), Columna sysname NOT NULL, Valor nvarchar(60) NOT NULL);
    INSERT INTO @Opcionales (Columna, Valor) VALUES
        ('code_RC', 'NULL'), ('unidad', 'NULL'), ('cantidad', 'NULL'),
        ('width', 'NULL'), ('lenght', 'NULL'), ('msi', 'NULL');

    -- Las columnas obligatorias se comprueban una a una: si falta alguna, el error
    -- del INSERT seria mas dificil de leer que este aviso.
    IF EXISTS (SELECT 1 FROM (VALUES ('Product_ID'), ('Product_Name'), ('Product_Descrip'),
                              ('Product_Ref'), ('Codebar'), ('Category_ID'),
                              ('MasterRolls'), ('rollo_cortado'), ('Resmas'), ('Graphics'),
                              ('anulado'), ('precio'), ('ratio'))
                    AS Necesarias(Columna)
               WHERE NOT EXISTS (SELECT 1 FROM sys.columns sc
                                  WHERE sc.object_id = OBJECT_ID('dbo.producto')
                                    AND sc.name = Necesarias.Columna))
    BEGIN
        ;THROW 50004, 'Falta alguna columna obligatoria de dbo.producto en esta base: se revirtio todo.', 1;
    END

    DECLARE @Cols nvarchar(max) =
        N'Product_ID, Product_Name, Product_Descrip, Product_Ref, Codebar, Category_ID,'
      + N' MasterRolls, rollo_cortado, Resmas, Graphics, anulado, precio, ratio';
    DECLARE @Vals nvarchar(max) =
        N'c.Product_ID, c.Product_Name, c.Product_Descrip, c.Product_Ref, c.Codebar, c.Category_ID,'
      + N' 0, 0, 1, 0, 0, c.precio, c.ratio';

    SELECT @Cols = @Cols + N', ' + o.Columna,
           @Vals = @Vals + N', ' + o.Valor
    FROM @Opcionales o
    WHERE EXISTS (SELECT 1 FROM sys.columns sc
                   WHERE sc.object_id = OBJECT_ID('dbo.producto') AND sc.name = o.Columna)
    ORDER BY o.Posicion;

    PRINT '';
    PRINT 'Columnas escritas: ' + @Cols;

    -- El INSERT se arma entero en una variable: EXEC no admite una expresion concatenada
    -- como parametro (error 102 "sintaxis incorrecta cerca de '+'"), solo un identificador.
    DECLARE @Stmt nvarchar(max) =
        N'INSERT INTO dbo.producto (' + @Cols + N')'
      + N' SELECT ' + @Vals
      + N' FROM #Catalogo c'
      + N' WHERE NOT EXISTS (SELECT 1 FROM dbo.producto x'
      + N'                   WHERE x.Product_ID = c.Product_ID AND x.Resmas = 1);';

    EXEC sp_executesql @Stmt;

    SET @Insertados = @@ROWCOUNT;
    PRINT '';
    PRINT 'Hojas insertadas: ' + CAST(@Insertados AS nvarchar(10));

    -- 3b) VERIFICACION POST-ESCRITURA (si algo no cuadra, ROLLBACK).
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

    -- Hojas del catalogo que no quedaron cargadas (debe dar 0 filas).
    SELECT c.Product_ID AS sin_cargar, c.Product_Name
    FROM #Catalogo c
    WHERE NOT EXISTS (SELECT 1 FROM dbo.producto x
                      WHERE x.Product_ID = c.Product_ID AND x.Resmas = 1)
    ORDER BY c.Product_ID;

    -- Hojas con la categoria inconsistente (debe dar 0).
    SELECT COUNT(*) AS hojas_inconsistentes
    FROM dbo.producto p
    WHERE p.Resmas = 1
      AND (ISNULL(CAST(p.MasterRolls AS int), 0) + ISNULL(CAST(p.rollo_cortado AS int), 0)
         + ISNULL(CAST(p.Resmas AS int), 0) + ISNULL(CAST(p.Graphics AS int), 0) <> 1
           OR p.MasterRolls = 1 OR p.rollo_cortado = 1 OR p.Graphics = 1);

    -- Productos con la categoria inconsistente en total (debe dar 0).
    SELECT COUNT(*) AS productos_sin_un_bit
    FROM dbo.producto p
    WHERE ISNULL(CAST(p.MasterRolls AS int), 0) + ISNULL(CAST(p.rollo_cortado AS int), 0)
        + ISNULL(CAST(p.Resmas AS int), 0) + ISNULL(CAST(p.Graphics AS int), 0) <> 1;

    -- Muestra de las Hojas cargadas.
    SELECT TOP 15 Product_ID, LEFT(Product_Name, 45) AS nombre, ratio, anulado
    FROM dbo.producto
    WHERE Resmas = 1
    ORDER BY Product_ID;

    IF EXISTS (SELECT 1 FROM #Catalogo c
               WHERE NOT EXISTS (SELECT 1 FROM dbo.producto x
                                 WHERE x.Product_ID = c.Product_ID AND x.Resmas = 1))
    BEGIN
        ;THROW 50002, 'Quedaron hojas del catalogo sin cargar: se revirtio todo.', 1;
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
-- Borra las Hojas cargadas por este script. Antes de esto habia 0 Resmas en la
-- tabla, asi que basta con borrar todas las que tengan Resmas = 1.
-- Si despues se cargan Hojas de otra fuente, restringir a esos 25 codigos.
--
-- BEGIN TRANSACTION;
--
-- DELETE FROM dbo.producto WHERE Resmas = 1;
--
-- COMMIT;
