-- ============================================================================
-- 05_Crear_Tabla_Control.sql (Ritrama2025)
-- Que hace: crea la tabla control (consecutivos OC/PED) si no existe.
--   Es la unica tabla legacy con forma 100% conocida por el codigo:
--   ServiceDataCommon.cs lee "select par1 from control where filter=@p1" y
--   004/Setup_Pedidos insertan (filter, par1). Performance_Indexes.sql le
--   agrega despues id IDENTITY + PK clustered, idempotente.
-- Idempotente: re-ejecutable sin error.
-- NOTA arranque-de-cero: las demas tablas legacy (orden_corte, customer,
--   producto, MasterInic, ItemsMateria, rolls_details, cortes, despacho,
--   vueltas, operadores...) NO tienen DDL en Scripts/ y la app las exige.
--   Hay que importar su esquema (sin datos) desde el server donde viven hoy:
--   SSMS > Tasks > Generate Scripts > Schema only, o:
--   sqlpackage /Action:Extract /SourceServerName:<SERVER_ORIGEN> ^
--     /SourceDatabaseName:<BD_ORIGEN> /TargetFile:E:\baseline.dacpac /p:ExtractAllTableData=False
--   y publicarlo aqui antes del paso 10. 30_Verificar lo comprueba.
-- ============================================================================
SET NOCOUNT ON;
GO

IF OBJECT_ID(N'dbo.control', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.control (
        filter NVARCHAR(50)  NOT NULL,
        par1   NVARCHAR(100) NULL,
        CONSTRAINT UQ_control_filter UNIQUE (filter)
    );
    PRINT 'Tabla control creada.';
END
ELSE
BEGIN
    PRINT 'Tabla control ya existe (sin cambios).';
END
GO

-- Semillas que 004 y Setup_Pedidos esperan (por si este script corre solo).
IF NOT EXISTS (SELECT 1 FROM dbo.control WHERE filter = N'OC')
    INSERT INTO dbo.control (filter, par1) VALUES (N'OC', N'0');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.control WHERE filter = N'PED')
    INSERT INTO dbo.control (filter, par1) VALUES (N'PED', N'0');
GO

-- Semillas codigo_interno: clientes/proveedores/vendedores empiezan en 1.
-- El contador queda en 0 y la primera alta toma el 1 (UPDATE ... OUTPUT).
-- Script_Agregar_Columna_CodigoInterno.sql las refuerza (idempotente).
IF NOT EXISTS (SELECT 1 FROM dbo.control WHERE filter = N'CLI')
    INSERT INTO dbo.control (filter, par1) VALUES (N'CLI', N'0');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.control WHERE filter = N'PROV')
    INSERT INTO dbo.control (filter, par1) VALUES (N'PROV', N'0');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.control WHERE filter = N'VEND')
    INSERT INTO dbo.control (filter, par1) VALUES (N'VEND', N'0');
GO

-- Contadores operativos que el codigo exige (UPDATE...OUTPUT devuelve NULL y
-- revienta si la fila no existe): COC/UC (ConsecutivosService.cs:30,65),
-- CMP (ServiceMateriaPrima.cs:277), OC/PED (R.cs:61,66,177,181).
-- Se siembran en 0; AJUSTA al ultimo numero real antes del go-live
-- (si no, la numeracion arranca de cero y choca con documentos impresos).
IF NOT EXISTS (SELECT 1 FROM dbo.control WHERE filter = N'COC')
    INSERT INTO dbo.control (filter, par1) VALUES (N'COC', N'0');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.control WHERE filter = N'UC')
    INSERT INTO dbo.control (filter, par1) VALUES (N'UC', N'0');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.control WHERE filter = N'CMP')
    INSERT INTO dbo.control (filter, par1) VALUES (N'CMP', N'0');
GO

SELECT filter, par1 FROM dbo.control
WHERE filter IN (N'OC', N'PED', N'CLI', N'PROV', N'VEND', N'COC', N'UC', N'CMP');
GO
