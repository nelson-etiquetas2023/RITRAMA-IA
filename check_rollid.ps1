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
$cmd.CommandText = "SELECT roll_id, part_number FROM MasterInic WHERE roll_id LIKE '%225176002%'"
$reader = $cmd.ExecuteReader()
$found = $false
while ($reader.Read()) {
    $found = $true
    Write-Host "roll_id: " $reader.GetString(0)
    Write-Host "part_number: " $reader.GetString(1)
}
if (-not $found) { Write-Host 'NO HAY RESULTADOS' }
$conn.Close()
