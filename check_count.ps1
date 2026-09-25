$assembly = [System.Reflection.Assembly]::LoadWithPartialName('Microsoft.Data.SqlClient')
if (-not $assembly) {
    Write-Host 'Cannot load Microsoft.Data.SqlClient'
    return
}
$connString = 'Data Source=192.168.10.10; Initial Catalog=RITRAMASQL2017;User Id=Npino;Password=Jossycar5%;Encrypt=True;TrustServerCertificate=True;'
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