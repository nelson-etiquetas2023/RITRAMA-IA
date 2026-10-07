-- ============================================================================
-- 35_Migrar_Maestros_Desde_Legacy.sql (Ritrama2025)
-- Que hace: trae productos, clientes, proveedores y vendedores desde la BD de
--   la version anterior a ritrama2026 vacia. Re-ejecutable: solo inserta lo
--   que falta (por PK) y nunca duplica ni pisa datos existentes.
-- Prerequisitos:
--   1. La legacy restaurada en LA MISMA instancia SQL 2017 (3-part names).
--      Ej: RESTORE DATABASE [RITRAMASQL2017] FROM DISK = 'E:\...bak' ...
--   2. Esquema destino aplicado (10_Desplegar_Esquema.ps1) + baseline legacy.
-- Uso (LOCAL en el server):
--   sqlcmd -S RITRAMASRV01 -d ritrama2026 -E -C -b ^
--     -v SourceDB="RITRAMASQL2017" -i Scripts\Deploy\35_Migrar_Maestros_Desde_Legacy.sql
-- Tolerante a version anterior: solo exige PK + nombre; las columnas nuevas
--   (persona_contacto, zona, costo, categoria, codigo_interno, direcciones
--   nuevas) se copian si existen y quedan NULL si no. direccion_facturacion /
--   direccion_entrega heredan de Customer_Dir/customer_address como hizo
--   Migrar_Direcciones_Cliente.sql. codigo_interno se asigna 1..N por nombre
--   y se sincroniza control CLI/PROV/VEND.
-- ============================================================================

:setvar SourceDB "RITRAMASQL2017"

SET NOCOUNT ON;
SET XACT_ABORT ON;
SET QUOTED_IDENTIFIER ON;
GO

PRINT 'Origen legacy: [$(SourceDB)]  Destino: ritrama2026';
GO

-- ============================================================================
-- BLOQUE 1: customer (clave customer_id, codigo_interno CLI)
-- ============================================================================
IF OBJECT_ID(N'[$(SourceDB)].dbo.customer', N'U') IS NULL
BEGIN
    ;THROW 50001, 'Falta [$(SourceDB)].dbo.customer. Restaura la legacy en la misma instancia.', 1;
END
GO

IF OBJECT_ID(N'dbo.customer', N'U') IS NULL
BEGIN
    ;THROW 50002, 'Falta dbo.customer en destino. Corre el 10 primero.', 1;
END
GO

SELECT * INTO #src_cli FROM [$(SourceDB)].dbo.customer;
GO

IF NOT EXISTS (SELECT 1 FROM tempdb.sys.columns WHERE object_id = OBJECT_ID(N'tempdb..#src_cli') AND name = 'customer_id')
BEGIN
    ;THROW 50003, 'Legacy customer sin customer_id.', 1;
END
GO

IF NOT EXISTS (SELECT 1 FROM tempdb.sys.columns WHERE object_id = OBJECT_ID(N'tempdb..#src_cli') AND name = 'customer_name')
BEGIN
    ;THROW 50003, 'Legacy customer sin customer_name.', 1;
END
GO

IF EXISTS (SELECT 1 FROM #src_cli WHERE LEN(customer_name) > 100)
BEGIN
    ;THROW 50004, 'Hay customer_name de mas de 100 caracteres. Revisar antes de migrar.', 1;
END
GO

DECLARE @c NVARCHAR(MAX) = N'', @s NVARCHAR(MAX) = N'';
DECLARE @has BIT;

-- Requeridas
SET @c += N'customer_id,';            SET @s += N's.customer_id,';
SET @c += N'customer_name,';          SET @s += N's.customer_name,';

-- Opcionales mismo nombre (NULL si no existen en la legacy)
DECLARE @opt TABLE (col sysname NOT NULL);
INSERT INTO @opt VALUES
    (N'identificacion'), (N'empresa'), (N'customer_category'), (N'phone'),
    (N'contacto'), (N'persona_contacto'), (N'customer_email'), (N'condicion_pago'),
    (N'impuesto'), (N'unity1'), (N'unity2');
DECLARE @col sysname;
DECLARE cur CURSOR LOCAL FAST_FORWARD FOR SELECT col FROM @opt;
OPEN cur; FETCH NEXT FROM cur INTO @col;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @c += QUOTENAME(@col) + N',';
    IF EXISTS (SELECT 1 FROM tempdb.sys.columns WHERE object_id = OBJECT_ID(N'tempdb..#src_cli') AND name = @col)
        SET @s += N's.' + QUOTENAME(@col) + N',';
    ELSE
        SET @s += N'NULL,';
    FETCH NEXT FROM cur INTO @col;
END
CLOSE cur; DEALLOCATE cur;

-- direccion_facturacion / direccion_entrega con herencia legacy
DECLARE @dirExpr NVARCHAR(500);
IF EXISTS (SELECT 1 FROM tempdb.sys.columns WHERE object_id = OBJECT_ID(N'tempdb..#src_cli') AND name = 'direccion_facturacion')
    SET @dirExpr = N's.direccion_facturacion,';
ELSE IF EXISTS (SELECT 1 FROM tempdb.sys.columns WHERE object_id = OBJECT_ID(N'tempdb..#src_cli') AND name = 'Customer_Dir')
    SET @dirExpr = N's.Customer_Dir,';
ELSE IF EXISTS (SELECT 1 FROM tempdb.sys.columns WHERE object_id = OBJECT_ID(N'tempdb..#src_cli') AND name = 'customer_address')
    SET @dirExpr = N's.customer_address,';
ELSE
    SET @dirExpr = N'NULL,';
SET @c += N'direccion_facturacion,'; SET @s += @dirExpr;

IF EXISTS (SELECT 1 FROM tempdb.sys.columns WHERE object_id = OBJECT_ID(N'tempdb..#src_cli') AND name = 'direccion_entrega')
    SET @dirExpr = N's.direccion_entrega,';
ELSE IF EXISTS (SELECT 1 FROM tempdb.sys.columns WHERE object_id = OBJECT_ID(N'tempdb..#src_cli') AND name = 'Customer_Dir')
    SET @dirExpr = N's.Customer_Dir,';
ELSE IF EXISTS (SELECT 1 FROM tempdb.sys.columns WHERE object_id = OBJECT_ID(N'tempdb..#src_cli') AND name = 'customer_address')
    SET @dirExpr = N's.customer_address,';
ELSE
    SET @dirExpr = N'NULL,';
SET @c += N'direccion_entrega,'; SET @s += @dirExpr;

SET @c += N'anulado,';
IF EXISTS (SELECT 1 FROM tempdb.sys.columns WHERE object_id = OBJECT_ID(N'tempdb..#src_cli') AND name = 'anulado')
    SET @s += N's.anulado,';
ELSE
    SET @s += N'0,';

SET @c = LEFT(@c, LEN(@c) - 1);
SET @s = LEFT(@s, LEN(@s) - 1);

-- COLLATE solo si la PK es texto (uniqueidentifier no lo admite).
DECLARE @cmp NVARCHAR(300);
IF EXISTS (SELECT 1 FROM tempdb.sys.columns
           WHERE object_id = OBJECT_ID(N'tempdb..#src_cli') AND name = 'customer_id'
             AND system_type_id IN (167, 175, 231, 239, 35, 99))
    SET @cmp = N't.customer_id = s.customer_id COLLATE DATABASE_DEFAULT';
ELSE
    SET @cmp = N't.customer_id = s.customer_id';

DECLARE @sql NVARCHAR(MAX) = N'INSERT INTO dbo.customer (' + @c + N') ' +
    N'SELECT ' + @s + N' FROM #src_cli s ' +
    N'WHERE NOT EXISTS (SELECT 1 FROM dbo.customer t WHERE ' + @cmp + N');';
EXEC sp_executesql @sql;
PRINT 'customer migrados (esta pasada): ' + CAST(@@ROWCOUNT AS NVARCHAR(20));
GO

-- codigo_interno 1..N por nombre a los que quedaron NULL + sync control CLI
DECLARE @baseCli INT = ISNULL((SELECT MAX(codigo_interno) FROM dbo.customer), 0);
;WITH N AS (
    SELECT codigo_interno, ROW_NUMBER() OVER (ORDER BY customer_name, customer_id) AS rn
    FROM dbo.customer WHERE codigo_interno IS NULL
)
UPDATE N SET codigo_interno = @baseCli + rn;
IF NOT EXISTS (SELECT 1 FROM dbo.control WHERE filter = N'CLI')
    INSERT INTO dbo.control (filter, par1) VALUES (N'CLI', N'0');
UPDATE dbo.control
SET par1 = CAST(ISNULL((SELECT MAX(codigo_interno) FROM dbo.customer), 0) AS NVARCHAR(100))
WHERE filter = N'CLI';
SELECT 'customer' AS tabla,
    (SELECT COUNT(*) FROM [$(SourceDB)].dbo.customer) AS en_origen,
    (SELECT COUNT(*) FROM dbo.customer) AS en_destino,
    (SELECT COUNT(*) FROM dbo.customer WHERE codigo_interno IS NULL) AS sin_interno,
    (SELECT par1 FROM dbo.control WHERE filter = N'CLI') AS control_CLI;
GO

DROP TABLE #src_cli;
GO

-- ============================================================================
-- BLOQUE 2: provider (clave Proveedor_Id, codigo_interno PROV)
-- ============================================================================
IF OBJECT_ID(N'[$(SourceDB)].dbo.provider', N'U') IS NULL
BEGIN
    ;THROW 50001, 'Falta [$(SourceDB)].dbo.provider. Restaura la legacy en la misma instancia.', 1;
END
GO

IF OBJECT_ID(N'dbo.provider', N'U') IS NULL
BEGIN
    ;THROW 50002, 'Falta dbo.provider en destino. Corre el 10 primero (baseline).', 1;
END
GO

SELECT * INTO #src_prov FROM [$(SourceDB)].dbo.provider;
GO

IF NOT EXISTS (SELECT 1 FROM tempdb.sys.columns WHERE object_id = OBJECT_ID(N'tempdb..#src_prov') AND name = 'Proveedor_Id')
BEGIN
    ;THROW 50003, 'Legacy provider sin Proveedor_Id.', 1;
END
GO

IF NOT EXISTS (SELECT 1 FROM tempdb.sys.columns WHERE object_id = OBJECT_ID(N'tempdb..#src_prov') AND name = 'Proveedor_Name')
BEGIN
    ;THROW 50003, 'Legacy provider sin Proveedor_Name.', 1;
END
GO

IF EXISTS (SELECT 1 FROM #src_prov WHERE LEN(Proveedor_Name) > 100)
BEGIN
    ;THROW 50004, 'Hay Proveedor_Name de mas de 100 caracteres. Revisar antes de migrar.', 1;
END
GO

DECLARE @c2 NVARCHAR(MAX) = N'Proveedor_Id,Proveedor_Name,', @s2 NVARCHAR(MAX) = N's.Proveedor_Id,s.Proveedor_Name,';
DECLARE @opt2 TABLE (col sysname NOT NULL, def NVARCHAR(50) NOT NULL);
INSERT INTO @opt2 VALUES
    (N'phone', N'NULL'), (N'persona_contacto', N'NULL'), (N'direccion', N'NULL'),
    (N'email', N'NULL'), (N'unidad_master_1', N'NULL'), (N'unidad_master_2', N'NULL'),
    (N'anulado', N'0'), (N'categoria', N'NULL'), (N'direccion_entrega', N'NULL');
DECLARE @col2 sysname, @def2 NVARCHAR(50);
DECLARE cur2 CURSOR LOCAL FAST_FORWARD FOR SELECT col, def FROM @opt2;
OPEN cur2; FETCH NEXT FROM cur2 INTO @col2, @def2;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @c2 += QUOTENAME(@col2) + N',';
    IF EXISTS (SELECT 1 FROM tempdb.sys.columns WHERE object_id = OBJECT_ID(N'tempdb..#src_prov') AND name = @col2)
        SET @s2 += N's.' + QUOTENAME(@col2) + N',';
    ELSE
        SET @s2 += @def2 + N',';
    FETCH NEXT FROM cur2 INTO @col2, @def2;
END
CLOSE cur2; DEALLOCATE cur2;
SET @c2 = LEFT(@c2, LEN(@c2) - 1);
SET @s2 = LEFT(@s2, LEN(@s2) - 1);

DECLARE @cmp2 NVARCHAR(300);
IF EXISTS (SELECT 1 FROM tempdb.sys.columns
           WHERE object_id = OBJECT_ID(N'tempdb..#src_prov') AND name = 'Proveedor_Id'
             AND system_type_id IN (167, 175, 231, 239, 35, 99))
    SET @cmp2 = N't.Proveedor_Id = s.Proveedor_Id COLLATE DATABASE_DEFAULT';
ELSE
    SET @cmp2 = N't.Proveedor_Id = s.Proveedor_Id';

DECLARE @sql2 NVARCHAR(MAX) = N'INSERT INTO dbo.provider (' + @c2 + N') ' +
    N'SELECT ' + @s2 + N' FROM #src_prov s ' +
    N'WHERE NOT EXISTS (SELECT 1 FROM dbo.provider t WHERE ' + @cmp2 + N');';
EXEC sp_executesql @sql2;
PRINT 'provider migrados (esta pasada): ' + CAST(@@ROWCOUNT AS NVARCHAR(20));
GO

DECLARE @baseProv INT = ISNULL((SELECT MAX(codigo_interno) FROM dbo.provider), 0);
;WITH N AS (
    SELECT codigo_interno, ROW_NUMBER() OVER (ORDER BY Proveedor_Name, Proveedor_Id) AS rn
    FROM dbo.provider WHERE codigo_interno IS NULL
)
UPDATE N SET codigo_interno = @baseProv + rn;
IF NOT EXISTS (SELECT 1 FROM dbo.control WHERE filter = N'PROV')
    INSERT INTO dbo.control (filter, par1) VALUES (N'PROV', N'0');
UPDATE dbo.control
SET par1 = CAST(ISNULL((SELECT MAX(codigo_interno) FROM dbo.provider), 0) AS NVARCHAR(100))
WHERE filter = N'PROV';
SELECT 'provider' AS tabla,
    (SELECT COUNT(*) FROM [$(SourceDB)].dbo.provider) AS en_origen,
    (SELECT COUNT(*) FROM dbo.provider) AS en_destino,
    (SELECT COUNT(*) FROM dbo.provider WHERE codigo_interno IS NULL) AS sin_interno,
    (SELECT par1 FROM dbo.control WHERE filter = N'PROV') AS control_PROV;
GO

DROP TABLE #src_prov;
GO

-- ============================================================================
-- BLOQUE 3: vendedor (clave vendor_id, codigo_interno VEND)
-- ============================================================================
IF OBJECT_ID(N'[$(SourceDB)].dbo.vendedor', N'U') IS NULL
BEGIN
    ;THROW 50001, 'Falta [$(SourceDB)].dbo.vendedor. Restaura la legacy en la misma instancia.', 1;
END
GO

IF OBJECT_ID(N'dbo.vendedor', N'U') IS NULL
BEGIN
    ;THROW 50002, 'Falta dbo.vendedor en destino. Corre el 10 primero (baseline).', 1;
END
GO

SELECT * INTO #src_vend FROM [$(SourceDB)].dbo.vendedor;
GO

IF NOT EXISTS (SELECT 1 FROM tempdb.sys.columns WHERE object_id = OBJECT_ID(N'tempdb..#src_vend') AND name = 'vendor_id')
BEGIN
    ;THROW 50003, 'Legacy vendedor sin vendor_id.', 1;
END
GO

IF NOT EXISTS (SELECT 1 FROM tempdb.sys.columns WHERE object_id = OBJECT_ID(N'tempdb..#src_vend') AND name = 'vendor_name')
BEGIN
    ;THROW 50003, 'Legacy vendedor sin vendor_name.', 1;
END
GO

IF EXISTS (SELECT 1 FROM #src_vend WHERE LEN(vendor_name) > 100)
BEGIN
    ;THROW 50004, 'Hay vendor_name de mas de 100 caracteres. Revisar antes de migrar.', 1;
END
GO

DECLARE @c3 NVARCHAR(MAX) = N'vendor_id,vendor_name,', @s3 NVARCHAR(MAX) = N's.vendor_id,s.vendor_name,';
DECLARE @opt3 TABLE (col sysname NOT NULL, def NVARCHAR(50) NOT NULL);
INSERT INTO @opt3 VALUES
    (N'correo', N'NULL'), (N'phone', N'NULL'), (N'zona', N'NULL'), (N'anulado', N'0');
DECLARE @col3 sysname, @def3 NVARCHAR(50);
DECLARE cur3 CURSOR LOCAL FAST_FORWARD FOR SELECT col, def FROM @opt3;
OPEN cur3; FETCH NEXT FROM cur3 INTO @col3, @def3;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @c3 += QUOTENAME(@col3) + N',';
    IF EXISTS (SELECT 1 FROM tempdb.sys.columns WHERE object_id = OBJECT_ID(N'tempdb..#src_vend') AND name = @col3)
        SET @s3 += N's.' + QUOTENAME(@col3) + N',';
    ELSE
        SET @s3 += @def3 + N',';
    FETCH NEXT FROM cur3 INTO @col3, @def3;
END
CLOSE cur3; DEALLOCATE cur3;
SET @c3 = LEFT(@c3, LEN(@c3) - 1);
SET @s3 = LEFT(@s3, LEN(@s3) - 1);

DECLARE @cmp3 NVARCHAR(300);
IF EXISTS (SELECT 1 FROM tempdb.sys.columns
           WHERE object_id = OBJECT_ID(N'tempdb..#src_vend') AND name = 'vendor_id'
             AND system_type_id IN (167, 175, 231, 239, 35, 99))
    SET @cmp3 = N't.vendor_id = s.vendor_id COLLATE DATABASE_DEFAULT';
ELSE
    SET @cmp3 = N't.vendor_id = s.vendor_id';

DECLARE @sql3 NVARCHAR(MAX) = N'INSERT INTO dbo.vendedor (' + @c3 + N') ' +
    N'SELECT ' + @s3 + N' FROM #src_vend s ' +
    N'WHERE NOT EXISTS (SELECT 1 FROM dbo.vendedor t WHERE ' + @cmp3 + N');';
EXEC sp_executesql @sql3;
PRINT 'vendedor migrados (esta pasada): ' + CAST(@@ROWCOUNT AS NVARCHAR(20));
GO

DECLARE @baseVend INT = ISNULL((SELECT MAX(codigo_interno) FROM dbo.vendedor), 0);
;WITH N AS (
    SELECT codigo_interno, ROW_NUMBER() OVER (ORDER BY vendor_name, vendor_id) AS rn
    FROM dbo.vendedor WHERE codigo_interno IS NULL
)
UPDATE N SET codigo_interno = @baseVend + rn;
IF NOT EXISTS (SELECT 1 FROM dbo.control WHERE filter = N'VEND')
    INSERT INTO dbo.control (filter, par1) VALUES (N'VEND', N'0');
UPDATE dbo.control
SET par1 = CAST(ISNULL((SELECT MAX(codigo_interno) FROM dbo.vendedor), 0) AS NVARCHAR(100))
WHERE filter = N'VEND';
SELECT 'vendedor' AS tabla,
    (SELECT COUNT(*) FROM [$(SourceDB)].dbo.vendedor) AS en_origen,
    (SELECT COUNT(*) FROM dbo.vendedor) AS en_destino,
    (SELECT COUNT(*) FROM dbo.vendedor WHERE codigo_interno IS NULL) AS sin_interno,
    (SELECT par1 FROM dbo.control WHERE filter = N'VEND') AS control_VEND;
GO

DROP TABLE #src_vend;
GO

-- ============================================================================
-- BLOQUE 4: producto (clave Product_ID, sin codigo_interno)
-- ============================================================================
IF OBJECT_ID(N'[$(SourceDB)].dbo.producto', N'U') IS NULL
BEGIN
    ;THROW 50001, 'Falta [$(SourceDB)].dbo.producto. Restaura la legacy en la misma instancia.', 1;
END
GO

IF OBJECT_ID(N'dbo.producto', N'U') IS NULL
BEGIN
    ;THROW 50002, 'Falta dbo.producto en destino. Corre el 10 primero (baseline).', 1;
END
GO

SELECT * INTO #src_prod FROM [$(SourceDB)].dbo.producto;
GO

IF NOT EXISTS (SELECT 1 FROM tempdb.sys.columns WHERE object_id = OBJECT_ID(N'tempdb..#src_prod') AND name = 'Product_ID')
BEGIN
    ;THROW 50003, 'Legacy producto sin Product_ID.', 1;
END
GO

IF NOT EXISTS (SELECT 1 FROM tempdb.sys.columns WHERE object_id = OBJECT_ID(N'tempdb..#src_prod') AND name = 'Product_Name')
BEGIN
    ;THROW 50003, 'Legacy producto sin Product_Name.', 1;
END
GO

DECLARE @c4 NVARCHAR(MAX) = N'', @s4 NVARCHAR(MAX) = N'';
DECLARE @opt4 TABLE (col sysname NOT NULL, def NVARCHAR(50) NOT NULL);
INSERT INTO @opt4 VALUES
    (N'Product_ID', N''), (N'Product_Name', N''),
    (N'Product_Descrip', N'NULL'), (N'Product_Ref', N'NULL'),
    (N'Codebar', N'NULL'), (N'category_id', N'NULL'),
    (N'MasterRolls', N'0'), (N'rollo_cortado', N'0'),
    (N'Resmas', N'0'), (N'Graphics', N'0'), (N'anulado', N'0'),
    (N'precio', N'NULL'), (N'costo', N'NULL'), (N'code_rc', N'NULL'),
    (N'ratio', N'NULL'), (N'unidad', N'NULL'), (N'cantidad', N'NULL'),
    (N'width', N'NULL'), (N'lenght', N'NULL'), (N'msi', N'NULL');
DECLARE @col4 sysname, @def4 NVARCHAR(50);
DECLARE cur4 CURSOR LOCAL FAST_FORWARD FOR SELECT col, def FROM @opt4;
OPEN cur4;
FETCH NEXT FROM cur4 INTO @col4, @def4;
WHILE @@FETCH_STATUS = 0
BEGIN
    IF EXISTS (SELECT 1 FROM tempdb.sys.columns WHERE object_id = OBJECT_ID(N'tempdb..#src_prod') AND name = @col4)
       AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.producto') AND name = @col4)
    BEGIN
        SET @c4 += QUOTENAME(@col4) + N',';
        SET @s4 += N's.' + QUOTENAME(@col4) + N',';
    END
    IF NOT EXISTS (SELECT 1 FROM tempdb.sys.columns WHERE object_id = OBJECT_ID(N'tempdb..#src_prod') AND name = @col4)
       AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.producto') AND name = @col4)
    BEGIN
        -- Existe en destino pero no en la legacy: va con default.
        SET @c4 += QUOTENAME(@col4) + N',';
        SET @s4 += @def4 + N',';
    END
    -- Si no existe en ninguna: se omite.
    FETCH NEXT FROM cur4 INTO @col4, @def4;
END
CLOSE cur4; DEALLOCATE cur4;
SET @c4 = LEFT(@c4, LEN(@c4) - 1);
SET @s4 = LEFT(@s4, LEN(@s4) - 1);

DECLARE @cmp4 NVARCHAR(300);
IF EXISTS (SELECT 1 FROM tempdb.sys.columns
           WHERE object_id = OBJECT_ID(N'tempdb..#src_prod') AND name = 'Product_ID'
             AND system_type_id IN (167, 175, 231, 239, 35, 99))
    SET @cmp4 = N't.Product_ID = s.Product_ID COLLATE DATABASE_DEFAULT';
ELSE
    SET @cmp4 = N't.Product_ID = s.Product_ID';

DECLARE @sql4 NVARCHAR(MAX) = N'INSERT INTO dbo.producto (' + @c4 + N') ' +
    N'SELECT ' + @s4 + N' FROM #src_prod s ' +
    N'WHERE NOT EXISTS (SELECT 1 FROM dbo.producto t WHERE ' + @cmp4 + N');';
EXEC sp_executesql @sql4;
PRINT 'producto migrados (esta pasada): ' + CAST(@@ROWCOUNT AS NVARCHAR(20));
GO

SELECT 'producto' AS tabla,
    (SELECT COUNT(*) FROM [$(SourceDB)].dbo.producto) AS en_origen,
    (SELECT COUNT(*) FROM dbo.producto) AS en_destino;
GO

DROP TABLE #src_prod;
GO

PRINT '=== 35_Migrar_Maestros completado. Verifica en_origen vs en_destino y sin_interno = 0. ===';
GO
