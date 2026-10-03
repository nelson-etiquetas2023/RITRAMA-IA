-- Proveedores: columna 'direccion_entrega' (puede diferir de 'direccion').
-- Clientes ya trae 'direccion_facturacion' y 'direccion_entrega', así que aquí no se toca.
-- NULL en filas legadas: la app muestra "—" hasta que se capture al editar.
-- Re-ejecutable (idempotente).

IF COL_LENGTH('dbo.provider', 'direccion_entrega') IS NULL
    ALTER TABLE dbo.provider ADD direccion_entrega nvarchar(200) NULL;
GO
