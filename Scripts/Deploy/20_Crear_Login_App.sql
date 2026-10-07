-- ============================================================================
-- 20_Crear_Login_App.sql (Ritrama2025)
-- Que hace: crea el login de servidor + usuario de base con privilegio minimo
--   para la app WinForms (db_datareader + db_datawriter + EXECUTE), sin usar SA.
-- Como se usa (LOCAL, password por variable sqlcmd, nunca en el repo):
--   sqlcmd -S RITRAMASRV01 -E -C -b -v Database="ritrama2026" AppLogin="ritrama_app" AppPassword="ClaveFuerte Aqui" -i Scripts\Deploy\20_Crear_Login_App.sql
-- Idempotente: si login/usuario ya existen no duplica, solo refuerza roles.
-- ============================================================================
SET NOCOUNT ON;
GO

-- 1) Login a nivel servidor.
IF SUSER_ID(N'$(AppLogin)') IS NULL
BEGIN
    PRINT 'Creando login $(AppLogin)...';
    EXEC (N'CREATE LOGIN [' + REPLACE(N'$(AppLogin)', N']', N']]') + N'] WITH PASSWORD = N''' + REPLACE(N'$(AppPassword)', N'''', N'''''') + N''', CHECK_POLICY = ON, CHECK_EXPIRATION = OFF, DEFAULT_DATABASE = [$(Database)];');
END
ELSE
BEGIN
    PRINT 'Login $(AppLogin) ya existe (sin cambios).';
END
GO

-- 2) Usuario dentro de la base + roles minimos.
USE [$(Database)];
GO

IF DATABASE_PRINCIPAL_ID(N'$(AppLogin)') IS NULL
BEGIN
    PRINT 'Creando usuario $(AppLogin) en $(Database)...';
    EXEC (N'CREATE USER [' + REPLACE(N'$(AppLogin)', N']', N']]') + N'] FOR LOGIN [' + REPLACE(N'$(AppLogin)', N']', N']]') + N'];');
END
ELSE
BEGIN
    PRINT 'Usuario $(AppLogin) ya existe (sin cambios).';
END
GO

IF IS_ROLEMEMBER(N'db_datareader', N'$(AppLogin)') <> 1
    ALTER ROLE [db_datareader] ADD MEMBER [$(AppLogin)];
GO

IF IS_ROLEMEMBER(N'db_datawriter', N'$(AppLogin)') <> 1
    ALTER ROLE [db_datawriter] ADD MEMBER [$(AppLogin)];
GO

GRANT EXECUTE TO [$(AppLogin)];
GO

SELECT
    SUSER_NAME() AS ejecutado_por,
    USER_NAME() AS contexto_bd,
    IS_ROLEMEMBER(N'db_datareader', N'$(AppLogin)') AS es_reader,
    IS_ROLEMEMBER(N'db_datawriter', N'$(AppLogin)') AS es_writer;
GO

PRINT '20_Crear_Login_App completado. Usa este login en appsettings.Production.json, no SA.';
GO
