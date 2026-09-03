param(
    [int]$Port = 5000,
    [string]$IisExpress = "C:\Program Files\IIS Express\iisexpress.exe"
)

$ErrorActionPreference = "Stop"

$backendRoot = Split-Path -Parent $PSScriptRoot
$apiPath = Join-Path $backendRoot "src\Escola.Api"

if (-not (Test-Path -LiteralPath $IisExpress)) {
    throw "IIS Express not found at '$IisExpress'."
}

if (-not (Test-Path -LiteralPath (Join-Path $apiPath "Web.config"))) {
    throw "API project not found at '$apiPath'."
}

Write-Host "IIS Express: $IisExpress"
Write-Host "Physical path: $apiPath"
Write-Host "URL: http://localhost:$Port/"
Write-Host "Swagger: http://localhost:$Port/swagger"

$iisArgs = @(
    "/path:$apiPath",
    "/port:$Port",
    "/clr:v4.0"
)

& $IisExpress @iisArgs
if ($LASTEXITCODE -ne 0 -and $null -ne $LASTEXITCODE) {
    exit $LASTEXITCODE
}
