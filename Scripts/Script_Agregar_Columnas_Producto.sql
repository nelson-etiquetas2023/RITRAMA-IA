IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.producto') AND name = N'unidad')
    ALTER TABLE dbo.producto ADD unidad nvarchar(30) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.producto') AND name = N'cantidad')
    ALTER TABLE dbo.producto ADD cantidad decimal(18,4) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.producto') AND name = N'width')
    ALTER TABLE dbo.producto ADD width decimal(18,4) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.producto') AND name = N'lenght')
    ALTER TABLE dbo.producto ADD lenght decimal(18,4) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.producto') AND name = N'msi')
    ALTER TABLE dbo.producto ADD msi decimal(18,4) NULL;
GO