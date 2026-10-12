# 小豆 Windows 安装器源码

这是随素材发布的 C# WinForms 安装器，版本为 v1.0.2。发布文件为 `downloads/Xiaodou-Installer-v1.0.2.exe`，使用 .NET Framework 4，不含外部运行服务。

## 安装行为

程序从程序集中的私有资源读取图片和宠物信息，检查 SHA-256、图片尺寸和必需字段。安装时不访问网络，也不运行其他程序。

安装目标是当前用户的 `.codex/pets/xiaodou/`；如果进程环境包含 `CODEX_HOME`，则使用该目录下的 `pets/`。程序只写入 `spritesheet.png` 和 `pet.json`，并在安装期间创建关闭后自动释放的锁文件。

已有相同图片和信息时，返回「已安装」。同名目录或文件包含其他内容时，使用 `xiaodou-2` 等新目录，保留已有内容。文件以 `CreateNew` 模式写入。安装器不修改应用配置、不自动选用宠物，也不重启应用。

manifest 使用 `asInvoker`，不请求提权。发布文件未进行数字签名；无法运行时可使用首页提供的手动安装包。

## 构建

在完整仓库根目录运行，需本机有 .NET Framework C# 编译器：

```powershell
.\scripts\build-windows-installer.ps1
```

脚本使用 Windows .NET Framework 的 `csc.exe`，将 `desktop/xiaodou/` 中的两个文件和原始形象预览内置到安装器。生成文件写入 `downloads/`。相关机制见 [Microsoft 的编译器资源说明](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-options/resources)。

## 检查

以下入口供维护者验证。测试目录应使用一个全新的临时目录；测试不会调用默认安装目录。

```powershell
$testDirectory = Join-Path $env:TEMP ('xiaodou-test-' + [guid]::NewGuid())
$testProcess = Start-Process .\downloads\Xiaodou-Installer-v1.0.2.exe -ArgumentList @('--self-test', ('"' + $testDirectory + '"')) -WindowStyle Hidden -Wait -PassThru
$testProcess.ExitCode
Get-Content (Join-Path $testDirectory 'result.json')
```

检查覆盖初次安装、重复安装、已有文件夹与文件的保留、中文及空格路径、自定义目录和锁文件清理。界面预览使用程序自身的 `DrawToBitmap` 导出，不执行安装。检查摘要保存在 `validation/native-windows-installer.json`。
