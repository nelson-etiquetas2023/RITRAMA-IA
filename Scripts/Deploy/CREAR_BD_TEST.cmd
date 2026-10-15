@echo off
setlocal EnableExtensions
REM ============================================================================
REM CREAR_BD_TEST.cmd (Ritrama2025)
REM ----------------------------------------------------------------------------
REM Que hace: crea la base de PRUEBA (default ritrama2026-test) con TODO el
REM   esquema (legacy + nuevo) y SIN datos de negocio. Solo queda la semilla:
REM   usuario admin (clave admin), roles, permisos y contadores en 0.
REM Como se usa (LOCAL en el server, sesion AnyDesk, NO por WAN):
REM   CREAR_BD_TEST.cmd [servidor] [BD destino] [BD origen legacy]
REM   CREAR_BD_TEST.cmd
REM   CREAR_BD_TEST.cmd RITRAMASRV01 ritrama2026-test RITRAMASQL2017
REM Pasos:
REM   1/4 crea la BD y clona el esquema legacy SIN datos (02_Clonar_Esquema)
REM   2/4 esquema nuevo + semilla (seguridad, logs, OC, pedidos, columnas,
REM        indices). NO corre Migrar_*, Backfill_*, Poblar_* ni 35/36: son datos.
REM   3/4 login ritrama_app (SOLO si existe la variable entorno RITRAMA_APP_PWD)
REM   4/4 verificacion (30_Verificar_Despliegue)
REM Auth: -E integrada por defecto. Para SQL auth define RITRAMA_SQL_USER y
REM   RITRAMA_SQL_PWD ANTES de llamar (nada de claves en la linea de comandos).
REM Log: logs\crear_bd_test_yyyyMMdd_HHmmss.log (carpeta de este archivo).
REM Nota: 02 se ejecuta desde una copia sin sus :setvar porque en sqlcmd el
REM   :setvar del script pisa al -v (doc Microsoft); asi el nombre de BD
REM   destino/origen pasado por -v aplica seguro.
REM Re-ejecutable: lo que ya existe se salta con PRINT.
REM ============================================================================
REM --- Parametros por defecto (sobrecargables por posicion) -------------------
set "SERVER=RITRAMASRV01"
set "TARGETDB=ritrama2026-test"
set "SOURCEDB=RITRAMASQL2017"
if not "%~1"=="" set "SERVER=%~1"
if not "%~2"=="" set "TARGETDB=%~2"
if not "%~3"=="" set "SOURCEDB=%~3"

set "DEPLOY=%~dp0"
set "SCRIPTS=%~dp0.."
set "FAILED="

REM --- Timestamp + log -------------------------------------------------------
set "TS="
for /f %%i in ('powershell -NoProfile -Command "Get-Date -Format yyyyMMdd_HHmmss" 2^>nul') do set "TS=%%i"
if not defined TS set "TS=%RANDOM%%RANDOM%"
set "LOGDIR=%DEPLOY%logs"
if not exist "%LOGDIR%" mkdir "%LOGDIR%"
set "LOG=%LOGDIR%\crear_bd_test_%TS%.log"

REM --- Auth: integrada por defecto; SQL auth via variables de entorno --------
set "AUTH=-E -C"
if defined RITRAMA_SQL_USER set "AUTH=-U %RITRAMA_SQL_USER% -P %RITRAMA_SQL_PWD% -C"

echo ================================================================
echo  CREAR_BD_TEST
echo  servidor = %SERVER%
echo  BD destino = %TARGETDB%
echo  BD origen  = %SOURCEDB%
echo  log = %LOG%
echo ================================================================
>>"%LOG%" echo ================================================================
>>"%LOG%" echo CREAR_BD_TEST  servidor=%SERVER%  destino=%TARGETDB%  origen=%SOURCEDB%
>>"%LOG%" echo ================================================================

REM --- 1/4: crear BD + clona esquema legacy SIN datos -------------------------
echo [1/4] Creando base %TARGETDB% y clonando esquema de %SOURCEDB% (sin datos)...
set "TMP02=%TEMP%\02_sin_setvar_%TS%.sql"
findstr /v /b /c:":setvar SourceDB" /c:":setvar TargetDB" "%DEPLOY%02_Clonar_Esquema_Desde_Legacy.sql" > "%TMP02%"
if not exist "%TMP02%" goto :fail_prep
for %%A in ("%TMP02%") do if %%~zA==0 goto :fail_prep
findstr /b /c:":setvar" "%TMP02%" >nul 2>&1
if not errorlevel 1 goto :fail_prep
sqlcmd -S %SERVER% %AUTH% -b -d master -v SourceDB="%SOURCEDB%" -v TargetDB="%TARGETDB%" -i "%TMP02%" >>"%LOG%" 2>&1
if errorlevel 1 set "FAILED=02_Clonar_Esquema_Desde_Legacy.sql"
if defined FAILED goto :fail
del "%TMP02%" >nul 2>&1

REM --- 2/4: esquema nuevo + semilla (solo scripts de ESQUEMA) -----------------
echo [2/4] Aplicando esquema nuevo + semilla admin en %TARGETDB%...
for %%F in (
    "Deploy\05_Crear_Tabla_Control.sql"
    "Script_Seguridad.sql"
    "Script_OperacionesLog.sql"
    "002_CrearTablasLog.sql"
    "001_AgregarPendingOperation.sql"
    "004_CrearTablas_OrdenesCompra.sql"
    "Setup_Pedidos.sql"
    "Script_Agregar_Columnas_Producto.sql"
    "Script_Agregar_Columna_CodigoInterno.sql"
    "Script_Agregar_Columna_Costo.sql"
    "Script_Agregar_Columna_Categoria_Proveedor.sql"
    "Script_Agregar_Columna_DireccionEntrega_Proveedor.sql"
    "Script_Agregar_Columna_PersonaContacto.sql"
    "Script_Agregar_Columna_Documento_Master.sql"
    "Script_Agregar_Columna_Zona_Vendedor.sql"
    "Script_Agregar_Columnas_Usuario_Cargo_Departamento.sql"
    "Script_Roles_Usuarios_Admin_SuperAdmin_UserDefault_Invitado.sql"
    "Performance_Indexes.sql"
) do (
    call :step "%TARGETDB%" "%SCRIPTS%\%%~F"
    if errorlevel 1 set "FAILED=%%~F"
    if defined FAILED goto :fail
)

REM --- 3/4: login de la app (opcional) ----------------------------------------
if not defined RITRAMA_APP_PWD goto :skip_login
echo [3/4] Creando login ritrama_app en %TARGETDB%...
>>"%LOG%" echo [3/4] 20_Crear_Login_App.sql
sqlcmd -S %SERVER% %AUTH% -b -v Database="%TARGETDB%" -v AppLogin="ritrama_app" -v AppPassword="%RITRAMA_APP_PWD%" -i "%DEPLOY%20_Crear_Login_App.sql" >>"%LOG%" 2>&1
if errorlevel 1 set "FAILED=20_Crear_Login_App.sql"
if defined FAILED goto :fail
goto :verify

:skip_login
echo [3/4] Login ritrama_app omitido (define RITRAMA_APP_PWD si lo necesitas).

REM --- 4/4: verificacion -------------------------------------------------------
:verify
echo [4/4] Verificando %TARGETDB%...
call :step "%TARGETDB%" "%DEPLOY%30_Verificar_Despliegue.sql"
if errorlevel 1 set "FAILED=30_Verificar_Despliegue.sql"
if defined FAILED goto :fail

echo.
echo [OK] Base %TARGETDB% lista: esquema completo + semilla admin ^(admin/admin^).
echo      Log: %LOG%
echo      Resultado de 30_Verificar:
powershell -NoProfile -Command "Get-Content -LiteralPath '%LOG%' -Tail 25"
exit /b 0

REM --- Manejo de errores -------------------------------------------------------
:fail_prep
echo.
echo *** FALLO preparando 02_Clonar_Esquema_Desde_Legacy.sql (findstr). ***
echo *** Revisa que el archivo exista en %DEPLOY%
exit /b 1

:fail
del "%TMP02%" >nul 2>&1
echo.
echo *** FALLO en %FAILED% ***
echo *** Log completo: %LOG%
echo *** Ultimas lineas del log:
powershell -NoProfile -Command "Get-Content -LiteralPath '%LOG%' -Tail 15"
exit /b 1

REM --- Subrutina: corre un script contra la BD indicada ------------------------
:step
echo     - %~nx2
>>"%LOG%" echo [%TIME%] %~nx2
sqlcmd -S %SERVER% %AUTH% -d %~1 -b -i "%~2" >>"%LOG%" 2>&1
exit /b %ERRORLEVEL%
