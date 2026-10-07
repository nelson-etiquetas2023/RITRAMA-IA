-- ============================================================================
-- 02_Clonar_Esquema_Desde_Legacy.sql (Ritrama2025)
-- Que hace: crea ritrama2026 si no existe (COMPAT 140) y le copia TODA la
--   estructura de la BD anterior SIN datos: schemas, tablas (tipos, nulos,
--   identity, defaults, collation), PKs, FKs, indices, vistas, procedures,
--   funciones y triggers. Un solo script, una sola corrida, por consola.
-- Uso (LOCAL en el server/VM):
--   sqlcmd -S localhost -E -C -b -v SourceDB="RITRAMASQL2017" -i Scripts\Deploy\02_Clonar_Esquema_Desde_Legacy.sql
--   (cambia -S por RITRAMASRV01 en el server real)
-- Requiere: origen y destino en LA MISMA instancia 2017. Re-ejecutable: lo que
--   ya existe se salta con PRINT. No toca datos.
-- Despues de este: 05 -> 10 -> 20 -> 35 -> 36 -> 30.
-- ============================================================================

:setvar SourceDB "RITRAMASQL2017"
:setvar TargetDB "ritrama2026"

SET NOCOUNT ON;
SET XACT_ABORT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

PRINT 'Origen: [$(SourceDB)]  Destino: [$(TargetDB)]';
GO

-- ---- 0) Base destino ---------------------------------------------------------
IF DB_ID(N'$(TargetDB)') IS NULL
BEGIN
    PRINT 'Creando base [$(TargetDB)]...';
    EXEC (N'CREATE DATABASE [$(TargetDB)] COLLATE Modern_Spanish_CI_AS;');
END
ELSE
    PRINT 'La base [$(TargetDB)] ya existe (no se borra nada).';
GO

IF EXISTS (SELECT 1 FROM sys.databases WHERE name = N'$(TargetDB)' AND compatibility_level <> 140)
    EXEC (N'ALTER DATABASE [$(TargetDB)] SET COMPATIBILITY_LEVEL = 140;');
GO

IF DB_ID(N'$(SourceDB)') IS NULL
BEGIN
    ;THROW 50001, 'No se ve [$(SourceDB)] desde esta instancia. Restaura la legacy aqui primero.', 1;
END
GO

-- ---- 1) Schemas de usuario ---------------------------------------------------
DECLARE @sch sysname;
DECLARE curS CURSOR LOCAL FAST_FORWARD FOR
    SELECT name FROM [$(SourceDB)].sys.schemas
    WHERE schema_id > 4 AND name NOT IN (N'guest', N'INFORMATION_SCHEMA', N'sys')
      AND name NOT LIKE N'db[_]%';
OPEN curS; FETCH NEXT FROM curS INTO @sch;
WHILE @@FETCH_STATUS = 0
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [$(TargetDB)].sys.schemas WHERE name = @sch)
    BEGIN
        DECLARE @csql NVARCHAR(400) = N'CREATE SCHEMA ' + QUOTENAME(@sch) + N';';
        EXEC [$(TargetDB)].sys.sp_executesql @csql;
        PRINT 'Schema creado: ' + @sch;
    END
    FETCH NEXT FROM curS INTO @sch;
END
CLOSE curS; DEALLOCATE curS;
GO

-- ---- 2) Tablas (columnas + identity + defaults + collation) -------------------
DECLARE @t NVARCHAR(261), @sch2 sysname, @tab sysname, @oid INT;
DECLARE @create NVARCHAR(MAX), @cols NVARCHAR(MAX);
DECLARE curT CURSOR LOCAL FAST_FORWARD FOR
    SELECT QUOTENAME(SCHEMA_NAME(t.schema_id)) + N'.' + QUOTENAME(t.name),
           SCHEMA_NAME(t.schema_id), t.name, t.object_id
    FROM [$(SourceDB)].sys.tables t ORDER BY SCHEMA_NAME(t.schema_id), t.name;
OPEN curT; FETCH NEXT FROM curT INTO @t, @sch2, @tab, @oid;
WHILE @@FETCH_STATUS = 0
BEGIN
    IF OBJECT_ID(N'[$(TargetDB)].' + @t, N'U') IS NULL
    BEGIN
        SET @cols = N'';
        SELECT @cols += N', ' + QUOTENAME(c.name) + N' ' + UPPER(st.name) +
            CASE WHEN st.name IN (N'varchar', N'char', N'varbinary', N'binary')
                      THEN N'(' + CASE WHEN c.max_length = -1 THEN N'MAX' ELSE CAST(c.max_length AS NVARCHAR(10)) END + N')'
                 WHEN st.name IN (N'nvarchar', N'nchar')
                      THEN N'(' + CASE WHEN c.max_length = -1 THEN N'MAX' ELSE CAST(c.max_length / 2 AS NVARCHAR(10)) END + N')'
                 WHEN st.name IN (N'decimal', N'numeric')
                      THEN N'(' + CAST(c.precision AS NVARCHAR(10)) + N',' + CAST(c.scale AS NVARCHAR(10)) + N')'
                 WHEN st.name IN (N'datetime2', N'datetimeoffset', N'time')
                      THEN N'(' + CAST(c.scale AS NVARCHAR(10)) + N')'
                 ELSE N'' END +
            CASE WHEN st.name IN (N'char', N'varchar', N'nchar', N'nvarchar')
                      THEN N' COLLATE ' + c.collation_name ELSE N'' END +
            CASE WHEN c.is_identity = 1
                      THEN N' IDENTITY(' + CAST(IDENT_SEED(N'[$(SourceDB)].' + @t) AS NVARCHAR(20)) +
                           N',' + CAST(IDENT_INCR(N'[$(SourceDB)].' + @t) AS NVARCHAR(20)) + N')'
                 ELSE N'' END +
            CASE WHEN c.is_nullable = 0 THEN N' NOT NULL' ELSE N' NULL' END +
            ISNULL(N' DEFAULT ' + dc.definition, N'')
        FROM [$(SourceDB)].sys.columns c
        JOIN [$(SourceDB)].sys.types st ON st.system_type_id = c.system_type_id AND st.user_type_id = st.system_type_id
        LEFT JOIN [$(SourceDB)].sys.default_constraints dc
               ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
        WHERE c.object_id = @oid AND c.is_computed = 0
        ORDER BY c.column_id;

        -- Columnas calculadas
        SELECT @cols += N', ' + QUOTENAME(cc.name) + N' AS ' + cc.definition +
               CASE WHEN cc.is_persisted = 1 THEN N' PERSISTED' ELSE N'' END
        FROM [$(SourceDB)].sys.computed_columns cc
        WHERE cc.object_id = @oid
        ORDER BY cc.column_id;

        SET @create = N'CREATE TABLE [$(TargetDB)].' + @t + N' (' + STUFF(@cols, 1, 2, N'') + N');';
        EXEC [$(TargetDB)].sys.sp_executesql @create;
        PRINT 'Tabla creada: ' + @t;
    END
    ELSE
        PRINT 'Tabla ya existe (sin cambios): ' + @t;
    FETCH NEXT FROM curT INTO @t, @sch2, @tab, @oid;
END
CLOSE curT; DEALLOCATE curT;
GO

-- ---- 3) Primary Keys + Unique constraints -------------------------------------------
-- (los UNIQUE son candidatos para FKs: ej. pedido.numero es UQ, no PK)
DECLARE @pk NVARCHAR(MAX), @pkcols NVARCHAR(MAX), @pkname sysname, @pkoid INT, @pktab NVARCHAR(261);
DECLARE @pkidx INT, @pkisPK BIT;
DECLARE curP CURSOR LOCAL FAST_FORWARD FOR
    SELECT kc.name, kc.parent_object_id,
           QUOTENAME(SCHEMA_NAME(t.schema_id)) + N'.' + QUOTENAME(t.name),
           kc.unique_index_id, CASE WHEN kc.type = N'PK' THEN 1 ELSE 0 END
    FROM [$(SourceDB)].sys.key_constraints kc
    JOIN [$(SourceDB)].sys.tables t ON t.object_id = kc.parent_object_id;
OPEN curP; FETCH NEXT FROM curP INTO @pkname, @pkoid, @pktab, @pkidx, @pkisPK;
WHILE @@FETCH_STATUS = 0
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [$(TargetDB)].sys.key_constraints WHERE name = @pkname
                   AND parent_object_id = OBJECT_ID(N'[$(TargetDB)].' + @pktab, N'U'))
    BEGIN
        SET @pkcols = N'';
        SELECT @pkcols += N', ' + QUOTENAME(c.name) + CASE WHEN ic.is_descending_key = 1 THEN N' DESC' ELSE N' ASC' END
        FROM [$(SourceDB)].sys.index_columns ic
        JOIN [$(SourceDB)].sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
        WHERE ic.object_id = @pkoid AND ic.index_id = @pkidx
        ORDER BY ic.key_ordinal;
        SET @pk = N'ALTER TABLE [$(TargetDB)].' + @pktab + N' ADD CONSTRAINT ' + QUOTENAME(@pkname) +
                  CASE WHEN @pkisPK = 1 THEN N' PRIMARY KEY ' ELSE N' UNIQUE ' END +
                  (SELECT CASE WHEN type_desc = N'CLUSTERED' THEN N'CLUSTERED ' ELSE N'NONCLUSTERED ' END
                   FROM [$(SourceDB)].sys.indexes WHERE object_id = @pkoid AND index_id = @pkidx) +
                  N'(' + STUFF(@pkcols, 1, 2, N'') + N');';
        EXEC [$(TargetDB)].sys.sp_executesql @pk;
        PRINT CASE WHEN @pkisPK = 1 THEN N'PK creada: ' ELSE N'UQ creada: ' END + @pkname;
    END
    FETCH NEXT FROM curP INTO @pkname, @pkoid, @pktab, @pkidx, @pkisPK;
END
CLOSE curP; DEALLOCATE curP;
GO

-- ---- 4) Foreign Keys ------------------------------------------------------------------
DECLARE @fk NVARCHAR(MAX), @fkname sysname, @parent NVARCHAR(261), @ref NVARCHAR(261);
DECLARE @fkcols NVARCHAR(MAX), @refcols NVARCHAR(MAX), @fkid INT;
DECLARE @del NVARCHAR(20), @upd NVARCHAR(20);
DECLARE curF CURSOR LOCAL FAST_FORWARD FOR
    SELECT fk.object_id, fk.name,
           QUOTENAME(SCHEMA_NAME(tp.schema_id)) + N'.' + QUOTENAME(tp.name),
           QUOTENAME(SCHEMA_NAME(tr.schema_id)) + N'.' + QUOTENAME(tr.name),
           fk.delete_referential_action_desc, fk.update_referential_action_desc
    FROM [$(SourceDB)].sys.foreign_keys fk
    JOIN [$(SourceDB)].sys.tables tp ON tp.object_id = fk.parent_object_id
    JOIN [$(SourceDB)].sys.tables tr ON tr.object_id = fk.referenced_object_id;
OPEN curF; FETCH NEXT FROM curF INTO @fkid, @fkname, @parent, @ref, @del, @upd;
WHILE @@FETCH_STATUS = 0
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [$(TargetDB)].sys.foreign_keys WHERE name = @fkname)
    BEGIN
        SET @fkcols = N''; SET @refcols = N'';
        SELECT @fkcols += N', ' + QUOTENAME(cp.name),
               @refcols += N', ' + QUOTENAME(cr.name)
        FROM [$(SourceDB)].sys.foreign_key_columns fkc
        JOIN [$(SourceDB)].sys.columns cp ON cp.object_id = fkc.parent_object_id AND cp.column_id = fkc.parent_column_id
        JOIN [$(SourceDB)].sys.columns cr ON cr.object_id = fkc.referenced_object_id AND cr.column_id = fkc.referenced_column_id
        WHERE fkc.constraint_object_id = @fkid
        ORDER BY fkc.constraint_column_id;
        SET @fk = N'ALTER TABLE [$(TargetDB)].' + @parent + N' WITH CHECK ADD CONSTRAINT ' + QUOTENAME(@fkname) +
                  N' FOREIGN KEY (' + STUFF(@fkcols, 1, 2, N'') + N') REFERENCES [$(TargetDB)].' + @ref +
                  N' (' + STUFF(@refcols, 1, 2, N'') + N')' +
                  CASE WHEN @del <> N'NO_ACTION' THEN N' ON DELETE ' + REPLACE(@del, N'_', N' ') ELSE N'' END +
                  CASE WHEN @upd <> N'NO_ACTION' THEN N' ON UPDATE ' + REPLACE(@upd, N'_', N' ') ELSE N'' END + N';' +
                  N'ALTER TABLE [$(TargetDB)].' + @parent + N' CHECK CONSTRAINT ' + QUOTENAME(@fkname) + N';';
        EXEC [$(TargetDB)].sys.sp_executesql @fk;
        PRINT 'FK creada: ' + @fkname;
    END
    FETCH NEXT FROM curF INTO @fkid, @fkname, @parent, @ref, @del, @upd;
END
CLOSE curF; DEALLOCATE curF;
GO

-- ---- 5) Indices (no-PK) -------------------------------------------------------------------
DECLARE @ix NVARCHAR(MAX), @ixname sysname, @ixtab NVARCHAR(261), @ixid INT, @ixtype NVARCHAR(60);
DECLARE @ixuniq BIT, @ixcols NVARCHAR(MAX), @ixinc NVARCHAR(MAX), @ixfilter NVARCHAR(MAX);
DECLARE curI CURSOR LOCAL FAST_FORWARD FOR
    SELECT i.name, QUOTENAME(SCHEMA_NAME(t.schema_id)) + N'.' + QUOTENAME(t.name),
           i.index_id, i.type_desc, i.is_unique
    FROM [$(SourceDB)].sys.indexes i
    JOIN [$(SourceDB)].sys.tables t ON t.object_id = i.object_id
    WHERE i.is_primary_key = 0 AND i.is_unique_constraint = 0 AND i.type > 0
      AND i.type_desc IN (N'CLUSTERED', N'NONCLUSTERED');
OPEN curI; FETCH NEXT FROM curI INTO @ixname, @ixtab, @ixid, @ixtype, @ixuniq;
WHILE @@FETCH_STATUS = 0
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [$(TargetDB)].sys.indexes WHERE name = @ixname
                   AND object_id = OBJECT_ID(N'[$(TargetDB)].' + @ixtab, N'U'))
    BEGIN
        DECLARE @ixoid INT = OBJECT_ID(N'[$(SourceDB)].' + @ixtab, N'U');
        SET @ixcols = N'';
        SELECT @ixcols += N', ' + QUOTENAME(c.name) + CASE WHEN ic.is_descending_key = 1 THEN N' DESC' ELSE N' ASC' END
        FROM [$(SourceDB)].sys.index_columns ic
        JOIN [$(SourceDB)].sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
        WHERE ic.object_id = @ixoid AND ic.index_id = @ixid AND ic.is_included_column = 0
        ORDER BY ic.key_ordinal;
        SET @ixinc = N'';
        SELECT @ixinc += N', ' + QUOTENAME(c.name)
        FROM [$(SourceDB)].sys.index_columns ic
        JOIN [$(SourceDB)].sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
        WHERE ic.object_id = @ixoid AND ic.index_id = @ixid AND ic.is_included_column = 1
        ORDER BY ic.index_column_id;
        SELECT @ixfilter = filter_definition FROM [$(SourceDB)].sys.indexes
        WHERE object_id = @ixoid AND index_id = @ixid;
        SET @ix = N'CREATE ' + CASE WHEN @ixuniq = 1 THEN N'UNIQUE ' ELSE N'' END +
                  CASE WHEN @ixtype = N'CLUSTERED' THEN N'CLUSTERED ' ELSE N'NONCLUSTERED ' END +
                  N'INDEX ' + QUOTENAME(@ixname) + N' ON [$(TargetDB)].' + @ixtab +
                  N' (' + STUFF(@ixcols, 1, 2, N'') + N')' +
                  CASE WHEN LEN(@ixinc) > 0 THEN N' INCLUDE (' + STUFF(@ixinc, 1, 2, N'') + N')' ELSE N'' END +
                  CASE WHEN @ixfilter IS NOT NULL THEN N' WHERE ' + @ixfilter ELSE N'' END + N';';
        EXEC [$(TargetDB)].sys.sp_executesql @ix;
        PRINT 'Indice creado: ' + @ixname;
    END
    FETCH NEXT FROM curI INTO @ixname, @ixtab, @ixid, @ixtype, @ixuniq;
END
CLOSE curI; DEALLOCATE curI;
GO

-- ---- 6) Vistas, procedures, funciones y triggers ----------------------------------------------
DECLARE @mod NVARCHAR(MAX), @moname sysname, @motype NVARCHAR(10);
DECLARE curM CURSOR LOCAL FAST_FORWARD FOR
    SELECT o.name, o.type
    FROM [$(SourceDB)].sys.sql_modules m
    JOIN [$(SourceDB)].sys.objects o ON o.object_id = m.object_id
    WHERE o.type IN (N'V', N'P', N'FN', N'IF', N'TF', N'TR') AND m.definition IS NOT NULL
    ORDER BY CASE o.type WHEN N'V' THEN 1 WHEN N'FN' THEN 2 WHEN N'IF' THEN 3 WHEN N'TF' THEN 4 ELSE 5 END, o.name;
OPEN curM; FETCH NEXT FROM curM INTO @moname, @motype;
WHILE @@FETCH_STATUS = 0
BEGIN
    SELECT @mod = definition FROM [$(SourceDB)].sys.sql_modules m
    JOIN [$(SourceDB)].sys.objects o ON o.object_id = m.object_id
    WHERE o.name = @moname AND o.type = @motype;
    BEGIN TRY
        EXEC [$(TargetDB)].sys.sp_executesql @mod;
        PRINT 'Modulo creado: ' + @moname + N' (' + @motype + N')';
    END TRY
    BEGIN CATCH
        PRINT 'AVISO: no se pudo crear ' + @moname + N': ' + ERROR_MESSAGE();
    END CATCH
    FETCH NEXT FROM curM INTO @moname, @motype;
END
CLOSE curM; DEALLOCATE curM;
GO

-- ---- 7) Resumen ------------------------------------------------------------------------------------
SELECT 'tablas_origen' AS c, COUNT(*) AS n FROM [$(SourceDB)].sys.tables
UNION ALL SELECT 'tablas_destino', COUNT(*) FROM [$(TargetDB)].sys.tables
UNION ALL SELECT 'compat_destino', compatibility_level FROM sys.databases WHERE name = N'$(TargetDB)';
GO

PRINT '=== 02_Clonar_Esquema completado. Siguiente: 05 -> 10 -> 20 -> 35 -> 36 -> 30. ===';
GO
