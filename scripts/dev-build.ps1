# Development build script - builds and installs mod to game folder
param(
    [switch]$Release,
    [switch]$Test,
    [switch]$Watch
)

$ErrorActionPreference = "Stop"
$config = if ($Release) { "Release" } else { "Debug" }

Write-Host "🔨 Building StardewFOMO ($config)..." -ForegroundColor Cyan

if ($Test) {
    Write-Host "🧪 Running tests..." -ForegroundColor Yellow
    dotnet test tests/StardewFOMO.Core.Tests/StardewFOMO.Core.Tests.csproj -c $config --verbosity minimal
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Tests failed!" -ForegroundColor Red
        exit 1
    }
    Write-Host "✅ Tests passed!" -ForegroundColor Green
}

if ($Watch) {
    Write-Host "👀 Watching for changes (Ctrl+C to stop)..." -ForegroundColor Yellow
    dotnet watch build --project src/StardewFOMO.Mod/StardewFOMO.Mod.csproj -c $config
} else {
    dotnet build src/StardewFOMO.Mod/StardewFOMO.Mod.csproj -c $config
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "✅ Build successful! Mod installed to game folder." -ForegroundColor Green
        Write-Host "   Launch Stardew Valley through SMAPI to test." -ForegroundColor Gray
    } else {
        Write-Host "❌ Build failed!" -ForegroundColor Red
        exit 1
    }
}
