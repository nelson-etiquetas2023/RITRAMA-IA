-- ============================================================================
-- POBLAR: productos tipo Graphics en dbo.producto (Ritrama2025 / RITRAMASQL2017)
-- ============================================================================
--
-- Fuente (verificada 30/09/2026): catalogo Graphics RITRAMA de Fedrigoni
-- Self-Adhesives:
--   https://myselfadhesives.fedrigoni.com/es_es/graphics/ritrama.html
--   ("Producto(s): 74" en 3 paginas de 25/25/24; los 74 estan listados abajo).
--
-- Estado antes de esto: RITRAMASQL2017.dbo.producto tiene 136 productos
-- (68 Master + 68 Rollo Cortado creados por Poblar_Rollos_Cortados.sql) y
-- CERO Graphics, asi que la pestana/busqueda "Graphics" sale vacia.
--
-- Convencion usada (por que cada columna queda como queda):
--   * Product_ID  = el codigo publicado en la web (SKU), tal cual, p.ej.
--                   'rijetm100rem'. Se deja la forma original para poder
--                   contrastar 1:1 contra el catalogo; no choca con ningun ID
--                   existente (los actuales son numericos o tipo 'B0711').
--                   OJO: Product_ID es nvarchar(25) en la tabla y unico SKU
--                   del catalogo mide 26 ('rilamc30ultimateultraclear'), asi
--                   que ese se carga como 'rilamc30ultimateuc' y el codigo
--                   completo de la web queda en Product_Descrip (decision del
--                   usuario: no se altera el esquema).
--   * Product_Name = el nombre tal cual aparece en el listado del sitio,
--                   p.ej. 'Ritrama Ri-Jet M100 Rem Series'.
--   * Product_Descrip = el espesor de la ficha ('Espesor 100 um'); en las 3
--                   series donde la web no publica espesor se describe el
--                   frontal/durabilidad que si publica la ficha.
--   * Product_Ref = mismo SKU si cabe (Product_Ref es nvarchar(20); 7 de los
--                   74 SKU son mas largos y quedan en NULL), para imitar el
--                   "referencia = propio codigo" de los Graphics legacy.
--   * Category_ID = '0' (valor mayoritario en la tabla: 114 de 136 filas).
--   * Graphics = 1 y MasterRolls / rollo_cortado / Resmas = 0 (bits mutuamente
--                   excluyentes: ver Core/ProductCategoryRules.cs y el CASE de
--                   R.SELECT_QUERY_PRODUCTS en R.cs).
--   * anulado = 0, precio = 0, ratio = 0 (igual que los 68 masters actuales,
--                   que estan en precio .00 / ratio .00).
--   * Codebar, code_RC, unidad, cantidad, width, lenght, msi = NULL (la web no
--                   publica esos datos y las columnas estan a NULL en los
--                   productos existentes).
--
-- Como se usa (dos pasadas, igual que Reclasificar_Productos_Categorias.sql y
-- Poblar_Rollos_Cortados.sql):
--   1) Dejar @Aplicar = 0 y ejecutar: valida, imprime el plan y REVIERTE.
--   2) Cambiar @Aplicar a 1 y volver a ejecutar: COMMIT.
--   Idempotente: un SKU que ya existe como Graphics no se vuelve a insertar
--   (se cuenta como "ya existente"); si existe con OTRA categoria, es error.
--
-- Despues de aplicar: cerrar y volver a abrir FrmProductos.
--
-- REVERTIR: ver el bloque comentado del final del archivo.
-- ============================================================================

SET NOCOUNT ON;
SET XACT_ABORT ON;
-- El INSERT exige QUOTED_IDENTIFIER ON (hay indices/vistas calculadas en la BD);
-- sqlcmd lo trae OFF por defecto.
SET QUOTED_IDENTIFIER ON;
GO

-- ============================================================================
-- 1) CONFIGURACION + CATALOGO DE LA WEB
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
    Product_Descrip nvarchar(400) COLLATE DATABASE_DEFAULT NULL
);

CREATE TABLE #Existe (Product_ID nvarchar(50) COLLATE DATABASE_DEFAULT NOT NULL);
CREATE TABLE #Error (Detalle nvarchar(260) NOT NULL);

-- Las 74 series del catalogo (SKU del sitio | nombre del listado | ficha).
INSERT INTO #Catalogo (Product_ID, Product_Name, Product_Descrip) VALUES
 (N'RIDOTM100REMPE',            N'Ritrama Ri-Dot M100 PE Series',                N'Espesor 100 um'),
 (N'ridotm100',                 N'Ritrama Ri-Dot M100 Series',                   N'Espesor 100 um'),
 (N'rijetc50permltsb',          N'Ritrama Ri-Jet C50 Perm LT SB Series',         N'Espesor 50 um'),
 (N'rijetc50permsb',            N'Ritrama Ri-Jet C50 Perm SB Series',            N'Espesor 50 um'),
 (N'rijetc50ultimatest',        N'Ritrama Ri-Jet C50 Ultimate S&T Series',       N'Espesor 50 um'),
 (N'RIJETC50ULTRACLEAR',        N'Ritrama Ri-Jet C50 Ultra-clear Series',        N'Espesor 50 um'),
 (N'rijetdecobusm85',           N'Ritrama Ri-Jet Deco-Bus M85 Series',           N'Espesor 85 um'),
 (N'rijetfloortalkerall1layer', N'Ritrama Ri-Jet Floor Talker all1Layer Series', N'Espesor 200 um'),
 (N'rijetfloortalkerm100',      N'Ritrama Ri-Jet Floor Talker M100 Series',      N'Espesor 100 um'),
 (N'rijetm100higrip',           N'Ritrama Ri-Jet M100 Hi-Grip Series',           N'Espesor 100 um'),
 (N'rijetm100permblack',        N'Ritrama Ri-Jet M100 Perm Black Series',        N'Espesor 100 um'),
 (N'rijetm100permgrey',         N'Ritrama Ri-Jet M100 Perm Grey Series',         N'Espesor 100 um'),
 (N'rijetm100perm',             N'Ritrama Ri-Jet M100 Perm Series',              N'Espesor 100 um'),
 (N'rijetm100remblack',         N'Ritrama Ri-Jet M100 Rem Black Series',         N'Espesor 100 um'),
 (N'rijetm100remgrey',          N'Ritrama Ri-Jet M100 Rem Grey Series',          N'Espesor 100 um'),
 (N'rijetm100remsb',            N'Ritrama Ri-Jet M100 Rem SB Series',            N'Espesor 100 um'),
 (N'rijetm100rem',              N'Ritrama Ri-Jet M100 Rem Series',               N'Espesor 100 um'),
 (N'rijetm150static',           N'Ritrama Ri-Jet M150 Static Series',            N'Espesor 150 um'),
 (N'rijetm170rem',              N'Ritrama Ri-Jet M170 Rem Series',               N'Espesor 170 um'),
 (N'rijetm96pe',                N'Ritrama Ri-Jet M96 PE Series',                 N'Espesor 96 um'),
 (N'rijetoptimap75',            N'Ritrama Ri-Jet Optima P75 Series',             N'Espesor 75 um'),
 (N'rijetp100ht',               N'Ritrama Ri-Jet P100 HT Series',                N'Espesor 100 um'),
 (N'rijetp75higrip',            N'Ritrama Ri-Jet P75 Hi-Grip Series',            N'Espesor 75 um'),
 (N'rijetp75permlt',            N'Ritrama Ri-Jet P75 Perm LT Series',            N'Espesor 75 um'),
 (N'rijetp75permsb',            N'Ritrama Ri-Jet P75 Perm SB Series',            N'Espesor 75 um');


INSERT INTO #Catalogo (Product_ID, Product_Name, Product_Descrip) VALUES
 (N'rijetp90translucent',       N'Ritrama Ri-Jet P90 Translucent Series',        N'Espesor 90 um'),
 (N'rijetpet75ultraclearsb',    N'Ritrama Ri-Jet PET75 Ultra-Clear SB Series',   N'Espesor 75 um'),
 (N'rijetpo100ecosolvent',      N'Ritrama Ri-Jet PO100 Eco-Solvent Series',      N'Espesor 100 um'),
 (N'rijetpo100',                N'Ritrama Ri-Jet PO100 Series',                  N'Espesor 100 um'),
 (N'rijetpromoriview',          N'Ritrama Ri-Jet Promo Ri-View Series',          N'Espesor 145 um'),
 (N'rijetreflective',           N'Ritrama Ri-Jet Reflective Series',             N'Espesor 110 um'),
 (N'rijetseethroughremsb',      N'Ritrama Ri-Jet Seethrough Rem SB Series',      N'Espesor 140 um'),
 (N'rilamc30c50ultraclear',     N'Ritrama Ri-Lam C30 & C50 Ultra-clear Series',  N'Espesor 30 um'),
 (N'rilamc30ultimate',          N'Ritrama Ri-Lam C30 Ultimate Series',           N'Espesor 30 um'),
 (N'rilamc30ultimateuc',        N'Ritrama Ri-Lam C30 Ultimate Ultra Clear Series', N'Espesor 30 um (codigo web: rilamc30ultimateultraclear)'),
 (N'rilamfloortalkerm100',      N'Ritrama Ri-Lam Floor Talker M100 Series',      N'Espesor 100 um'),
 (N'rilamfloortalkerm200',      N'Ritrama Ri-Lam Floor Talker M200 Series',      N'Espesor 200 um'),
 (N'rilamm70sb',                N'Ritrama Ri-Lam M70 SB Series',                 N'Espesor 70 um'),
 (N'rilamm70',                  N'Ritrama Ri-Lam M70 Series',                    N'Espesor 70 um'),
 (N'rilamp300plasticone',       N'Ritrama Ri-Lam P300 Plasticone Series',        N'Espesor 300 um'),
 (N'rilamp75sb',                N'Ritrama Ri-Lam P75 SB Series',                 N'Laminacion PVC polimerico, durabilidad 2 anos'),
 (N'rilamp75',                  N'Ritrama Ri-Lam P75 Series',                    N'Espesor 75 um'),
 (N'rilampet20',                N'Ritrama Ri-Lam PET20 Series',                  N'Espesor 20 um'),
 (N'rilampo60sb',               N'Ritrama Ri-Lam PO60 SB Series',                N'Espesor 60 um'),
 (N'rilamtoptimap75',           N'Ritrama Ri-Lam T Optima P75 Series',           N'Espesor 75 um'),
 (N'rimarketchedglass',         N'Ritrama Ri-Mark Etched glass Series',          N'Espesor 80 um'),
 (N'rimarkevent',               N'Ritrama Ri-Mark Event Series',                 N'Espesor 70 um'),
 (N'rimarkfluorescent',         N'Ritrama Ri-Mark Fluorescent Series',           N'Espesor 100 um'),
 (N'rimarkoptima',              N'Ritrama Ri-Mark Optima Series',                N'Espesor 75 um'),
 (N'rimarksandblast',           N'Ritrama Ri-Mark Sandblast Series',             N'Espesor 280 um');


INSERT INTO #Catalogo (Product_ID, Product_Name, Product_Descrip) VALUES
 (N'rimarktranslucent',         N'Ritrama Ri-Mark Translucent Series',           N'Espesor 80 um'),
 (N'rimarktransparent',         N'Ritrama Ri-Mark Transparent Series',           N'Espesor 80 um'),
 (N'rimask',                    N'Ritrama Ri-Mask Series',                       N'Espesor 70 um'),
 (N'rimountmonotape930',        N'Ritrama Ri-Mount Monotape 930 Series',         N'Espesor 23 um'),
 (N'rimountmonotape940',        N'Ritrama Ri-Mount Monotape 940 Series',         N'Espesor 12 um'),
 (N'rimountmonotape950',        N'Ritrama Ri-Mount Monotape 950 Series',         N'Espesor 12 um'),
 (N'rimountpaper60',            N'Ritrama Ri-Mount Paper 60 Series',             N'Espesor 60 um'),
 (N'rimountpet12',              N'Ritrama Ri-Mount PET12 Series',                N'Espesor 12 um'),
 (N'rimountultracrystalline',   N'Ritrama Ri-Mount Ultracrystalline Series',     N'Espesor 30 um'),
 (N'riscreenalufoil',           N'Ritrama Ri-Screen Alu foil Series',            N'Espesor 40 um'),
 (N'riscreenm100perm',          N'Ritrama Ri-Screen M100 Perm Series',           N'Espesor 100 um'),
 (N'riscreenm190',              N'Ritrama Ri-Screen M190 Series',                N'Espesor 190 um'),
 (N'riscreenm200',              N'Ritrama Ri-Screen M200 Series',                N'Espesor 200 um'),
 (N'riscreenm70colours',        N'Ritrama Ri-Screen M70 colours Series',         N'Espesor 70 um'),
 (N'riscreenm80',               N'Ritrama Ri-Screen M80 Series',                 N'Espesor 80 um'),
 (N'riscreenp75htsb',           N'Ritrama Ri-Screen P75 HT SB Series',           N'Espesor 75 um'),
 (N'riscreenp80phthalatefree',  N'Ritrama Ri-Screen P80 Phthalate Free Series',  N'Espesor 80 um'),
 (N'riscreenpetperm',           N'Ritrama Ri-Screen PET Perm Series',            N'Espesor 23 um'),
 (N'riscreenphotoluminescent',  N'Ritrama Ri-Screen Photoluminescent Series',    N'Espesor 270 um'),
 (N'riscreenpp60',              N'Ritrama Ri-Screen PP60 Series',                N'Espesor 60 um'),
 (N'riscreenrasoacetate',       N'Ritrama Ri-Screen Raso Acetate Series',        N'Espesor 80 um'),
 (N'riwrapppf',                 N'Ritrama Ri-Wrap PPF Series',                   N'Espesor 150 um'),
 (N'riwrapppfprem',             N'Ritrama Ri-Wrap PPF Premium Series',           N'Frontal PU, durabilidad 8 anos'),
 (N'rijetm100remPT',            N'Ritrama Ri-Jet M100 Rem SB Public Transport Series', N'Espesor 100 um, durabilidad 3 anos');


-- ============================================================================
-- 2) VALIDACIONES + PLAN (antes de tocar nada)
-- ============================================================================

-- SKU repetido dentro del catalogo del script.
INSERT INTO #Error (Detalle)
SELECT 'SKU duplicado en el catalogo: ' + Product_ID
FROM #Catalogo
GROUP BY Product_ID
HAVING COUNT(*) > 1;

-- SKU demasiado largo para las columnas reales de la tabla:
-- Product_ID nvarchar(25), Product_Name/Product_Descrip nvarchar(200).
INSERT INTO #Error (Detalle)
SELECT 'codigo demasiado largo para Product_ID (25): ' + Product_ID
FROM #Catalogo
WHERE LEN(Product_ID) > 25;

INSERT INTO #Error (Detalle)
SELECT 'nombre demasiado largo para Product_Name (200): ' + Product_ID
FROM #Catalogo
WHERE LEN(Product_Name) > 200;

INSERT INTO #Error (Detalle)
SELECT 'descripcion demasiada larga para Product_Descrip (200): ' + Product_ID
FROM #Catalogo
WHERE LEN(Product_Descrip) > 200;

-- El ID ya existe en la tabla con OTRA categoria: no se pisa nunca.
INSERT INTO #Error (Detalle)
SELECT 'el codigo ' + c.Product_ID + ' ya existe y NO es Graphics (Product_Name: ' + ISNULL(x.Product_Name, N'') + ')'
FROM #Catalogo c
INNER JOIN dbo.producto x ON x.Product_ID = c.Product_ID
WHERE x.Graphics = 0;

-- Los que ya estan cargados como Graphics no se reinsertan (idempotencia).
INSERT INTO #Existe (Product_ID)
SELECT c.Product_ID
FROM #Catalogo c
INNER JOIN dbo.producto x ON x.Product_ID = c.Product_ID AND x.Graphics = 1;

DECLARE @NTotal int, @NCrear int, @NExiste int, @NErrores int;

SELECT @NTotal = COUNT(*) FROM #Catalogo;
-- Si hay errores el script aborta antes de escribir, asi que "a crear" es
-- simplemente lo que no esta ya cargado como Graphics.
SELECT @NCrear = COUNT(*) FROM #Catalogo
WHERE Product_ID NOT IN (SELECT Product_ID FROM #Existe);
SELECT @NExiste = COUNT(*) FROM #Existe;
SELECT @NErrores = COUNT(*) FROM #Error;

PRINT '=========== PLAN ===========';
PRINT 'Series en el catalogo (web) .......... ' + CAST(@NTotal AS nvarchar(10));
PRINT 'Graphics a crear ..................... ' + CAST(@NCrear AS nvarchar(10));
PRINT 'Graphics ya existentes (se omiten) ... ' + CAST(@NExiste AS nvarchar(10));
PRINT 'Errores .............................. ' + CAST(@NErrores AS nvarchar(10));

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
     , c.Product_Descrip
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

    -- 3a) INSERT de los Graphics nuevos. Los que ya existian como Graphics no
    --     se tocan (#Existe); los que chocan con otra categoria ni llegan aqui
    --     (lo aborta la validacion de la seccion 2).
    INSERT INTO dbo.producto
        (Product_ID, Product_Name, Product_Descrip, Product_Ref, Codebar, Category_ID,
         MasterRolls, rollo_cortado, Resmas, Graphics, anulado,
         precio, code_RC, ratio, unidad, cantidad, width, lenght, msi)
    SELECT c.Product_ID, c.Product_Name, c.Product_Descrip,
           -- Product_Ref es nvarchar(20): solo se copia el SKU si cabe.
           CASE WHEN LEN(c.Product_ID) <= 20 THEN c.Product_ID ELSE NULL END,
           NULL, N'0',
           0, 0, 0, 1, 0,
           0, NULL, 0, NULL, NULL, NULL, NULL, NULL
    FROM #Catalogo c
    WHERE NOT EXISTS (SELECT 1 FROM dbo.producto x
                      WHERE x.Product_ID = c.Product_ID AND x.Graphics = 1);

    SET @Insertados = @@ROWCOUNT;
    PRINT '';
    PRINT 'Graphics insertados: ' + CAST(@Insertados AS nvarchar(10));

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

    -- Series del catalogo que no quedaron cargadas (debe dar 0 filas).
    SELECT c.Product_ID AS sin_cargar, c.Product_Name
    FROM #Catalogo c
    WHERE NOT EXISTS (SELECT 1 FROM dbo.producto x
                      WHERE x.Product_ID = c.Product_ID AND x.Graphics = 1)
    ORDER BY c.Product_ID;

    -- Graphics con la categoria inconsistente (debe dar 0).
    SELECT COUNT(*) AS graphics_inconsistentes
    FROM dbo.producto p
    WHERE p.Graphics = 1
      AND (ISNULL(CAST(p.MasterRolls AS int), 0) + ISNULL(CAST(p.rollo_cortado AS int), 0)
         + ISNULL(CAST(p.Resmas AS int), 0) + ISNULL(CAST(p.Graphics AS int), 0) <> 1
           OR p.MasterRolls = 1 OR p.rollo_cortado = 1 OR p.Resmas = 1);

    -- Productos con la categoria inconsistente en total (debe dar 0).
    SELECT COUNT(*) AS productos_sin_un_bit
    FROM dbo.producto p
    WHERE ISNULL(CAST(p.MasterRolls AS int), 0) + ISNULL(CAST(p.rollo_cortado AS int), 0)
        + ISNULL(CAST(p.Resmas AS int), 0) + ISNULL(CAST(p.Graphics AS int), 0) <> 1;

    -- Muestra de los Graphics cargados.
    SELECT TOP 15 Product_ID, LEFT(Product_Name, 45) AS nombre, Product_Descrip, Graphics, anulado
    FROM dbo.producto
    WHERE Graphics = 1
    ORDER BY Product_ID;

    IF EXISTS (SELECT 1 FROM #Catalogo c
               WHERE NOT EXISTS (SELECT 1 FROM dbo.producto x
                                 WHERE x.Product_ID = c.Product_ID AND x.Graphics = 1))
    BEGIN
        ;THROW 50002, 'Quedaron series del catalogo sin cargar: se revirtio todo.', 1;
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
-- Borra los Graphics cargados por este script. Antes de esto habia 0 Graphics
-- en la tabla, asi que basta con borrar todos los que tengan Graphics = 1.
-- Si despues se cargan Graphics de otra fuente, restringir a esos SKU.
--
-- BEGIN TRANSACTION;
--
-- DELETE FROM dbo.producto WHERE Graphics = 1;
--
-- COMMIT;

