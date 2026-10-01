-- Productos: columna 'costo' = costo unitario de compra (lo que se paga al proveedor).
-- Es distinta de 'precio' (lo que se cobra al cliente); se muestra en el detalle
-- de FrmProductos justo debajo de Precio.
-- Re-ejecutable (idempotente).

IF COL_LENGTH('dbo.producto', 'costo') IS NULL
    ALTER TABLE dbo.producto ADD costo decimal(18,2) NULL;
GO
