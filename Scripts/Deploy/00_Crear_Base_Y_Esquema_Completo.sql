-- ============================================================================
-- 00_Crear_Base_Y_Esquema_COMPLETO.sql (Ritrama2025 - Versión Simplificada y Robusta)
-- Crea base RITRAMA2026-TEST y clona TODAS las tablas desde RITRAMASQL2017
-- Manejo de errores por tabla: si una falla, continúa con la siguiente
-- Ejecutar: sqlcmd -S SERVIDOR -E -C -b -i Scripts\Deploy\00_Crear_Base_Y_Esquema_COMPLETO.sql
-- ============================================================================
SET NOCOUNT ON;
GO

-- ============================================================================
-- CONFIGURACIÓN (literales puros - nunca se pierden en batches)
-- ============================================================================
DECLARE @SourceDB NVARCHAR(128) = 'RITRAMASQL2017';
DECLARE @TargetDB NVARCHAR(128) = 'RITRAMA2026-TEST';

-- ============================================================================
-- PASO 1: Crear base de datos destino (con literal, sin variables riesgosas)
-- ============================================================================
USE master;
GO

IF DB_ID('RITRAMA2026-TEST') IS NULL
BEGIN
    PRINT 'Creando base de datos: RITRAMA2026-TEST';
    CREATE DATABASE [RITRAMA2026-TEST] COLLATE Modern_Spanish_CI_AS;
    PRINT '✓ Base de datos creada';
END
ELSE
BEGIN
    PRINT '✓ La base de datos RITRAMA2026-TEST ya existe (no se tocará)';
END
GO

-- Compatibilidad SQL Server 2017 (140)
IF DB_ID('RITRAMA2026-TEST') IS NOT NULL
BEGIN
    ALTER DATABASE [RITRAMA2026-TEST] SET COMPATIBILITY_LEVEL = 140;
    PRINT '✓ Nivel de compatibilidad ajustado a 140';
END
GO

-- Cambio de contexto usando LITERAL (la clave del éxito)
USE [RITRAMA2026-TEST];
GO

-- ============================================================================
-- PASO 2: Clonar TODAS las tablas desde RITRAMASQL2017
-- ============================================================================
PRINT 'Iniciando clonación de TODAS las tablas desde ' + @SourceDB;
PRINT '';

-- 2a) Obtener lista de todas las tablas del schema de legacy
DECLARE @TableName NVARCHAR(261), @SchemaName NVARCHAR(128);
DECLARE @TotalTables INT, @TableCounter INT = 0, @Success INT = 0, @Fail INT = 0;

DECLARE curTables CURSOR LOCAL FAST_FORWARD FOR
    SELECT SCHEMA_NAME(t.schema_id) AS SchemaName, t.name AS TableName
    FROM [@SourceDB].sys.tables t
    WHERE t.type = 'U'
    ORDER BY SCHEMA_NAME(t.schema_id), t.name;
OPEN curTables;

-- Conteo total antes de empezar
SELECT @TotalTables = COUNT(*) FROM [@SourceDB].sys.tables WHERE type = 'U';
PRINT 'Total de tablas para clonar desde ' + @SourceDB + ': ' + CAST(@TotalTables AS VARCHAR(10));
PRINT '------------------------------------------------------';

FETCH NEXT FROM curTables INTO @SchemaName, @TableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @TableCounter = @TableCounter + 1;

    -- Determinar schema para CREATE TABLE (usar dbo si no existe el schema en nuevo)
    DECLARE @CreateSchema NVARCHAR(128) = 'dbo';
    IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = @SchemaName)
        SET @CreateSchema = 'dbo';
    ELSE
        SET @CreateSchema = @SchemaName;

    BEGIN TRY
        -- 1) Construir CREATE TABLE con columnas seguras
        DECLARE @ColsSql NVARCHAR(MAX) = N'';

        SELECT @ColsSql = N', ' + QUOTENAME(c.name) + N' ' + 
            CASE WHEN st.name IN (N'varchar', N'char', N'varbinary', N'binary') 
                 THEN N'(' + CASE WHEN c.max_length = -1 THEN N'MAX' ELSE CAST(c.max_length AS NVARCHAR(10)) END + N')'
                 WHEN st.name IN (N'nvarchar', N'nchar') 
                 THEN N'(' + CASE WHEN c.max_length = -1 THEN N'MAX' ELSE CAST(c.max_length/2 AS NVARCHAR(10)) END + N')'
                 WHEN st.name IN (N'decimal', N'numeric') 
                 THEN N'(' + CAST(c.precision AS NVARCHAR(10)) + N',' + CAST(c.scale AS NVARCHAR(10)) + N')'
                 WHEN st.name IN (N'datetime2', N'datetime', N'time') 
                 THEN N'(' + CAST(c.scale AS NVARCHAR(10)) + N')'
                 ELSE N'' END +
            CASE WHEN c.is_nullable = 0 THEN N' NOT NULL' ELSE N' NULL' END +
            ISNULL(N' DEFAULT ' + cp.definition, N'')
        FROM [@SourceDB].sys.columns c
        JOIN [@SourceDB].sys.types st ON st.system_type_id = c.system_type_id
        LEFT JOIN [@SourceDB].sys.default_constraints cp ON cp.parent_object_id = c.object_id AND cp.parent_column_id = c.column_id
        WHERE c.object_id = OBJECT_ID(QUOTENAME(@SchemaName) + N'.' + QUOTENAME(@TableName))
        ORDER BY c.column_id;

        -- Si tenemos columnas (longitud > 2 porque empieza con ", ")
        IF LEN(@ColsSql) > 2
        BEGIN
            -- Quitar la primera ", " inicial para dejar solo los nombres de columnas
            SET @ColsSql = STUFF(@ColsSql, 1, 2, N'');

            DECLARE @CreateSql NVARCHAR(MAX) = N'CREATE TABLE [' + @CreateSchema + N'].dbo.' + QUOTENAME(@TableName) + N' (' + @ColsSql + N')';

            -- Ejecutar creación de tabla
            EXEC (@CreateSql);

            SET @Success = @Success + 1;
            PRINT '[' + CAST(@TableCounter AS VARCHAR(10)) + '/' + CAST(@TotalTables AS VARCHAR(10)) + '] ✓ ' + @SchemaName + N'.' + @TableName + ' clonada OK';
        END
        ELSE
        BEGIN
            PRINT '[' + CAST(@TableCounter AS VARCHAR(10)) + '/' + CAST(@TotalTables AS VARCHAR(10)) + '] ⚠ ' + @SchemaName + N'.' + @TableName + ' - Sin columnas definidas';
            SET @Fail = @Fail + 1;
        END
    END TRY
    BEGIN CATCH
        -- AQUÍ ESTÁ LA CLAVE: Si falla una tabla, SÓLO esa imprime error y continúa
        PRINT '[' + CAST(@TableCounter AS VARCHAR(10)) + '/' + CAST(@TotalTables AS VARCHAR(10)) + '] ✗ ERROR en ' + @SchemaName + N'.' + @TableName;
        PRINT '  Error: ' + ERROR_MESSAGE();
        SET @Fail = @Fail + 1;
    END CATCH

    FETCH NEXT FROM curTables INTO @SchemaName, @TableName;
END

CLOSE curTables;
DEALLOCATE curTables;
GO

-- ============================================================================
-- PASO 3: Resumen final (solo números, sin variables complejas)
-- ============================================================================
PRINT '';
PRINT '================================================================';
PRINT '  CLONACIÓN TERMINADA';
PRINT '================================================================';
PRINT '  Origen: ' + @SourceDB;
PRINT '  Destino: ' + @TargetDB;
PRINT '  Tablas exitosas: ' + CAST(@Success AS VARCHAR(10));
PRINT '  Tablas fallidas: ' + CAST(@Fail AS VARCHAR(10));
PRINT '  Total procesadas: ' + CAST(@TableCounter AS VARCHAR(10));
PRINT '  Tablas esperadas: ' + CAST(@TotalTables AS VARCHAR(10));
PRINT '================================================================';
GO