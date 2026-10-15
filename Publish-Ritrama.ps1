# Builds Ritrama2025 once (Release, win-x64 self-contained) and prepares
# two deployment folders: TEST and PROD. Run from the repo root.
param(
    [string]$Version = "v1.02",
    [string]$OutputRoot = ".\publish"
)

$ErrorActionPreference = "Stop"
$project = "Ritrama2025.csproj"
$releaseDir = Join-Path $OutputRoot $Version
$deployRoot = Join-Path $OutputRoot "deploy"
$prodDir = Join-Path $deployRoot "prod"
$testDir = Join-Path $deployRoot "test"

if (Test-Path $releaseDir) { Remove-Item $releaseDir -Recurse -Force }
New-Item -ItemType Directory -Force -Path $releaseDir, $deployRoot | Out-Null

Write-Host "== Building $project (Release, win-x64, self-contained) =="
dotnet publish $project -c Release -r win-x64 --self-contained true -o $releaseDir
if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed" }

# PROD folder: keep appsettings.Production.json, exclude Testing launcher/config
if (Test-Path $prodDir) { Remove-Item $prodDir -Recurse -Force }
Copy-Item $releaseDir $prodDir -Recurse

# TEST folder: neutralize production config and promote Testing config
if (Test-Path $testDir) { Remove-Item $testDir -Recurse -Force }
Copy-Item $releaseDir $testDir -Recurse

$testAppsettings = Join-Path $testDir "appsettings.json"
$testingTemplate = Join-Path (Get-Location) "appsettings.Testing.json"
Copy-Item $testingTemplate $testAppsettings -Force
Remove-Item (Join-Path $testDir "appsettings.Production.json") -ErrorAction SilentlyContinue

# Copy launchers (already in repo root)
Copy-Item "Run-Ritrama-PROD.cmd" (Join-Path $prodDir "Run-Ritrama-PROD.cmd") -Force
Copy-Item "Run-Ritrama-TEST.cmd"  (Join-Path $testDir  "Run-Ritrama-TEST.cmd") -Force

Write-Host ""
Write-Host "OK - artefactos listos:"
Write-Host "  PROD -> $prodDir"
Write-Host "  TEST -> $testDir"
Write-Host ""
Write-Host "Siguiente: copiar a los folders del servidor con robocopy, sin sobrescribir los appsettings personalizados."