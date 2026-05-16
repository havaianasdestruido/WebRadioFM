param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [string[]]$Platforms = @('x86', 'x64'),

    [switch]$Clean
)

# Normalize: if Platforms is a single comma-separated string, split it
if ($Platforms.Count -eq 1 -and $Platforms[0] -match ',') {
    $Platforms = $Platforms[0] -split ','
}

$SolutionPath = "$PSScriptRoot\WebRadioFM.sln"
$OutputRoot = "$PSScriptRoot\BuildOutput"

# --- Locate MSBuild ---
$MSBuildCandidates = @(
    "${env:ProgramFiles}\Microsoft Visual Studio\2022\Community\MSBuild\Current\bin\MSBuild.exe",
    "${env:ProgramFiles}\Microsoft Visual Studio\18\Community\MSBuild\Current\bin\MSBuild.exe",
    "${env:ProgramFiles(x86)}\MSBuild\12.0\bin\MSBuild.exe"
)
$MSBuild = $null
foreach ($candidate in $MSBuildCandidates) {
    if (Test-Path -LiteralPath $candidate) {
        $MSBuild = $candidate
        break
    }
}

if (-not $MSBuild) {
    $found = Get-Command msbuild.exe -ErrorAction SilentlyContinue
    if ($found) { $MSBuild = $found.Source }
}

if (-not $MSBuild) {
    Write-Host "ERROR: MSBuild not found. Install Visual Studio or Build Tools." -ForegroundColor Red
    exit 1
}

Write-Host "MSBuild: $MSBuild" -ForegroundColor DarkGray

if ($Clean) {
    Write-Host "Cleaning build artifacts..." -ForegroundColor Yellow
    Remove-Item -LiteralPath $OutputRoot -Recurse -Force -ErrorAction SilentlyContinue
    foreach ($Platform in $Platforms) {
        & $MSBuild $SolutionPath /t:Clean /p:Configuration=$Configuration /p:Platform=$Platform /nologo
    }
    return
}

# --- Restore NuGet packages ---
$NuGetExe = "$PSScriptRoot\.nuget\NuGet.exe"
if (Test-Path -LiteralPath $NuGetExe) {
    Write-Host "Restoring NuGet packages..." -ForegroundColor Green
    & $NuGetExe restore $SolutionPath
}

Write-Host "=== Building WebRadioFM ===" -ForegroundColor Cyan
Write-Host "Configuration: $Configuration" -ForegroundColor Gray
Write-Host "Platforms:     $($Platforms -join ', ')" -ForegroundColor Gray
Write-Host ""

$failed = $false

:platforms foreach ($Platform in $Platforms) {
    Write-Host "[$Platform] Building..." -ForegroundColor Green

    & $MSBuild $SolutionPath /t:Build /p:Configuration=$Configuration /p:Platform=$Platform /nologo /verbosity:minimal
    if ($LASTEXITCODE -ne 0) {
        Write-Host "[$Platform] Build failed (exit code $LASTEXITCODE)" -ForegroundColor Red
        $failed = $true
        continue
    }

    # Locate .XAP
    $XapName = "WebRadioFM_${Configuration}_${Platform}.xap"
    $XapSource = "$PSScriptRoot\WebRadioFM\Bin\$Platform\$Configuration\$XapName"
    if (-not (Test-Path -LiteralPath $XapSource)) {
        $XapSource = "$PSScriptRoot\WebRadioFM\Bin\$Configuration\$XapName"
    }

    if (Test-Path -LiteralPath $XapSource) {
        $PlatformDir = "$OutputRoot\$Platform"
        New-Item -ItemType Directory -Path $PlatformDir -Force | Out-Null
        Copy-Item -LiteralPath $XapSource -Destination "$PlatformDir\$XapName" -Force
        Write-Host "[$Platform] XAP: $PlatformDir\$XapName" -ForegroundColor Green
    } else {
        Write-Host "[$Platform] WARNING: .XAP not found" -ForegroundColor Yellow
    }
}

Write-Host ""
if ($failed) {
    Write-Host "One or more platforms failed to build." -ForegroundColor Red
    exit 1
}

Write-Host "NOTE:" -ForegroundColor Yellow
Write-Host "This is a Windows Phone 8.1 Silverlight project. It only produces .XAP output." -ForegroundColor Yellow
Write-Host ".APPX and .APPBUNDLE are UWP output formats. To produce them:" -ForegroundColor Yellow
Write-Host "  1. Convert the project to UWP or create a new UWP project" -ForegroundColor Yellow
Write-Host "  2. Use: msbuild /t:Build /p:AppxPackage=true /p:Configuration=Release" -ForegroundColor Yellow
Write-Host ""
Write-Host "Build complete!" -ForegroundColor Cyan
