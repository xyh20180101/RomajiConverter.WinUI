$project = "RomajiConverter.WinUI.csproj"
$outRoot = "bin\publish"
$tmpName = "RomajiConverter.WinUI"

$configs = @(
    @{
        Name = "framework-dependent-win-x64"
        Rid = "win-x64"
        Platform = "x64"
        SelfContained = "false"
        OutDir = "fd-win-x64"
    },
    @{
        Name = "self-contained-win-x64"
        Rid = "win-x64"
        Platform = "x64"
        SelfContained = "true"
        OutDir = "sc-win-x64"
    })

foreach ($c in $configs) {
    $publishDir = Join-Path $outRoot $c.OutDir

    dotnet publish $project `
        -c Release `
        -r $c.Rid `
        -p:Platform=$($c.Platform) `
        -p:SelfContained=$($c.SelfContained) `
        -p:PublishReadyToRun=true `
        -o $publishDir

    Push-Location $outRoot

    Rename-Item $c.OutDir $tmpName
    Compress-Archive `
        -Path $tmpName `
        -DestinationPath "RomajiConverter.WinUI-$($c.Name).zip" `
        -CompressionLevel Optimal `
        -Force
    Rename-Item $tmpName $c.OutDir

    Pop-Location
}