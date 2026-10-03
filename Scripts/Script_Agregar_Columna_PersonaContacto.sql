-- Script para agregar el campo PersonaContacto a las tablas customer y provider
-- Fecha: 2026-10-03
-- Descripción: Agrega la columna persona_contacto (NVARCHAR(100)) a ambas tablas

-- Tabla customer
IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID('customer') 
    AND name = 'persona_contacto'
)
BEGIN
    ALTER TABLE customer
    ADD persona_contacto NVARCHAR(100) NULL;
    
    PRINT 'Columna persona_contacto agregada a la tabla customer';
END
ELSE
BEGIN
    PRINT 'La columna persona_contacto ya existe en la tabla customer';
END
GO

-- Tabla provider
IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID('provider') 
    AND name = 'persona_contacto'
)
BEGIN
    ALTER TABLE provider
    ADD persona_contacto NVARCHAR(100) NULL;
    
    PRINT 'Columna persona_contacto agregada a la tabla provider';
END
ELSE
BEGIN
    PRINT 'La columna persona_contacto ya existe en la tabla provider';
END
GO
