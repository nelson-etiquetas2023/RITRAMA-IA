$assembly = [System.Reflection.Assembly]::LoadWithPartialName('Microsoft.Data.SqlClient')
if (-not $assembly) {
    Write-Host 'Cannot load Microsoft.Data.SqlClient'
    return
}
$connString = 'Data Source=192.168.10.10; Initial Catalog=RITRAMASQL2017;User Id=Npino;Password=Jossycar5%;Encrypt=True;TrustServerCertificate=True;'
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