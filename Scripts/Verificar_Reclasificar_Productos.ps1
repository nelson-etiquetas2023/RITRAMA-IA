param(
    [string]$SqlPath = '',
    [string]$CsvPath = '',
    [string]$ConnectionString = ''
)

# Verificacion (SOLO LECTURA) del mapping de Reclasificar_Productos_Categorias.sql.
# Compara los producto_id del script y del CSV con los que hay en dbo.producto y
# avisa de IDs que no existen, duplicados, IDs de la base que faltan en la lista
# y diferencias entre el .sql y el .csv. No escribe nada.
#
# Uso:  .\Verificar_Reclasificar_Productos.ps1
#       .\Verificar_Reclasificar_Productos.ps1 -ConnectionString "Data Source=..."

if ([string]::IsNullOrWhiteSpace($SqlPath)) {
    $SqlPath = Join-Path $PSScriptRoot 'Reclasificar_Productos_Categorias.sql'
}
if ([string]::IsNullOrWhiteSpace($CsvPath)) {
    $CsvPath = Join-Path $PSScriptRoot 'Reclasificar_Productos_Mapping.csv'
}

# La cadena de conexion no esta en este archivo: se lee de appsettings.Development.json
# (ignorado por git), igual que hacen los demas scripts de utileria.
$configPath = Join-Path (Split-Path -Parent $PSScriptRoot) 'appsettings.Development.json'
if ([string]::IsNullOrWhiteSpace($ConnectionString)) {
    if (-not (Test-Path -LiteralPath $configPath)) {
        Write-Host ('Falta la cadena de conexion: no se encontro ' + $configPath)
        Write-Host 'Pasa el parametro -ConnectionString para indicarla.'
        exit 1
    }
    $config = Get-Content -LiteralPath $configPath -Raw | ConvertFrom-Json
    $ConnectionString = $config.ConnectionStringsEnvironment.Desarrollo
}
if ([string]::IsNullOrWhiteSpace($ConnectionString)) {
    Write-Host 'Falta la cadena de conexion (ConnectionStringsEnvironment:Desarrollo esta vacia).'
    exit 1
}

# pwsh no resuelve Microsoft.Data.SqlClient por nombre (no esta en el GAC), asi que
# se busca primero junto al bin del proyecto, donde estan sus dependencias, y despues
# en la cache de NuGet. Si nada funciona queda System.Data.SqlClient, que existe en
# Windows PowerShell.
$tipoConexion = $null
$raiz = Split-Path -Parent $PSScriptRoot

$dlls = @()
$dlls += Get-ChildItem -Path $raiz -Recurse -Filter 'Microsoft.Data.SqlClient.dll' -File -ErrorAction SilentlyContinue |
         Where-Object { $_.FullName -notmatch '\\(ref|obj)\\' } |
         Select-Object -First 3
$cache = Join-Path $env:USERPROFILE '.nuget\packages\microsoft.data.sqlclient'
if (Test-Path -LiteralPath $cache) {
    $dlls += Get-ChildItem -Path $cache -Recurse -Filter 'Microsoft.Data.SqlClient.dll' -File -ErrorAction SilentlyContinue |
             Where-Object { $_.FullName -match '\\lib\\(net9\.0|net8\.0|netstandard2\.0)\\' } |
             Sort-Object FullName -Descending
}

foreach ($dll in $dlls) {
    try {
        Add-Type -Path $dll.FullName -ErrorAction Stop
        $tipoConexion = [Type]::GetType('Microsoft.Data.SqlClient.SqlConnection, Microsoft.Data.SqlClient')
        if ($tipoConexion) { break }
    } catch {
        # Dependencia suelta o version incompatible: se prueba con la siguiente.
    }
}

# Candidatos, en orden: Microsoft.Data.SqlClient (el del proyecto) y, si al abrir
# falla por falta de SNI nativo, System.Data.SqlClient, que viene con Windows.
$candidatos = @($tipoConexion)
$tipoSds = [Type]::GetType('System.Data.SqlClient.SqlConnection, System.Data.SqlClient')
if ($tipoSds) { $candidatos += $tipoSds }
$candidatos = @($candidatos | Where-Object { $null -ne $_ })

if ($candidatos.Count -eq 0) {
    Write-Host 'No se pudo cargar ningun cliente SQL (ni Microsoft.Data.SqlClient ni System.Data.SqlClient).'
    Write-Host 'Corre el script con Windows PowerShell (powershell.exe).'
    exit 1
}


if (-not (Test-Path -LiteralPath $SqlPath)) {
    Write-Host ('No se encontro el script: ' + $SqlPath)
    exit 1
}

# --- IDs declarados en el bloque #Mapping del .sql ---------------------------
$sqlTexto = Get-Content -LiteralPath $SqlPath -Raw
# ^ anclado por fila: sin el ancla, un IN (N'a', N'b') del propio script se
# confunde con una fila del mapping.
$sqlFilas = @(
    [regex]::Matches($sqlTexto, "(?m)^[ \t]*\(N'([^']*)',[ \t]*N'([^']*)'\)") |
        ForEach-Object {
            [pscustomobject]@{
                Id   = ('' + $_.Groups[1].Value).Trim()
                Tipo = ('' + $_.Groups[2].Value).Trim()
            }
        }
)
$sqlIds = @($sqlFilas | ForEach-Object { $_.Id })

# --- IDs declarados en el CSV ------------------------------------------------
$csvFilas = @()
if (Test-Path -LiteralPath $CsvPath) {
    $csvFilas = @(
        Import-Csv -LiteralPath $CsvPath | ForEach-Object {
            [pscustomobject]@{
                Id   = ('' + $_.product_id).Trim()
                Tipo = ('' + $_.tipo).Trim()
            }
        }
    )
}
$csvIds = @($csvFilas | ForEach-Object { $_.Id })

# Tipos aceptados: los mismos que el .sql normaliza y deja en los cuatro canonicos.
$tiposValidos = @('Master', 'Rollo Cortado', 'Rollos Cortados', 'Rollo', 'Hoja', 'Hojas', 'Resma', 'Resmas', 'Graphics', 'Grafica', 'Graficas')

Write-Host ''
Write-Host '=== Tipos escritos hasta ahora ==='
$malEscritos = @()
foreach ($origen in @(@{ Nombre = '.sql'; Filas = $sqlFilas }, @{ Nombre = '.csv'; Filas = $csvFilas })) {
    foreach ($fila in $origen.Filas) {
        if ($fila.Tipo -ne '') {
            Write-Host ('  ' + $origen.Nombre + ' ' + $fila.Id + ' -> ' + $fila.Tipo)
            if ($tiposValidos -notcontains $fila.Tipo) {
                $malEscritos += ($origen.Nombre + ' ' + $fila.Id + " (" + $fila.Tipo + ")")
            }
        }
    }
}
if ($malEscritos.Count -gt 0) {
    Write-Host ('  TIPOS NO RECONOCIDOS: ' + ($malEscritos -join ', '))
}

# --- IDs reales de la base (consulta de solo lectura) ------------------------
$conn = $null
foreach ($tipo in $candidatos) {
    try {
        $prueba = $tipo::new($ConnectionString)
        $prueba.Open()
        $conn = $prueba
        Write-Host ('Cliente SQL: ' + $tipo.FullName)
        break
    } catch {
        Write-Host ('  descartado ' + $tipo.FullName + ': ' + $_.Exception.Message)
    }
}

if (-not $conn) {
    Write-Host 'No se pudo abrir conexion con ninguno de los clientes SQL disponibles.'
    exit 1
}

$cmd = $conn.CreateCommand()
$cmd.CommandText = @'
SELECT LTRIM(RTRIM(p.Product_ID)) AS Product_ID,
       MAX(ISNULL(p.Product_Name, '')) AS Nombre,
       MAX(CAST(p.anulado AS int)) AS Anulado,
       MAX(CASE WHEN p.MasterRolls = 1 THEN 'Master'
                WHEN p.rollo_cortado = 1 THEN 'Rollo Cortado'
                WHEN p.Resmas = 1 THEN 'Resma'
                WHEN p.Graphics = 1 THEN 'Graphics'
                ELSE '(sin categoria)' END) AS TipoActual
FROM dbo.producto p
GROUP BY LTRIM(RTRIM(p.Product_ID))
'@
$reader = $cmd.ExecuteReader()
$base = @{}
while ($reader.Read()) {
    $base[$reader.GetString(0)] = [pscustomobject]@{
        Nombre  = $reader.GetString(1)
        Anulado = $reader.GetInt32(2)
        Tipo    = $reader.GetString(3)
    }
}
$reader.Close()

$distCmd = $conn.CreateCommand()
$distCmd.CommandText = @'
SELECT t.Tipo, COUNT(*) AS Productos
FROM dbo.producto p
CROSS APPLY (SELECT CASE WHEN p.MasterRolls = 1 THEN 'Master'
                         WHEN p.rollo_cortado = 1 THEN 'Rollo Cortado'
                         WHEN p.Resmas = 1 THEN 'Resma'
                         WHEN p.Graphics = 1 THEN 'Graphics'
                         ELSE '(sin categoria)' END AS Tipo) t
GROUP BY t.Tipo
'@
$dist = $distCmd.ExecuteReader()
Write-Host '=== Distribucion actual en dbo.producto ==='
while ($dist.Read()) {
    Write-Host ('  ' + $dist.GetString(0) + ': ' + $dist.GetInt32(1))
}
$dist.Close()
$conn.Close()

# --- Comparaciones -----------------------------------------------------------
$sqlIdsTrimmed = @($sqlIds | ForEach-Object { $_.Trim() })
$sqlUnicos = @($sqlIdsTrimmed | Sort-Object -Unique)
$csvUnicos = @($csvIds | Sort-Object -Unique)

Write-Host ''
Write-Host '=== Cobertura del mapping ==='
Write-Host ('  filas en el .sql     : ' + $sqlIdsTrimmed.Count + ' (unicos: ' + $sqlUnicos.Count + ')')
Write-Host ('  filas en el .csv     : ' + $csvIds.Count + ' (unicos: ' + $csvUnicos.Count + ')')
Write-Host ('  productos en la base : ' + $base.Count)

$problemas = 0

$duplicadosSql = @($sqlIdsTrimmed | Group-Object | Where-Object { $_.Count -gt 1 } | ForEach-Object { $_.Name })
if ($duplicadosSql.Count -gt 0) {
    $problemas++
    Write-Host ('  DUPLICADOS en el .sql: ' + ($duplicadosSql -join ', '))
}

$inexistentes = @($sqlUnicos | Where-Object { -not $base.ContainsKey($_) })
if ($inexistentes.Count -gt 0) {
    $problemas++
    Write-Host ('  NO EXISTEN en dbo.producto: ' + ($inexistentes -join ', '))
}

$faltantes = @($base.Keys | Where-Object { $sqlUnicos -notcontains $_ } | Sort-Object)
if ($faltantes.Count -gt 0) {
    Write-Host ('  en la base pero FUERA del mapping (' + $faltantes.Count + '), quedan como estan:')
    foreach ($id in $faltantes) {
        $marca = ''
        if ($base[$id].Anulado -eq 1) { $marca = ' | ANULADO' }
        Write-Host ('    ' + $id + ' | ' + $base[$id].Tipo + $marca)
    }
}

if ($csvUnicos.Count -gt 0) {
    $soloCsv = @($csvUnicos | Where-Object { $sqlUnicos -notcontains $_ })
    $soloSql = @($sqlUnicos | Where-Object { $csvIds -notcontains $_ })
    if ($soloCsv.Count -gt 0 -or $soloSql.Count -gt 0) {
        $problemas++
        Write-Host '  DESACUERDO entre .csv y .sql:'
        if ($soloCsv.Count -gt 0) { Write-Host ('    solo en .csv: ' + ($soloCsv -join ', ')) }
        if ($soloSql.Count -gt 0) { Write-Host ('    solo en .sql: ' + ($soloSql -join ', ')) }
    }
}

if ($malEscritos.Count -gt 0) {
    $problemas++
}

Write-Host ''
if ($problemas -eq 0) {
    Write-Host 'OK: los IDs del script existen en la base y no hay duplicados.'
} else {
    Write-Host ('Hay ' + $problemas + ' detalle(s) para revisar antes de ejecutar el script.')
}

