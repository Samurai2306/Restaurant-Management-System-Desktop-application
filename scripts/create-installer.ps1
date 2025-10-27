# PowerShell script to create Windows installer
# This script builds the app and creates an installer using Inno Setup

Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "Restaurant Management System - Installer" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

$ErrorActionPreference = "Stop"

# Check if Inno Setup is installed
$InnoSetupPath = "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe"

if (-not (Test-Path $InnoSetupPath)) {
    Write-Host "Inno Setup not found!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please install Inno Setup 6 from:" -ForegroundColor Yellow
    Write-Host "https://jrsoftware.org/isdl.php" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "After installation, run this script again." -ForegroundColor Yellow
    exit 1
}

# Step 1: Publish the application
Write-Host "Step 1: Publishing application..." -ForegroundColor Yellow
& "$PSScriptRoot\publish-app.ps1"

if ($LASTEXITCODE -ne 0) {
    Write-Host "Publishing failed!" -ForegroundColor Red
    exit 1
}

# Step 2: Create installer directory
Write-Host ""
Write-Host "Step 2: Preparing installer..." -ForegroundColor Yellow
$InstallerDir = "installer"
if (-not (Test-Path $InstallerDir)) {
    New-Item -ItemType Directory -Path $InstallerDir | Out-Null
}

# Step 3: Build installer with Inno Setup
Write-Host ""
Write-Host "Step 3: Building installer..." -ForegroundColor Yellow
& $InnoSetupPath "setup\setup.iss"

if ($LASTEXITCODE -ne 0) {
    Write-Host "Installer creation failed!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "=============================================" -ForegroundColor Green
Write-Host "Installer created successfully!" -ForegroundColor Green
Write-Host "=============================================" -ForegroundColor Green
Write-Host ""
Write-Host "Installer location: $InstallerDir\" -ForegroundColor Green
Get-ChildItem $InstallerDir -Filter "*.exe" | ForEach-Object {
    Write-Host "  - $($_.Name)" -ForegroundColor Cyan
}
Write-Host ""

