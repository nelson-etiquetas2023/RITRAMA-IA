param(
    [string]$ConnectionString = ''
)

# La cadena de conexion NO esta en este archivo: se lee de la configuracion del
# proyecto, que vive en appsettings.Development.json (archivo ignorado por git).
# Para correr contra otra base sin tocar el archivo se puede pasar -ConnectionString.
$configPath = Join-Path $PSScriptRoot 'appsettings.Development.json'
$configSection = 'ConnectionStringsEnvironment'
$configKey = 'Desarrollo'

if ([string]::IsNullOrWhiteSpace($ConnectionString)) {
    if (-not (Test-Path -LiteralPath $configPath)) {
        Write-Host ('Falta la cadena de conexion: no se encontro el archivo de configuracion ' + $configPath)
        Write-Host ('Definela en ' + $configSection + ':' + $configKey + ' de appsettings.Development.json, o pasa el parametro -ConnectionString.')
        exit 1
    }
    try {
        $config = Get-Content -LiteralPath $configPath -Raw | ConvertFrom-Json
        $ConnectionString = $config.$configSection.$configKey
    } catch {
        Write-Host ('Falta la cadena de conexion: no se pudo leer ' + $configPath + ' (' + $_.Exception.Message + ')')
        Write-Host ('Definela en ' + $configSection + ':' + $configKey + ' de appsettings.Development.json, o pasa el parametro -ConnectionString.')
        exit 1
    }
}

if ([string]::IsNullOrWhiteSpace($ConnectionString)) {
    Write-Host ('Falta la cadena de conexion: la clave ' + $configSection + ':' + $configKey + ' de appsettings.Development.json esta vacia o no existe.')
    Write-Host 'Pasa el parametro -ConnectionString para correr contra otra base sin tocar el archivo.'
    exit 1
}

$assembly = [System.Reflection.Assembly]::LoadWithPartialName('Microsoft.Data.SqlClient')
if (-not $assembly) {
    Write-Host 'No se pudo cargar Microsoft.Data.SqlClient'
    exit 1
}

$connString = $ConnectionString
$conn = New-Object Microsoft.Data.SqlClient.SqlConnection $connString
$conn.Open()
$cmd = $conn.CreateCommand()
$cmd.CommandText = 'SELECT COUNT(*) FROM MasterInic'
$result = $cmd.ExecuteScalar()
$total = [int]$result
Write-Host ('Total filas en MasterInic: ' + $total.ToString())
$cmd2 = $conn.CreateCommand()
$cmd2.CommandText = 'SELECT roll_id FROM MasterInic ORDER BY roll_id'
$reader = $cmd2.ExecuteReader()
$count = 0
while ($reader.Read()) {
    $count++
    $rowVal = $reader.GetString(0)
    if ($count -le 10) {
        Write-Host ('Row ' + $count.ToString() + ': ' + $rowVal)
    }
}
if ($count -gt 10) {
    Write-Host ('...' + ' Total mostrado: 10 de ' + $count.ToString() + ' filas')
}
Write-Host ('---')
$reader.Close()
$conn.Close()
