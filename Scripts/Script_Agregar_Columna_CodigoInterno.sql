-- Clientes, Proveedores y Vendedores: columna 'codigo_interno' = segundo ID del
-- registro, consecutivo 1, 2, 3... independiente por módulo (filtros CLI / PROV /
-- VEND en la tabla control). El primario sigue siendo el GUID que genera el sistema
-- al dar de alta (customer_id, Proveedor_Id, vendor_id).
-- NULL en filas legadas anteriores a la migración: la app las muestra como "—".
-- Re-ejecutable (idempotente).

IF COL_LENGTH('dbo.customer', 'codigo_interno') IS NULL
    ALTER TABLE dbo.customer ADD codigo_interno int NULL;
GO

IF COL_LENGTH('dbo.provider', 'codigo_interno') IS NULL
    ALTER TABLE dbo.provider ADD codigo_interno int NULL;
GO

IF COL_LENGTH('dbo.vendedor', 'codigo_interno') IS NULL
    ALTER TABLE dbo.vendedor ADD codigo_interno int NULL;
GO

-- Contadores en 0: la primera alta de cada módulo toma el 1
-- (UPDATE ... OUTPUT INSERTED.par1).
IF NOT EXISTS (SELECT 1 FROM dbo.control WHERE filter = 'CLI')
    INSERT INTO dbo.control (filter, par1) VALUES ('CLI', 0);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.control WHERE filter = 'PROV')
    INSERT INTO dbo.control (filter, par1) VALUES ('PROV', 0);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.control WHERE filter = 'VEND')
    INSERT INTO dbo.control (filter, par1) VALUES ('VEND', 0);
GO
