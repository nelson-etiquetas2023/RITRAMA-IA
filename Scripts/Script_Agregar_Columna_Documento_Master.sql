-- Masters: columna 'documento' = numero de la Orden de Corte que uso el master (cruce master <-> OC).
-- Re-ejecutable (idempotente).

IF COL_LENGTH('dbo.MasterInic', 'documento') IS NULL
    ALTER TABLE dbo.MasterInic ADD documento INT NULL;
GO

IF COL_LENGTH('dbo.ItemsMateria', 'documento') IS NULL
    ALTER TABLE dbo.ItemsMateria ADD documento INT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MasterInic_documento' AND object_id = OBJECT_ID('dbo.MasterInic'))
    CREATE NONCLUSTERED INDEX IX_MasterInic_documento ON dbo.MasterInic(documento) INCLUDE (Roll_Id);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ItemsMateria_documento' AND object_id = OBJECT_ID('dbo.ItemsMateria'))
    CREATE NONCLUSTERED INDEX IX_ItemsMateria_documento ON dbo.ItemsMateria(documento) INCLUDE (rollid);
GO