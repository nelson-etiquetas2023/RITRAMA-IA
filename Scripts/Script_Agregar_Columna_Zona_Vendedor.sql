-- Script para agregar el campo zona a la tabla vendedor
-- Fecha: 2026-10-03
-- Descripción: Agrega la columna zona (NVARCHAR(100)) a la tabla vendedor

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('vendedor')
    AND name = 'zona'
)
BEGIN
    ALTER TABLE vendedor
    ADD zona NVARCHAR(100) NULL;

    PRINT 'Columna zona agregada a la tabla vendedor';
END
ELSE
BEGIN
    PRINT 'La columna zona ya existe en la tabla vendedor';
END
GO
