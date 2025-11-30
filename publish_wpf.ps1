# Publish WPF App as Single File Executable
Write-Host "Publishing HarborFlow.Wpf..." -ForegroundColor Cyan

# Define output directory
$outputDir = "./Publish/Wpf"

# Clean previous publish
if (Test-Path $outputDir) {
    Remove-Item $outputDir -Recurse -Force
}

# Run dotnet publish
# -c Release: Optimized build
# -r win-x64: Target Windows 64-bit
# --self-contained true: Include .NET Runtime (no install needed on target)
# -p:PublishSingleFile=true: Bundle everything into one .exe
# -p:IncludeNativeLibrariesForSelfExtract=true: Extract native libs to temp folder (avoids some issues)
dotnet publish HarborFlow.Wpf/HarborFlow.Wpf.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o $outputDir

if ($LASTEXITCODE -eq 0) {
    Write-Host "Build Success!" -ForegroundColor Green
    Write-Host "Executable located at: $(Resolve-Path $outputDir)\HarborFlow.Wpf.exe" -ForegroundColor Green
} else {
    Write-Host "Build Failed." -ForegroundColor Red
}
