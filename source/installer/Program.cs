using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;

[assembly: AssemblyTitle("Xiaodou Pet Installer")]
[assembly: AssemblyProduct("Xiaodou Pet Installer")]
[assembly: AssemblyVersion("1.0.2.0")]
[assembly: AssemblyFileVersion("1.0.2.0")]

namespace XiaodouInstaller
{
    internal sealed class InstallResult
    {
        public string Status { get; set; }
        public string DirectoryPath { get; set; }
    }

    internal static class PetPackage
    {
        internal const string ExpectedHash = "d1110cab8de8f34a81e7132dd54abcb0b88e895e9891ff64e6b3fb0521e1dfac";
        internal static readonly byte[] ImageBytes = Resource("Xiaodou.Spritesheet");
        internal static readonly byte[] ManifestBytes = Resource("Xiaodou.Manifest");

        internal static byte[] Resource(string name)
        {
            using (Stream source = Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
            {
                if (source == null) throw new InvalidDataException("安装器缺少素材，请重新下载。");
                using (MemoryStream output = new MemoryStream())
                {
                    source.CopyTo(output);
                    return output.ToArray();
                }
            }
        }

        internal static string Hash(byte[] bytes)
        {
            using (SHA256 sha = SHA256.Create())
                return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant();
        }

        internal static void Validate()
        {
            if (Hash(ImageBytes) != ExpectedHash) throw new InvalidDataException("素材校验未通过，请重新下载安装器。");
            using (MemoryStream bytes = new MemoryStream(ImageBytes))
            using (Image image = Image.FromStream(bytes))
                if (image.Width != 1536 || image.Height != 2288) throw new InvalidDataException("宠物素材尺寸不正确。");
            string json = new UTF8Encoding(false, true).GetString(ManifestBytes);
            Dictionary<string, object> manifest = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(json);
            if (!manifest.ContainsKey("displayName") || (string)manifest["displayName"] != "小豆" ||
                !manifest.ContainsKey("spriteVersionNumber") || Convert.ToInt32(manifest["spriteVersionNumber"]) != 2 ||
                !manifest.ContainsKey("spritesheetPath") || (string)manifest["spritesheetPath"] != "spritesheet.png")
                throw new InvalidDataException("宠物信息不完整，请重新下载安装器。");
        }

        internal static string DefaultHome()
        {
            string configured = Environment.GetEnvironmentVariable("CODEX_HOME");
            return String.IsNullOrWhiteSpace(configured)
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".codex")
                : Path.GetFullPath(configured);
        }

        private static bool SamePet(string directory)
        {
            try
            {
                if ((File.GetAttributes(directory) & FileAttributes.ReparsePoint) != 0) return false;
                string image = Path.Combine(directory, "spritesheet.png");
                string manifest = Path.Combine(directory, "pet.json");
                return File.Exists(image) && File.Exists(manifest) && Hash(File.ReadAllBytes(image)) == ExpectedHash &&
                    Hash(File.ReadAllBytes(manifest)) == Hash(ManifestBytes);
            }
            catch (IOException) { return false; }
            catch (UnauthorizedAccessException) { return false; }
        }

        private static void WriteNew(string file, byte[] bytes)
        {
            using (FileStream output = new FileStream(file, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                output.Write(bytes, 0, bytes.Length);
        }

        internal static InstallResult Install(string home)
        {
            Validate();
            string root = Path.Combine(Path.GetFullPath(home), "pets");
            Directory.CreateDirectory(root);
            // DeleteOnClose releases this installer-owned lock even after an interrupted process.
            string lockFile = Path.Combine(root, ".xiaodou-installer.lock");
            using (FileStream installerLock = new FileStream(lockFile, FileMode.CreateNew, FileAccess.Write, FileShare.None, 1, FileOptions.DeleteOnClose))
            {
                int suffix = 1;
                string folder;
                for (;;)
                {
                    folder = Path.Combine(root, suffix == 1 ? "xiaodou" : "xiaodou-" + suffix);
                    if (!Directory.Exists(folder) && !File.Exists(folder)) break;
                    if (Directory.Exists(folder) && SamePet(folder))
                        return new InstallResult { Status = "already-installed", DirectoryPath = folder };
                    suffix++;
                }
                Directory.CreateDirectory(folder);
                if ((File.GetAttributes(folder) & FileAttributes.ReparsePoint) != 0 || Directory.GetFileSystemEntries(folder).Length != 0)
                    throw new IOException("安装目录发生变化，请关闭其他安装器后重试。");
                WriteNew(Path.Combine(folder, "spritesheet.png"), ImageBytes);
                WriteNew(Path.Combine(folder, "pet.json"), ManifestBytes);
                return new InstallResult { Status = "installed", DirectoryPath = folder };
            }
        }

        internal static void SelfTest(string testRoot)
        {
            string root = Path.GetFullPath(testRoot);
            Directory.CreateDirectory(root);
            InstallResult first = Install(Path.Combine(root, "fresh"));
            InstallResult repeat = Install(Path.Combine(root, "fresh"));
            if (first.Status != "installed" || repeat.Status != "already-installed" || first.DirectoryPath != repeat.DirectoryPath)
                throw new Exception("First-install or repeat-install test failed.");
            string collisionHome = Path.Combine(root, "collision");
            string existing = Path.Combine(collisionHome, "pets", "xiaodou");
            Directory.CreateDirectory(existing);
            string sentinel = Path.Combine(existing, "keep.txt");
            WriteNew(sentinel, Encoding.UTF8.GetBytes("preserve existing pet"));
            InstallResult collision = Install(collisionHome);
            if (Path.GetFileName(collision.DirectoryPath) != "xiaodou-2" || File.ReadAllText(sentinel) != "preserve existing pet")
                throw new Exception("Existing-folder preservation test failed.");
            string collisionFileHome = Path.Combine(root, "file-collision");
            Directory.CreateDirectory(Path.Combine(collisionFileHome, "pets"));
            WriteNew(Path.Combine(collisionFileHome, "pets", "xiaodou"), Encoding.UTF8.GetBytes("preserve existing file"));
            InstallResult collisionFile = Install(collisionFileHome);
            if (Path.GetFileName(collisionFile.DirectoryPath) != "xiaodou-2") throw new Exception("Existing-file preservation test failed.");
            string unicodeHome = Path.Combine(root, "含 空格的目录");
            InstallResult unicode = Install(unicodeHome);
            if (!File.Exists(Path.Combine(unicode.DirectoryPath, "pet.json"))) throw new Exception("Unicode-path test failed.");
            if (Directory.GetFiles(Path.Combine(root, "fresh", "pets"), ".xiaodou-installer.lock").Length != 0)
                throw new Exception("Installer lock was not cleaned up.");
            var report = new Dictionary<string, object> {
                { "ok", true }, { "embedded_image_sha256", Hash(ImageBytes) },
                { "first_install", "passed" }, { "repeat_install", "no duplicate" },
                { "existing_directory", "preserved" }, { "existing_file", "preserved" },
                { "unicode_and_space_path", "passed" }, { "custom_home", "passed" },
                { "lock_cleanup", "passed" }
            };
            File.WriteAllText(Path.Combine(root, "result.json"), new JavaScriptSerializer().Serialize(report), new UTF8Encoding(false));
        }
    }

    internal sealed class InstallerWindow : Form
    {
        private readonly Button installButton;
        private readonly Label status;
        private readonly PictureBox portrait;
        private readonly bool previewOnly;

        internal InstallerWindow(bool previewOnly = false)
        {
            this.previewOnly = previewOnly;
            Text = "小豆 · 桌面宠物安装";
            Font = new Font("Microsoft YaHei UI", 10F);
            BackColor = Color.FromArgb(250, 248, 245);
            ClientSize = new Size(610, 370);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScaleDimensions = new SizeF(96F, 96F);
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(26), ColumnCount = 2, RowCount = 1 };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 205));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            Controls.Add(layout);
            portrait = new PictureBox { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom, Padding = new Padding(8, 12, 22, 12) };
            using (var bytes = new MemoryStream(PetPackage.Resource("Xiaodou.Preview")))
            using (Image original = Image.FromStream(bytes)) portrait.Image = new Bitmap(original);
            layout.Controls.Add(portrait, 0, 0);
            var content = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 5, Padding = new Padding(12, 17, 0, 0) };
            content.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            content.RowStyles.Add(new RowStyle(SizeType.Absolute, 85));
            content.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
            content.RowStyles.Add(new RowStyle(SizeType.Absolute, 84));
            content.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.Controls.Add(content, 1, 0);
            content.Controls.Add(new Label { Text = "把小豆带回桌面", Font = new Font(Font.FontFamily, 20F, FontStyle.Bold), Dock = DockStyle.Fill }, 0, 0);
            content.Controls.Add(new Label { Text = "一个会眨眼、挥手和跳跃的小伙伴。\r\n点击下方按钮，安装到你的宠物列表。", Dock = DockStyle.Fill, ForeColor = Color.FromArgb(85, 82, 78) }, 0, 1);
            installButton = new Button { Text = "安装小豆", Width = 178, Height = 42, BackColor = Color.FromArgb(38, 112, 92), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Font = new Font(Font.FontFamily, 11F, FontStyle.Bold), UseVisualStyleBackColor = false };
            installButton.FlatAppearance.BorderSize = 0;
            installButton.Click += InstallClicked;
            content.Controls.Add(installButton, 0, 2);
            status = new Label { Text = "安装后，在「Mini 与虚拟宠物」中\r\n刷新列表并选择「小豆」。", Dock = DockStyle.Fill, ForeColor = Color.FromArgb(85, 82, 78) };
            content.Controls.Add(status, 0, 3);
            content.Controls.Add(new Label { Text = "v1.0.2 · 爱好者制作的桌面伙伴", Dock = DockStyle.Fill, Font = new Font(Font.FontFamily, 8F), ForeColor = Color.FromArgb(135, 131, 125) }, 0, 4);
            AcceptButton = installButton;
        }

        private void InstallClicked(object sender, EventArgs e)
        {
            installButton.Enabled = false;
            installButton.Text = "正在安装…";
            try
            {
                InstallResult result = PetPackage.Install(PetPackage.DefaultHome());
                status.Text = result.Status == "already-installed"
                    ? "你已安装小豆。\r\n回到应用刷新列表，选择它即可。"
                    : "小豆安装好了！\r\n回到应用刷新宠物列表，选择「小豆」。\r\n没有出现时，重启应用后再查看。";
                status.ForeColor = Color.FromArgb(38, 112, 92);
                installButton.Text = "完成，关闭";
                installButton.Click -= InstallClicked;
                installButton.Click += delegate { Close(); };
            }
            catch (Exception error)
            {
                status.Text = "安装没有完成，请重试，或使用仓库中的手动安装包。";
                status.ForeColor = Color.DarkRed;
                installButton.Text = "重试安装";
                MessageBox.Show(this, error.Message, "安装遇到问题", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally { installButton.Enabled = true; }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && portrait != null && portrait.Image != null) portrait.Image.Dispose();
            base.Dispose(disposing);
        }

        protected override bool ShowWithoutActivation { get { return previewOnly; } }
    }

    internal static class Program
    {
        [STAThread]
        private static int Main(string[] args)
        {
            try
            {
                if (args.Length == 2 && args[0] == "--self-test")
                {
                    PetPackage.SelfTest(args[1]);
                    return 0;
                }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                if (args.Length == 2 && args[0] == "--render-preview")
                {
                    using (var window = new InstallerWindow(true))
                    {
                        window.ShowInTaskbar = false;
                        window.Opacity = 0;
                        window.Show();
                        window.PerformLayout();
                        window.Update();
                        Application.DoEvents();
                        using (var image = new Bitmap(window.Width, window.Height))
                        {
                            window.DrawToBitmap(image, new Rectangle(0, 0, image.Width, image.Height));
                            image.Save(args[1], ImageFormat.Png);
                        }
                    }
                    return 0;
                }
                if (args.Length != 0) return 2;
                Application.Run(new InstallerWindow());
                return 0;
            }
            catch (Exception error)
            {
                if (args.Length == 0) MessageBox.Show(error.Message, "小豆安装器", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else if (args.Length == 2 && args[0] == "--self-test")
                {
                    Directory.CreateDirectory(args[1]);
                    File.WriteAllText(Path.Combine(args[1], "error.txt"), error.ToString(), new UTF8Encoding(false));
                }
                return 1;
            }
        }
    }
}
