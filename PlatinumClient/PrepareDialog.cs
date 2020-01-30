namespace PlatinumClient
{
    using MaterialSkin;
    using MaterialSkin.Controls;
    using Microsoft.Win32;
    using PlatinumClient.Properties;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Net;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.ServiceProcess;
    using System.Text;
    using System.Windows.Forms;
    using WUApiLib;

    public class PrepareDialog : MaterialForm
    {
        private string newClientName;
        private IContainer components;
        private PictureBox pictureBox2;
        private Panel panel;
        private PictureBox promptIcon;
        private MaterialLabel lblProgress;
        private ProgressBar progress;
        private BackgroundWorker checkUpdateWorker;

        public PrepareDialog()
        {
            this.InitializeComponent();
            MaterialSkinManager instance = MaterialSkinManager.Instance;
            instance.Theme = MaterialSkinManager.Themes.LIGHT;
            instance.ColorScheme = new ColorScheme(Primary.Grey900, Primary.Grey800, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
            instance.AddFormToManage(this);
        }

        private void checkClientUpdates()
        {
            this.lblProgress.Text = "Making sure client is up-to-date...";
            this.checkUpdateWorker.RunWorkerAsync();
        }

        private void checkConfiguration()
        {
            if (!CheckSteamClosed())
            {
                new MaterialMessageBox("It's not safe to have Steam, the game, or any anti-cheat clients open now.\n\nPlease keep them closed until we tell you it's safe to open them.", "Security Warning", MessageBoxIcon.None).ShowDialog();
            }
            this.lblProgress.Text = "Checking configuration...";
            OperatingSystem oSVersion = Environment.OSVersion;
            if ((oSVersion.Version.Major == 6) && (oSVersion.Version.Minor == 1))
            {
                if (CheckWindowsUpdate("3033929") || CheckWindowsUpdate("3185330"))
                {
                }
            }
            else if (((oSVersion.Version.Major == 10) && (oSVersion.Version.Build >= 0x3839)) && !CheckSecureBoot())
            {
                new MaterialMessageBox("You must disable Secure Boot in your UEFI to use Platinum Cheats.", "Setup Error", MessageBoxIcon.Hand).ShowDialog();
                Process.Start("https://msdn.microsoft.com/en-us/windows/hardware/commercialize/manufacture/desktop/disabling-secure-boot");
                return;
            }
            disableSuperfetch();
            if (Assembly.GetEntryAssembly().Location.StartsWith("C:"))
            {
                new MaterialMessageBox("You are currently running the Platinum Cheats client from the main hard drive C:. We do not recommend running the client in this location.\n\nFor optimal security, we strongly recommand running the client from a FAT32/exFAT USB drive or an ejectable FAT32 partition.\n\nCheck the knowledgebase at https://platinumcheats.net/support for more info.", "Security Warning", MessageBoxIcon.None).ShowDialog();
            }
            base.Close();
        }

        private static bool CheckSecureBoot()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\SecureBoot\State"))
                {
                    if (key != null)
                    {
                        object obj2 = key.GetValue("UEFISecureBootEnabled");
                        if ((obj2 != null) && (((int) obj2) == 1))
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return true;
            }
            return true;
        }

        private static bool CheckSteamClosed()
        {
            bool flag = FindAndTermProcess("csgo");
            bool flag2 = FindAndTermProcess("SteamService");
            return ((!FindAndTermProcess("Steam") && !flag) && !flag2);
        }

        private void checkUpdateWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = API.checkClientHash();
        }

        private void checkUpdateWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!((bool) e.Result))
            {
                string fileName = generateName();
                API.client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(this.downloadClientProgress);
                API.client.DownloadFileCompleted += new AsyncCompletedEventHandler(this.downloadClientFinished);
                API.client.DownloadFileAsync(new Uri(API.API_PATH + "downloadClient"), fileName);
                this.newClientName = fileName;
            }
            else
            {
                this.checkConfiguration();
            }
        }

        private static bool CheckWindowsUpdate(string kb)
        {
            try
            {
                UpdateSession session1 = (UpdateSession) Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("4CB43D7F-7EEE-4906-8698-60DA1C38F2FE")));
                IUpdateSearcher searcher = session1.CreateUpdateSearcher();
                searcher.Online = false;
                IUpdateHistoryEntryCollection entrys = session1.QueryHistory(string.Empty, 0, searcher.GetTotalHistoryCount());
                for (int i = 0; i < entrys.Count; i++)
                {
                    IUpdateHistoryEntry entry = entrys[i];
                    if (((entry != null) && (entry.Title != null)) && entry.Title.Contains(kb))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return true;
            }
        }

        public static void disableSuperfetch()
        {
            ServiceController controller = new ServiceController("SysMain");
            if (controller.Status != ServiceControllerStatus.Stopped)
            {
                controller.Stop();
                controller.WaitForStatus(ServiceControllerStatus.Stopped);
            }
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management\PrefetchParameters", true);
            object obj2 = key.GetValue("EnablePrefetcher", null);
            object obj3 = key.GetValue("EnableSuperfetch", null);
            if (((obj2 == null) || (obj3 == null)) || ((Convert.ToInt32(obj2) != 0) || (Convert.ToInt32(obj3) != 0)))
            {
                try
                {
                    key.SetValue("EnablePrefetcher", 0);
                    key.SetValue("EnableSuperfetch", 0);
                }
                catch (Exception)
                {
                    MessageBox.Show("Prefetch or Superfetch could not be disabled.  Please manually disable prefetch.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void downloadClientFinished(object sender, AsyncCompletedEventArgs e)
        {
            API.client.DownloadProgressChanged -= new DownloadProgressChangedEventHandler(this.downloadClientProgress);
            API.client.DownloadFileCompleted -= new AsyncCompletedEventHandler(this.downloadClientFinished);
            Process.Start(this.newClientName, Assembly.GetEntryAssembly().Location);
            Application.Exit();
            this.checkConfiguration();
        }

        private void downloadClientProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            this.progress.Style = ProgressBarStyle.Continuous;
            this.progress.Value = e.ProgressPercentage;
        }

        private static bool FindAndTermProcess(string name)
        {
            bool flag = false;
            foreach (Process process in Process.GetProcessesByName(name))
            {
                try
                {
                    process.Kill();
                    flag = true;
                }
                catch (Exception)
                {
                }
            }
            return flag;
        }

        public static string generateName()
        {
            Random random = new Random();
            int capacity = random.Next(2, 14);
            string str = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ";
            StringBuilder builder = new StringBuilder(capacity);
            for (int i = 0; i < capacity; i++)
            {
                builder.Append(str[random.Next(str.Length)]);
            }
            return (builder.ToString() + ".exe");
        }

        private void InitializeComponent()
        {
            this.pictureBox2 = new PictureBox();
            this.panel = new Panel();
            this.promptIcon = new PictureBox();
            this.lblProgress = new MaterialLabel();
            this.progress = new ProgressBar();
            this.checkUpdateWorker = new BackgroundWorker();
            ((ISupportInitialize) this.pictureBox2).BeginInit();
            this.panel.SuspendLayout();
            ((ISupportInitialize) this.promptIcon).BeginInit();
            base.SuspendLayout();
            this.pictureBox2.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.Image = Resources.platinumcheats_wide_compressed_white;
            this.pictureBox2.Location = new Point(0xce, 0x1b);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new Size(0xa8, 0x20);
            this.pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 3;
            this.pictureBox2.TabStop = false;
            this.panel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.panel.Controls.Add(this.promptIcon);
            this.panel.Controls.Add(this.lblProgress);
            this.panel.Controls.Add(this.progress);
            this.panel.Location = new Point(1, 0x40);
            this.panel.Name = "panel";
            this.panel.Size = new Size(0x181, 0x3e);
            this.panel.TabIndex = 5;
            this.promptIcon.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.promptIcon.BackColor = System.Drawing.Color.Transparent;
            this.promptIcon.Image = Resources.ajax_loader;
            this.promptIcon.Location = new Point(12, 14);
            this.promptIcon.Name = "promptIcon";
            this.promptIcon.Size = new Size(0x20, 0x20);
            this.promptIcon.SizeMode = PictureBoxSizeMode.Zoom;
            this.promptIcon.TabIndex = 0;
            this.promptIcon.TabStop = false;
            this.lblProgress.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.lblProgress.AutoSize = true;
            this.lblProgress.BackColor = System.Drawing.Color.Transparent;
            this.lblProgress.Depth = 0;
            this.lblProgress.Font = new Font("Roboto", 11f);
            this.lblProgress.ForeColor = System.Drawing.Color.FromArgb(0xde, 0, 0, 0);
            this.lblProgress.Location = new Point(50, 14);
            this.lblProgress.MouseState = MouseState.HOVER;
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new Size(0xb9, 0x13);
            this.lblProgress.TabIndex = 1;
            this.lblProgress.Text = "Preparing your computer...";
            this.progress.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            this.progress.Location = new Point(0x36, 0x24);
            this.progress.MarqueeAnimationSpeed = 20;
            this.progress.Name = "progress";
            this.progress.Size = new Size(0x13b, 10);
            this.progress.Style = ProgressBarStyle.Marquee;
            this.progress.TabIndex = 3;
            this.checkUpdateWorker.DoWork += new DoWorkEventHandler(this.checkUpdateWorker_DoWork);
            this.checkUpdateWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.checkUpdateWorker_RunWorkerCompleted);
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x182, 0x7c);
            base.Controls.Add(this.panel);
            base.Controls.Add(this.pictureBox2);
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "PrepareDialog";
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Please Wait";
            base.TopMost = true;
            base.Load += new EventHandler(this.PrepareDialog_Load);
            ((ISupportInitialize) this.pictureBox2).EndInit();
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            ((ISupportInitialize) this.promptIcon).EndInit();
            base.ResumeLayout(false);
        }

        private void PrepareDialog_Load(object sender, EventArgs e)
        {
            this.checkClientUpdates();
        }
    }
}

