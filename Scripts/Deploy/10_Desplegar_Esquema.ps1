# ============================================================================
# 10_Desplegar_Esquema.ps1 (Ritrama2025)
# Que hace: ejecuta TUS Scripts/*.sql en orden seguro con sqlcmd LOCAL,
#   con log por archivo y parada ante error real. Re-ejecutable.
# No incluye 003_ConsultasLog.sql (son SELECTs manuales de diagnostico).
# Trata el error 50003 ("migracion ya ejecutada") como WARNING y sigue.
# Uso en el server (AnyDesk, sesion local, NO por WAN):
#   .\Scripts\Deploy\10_Desplegar_Esquema.ps1 -ServerInstance "RITRAMASRV01" -Database "ritrama2026"
#   .\Scripts\Deploy\10_Desplegar_Esquema.ps1 -ServerInstance ".\RITRAMA" -Database "ritrama2026" -User "sa"
#   password via env: $env:RITRAMA_SQL_PWD (no queda en historial ni en el repo)
# Requiere: sqlcmd en PATH (lo instala SQL Server 2017).
# ============================================================================
[CmdletBinding()]
param(
    [string]$ServerInstance = "RITRAMASRV01",
    [string]$Database = "ritrama2026",
    [string]$User = "",
    [switch]$UseIntegratedAuth
)

$ErrorActionPreference = "Stop"

$deployDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$scriptsDir = Split-Path -Parent $deployDir
$logDir = Join-Path $deployDir "logs"
New-Item -ItemType Directory -Force -Path $logDir | Out-Null
$mainLog = Join-Path $logDir ("deploy_{0}_{1:yyyyMMdd_HHmmss}.log" -f $Database, (Get-Date))

# Orden seguro: control -> base seguridad -> logs -> modulos -> columnas ->
# migraciones -> backfills -> poblamientos -> roles rename -> indices (al final).
# REQUIERE baseline legacy previo en BD vacia (ver 05_Crear_Tabla_Control.sql):
# orden_corte, customer, producto, MasterInic, ItemsMateria, despacho, etc.
$ordered = @(
    "Deploy\05_Crear_Tabla_Control.sql",
    "Script_Seguridad.sql",
    "Script_OperacionesLog.sql",
    "002_CrearTablasLog.sql",
    "001_AgregarPendingOperation.sql",
    "004_CrearTablas_OrdenesCompra.sql",
    "Setup_Pedidos.sql",
    "Script_Agregar_Columnas_Producto.sql",
    "Script_Agregar_Columna_CodigoInterno.sql",
    "Script_Agregar_Columna_Costo.sql",
    "Script_Agregar_Columna_Categoria_Proveedor.sql",
    "Script_Agregar_Columna_DireccionEntrega_Proveedor.sql",
    "Script_Agregar_Columna_PersonaContacto.sql",
    "Script_Agregar_Columna_Documento_Master.sql",
    "Script_Agregar_Columna_Zona_Vendedor.sql",
    "Script_Agregar_Columnas_Usuario_Cargo_Departamento.sql",
    "Migrar_Direcciones_Cliente.sql",
    "Migrar_Pedido_CondicionesPago.sql",
    "Migrar_Pedido_Numero_5Digitos.sql",
    "Migrar_Pedido_Numero_SO.sql",
    "Migrar_Pedido_Prioridad.sql",
    "Backfill_Pedidos_Completos.sql",
    "Backfill_Restante_OC.sql",
    "Backfill_TotalSalida_OC.sql",
    "Poblar_Graphics.sql",
    "Poblar_Hojas.sql",
    "Poblar_Rollos_Cortados.sql",
    "Reclasificar_Productos_Categorias.sql",
    "Script_Roles_Usuarios_Admin_SuperAdmin_UserDefault_Invitado.sql",
    "Performance_Indexes.sql"
)

function Get-SqlAuth {
    if ($UseIntegratedAuth -or [string]::IsNullOrWhiteSpace($User)) {
        return @("-E")
    }
    $pwd = $env:RITRAMA_SQL_PWD
    if ([string]::IsNullOrEmpty($pwd)) {
        $sec = Read-Host ("Password SQL para '{0}'@{1}" -f $User, $ServerInstance) -AsSecureString
        $ptr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($sec)
        try { $pwd = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($ptr) }
        finally { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($ptr) }
    }
    return @("-U", $User, "-P", $pwd)
}

$sqlcmd = Get-Command sqlcmd -ErrorAction Stop
"== Deploy {0} en {1}\{2}  {3} ==" -f $Database, $ServerInstance, $Database, (Get-Date) | Tee-Object $mainLog
"Scripts base: $scriptsDir" | Tee-Object $mainLog -Append

$failed = @()
$warned = @()

foreach ($f in $ordered) {
    $path = Join-Path $scriptsDir $f
    if (-not (Test-Path $path)) {
        "SKIP (no existe): $f" | Tee-Object $mainLog -Append
        continue
    }
    $log = Join-Path $logDir ("{0}_{1}.log" -f ([IO.Path]::GetFileNameWithoutExtension($f)), (Get-Date -Format "HHmmss"))
    "--> $f" | Tee-Object $mainLog -Append
    $auth = Get-SqlAuth
    & $sqlcmd.Source -S $ServerInstance -d $Database @auth -C -b -i $path -o $log | Out-Null
    $code = $LASTEXITCODE
    if ($code -eq 0) {
        "    OK" | Tee-Object $mainLog -Append
    }
    else {
        $txt = ""
        if (Test-Path $log) { $txt = Get-Content $log -Raw -ErrorAction SilentlyContinue }
        if ($txt -match "50003") {
            # Migracion ya aplicada en una pasada anterior: avisado por diseno (THROW 50003).
            "    WARNING 50003 (ya aplicada, se continua): $f" | Tee-Object $mainLog -Append
            $warned += $f
        }
        else {
            "    ERROR exit=$code, ver $log" | Tee-Object $mainLog -Append
            $failed += $f
            break
        }
    }
}

"----------------------------------------" | Tee-Object $mainLog -Append
if ($failed.Count -gt 0) {
    "FALLO en: $($failed -join ', ')" | Tee-Object $mainLog -Append
    "Revisa el .log del archivo y corrige antes de seguir. No sigas con la app." | Tee-Object $mainLog -Append
    exit 1
}
"COMPLETADO. Warnings 50003 (ya aplicadas): $($warned.Count)" | Tee-Object $mainLog -Append
"Siguiente: 20_Crear_Login_App.sql y 30_Verificar_Despliegue.sql" | Tee-Object $mainLog -Append
