-- ============================================================================
-- 30_Verificar_Despliegue.sql (Ritrama2025)
-- Que hace: chequeo go/no-go despues del deploy. Todo debe salir OK antes de
--   abrir la app contra ritrama2026.
-- Como se usa: sqlcmd -S RITRAMASRV01 -d ritrama2026 -E -C -b -i Scripts\Deploy\30_Verificar_Despliegue.sql
-- ============================================================================
SET NOCOUNT ON;
GO

-- 1) Version y compatibilidad (debe ser 2017 + 140).
SELECT
    @@VERSION AS version,
    SERVERPROPERTY('ProductVersion') AS product_version;
GO

SELECT
    name AS base,
    compatibility_level AS compat,
    CASE WHEN compatibility_level = 140 THEN 'OK' ELSE 'FALTA: debe ser 140 (SQL 2017)' END AS revision,
    collation_name AS collation,
    recovery_model_desc AS recovery
FROM sys.databases
WHERE name = DB_NAME();
GO

-- 2) Tablas clave del deploy (las crea Scripts/*.sql).
SELECT
    t.revision,
    CASE WHEN OBJECT_ID(N'dbo.' + t.revision, N'U') IS NOT NULL THEN 'OK' ELSE 'FALTA' END AS estado
FROM (VALUES
    ('usuarios'), ('roles'), ('permisos'), ('usuario_roles'), ('role_permisos'),
    ('operaciones_log'), ('operaciones_log_detalle'),
    ('pedido'), ('pedido_detalle'),
    ('orden_compra'), ('orden_compra_detalle')
) AS t(revision)
ORDER BY t.revision;
GO

-- 3) Tablas legacy del baseline (NO las crea Scripts/*.sql: vienen del
--    restore/bacpac de donde vive hoy ritrama2026). Si salen FALTA, el
--    baseline no se restauro y hay que hacerlo antes de usar la app.
SELECT
    t.revision,
    CASE WHEN OBJECT_ID(N'dbo.' + t.revision, N'U') IS NOT NULL THEN 'OK' ELSE 'FALTA: falta baseline' END AS estado
FROM (VALUES
    ('orden_corte'), ('customer'), ('producto'), ('control'),
    ('MasterInic'), ('ItemsMateria'), ('rolls_details'), ('cortes')
) AS t(revision)
ORDER BY t.revision;
GO

-- 4) Conteos rapidos (solo si las tablas existen).
IF OBJECT_ID(N'dbo.roles', N'U') IS NOT NULL
    SELECT 'roles' AS tabla, COUNT(*) AS filas FROM dbo.roles;
GO

IF OBJECT_ID(N'dbo.usuarios', N'U') IS NOT NULL
    SELECT 'usuarios' AS tabla, COUNT(*) AS filas FROM dbo.usuarios;
GO

IF OBJECT_ID(N'dbo.pedido', N'U') IS NOT NULL
    SELECT 'pedido' AS tabla, COUNT(*) AS filas FROM dbo.pedido;
GO

IF OBJECT_ID(N'dbo.orden_compra', N'U') IS NOT NULL
    SELECT 'orden_compra' AS tabla, COUNT(*) AS filas FROM dbo.orden_compra;
GO

-- sin_interno va por dinamico: si codigo_interno aun no existe, el SELECT
-- estatico no compilaria (deferred resolution no cubre columnas).
IF OBJECT_ID(N'dbo.customer', N'U') IS NOT NULL
BEGIN
    SELECT 'customer' AS tabla, COUNT(*) AS filas FROM dbo.customer;
    IF COL_LENGTH(N'dbo.customer', N'codigo_interno') IS NOT NULL
        EXEC sp_executesql N'SELECT ''customer_sin_interno'' AS revision, COUNT(*) AS filas FROM dbo.customer WHERE codigo_interno IS NULL;';
    ELSE
        PRINT 'customer.codigo_interno aun no existe (corre el 10).';
END
GO

IF OBJECT_ID(N'dbo.provider', N'U') IS NOT NULL
BEGIN
    SELECT 'provider' AS tabla, COUNT(*) AS filas FROM dbo.provider;
    IF COL_LENGTH(N'dbo.provider', N'codigo_interno') IS NOT NULL
        EXEC sp_executesql N'SELECT ''provider_sin_interno'' AS revision, COUNT(*) AS filas FROM dbo.provider WHERE codigo_interno IS NULL;';
    ELSE
        PRINT 'provider.codigo_interno aun no existe (corre el 10).';
END
GO

IF OBJECT_ID(N'dbo.vendedor', N'U') IS NOT NULL
BEGIN
    SELECT 'vendedor' AS tabla, COUNT(*) AS filas FROM dbo.vendedor;
    IF COL_LENGTH(N'dbo.vendedor', N'codigo_interno') IS NOT NULL
        EXEC sp_executesql N'SELECT ''vendedor_sin_interno'' AS revision, COUNT(*) AS filas FROM dbo.vendedor WHERE codigo_interno IS NULL;';
    ELSE
        PRINT 'vendedor.codigo_interno aun no existe (corre el 10).';
END
GO

IF OBJECT_ID(N'dbo.producto', N'U') IS NOT NULL
    SELECT 'producto' AS tabla, COUNT(*) AS filas FROM dbo.producto;
GO

SELECT filter AS contador, par1 AS valor FROM dbo.control
WHERE filter IN (N'OC', N'PED', N'CLI', N'PROV', N'VEND', N'COC', N'UC', N'CMP', N'PROD');
GO

PRINT '30_Verificar completado. Todo debe ser OK y compat=140 para dar go-live.';
GO
