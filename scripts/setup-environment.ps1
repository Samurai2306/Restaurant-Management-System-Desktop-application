<#
PowerShell helper to install and update developer tools described in TECH_STACK.md
Usage:
 # Dry-run (default) - shows what would be installed
 .\setup-environment.ps1

 # Perform installations (requires winget and elevated privileges for some packages)
 .\setup-environment.ps1 -Install

 # Force reinstall / install without prompts
 .\setup-environment.ps1 -Install -Force
#>

param(
 [switch]$Install,
 [switch]$Force
)

function Write-Info($text){ Write-Host $text -ForegroundColor Cyan }
function Write-Ok($text){ Write-Host $text -ForegroundColor Green }
function Write-Warn($text){ Write-Host $text -ForegroundColor Yellow }
function Write-Err($text){ Write-Host $text -ForegroundColor Red }

Write-Info "Starting environment setup (per TECH_STACK.md)"

# Check winget
$winget = Get-Command winget -ErrorAction SilentlyContinue
if (-not $winget) {
 Write-Err "winget not found. This script uses winget to automate installs."
 Write-Warn "Please install winget or run the installation steps from TECH_STACK.md manually."
 return
}

# Tools to check/install
$packages = @(
 @{ Id = 'Git.Git'; Name = 'Git' },
 @{ Id = 'SQLiteBrowser.SQLiteBrowser'; Name = 'DB Browser for SQLite' },
 @{ Id = 'WiXToolset.WiXToolset'; Name = 'WiX Toolset' },
 @{ Id = 'Postman.Postman'; Name = 'Postman' },
 @{ Id = 'Microsoft.DotNet.SDK.8'; Name = '.NET SDK8' }
)

# Helper to check winget list
function Winget-Installed($id){
 try {
 # Call winget with the call operator and proper redirection spacing
 $out = & winget list --id $id 2>$null
 return ($out -and ($out -notmatch "No installed package found"))
 } catch { return $false }
}

# Check dotnet SDK versions
Write-Info "\nChecking installed .NET SDKs..."
$sdks = & dotnet --list-sdks 2>$null
if ($sdks) { Write-Ok "Detected SDKs:`n$sdks" } else { Write-Warn "No .NET SDKs detected." }

foreach ($pkg in $packages) {
 Write-Info "\nChecking $($pkg.Name)..."
 $installed = Winget-Installed($pkg.Id)
 if ($installed) {
 Write-Ok "$($pkg.Name) already installed (winget reported)."
 } else {
 Write-Warn "$($pkg.Name) not found."
 if ($Install) {
 Write-Info "Installing $($pkg.Name) via winget..."
 # Build argument array for winget to avoid parsing issues
 $wingetArgs = @('install', '--id', $pkg.Id, '-e', '--accept-source-agreements', '--accept-package-agreements')
 if ($Force) { $wingetArgs += '--silent' }
 Write-Info "Running: winget $($wingetArgs -join ' ')"
 try {
 & winget @wingetArgs 2>&1 | ForEach-Object { Write-Host $_ }
 Write-Ok "$($pkg.Name) installation finished (check output)."
 } catch {
 Write-Err "Failed to install $($pkg.Name). See winget output above."
 }
 } else {
 Write-Info "Run this script with -Install to install missing packages. Example: .\setup-environment.ps1 -Install"
 }
 }
}

Write-Info "\nAdditional manual steps you may need (not performed by this script):"
Write-Info " - Install Visual Studio Insiders2026 (if you prefer the Insiders build). See: https://visualstudio.microsoft.com/"
Write-Info " - Install Visual Studio workloads: '.NET desktop development' and 'Data storage and processing'."
Write-Info " - If you need a different .NET SDK channel, install it from https://dotnet.microsoft.com/download/dotnet/8.0"

Write-Ok "\nEnvironment setup script finished."
