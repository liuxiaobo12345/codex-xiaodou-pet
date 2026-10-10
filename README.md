# 小豆 · Codex 虚拟宠物

一个短发、大眼睛、黑色小裙子的 3D 玩偶伙伴，陪你思考、工作和打招呼。包含 **9 组动作 + 16 个视线方向**。

![小豆动作预览](previews/animations.gif)

## 快速试用

需要支持自定义宠物的桌面应用，并先开启「Mini 与虚拟宠物 / Pets」功能。

**Windows**：打开 PowerShell，粘贴下面这条命令并回车。它只打开应用的宠物安装界面；确认安装后，在宠物列表中选择「小豆」。

```powershell
Start-Process 'codex://pets/install?name=%E5%B0%8F%E8%B1%86&imageUrl=https%3A%2F%2Fraw.githubusercontent.com%2Fliuxiaobo12345%2Fcodex-xiaodou-pet%2Fv1.0.0%2Fassets%2Fspritesheet.png&description=%E7%9F%AD%E5%8F%91%E3%80%81%E5%A4%A7%E7%9C%BC%E7%9D%9B%E3%80%81%E9%BB%91%E8%89%B2%E5%BC%80%E8%A1%AB%E4%B8%8E%E7%81%B0%E8%89%B2%E7%9F%AD%E8%A3%99%E7%9A%84%203D%20%E7%8E%A9%E5%81%B6%E4%BC%99%E4%BC%B4%E3%80%82&spriteVersionNumber=2'
```

**macOS**：在终端运行：

```bash
open 'codex://pets/install?name=%E5%B0%8F%E8%B1%86&imageUrl=https%3A%2F%2Fraw.githubusercontent.com%2Fliuxiaobo12345%2Fcodex-xiaodou-pet%2Fv1.0.0%2Fassets%2Fspritesheet.png&description=%E7%9F%AD%E5%8F%91%E3%80%81%E5%A4%A7%E7%9C%BC%E7%9D%9B%E3%80%81%E9%BB%91%E8%89%B2%E5%BC%80%E8%A1%AB%E4%B8%8E%E7%81%B0%E8%89%B2%E7%9F%AD%E8%A3%99%E7%9A%84%203D%20%E7%8E%A9%E5%81%B6%E4%BC%99%E4%BC%B4%E3%80%82&spriteVersionNumber=2'
```

也可以复制 [install-link.txt](install-link.txt) 中的完整链接到浏览器地址栏，允许浏览器打开桌面应用。安装入口可用性取决于应用版本与工作区设置。

安装链接采用官方的 `codex://pets/install` 格式，并指定 `spriteVersionNumber=2`。图片固定引用 `v1.0.0`，不会随主分支修改。

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

`assets/` 是可导入素材，`previews/` 是预览，`source/` 保存原始形象、动作源图和生成提示词，`pet.json` 记录布局与校验值，`validation/` 保存检查摘要。

## 制作与反馈

素材由内置 imagegen 生成，经过透明背景清理、动作与方向检查。角色受豆包 AI 形象启发；这是爱好者制作的分享，与豆包和 OpenAI 均无官方关联。

目前只有素材与说明，无需 API Key、付费接口或额外运行服务。欢迎在 Issues 中反馈你的操作系统、应用版本、导入结果和想要的新动作；提交前请隐去个人聊天内容。

参考：[官方宠物使用说明](https://learn.chatgpt.com/docs/pets) · [官方安装链接格式](https://learn.chatgpt.com/docs/reference/commands)。
