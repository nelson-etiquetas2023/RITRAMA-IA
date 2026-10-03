-- Usuarios: 'titulo_cargo' (puesto que ocupa la persona) y 'departamento' (area de
-- la empresa a la que pertenece). Los dos son de texto libre: no hay catalogo que
-- mantener, asi que se capturan a mano y quedan como los escribio el usuario.
-- NULL en filas legadas: la app los muestra como "—" hasta que se editen.
-- Re-ejecutable (idempotente).

IF COL_LENGTH('dbo.usuarios', 'titulo_cargo') IS NULL
    ALTER TABLE dbo.usuarios ADD titulo_cargo nvarchar(100) NULL;
GO

IF COL_LENGTH('dbo.usuarios', 'departamento') IS NULL
    ALTER TABLE dbo.usuarios ADD departamento nvarchar(100) NULL;
GO