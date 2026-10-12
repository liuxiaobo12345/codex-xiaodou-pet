# 小豆 · Codex 虚拟宠物

一个短发、大眼睛、黑色小裙子的 3D 玩偶伙伴，陪你思考、工作和打招呼。包含 **9 组动作 + 16 个视线方向**。

![小豆动作预览](previews/animations.gif)

## Windows：下载后双击安装

**[下载「小豆」Windows 安装器（约 3.8 MB）](https://raw.githubusercontent.com/liuxiaobo12345/codex-xiaodou-pet/v1.0.2/downloads/Xiaodou-Installer-v1.0.2.exe)**

1. 下载并双击 `Xiaodou-Installer-v1.0.2.exe`。
2. 点击 **安装小豆**。
3. 回到桌面应用的 **设置 → Mini 与虚拟宠物 / Pets**，刷新列表并选择「小豆」。没有显示时，重启应用后再查看。

![小豆 Windows 安装器](previews/windows-installer.png)

需要支持本地自定义宠物的 Windows 桌面应用。安装器内置全部素材，安装时无需联网、管理员权限或输入命令；重复安装不会增加相同的小豆，也会保留同名的其他文件。安装完成后由你在应用里选择宠物。

安装器尚未做数字签名。如果 Windows 或组织策略阻止运行，可使用下面的手动安装包，无需关闭安全防护。安装器源代码和构建说明见 [source/installer](source/installer/README.md)。

<details>
<summary>手动安装、macOS 与其他安装方式</summary>

### Windows 手动安装

1. [下载小豆桌面安装包（约 2.7 MB）](https://raw.githubusercontent.com/liuxiaobo12345/codex-xiaodou-pet/v1.0.1/downloads/xiaodou-desktop-v1.0.1.zip)，解压后得到 `xiaodou` 文件夹，里面有 `pet.json` 和 `spritesheet.png`。
2. 打开文件资源管理器，在地址栏输入 `%USERPROFILE%\.codex\pets`。如果 `pets` 文件夹不存在，先创建它。
3. 把整个 `xiaodou` 文件夹复制进去，最终文件位置为：

   ```text
   %USERPROFILE%\.codex\pets\xiaodou\pet.json
   %USERPROFILE%\.codex\pets\xiaodou\spritesheet.png
   ```

4. 打开应用的 **设置 → Mini 与虚拟宠物 / Pets**，点击刷新并选择「小豆」。如果没有显示，重启桌面应用后再查看。

如果已有 `xiaodou` 文件夹，请先给新文件夹换个名字，例如 `xiaodou-2`，保留已有素材。若你设置了 `CODEX_HOME`，请使用该目录下的 `pets` 文件夹。

### macOS

下载并解压同一个安装包，将 `xiaodou` 文件夹放到 `~/.codex/pets/`，然后刷新宠物列表。设置了 `CODEX_HOME` 的用户应放到该目录下的 `pets/`。

### 可选：Windows 脚本安装

下载完整仓库并解压，在仓库目录运行：

```powershell
.\scripts\install-windows.ps1
```

脚本会检查图片尺寸与校验值，重复安装会识别已有相同素材；遇到同名的其他内容会使用新文件夹，不覆盖它。使用 `-WhatIf` 可预览操作，使用 `-CodexHome 'D:\YourCodexHome'` 可指定目录。

如果执行策略阻止脚本，请使用上面的手动复制方式，无需更改系统执行策略。

### 可选：安装链接

[install-link.txt](install-link.txt) 保留官方格式的链接，仅适用于已启用宠物安装弹窗的账号与应用版本。**只打开应用、没有弹窗时，使用上面的本地安装包即可。**

本地安装包使用桌面应用读取的 `displayName`、`description`、`spriteVersionNumber` 和 `spritesheetPath` 字段，文件为 UTF-8 无 BOM。该格式已与 Windows 桌面版 26.1007.21434 的读取逻辑核对。网页宠物上传是另一个入口，见下文。

</details>

## 下载素材

- [完整透明精灵图 · v2](https://raw.githubusercontent.com/liuxiaobo12345/codex-xiaodou-pet/v1.0.0/assets/spritesheet.png)：1536 × 2288，包含 9 组动作和 16 个视线方向。
- [网页上传兼容精灵图 · v1](https://raw.githubusercontent.com/liuxiaobo12345/codex-xiaodou-pet/v1.0.0/assets/spritesheet-v1.png)：1536 × 1872，包含 9 组动作。支持网页宠物上传的账号可在 Settings → Personalization → Pet → Upload pet 使用这张图；网页版本的显示方式不同。
- [观看动画视频](previews/animations.mp4) · [查看视线变化](previews/look-loop.gif)
- 使用 GitHub 页面上的 **Code → Download ZIP** 下载整个素材包。

## 动作与文件

| 动作 | 帧数 |
| --- | ---: |
| 待机 / 呼吸 / 眨眼 | 6 |
| 向右跑 | 8 |
| 向左跑 | 8 |
| 挥手 | 4 |
| 跳跃 | 5 |
| 受挫 | 8 |
| 等待 | 6 |
| 思考 / 工作 | 6 |
| 查看 / 完成 | 6 |

`desktop/xiaodou/` 是桌面宠物文件，`downloads/` 包含 Windows 安装器与手动安装包，`scripts/` 保存安装与构建脚本，`assets/` 是可导入素材，`previews/` 是预览，`source/` 保存原始形象、动作源图、生成提示词与安装器源码，`pet.json` 记录布局与校验值，`validation/` 保存检查摘要。

## 制作与反馈

素材由内置 imagegen 生成，经过透明背景清理、动作与方向检查。角色受豆包 AI 形象启发；这是爱好者制作的分享，与豆包和 OpenAI 均无官方关联。

无需 API Key、付费接口或额外运行服务。欢迎在 Issues 中反馈你的操作系统、应用版本、导入结果和想要的新动作；提交前请隐去个人聊天内容。

参考：[官方宠物使用说明](https://learn.chatgpt.com/docs/pets) · [官方安装链接格式](https://learn.chatgpt.com/docs/reference/commands)。
