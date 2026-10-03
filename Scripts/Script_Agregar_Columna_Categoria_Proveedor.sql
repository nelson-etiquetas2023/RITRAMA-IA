-- Proveedores: columna 'categoria' (Nacional o Internacional, se elige en combo).
-- Clientes reusa su columna existente 'customer_category', así que aquí no se toca.
-- NULL en filas legadas: la app deja el combo vacío hasta que se elija al editar.
-- Re-ejecutable (idempotente).

IF COL_LENGTH('dbo.provider', 'categoria') IS NULL
    ALTER TABLE dbo.provider ADD categoria nvarchar(20) NULL;
GO
