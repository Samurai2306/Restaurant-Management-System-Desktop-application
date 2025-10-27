# PowerShell script to publish the Restaurant Management System
# This script publishes the application as a self-contained executable

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Restaurant Management System - Build" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

$ErrorActionPreference = "Stop"

# Configuration
$ProjectPath = "src\RestaurantSystem.UI\RestaurantSystem.UI.csproj"
$OutputPath = "publish\windows-x64"
$Runtime = "win-x64"

# Clean previous builds
Write-Host "Cleaning previous builds..." -ForegroundColor Yellow
if (Test-Path $OutputPath) {
    Remove-Item $OutputPath -Recurse -Force
}

# Build the application
Write-Host "Building application..." -ForegroundColor Yellow
dotnet build $ProjectPath --configuration Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

# Publish the application
Write-Host "Publishing application..." -ForegroundColor Yellow
dotnet publish $ProjectPath `
    --configuration Release `
    --runtime $Runtime `
    --self-contained true `
    --output $OutputPath `
    -p:PublishSingleFile=false `
    -p:PublishReadyToRun=true `
    -p:IncludeNativeLibrariesForSelfExtract=true

if ($LASTEXITCODE -ne 0) {
    Write-Host "Publish failed!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "=====================================" -ForegroundColor Green
Write-Host "Build completed successfully!" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green
Write-Host "Output directory: $OutputPath" -ForegroundColor Green
Write-Host ""

