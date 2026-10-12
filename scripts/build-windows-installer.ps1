$ErrorActionPreference = 'Stop'
$xiaodouRepository = Split-Path -Parent $PSScriptRoot
$xiaodouCompiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (-not (Test-Path -LiteralPath $xiaodouCompiler)) {
    $xiaodouCompiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe'
}
if (-not (Test-Path -LiteralPath $xiaodouCompiler)) { throw '.NET Framework C# compiler was not found.' }
$xiaodouOutput = Join-Path $xiaodouRepository 'downloads\Xiaodou-Installer-v1.0.2.exe'
$xiaodouProgram = Join-Path $xiaodouRepository 'source\installer\Program.cs'
$xiaodouManifest = Join-Path $xiaodouRepository 'source\installer\app.manifest'
$xiaodouSprite = Join-Path $xiaodouRepository 'desktop\xiaodou\spritesheet.png'
$xiaodouPet = Join-Path $xiaodouRepository 'desktop\xiaodou\pet.json'
$xiaodouPreview = Join-Path $xiaodouRepository 'source\original-concept.png'
& $xiaodouCompiler /nologo /target:winexe /platform:anycpu /optimize+ /codepage:65001 /utf8output `
    /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /reference:System.Web.Extensions.dll `
    "/win32manifest:$xiaodouManifest" "/out:$xiaodouOutput" `
    "/resource:$xiaodouSprite,Xiaodou.Spritesheet,private" "/resource:$xiaodouPet,Xiaodou.Manifest,private" `
    "/resource:$xiaodouPreview,Xiaodou.Preview,private" $xiaodouProgram
if ($LASTEXITCODE -ne 0) { throw 'Installer compilation failed.' }
Get-Item -LiteralPath $xiaodouOutput | Select-Object Name,Length
