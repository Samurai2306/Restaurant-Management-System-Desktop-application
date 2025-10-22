<#
Scaffold solution and projects for RestaurantSystem
Usage: Run from repository root as administrator
#>

param()

$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Definition
Set-Location -Path $repoRoot

$sln = "RestaurantSystem.sln"
if (-not (Test-Path $sln)) {
 Write-Host "Creating solution $sln"
 & dotnet new sln -n RestaurantSystem
} else {
 Write-Host "Solution $sln already exists"
}

$projects = @(
 @{ Name = 'RestaurantSystem.Core'; Type = 'classlib'; Path = 'src\RestaurantSystem.Core' },
 @{ Name = 'RestaurantSystem.Data'; Type = 'classlib'; Path = 'src\RestaurantSystem.Data' },
 @{ Name = 'RestaurantSystem.UI'; Type = 'wpf'; Path = 'src\RestaurantSystem.UI' },
 @{ Name = 'RestaurantSystem.Core.Tests'; Type = 'xunit'; Path = 'tests\RestaurantSystem.Core.Tests' },
 @{ Name = 'RestaurantSystem.Data.Tests'; Type = 'xunit'; Path = 'tests\RestaurantSystem.Data.Tests' }
)

foreach ($p in $projects) {
 $projDir = $p.Path
 $projFile = Join-Path $projDir "$($p.Name).csproj"
 if (-not (Test-Path $projFile)) {
 Write-Host "Creating project $($p.Name) of type $($p.Type) at $projDir"
 & dotnet new $($p.Type) -n $($p.Name) -o $projDir -f net8.0
 } else {
 Write-Host "Project $($p.Name) already exists"
 }

 # Add to solution if not present
 $projRelative = $projFile -replace '\\','/'

 $slnContent = Get-Content -Path $sln -ErrorAction SilentlyContinue -Raw

 if ($slnContent -and $slnContent.Contains($projRelative)) {
 Write-Host "Project $($p.Name) already added to solution"
 } else {
 Write-Host "Adding $($p.Name) to solution"
 & dotnet sln add $projFile
 }
}

Write-Host "\nAdding NuGet packages..."

function TryDotnetAdd($projectPath, $package, $args='') {
 try {
 Write-Host "Adding $package to $projectPath"
 & dotnet add $projectPath package $package $args
 } catch {
 # Avoid using $_ directly inside double-quoted strings - format message instead
 Write-Warning ("Failed to add {0} to {1}: {2}" -f $package, $projectPath, $_)
 }
}

# Core packages
TryDotnetAdd 'src\RestaurantSystem.Core\RestaurantSystem.Core.csproj' 'CommunityToolkit.Mvvm' '--prerelease'
TryDotnetAdd 'src\RestaurantSystem.Core\RestaurantSystem.Core.csproj' 'AutoMapper'
TryDotnetAdd 'src\RestaurantSystem.Core\RestaurantSystem.Core.csproj' 'Microsoft.Extensions.DependencyInjection'
TryDotnetAdd 'src\RestaurantSystem.Core\RestaurantSystem.Core.csproj' 'Microsoft.Extensions.Logging'

# Data packages
TryDotnetAdd 'src\RestaurantSystem.Data\RestaurantSystem.Data.csproj' 'Microsoft.EntityFrameworkCore'
TryDotnetAdd 'src\RestaurantSystem.Data\RestaurantSystem.Data.csproj' 'Microsoft.EntityFrameworkCore.Sqlite'
TryDotnetAdd 'src\RestaurantSystem.Data\RestaurantSystem.Data.csproj' 'Microsoft.EntityFrameworkCore.Design'
TryDotnetAdd 'src\RestaurantSystem.Data\RestaurantSystem.Data.csproj' 'Microsoft.Extensions.Configuration.Json'

# UI packages
TryDotnetAdd 'src\RestaurantSystem.UI\RestaurantSystem.UI.csproj' 'CommunityToolkit.Mvvm' '--prerelease'
TryDotnetAdd 'src\RestaurantSystem.UI\RestaurantSystem.UI.csproj' 'MahApps.Metro'
TryDotnetAdd 'src\RestaurantSystem.UI\RestaurantSystem.UI.csproj' 'MaterialDesignThemes'
TryDotnetAdd 'src\RestaurantSystem.UI\RestaurantSystem.UI.csproj' 'Microsoft.Extensions.Hosting'
TryDotnetAdd 'src\RestaurantSystem.UI\RestaurantSystem.UI.csproj' 'Microsoft.Extensions.DependencyInjection'

# Tests packages
TryDotnetAdd 'tests\RestaurantSystem.Core.Tests\RestaurantSystem.Core.Tests.csproj' 'Moq'
TryDotnetAdd 'tests\RestaurantSystem.Core.Tests\RestaurantSystem.Core.Tests.csproj' 'FluentAssertions'
TryDotnetAdd 'tests\RestaurantSystem.Data.Tests\RestaurantSystem.Data.Tests.csproj' 'Microsoft.EntityFrameworkCore.InMemory'

Write-Host "\nRestoring and building solution..."
& dotnet restore
& dotnet build

Write-Host "Scaffolding completed."
