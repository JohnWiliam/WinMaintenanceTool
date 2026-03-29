param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

$project = Join-Path $PSScriptRoot "src/WinMaintenanceTool/WinMaintenanceTool.csproj"
$buildDir = Join-Path $PSScriptRoot "Build"

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw ".NET SDK 10.0.201 não encontrado no PATH."
}

$version = (dotnet --version).Trim()
if ($version -ne "10.0.201") {
    Write-Warning "SDK detectado: $version. O recomendado para esta solução é 10.0.201."
}

if (Test-Path $buildDir) {
    Remove-Item $buildDir -Recurse -Force
}

New-Item -Path $buildDir -ItemType Directory | Out-Null

$publishArgs = @(
    "publish", $project,
    "-c", $Configuration,
    "-r", "win-x64",
    "-o", $buildDir,
    "--self-contained", "true",
    "-p:PublishSingleFile=true",
    "-p:PublishTrimmed=false",
    "-p:EnableCompressionInSingleFile=true",
    "-p:IncludeNativeLibrariesForSelfExtract=true"
)

Write-Host "Executando: dotnet $($publishArgs -join ' ')" -ForegroundColor Cyan

dotnet @publishArgs

Write-Host "Build portátil concluída em: $buildDir" -ForegroundColor Green
