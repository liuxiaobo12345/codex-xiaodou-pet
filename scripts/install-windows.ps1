[CmdletBinding(SupportsShouldProcess = $true)]
param([string]$CodexHome)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 2.0

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$sourceDirectory = Join-Path $repositoryRoot 'desktop\xiaodou'
$sourceManifest = Join-Path $sourceDirectory 'pet.json'
$sourceImage = Join-Path $sourceDirectory 'spritesheet.png'
$manifest = Get-Content -LiteralPath $sourceManifest -Raw -Encoding UTF8 | ConvertFrom-Json
if ($manifest.spriteVersionNumber -ne 2 -or $manifest.spritesheetPath -ne 'spritesheet.png' -or [string]::IsNullOrWhiteSpace($manifest.displayName)) {
    throw 'Invalid desktop pet manifest.'
}
$expectedHash = 'd1110cab8de8f34a81e7132dd54abcb0b88e895e9891ff64e6b3fb0521e1dfac'
$imageHash = (Get-FileHash -LiteralPath $sourceImage -Algorithm SHA256).Hash.ToLowerInvariant()
if ($imageHash -ne $expectedHash) { throw 'The pet image checksum does not match this release.' }
$imageBytes = [System.IO.File]::ReadAllBytes($sourceImage)
$signature = [System.BitConverter]::ToString($imageBytes[0..7])
$width = [System.Net.IPAddress]::NetworkToHostOrder([System.BitConverter]::ToInt32($imageBytes, 16))
$height = [System.Net.IPAddress]::NetworkToHostOrder([System.BitConverter]::ToInt32($imageBytes, 20))
if ($signature -ne '89-50-4E-47-0D-0A-1A-0A' -or $width -ne 1536 -or $height -ne 2288) {
    throw 'Expected a 1536 x 2288 PNG sprite sheet.'
}
if ([string]::IsNullOrWhiteSpace($CodexHome)) {
    $CodexHome = $env:CODEX_HOME
    if ([string]::IsNullOrWhiteSpace($CodexHome)) {
        $CodexHome = Join-Path $env:USERPROFILE '.codex'
    }
}
$petRoot = Join-Path ([System.IO.Path]::GetFullPath($CodexHome)) 'pets'
$folderName = 'xiaodou'
$suffix = 1
while (Test-Path -LiteralPath (Join-Path $petRoot $folderName)) {
    $existingDirectory = Join-Path $petRoot $folderName
    $existingManifest = Join-Path $existingDirectory 'pet.json'
    $existingImage = Join-Path $existingDirectory 'spritesheet.png'
    if ((Test-Path -LiteralPath $existingManifest -PathType Leaf) -and (Test-Path -LiteralPath $existingImage -PathType Leaf)) {
        $sameImage = (Get-FileHash -LiteralPath $existingImage -Algorithm SHA256).Hash -eq $imageHash
        $sameManifest = (Get-FileHash -LiteralPath $existingManifest -Algorithm SHA256).Hash -eq (Get-FileHash -LiteralPath $sourceManifest -Algorithm SHA256).Hash
        if ($sameImage -and $sameManifest) {
            [pscustomobject]@{ status = 'already-installed'; directory = $existingDirectory; petId = "custom:$folderName" }
            return
        }
    }
    $suffix += 1
    $folderName = "xiaodou-$suffix"
}
$destination = Join-Path $petRoot $folderName
if ($PSCmdlet.ShouldProcess($destination, 'Install the Xiaodou PNG and pet.json')) {
    [void][System.IO.Directory]::CreateDirectory($petRoot)
    [void](New-Item -ItemType Directory -Path $destination)
    Copy-Item -LiteralPath $sourceImage -Destination (Join-Path $destination 'spritesheet.png')
    Copy-Item -LiteralPath $sourceManifest -Destination (Join-Path $destination 'pet.json')
    [pscustomobject]@{ status = 'installed'; directory = $destination; petId = "custom:$folderName" }
    Write-Host 'Open Settings > Pets, refresh, and choose Xiaodou. Restart the desktop app if needed.'
}
