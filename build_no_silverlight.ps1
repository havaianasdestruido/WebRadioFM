param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [string[]]$Platforms = @('x86', 'x64'),
    [switch]$Clean,
    [switch]$SetupOnly
)

$SolutionPath = "$PSScriptRoot\WebRadioFM.sln"
$OutputRoot = "$PSScriptRoot\BuildOutput"
$ProjectDir = "$PSScriptRoot\WebRadioFM"

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
    Write-Host "ERROR: MSBuild not found." -ForegroundColor Red
    exit 1
}
Write-Host "MSBuild: $MSBuild" -ForegroundColor DarkGray

# --- Check if we need admin fixup ---
$needsAdminFix = $false
$RefAsmPath81 = "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\WindowsPhone\v8.1"
$RefAsmPath80 = "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\WindowsPhone\v8.0"

# Check if v8.1 reference assemblies exist
$v81Exists = Test-Path -LiteralPath $RefAsmPath81
if (-not $v81Exists) {
    Write-Host "[SETUP] v8.1 reference assemblies missing -> will create junction" -ForegroundColor Yellow
    $needsAdminFix = $true
}

# Check if 64-bit Silverlight SDK registry key exists
$slKey = "HKLM:\SOFTWARE\Microsoft\Microsoft SDKs\Silverlight\v4.0"
$slKeyExists = Test-Path -LiteralPath $slKey
if (-not $slKeyExists) {
    Write-Host "[SETUP] Silverlight 4 SDK (64-bit registry) missing -> will create" -ForegroundColor Yellow
    $needsAdminFix = $true
}

# Check if 64-bit WP 8.1 ReferenceAssemblies registry key has InstallationFolder
$wpKey = "HKLM:\SOFTWARE\Microsoft\Microsoft SDKs\WindowsPhone\v8.1\ReferenceAssemblies"
$wpKeyOk = $false
if (Test-Path -LiteralPath $wpKey) {
    $val = Get-ItemProperty -LiteralPath $wpKey -Name "InstallationFolder" -ErrorAction SilentlyContinue
    if ($val -and $val.InstallationFolder) { $wpKeyOk = $true }
}
if (-not $wpKeyOk) {
    Write-Host "[SETUP] WP 8.1 ReferenceAssemblies (64-bit) missing InstallationFolder -> will create" -ForegroundColor Yellow
    $needsAdminFix = $true
}

# --- Admin fixup ---
function Run-AdminFixup {
    Write-Host "=== Running admin setup (elevating) ===" -ForegroundColor Cyan
    $scriptBlock = {
        $RefAsmPath81 = "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\WindowsPhone\v8.1"
        $RefAsmPath80 = "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\WindowsPhone\v8.0"

        # 1. Create v8.1 reference assemblies junction -> v8.0
        $v81Exists = Test-Path -LiteralPath $RefAsmPath81
        if (-not $v81Exists -and (Test-Path -LiteralPath $RefAsmPath80)) {
            Write-Host "  Creating v8.1 junction -> v8.0..."
            New-Item -ItemType Junction -Path $RefAsmPath81 -Target $RefAsmPath80 -Force | Out-Null
            Write-Host "  OK" -ForegroundColor Green
        } elseif ($v81Exists) {
            Write-Host "  v8.1 already exists" -ForegroundColor Gray
        }

        # 2. Create 64-bit Silverlight 4 SDK registry keys
        $base = "HKLM:\SOFTWARE\Microsoft\Microsoft SDKs\Silverlight\v4.0"
        if (-not (Test-Path -LiteralPath $base)) {
            New-Item -Path $base -Force | Out-Null
        }
        Set-ItemProperty -LiteralPath $base -Name "Version" -Value "4.0.60310.0" -Type String -Force

        $installPathKey = "$base\Install Path"
        if (-not (Test-Path -LiteralPath $installPathKey)) {
            New-Item -Path $installPathKey -Force | Out-Null
        }
        Set-ItemProperty -LiteralPath $installPathKey -Name "Install Path" -Value "C:\Program Files (x86)\Microsoft SDKs\Silverlight\v4.0\" -Type String -Force

        $refAsmKey = "$base\ReferenceAssemblies"
        if (-not (Test-Path -LiteralPath $refAsmKey)) {
            New-Item -Path $refAsmKey -Force | Out-Null
        }
        Set-ItemProperty -LiteralPath $refAsmKey -Name "SLRuntimeInstallPath" -Value "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\Silverlight\v4.0\" -Type String -Force
        Set-ItemProperty -LiteralPath $refAsmKey -Name "SLRuntimeInstallVersion" -Value "4.0.60310.0" -Type String -Force

        # 3. Create 64-bit WP 8.1 ReferenceAssemblies key
        $wpBase = "HKLM:\SOFTWARE\Microsoft\Microsoft SDKs\WindowsPhone\v8.1"
        if (-not (Test-Path -LiteralPath $wpBase)) {
            New-Item -Path $wpBase -Force | Out-Null
        }
        Set-ItemProperty -LiteralPath $wpBase -Name "InstallationFolder" -Value "C:\Program Files (x86)\Windows Phone Silverlight Kits\8.1\" -Type String -Force

        $wpRefAsmKey = "$wpBase\ReferenceAssemblies"
        if (-not (Test-Path -LiteralPath $wpRefAsmKey)) {
            New-Item -Path $wpRefAsmKey -Force | Out-Null
        }
        Set-ItemProperty -LiteralPath $wpRefAsmKey -Name "InstallationFolder" -Value "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\WindowsPhone\v8.1\" -Type String -Force
        Set-ItemProperty -LiteralPath $wpRefAsmKey -Name "SLRuntimeInstallVersion" -Value "6.7.50308.0" -Type String -Force

        Write-Host "Admin setup complete!" -ForegroundColor Green
    }

    $scriptPath = Join-Path $env:TEMP "wp81_fixup.ps1"
    Set-Content -LiteralPath $scriptPath -Value $scriptBlock.ToString()
    try {
        $psi = New-Object System.Diagnostics.ProcessStartInfo
        $psi.FileName = "powershell.exe"
        $psi.Arguments = "-NoProfile -ExecutionPolicy Bypass -File `"$scriptPath`""
        $psi.Verb = "runas"
        $psi.UseShellExecute = $true
        $proc = [System.Diagnostics.Process]::Start($psi)
        if ($proc) {
            $proc.WaitForExit()
            if ($proc.ExitCode -eq 0) {
                Write-Host "Admin setup succeeded!" -ForegroundColor Green
                return $true
            }
        }
    } catch {
        Write-Host "Admin elevation failed or was cancelled." -ForegroundColor Red
    }
    return $false
}

if ($needsAdminFix) {
    Write-Host ""
    Write-Host "Build requires admin rights for one-time setup:" -ForegroundColor Yellow
    Write-Host "  1. Create v8.1 reference assemblies junction (from v8.0)" -ForegroundColor Yellow
    Write-Host "  2. Create 64-bit Silverlight 4 SDK registry keys" -ForegroundColor Yellow
    Write-Host "  3. Create 64-bit WP 8.1 ReferenceAssemblies registry keys" -ForegroundColor Yellow
    Write-Host ""

    if ($SetupOnly) {
        Run-AdminFixup | Out-Null
        return
    }

    $ok = Run-AdminFixup
    if (-not $ok) {
        Write-Host ""
        Write-Host "ERROR: Cannot continue without admin setup." -ForegroundColor Red
        Write-Host "Run this script with -SetupOnly flag as Administrator first:" -ForegroundColor Yellow
        Write-Host "  .\build_no_silverlight.ps1 -SetupOnly" -ForegroundColor Yellow
        Write-Host "Then run: .\build_no_silverlight.ps1" -ForegroundColor Yellow
        exit 1
    }
}

# --- Normalize Platforms ---
if ($Platforms.Count -eq 1 -and $Platforms[0] -match ',') {
    $Platforms = $Platforms[0] -split ','
}

# --- Clean ---
if ($Clean) {
    Write-Host "Cleaning build artifacts..." -ForegroundColor Yellow
    Remove-Item -LiteralPath $OutputRoot -Recurse -Force -ErrorAction SilentlyContinue
    foreach ($Platform in $Platforms) {
        & $MSBuild $SolutionPath /t:Clean /p:Configuration=$Configuration /p:Platform=$Platform /nologo
    }
    return
}

# --- Patch XAP helper ---
function Patch-XAP {
    param([string]$XapPath)
    Write-Host "  Patching $XapPath for WP 8.0 compat..." -ForegroundColor Gray
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $tempDir = Join-Path $env:TEMP ([System.Guid]::NewGuid().ToString())
    New-Item -ItemType Directory -Path $tempDir -Force | Out-Null
    try {
        # Extract, patch, and repack
        [System.IO.Compression.ZipFile]::ExtractToDirectory($XapPath, $tempDir)
        # Patch WMAppManifest.xml
        $wm = "$tempDir\WMAppManifest.xml"
        if (Test-Path $wm) {
            $content = Get-Content $wm -Raw
            $content = $content -replace 'AppPlatformVersion="8\.1"', 'AppPlatformVersion="8.0"'
            Set-Content -Path $wm -Value $content -NoNewline
        }
        # Patch AppManifest.xaml
        $am = "$tempDir\AppManifest.xaml"
        if (Test-Path $am) {
            $content = Get-Content $am -Raw
            $content = $content -replace 'RuntimeVersion="6\.7\.50308\.0"', 'RuntimeVersion="4.7.50308.0"'
            Set-Content -Path $am -Value $content -NoNewline
        }
        # Remove MDILProjectFiles.xml (WP 8.0 doesn't use it)
        $mdil = "$tempDir\MDILProjectFiles.xml"
        if (Test-Path $mdil) { Remove-Item $mdil -Force }
        # Repack (delete original, create new ZIP)
        Remove-Item $XapPath -Force
        [System.IO.Compression.ZipFile]::CreateFromDirectory($tempDir, $XapPath)
        Write-Host "  Patched successfully" -ForegroundColor Green
    } finally {
        Remove-Item $tempDir -Recurse -Force -ErrorAction SilentlyContinue
    }
}

# --- Build ---
Write-Host "=== Building WebRadioFM (Silverlight-less) ===" -ForegroundColor Cyan
Write-Host "Configuration: $Configuration" -ForegroundColor Gray
Write-Host "Platforms:     $($Platforms -join ', ')" -ForegroundColor Gray
Write-Host ""

$failed = $false

foreach ($Platform in $Platforms) {
    Write-Host "[$Platform] Building..." -ForegroundColor Green

    $exitCode = & $MSBuild $SolutionPath /t:Build /p:Configuration=$Configuration /p:Platform=$Platform /nologo /verbosity:minimal
    if ($LASTEXITCODE -ne 0) {
        Write-Host "[$Platform] Build failed (exit code $LASTEXITCODE)" -ForegroundColor Red
        $failed = $true
        continue
    }

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
        Patch-XAP "$PlatformDir\$XapName"
    } else {
        Write-Host "[$Platform] WARNING: .XAP not found" -ForegroundColor Yellow
    }
}

Write-Host ""
if ($failed) {
    Write-Host "One or more platforms failed to build." -ForegroundColor Red
    exit 1
}
Write-Host "Build complete!" -ForegroundColor Cyan
