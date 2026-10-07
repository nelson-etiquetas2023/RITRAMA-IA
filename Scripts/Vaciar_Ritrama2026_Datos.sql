-- ============================================================
-- VACIAR ritrama2026 (solo datos, conserva el esquema)
-- Base destino de DESARROLLO creada desde RITRAMASQL2017.
-- Deshabilita FKs, borra todas las filas, re-habilita FKs y
-- re-siembra las columnas identity a 1.
-- Re-ejecutable.
-- ============================================================
USE ritrama2026;
GO

-- 1. Deshabilitar todas las restricciones de clave foranea
DECLARE @sql NVARCHAR(MAX) = N'';
SELECT @sql += N'ALTER TABLE ' + QUOTENAME(SCHEMA_NAME(schema_id)) + N'.' + QUOTENAME(name)
             + N' NOCHECK CONSTRAINT ALL;' + CHAR(10)
FROM sys.tables;
EXEC sp_executesql @sql;
GO

-- (las opciones SET se fuerzan al inicio de cada batch: vistas indizadas
--  e indices calculados exigen QUOTED_IDENTIFIER ON)
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- 2. Borrar todos los datos
DECLARE @sql NVARCHAR(MAX) = N'';
SELECT @sql += N'DELETE FROM ' + QUOTENAME(SCHEMA_NAME(schema_id)) + N'.' + QUOTENAME(name)
             + N';' + CHAR(10)
FROM sys.tables;
EXEC sp_executesql @sql;
GO

-- 3. Re-habilitar las restricciones (WITH CHECK valida que no queden huesos)
DECLARE @sql NVARCHAR(MAX) = N'';
SELECT @sql += N'ALTER TABLE ' + QUOTENAME(SCHEMA_NAME(schema_id)) + N'.' + QUOTENAME(name)
             + N' WITH CHECK CHECK CONSTRAINT ALL;' + CHAR(10)
FROM sys.tables;
EXEC sp_executesql @sql;
GO

-- 4. Re-siembrar identidades: la siguiente fila vuelve a ser 1
DECLARE @sql NVARCHAR(MAX) = N'';
SELECT @sql += N'DBCC CHECKIDENT (''' + QUOTENAME(SCHEMA_NAME(t.schema_id)) + N'.' + QUOTENAME(t.name)
             + N''', RESEED, 0);' + CHAR(10)
FROM sys.tables t
JOIN sys.columns c ON c.object_id = t.object_id AND c.is_identity = 1;
IF @sql <> N''
    EXEC sp_executesql @sql;
GO

PRINT 'ritrama2026 vaciada. datos: 0 filas, esquema conservado.';
GO
