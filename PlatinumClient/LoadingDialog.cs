namespace PlatinumClient
{
    using MaterialSkin;
    using MaterialSkin.Controls;
    using PlatinumClient.Properties;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Management;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class LoadingDialog : MaterialForm
    {
        private long subid;
        private IContainer components;
        private PictureBox promptIcon;
        private MaterialLabel lblProgress;
        private PictureBox pictureBox2;
        private ProgressBar progress;
        private Panel panel;
        private PictureBox icon;
        private Label lblTitle;
        private MaterialLabel lblSubtitle;
        private BackgroundWorker worker;
        private System.Windows.Forms.Timer clock;
        private Label lblHint;
        private MaterialMessage lblPrompt;

        public LoadingDialog(long subid)
        {
            this.subid = subid;
            this.InitializeComponent();
        }

        private void clock_Tick(object sender, EventArgs e)
        {
            switch (GetCurrentStatus())
            {
                case 0:
                    this.lblTitle.Text = "Getting Ready";
                    this.lblSubtitle.Text = "This will only take a few moments";
                    this.lblProgress.Text = "Initializing trusted module...";
                    this.icon.Image = Resources.coffee;
                    return;

                case 1:
                    this.lblTitle.Text = "Getting Ready";
                    this.lblSubtitle.Text = "This will only take a few moments";
                    this.lblProgress.Text = "Initializing trusted module...";
                    this.icon.Image = Resources.coffee;
                    return;

                case 2:
                    this.lblTitle.Text = "Processing";
                    this.lblSubtitle.Text = "This will only take a few moments";
                    this.lblProgress.Text = "Processing subscription...";
                    this.icon.Image = Resources.settings_work_tool;
                    return;

                case 3:
                    this.lblTitle.Text = "Building";
                    this.lblSubtitle.Text = "We're building a custom cheat just for you";
                    this.lblProgress.Text = "In build queue...";
                    this.icon.Image = Resources.handyman_tools;
                    return;

                case 4:
                    this.TMComplete(null, null);
                    break;
            }
        }

        public void Complete(object sender, AsyncCompletedEventArgs e)
        {
            this.lblHint.Visible = false;
            this.lblTitle.Text = "Success";
            this.lblSubtitle.Text = "After closing this window, open Steam and start the game";
            this.icon.Image = Resources.success;
            this.promptIcon.Image = Resources.success;
            this.lblProgress.Text = "Complete";
            this.progress.Value = 100;
            this.progress.Style = ProgressBarStyle.Continuous;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        public void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.progress.Value = e.ProgressPercentage;
        }

        public void DownloadTMComplete(object sender, AsyncCompletedEventArgs e)
        {
            API.client.DownloadFileCompleted -= new AsyncCompletedEventHandler(this.DownloadTMComplete);
            this.InitializeTM();
        }

        public void DownloadVCRedist(object sender, AsyncCompletedEventArgs e)
        {
            API.client.DownloadFileCompleted -= new AsyncCompletedEventHandler(this.DownloadVCRedist);
            API.client.DownloadFileCompleted += new AsyncCompletedEventHandler(this.DownloadVCRedistCompleted);
            this.lblProgress.Text = "Acquiring vcredist.exe...";
            API.client.DownloadFileAsync(new Uri("https://download.microsoft.com/download/2/E/6/2E61CFA4-993B-4DD4-91DA-3737CD5CD6E3/vcredist_x86.exe"), "vcredist.exe");
        }

        public void DownloadVCRedistCompleted(object sender, AsyncCompletedEventArgs e)
        {
            API.client.DownloadFileCompleted -= new AsyncCompletedEventHandler(this.DownloadVCRedistCompleted);
            this.worker.DoWork += new DoWorkEventHandler(this.InstallVCRedist);
            this.worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.InstallVCRedistComplete);
            this.lblProgress.Text = "Waiting for Visual C++ Redistributable install to complete...";
            this.progress.Style = ProgressBarStyle.Marquee;
            this.worker.RunWorkerAsync();
        }

        [DllImport("tm.dll", CharSet=CharSet.Unicode)]
        private static extern int GetCurrentStatus();
        private void InitializeComponent()
        {
            this.components = new Container();
            this.lblProgress = new MaterialLabel();
            this.progress = new ProgressBar();
            this.panel = new Panel();
            this.promptIcon = new PictureBox();
            this.lblTitle = new Label();
            this.lblSubtitle = new MaterialLabel();
            this.worker = new BackgroundWorker();
            this.clock = new System.Windows.Forms.Timer(this.components);
            this.lblHint = new Label();
            this.icon = new PictureBox();
            this.pictureBox2 = new PictureBox();
            this.lblPrompt = new MaterialMessage();
            this.panel.SuspendLayout();
            ((ISupportInitialize) this.promptIcon).BeginInit();
            ((ISupportInitialize) this.icon).BeginInit();
            ((ISupportInitialize) this.pictureBox2).BeginInit();
            base.SuspendLayout();
            this.lblProgress.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.lblProgress.AutoSize = true;
            this.lblProgress.BackColor = Color.Transparent;
            this.lblProgress.Depth = 0;
            this.lblProgress.Font = new Font("Microsoft Sans Serif", 11f);
            this.lblProgress.ForeColor = Color.FromArgb(0xde, 0, 0, 0);
            this.lblProgress.Location = new Point(50, 14);
            this.lblProgress.MouseState = MouseState.HOVER;
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new Size(0xd5, 0x12);
            this.lblProgress.TabIndex = 1;
            this.lblProgress.Text = "Downloading Trusted Module...";
            this.progress.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            this.progress.Location = new Point(0x36, 0x24);
            this.progress.MarqueeAnimationSpeed = 20;
            this.progress.Name = "progress";
            this.progress.Size = new Size(0x22e, 10);
            this.progress.Style = ProgressBarStyle.Continuous;
            this.progress.TabIndex = 3;
            this.panel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            this.panel.Controls.Add(this.promptIcon);
            this.panel.Controls.Add(this.lblProgress);
            this.panel.Controls.Add(this.progress);
            this.panel.Location = new Point(0, 240);
            this.panel.Name = "panel";
            this.panel.Size = new Size(0x274, 0x3e);
            this.panel.TabIndex = 4;
            this.promptIcon.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.promptIcon.BackColor = Color.Transparent;
            this.promptIcon.Image = Resources.ajax_loader;
            this.promptIcon.Location = new Point(12, 14);
            this.promptIcon.Name = "promptIcon";
            this.promptIcon.Size = new Size(0x20, 0x20);
            this.promptIcon.SizeMode = PictureBoxSizeMode.Zoom;
            this.promptIcon.TabIndex = 0;
            this.promptIcon.TabStop = false;
            this.lblTitle.BackColor = Color.Transparent;
            this.lblTitle.Font = new Font("Arial", 14.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.lblTitle.Location = new Point(12, 0xae);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new Size(0x25c, 0x1b);
            this.lblTitle.TabIndex = 6;
            this.lblTitle.Text = "Downloading Prerequisites";
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            this.lblSubtitle.BackColor = Color.Transparent;
            this.lblSubtitle.Depth = 0;
            this.lblSubtitle.Font = new Font("Microsoft Sans Serif", 11f);
            this.lblSubtitle.ForeColor = Color.FromArgb(0xde, 0, 0, 0);
            this.lblSubtitle.Location = new Point(12, 0xc9);
            this.lblSubtitle.MouseState = MouseState.HOVER;
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new Size(600, 0x16);
            this.lblSubtitle.TabIndex = 7;
            this.lblSubtitle.Text = "This will only take a few moments";
            this.lblSubtitle.TextAlign = ContentAlignment.TopCenter;
            this.clock.Tick += new EventHandler(this.clock_Tick);
            this.lblHint.AutoSize = true;
            this.lblHint.BackColor = Color.Transparent;
            this.lblHint.Location = new Point(0xd6, 0xde);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new Size(0xc9, 13);
            this.lblHint.TabIndex = 8;
            this.lblHint.Text = "(not using a USB? just close this window)";
            this.lblHint.TextAlign = ContentAlignment.MiddleCenter;
            this.lblHint.Visible = false;
            this.icon.BackColor = Color.Transparent;
            this.icon.Image = Resources.cloud_dl;
            this.icon.Location = new Point(12, 0x4f);
            this.icon.Name = "icon";
            this.icon.Size = new Size(0x25c, 0x5c);
            this.icon.SizeMode = PictureBoxSizeMode.Zoom;
            this.icon.TabIndex = 5;
            this.icon.TabStop = false;
            this.pictureBox2.BackColor = Color.Transparent;
            this.pictureBox2.Image = Resources.platinumcheats_wide_compressed_white;
            this.pictureBox2.Location = new Point(0x1c0, 0x1b);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new Size(0xa8, 0x20);
            this.pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 2;
            this.pictureBox2.TabStop = false;
            this.lblPrompt.BackColor = Color.MistyRose;
            this.lblPrompt.Depth = 0;
            this.lblPrompt.Error = true;
            this.lblPrompt.Font = new Font("Roboto", 11f);
            this.lblPrompt.ForeColor = Color.FromArgb(0xde, 0, 0, 0);
            this.lblPrompt.Location = new Point(12, 0x4f);
            this.lblPrompt.MouseState = MouseState.HOVER;
            this.lblPrompt.Name = "lblPrompt";
            this.lblPrompt.Padding = new Padding(10);
            this.lblPrompt.Size = new Size(600, 0x27);
            this.lblPrompt.TabIndex = 9;
            this.lblPrompt.Text = "Debugging telemetry is enabled. Don't use on VAC and don't eject USB/drive.";
            this.lblPrompt.TextAlign = ContentAlignment.TopCenter;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x274, 0x12d);
            base.Controls.Add(this.lblPrompt);
            base.Controls.Add(this.lblHint);
            base.Controls.Add(this.lblSubtitle);
            base.Controls.Add(this.lblTitle);
            base.Controls.Add(this.icon);
            base.Controls.Add(this.panel);
            base.Controls.Add(this.pictureBox2);
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "LoadingDialog";
            base.StartPosition = FormStartPosition.CenterScreen;
            base.Load += new EventHandler(this.LoadingDialog_Load);
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            ((ISupportInitialize) this.promptIcon).EndInit();
            ((ISupportInitialize) this.icon).EndInit();
            ((ISupportInitialize) this.pictureBox2).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        [DllImport("tm.dll", CharSet=CharSet.Unicode)]
        private static extern void InitializePTM(string cookie, int subid, string binPath);
        public void InitializeTM()
        {
            this.lblProgress.Text = "Initializing trusted module...";
            this.progress.Style = ProgressBarStyle.Marquee;
            this.lblTitle.Text = "Getting Ready";
            this.icon.Image = Resources.coffee;
            this.clock.Enabled = true;
            this.clock.Start();
            this.worker.DoWork += new DoWorkEventHandler(this.InitializeTMWorker);
            this.worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.TMComplete);
            this.worker.RunWorkerAsync();
        }

        public void InitializeTMWorker(object sender, DoWorkEventArgs e)
        {
            this.worker.DoWork -= new DoWorkEventHandler(this.InitializeTMWorker);
            InitializePTM(API.client.CookieContainer.GetCookies(new Uri(API.API_PATH))["JSESSIONID"].Value, (int) this.subid, Application.StartupPath);
        }

        public void InstallVCRedist(object sender, DoWorkEventArgs e)
        {
            this.worker.DoWork -= new DoWorkEventHandler(this.InstallVCRedist);
            Process.Start("vcredist.exe").WaitForExit();
        }

        public void InstallVCRedistComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            this.worker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(this.InstallVCRedistComplete);
            this.progress.Style = ProgressBarStyle.Continuous;
            System.IO.File.Delete("vcredist.exe");
            this.InitializeTM();
        }

        public bool IsMVCR2013Installed() => 
            System.IO.File.Exists(Environment.SystemDirectory + @"\msvcr120.dll");

        private void LoadingDialog_Load(object sender, EventArgs e)
        {
            this.lblPrompt.Visible = Telemetry.status();
            this.lblProgress.Text = "Acquiring tm.dll...";
            API.client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(this.DownloadProgressChanged);
            if (!this.IsMVCR2013Installed())
            {
                API.client.DownloadFileCompleted += new AsyncCompletedEventHandler(this.DownloadVCRedist);
            }
            else
            {
                API.client.DownloadFileCompleted += new AsyncCompletedEventHandler(this.DownloadTMComplete);
            }
            API.client.DownloadFileAsync(new Uri(API.API_PATH + "downloadTM"), "tm.dll");
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);
        public void TMComplete(object sender, AsyncCompletedEventArgs e)
        {
            this.clock.Stop();
            this.clock.Enabled = false;
            if (Application.StartupPath.StartsWith("C"))
            {
                this.Complete(null, null);
            }
            else
            {
                this.worker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(this.TMComplete);
                this.lblTitle.Text = "Eject USB/Drive";
                this.lblSubtitle.Text = "Please forcefully remove your USB drive now";
                this.icon.Image = Resources.memory_stick;
                this.lblProgress.Text = "Waiting for USB ejection";
                this.lblHint.Visible = true;
                this.worker.DoWork += new DoWorkEventHandler(this.WaitForDriveEjection);
                this.worker.RunWorkerAsync();
            }
        }

        public void USBEventArrived(object sender, EventArrivedEventArgs e)
        {
            string str = e.NewEvent.Properties["DriveName"].Value.ToString();
            if (Application.StartupPath.StartsWith(str))
            {
                this.worker.CancelAsync();
            }
        }

        public void WaitForDriveEjection(object sender, DoWorkEventArgs e)
        {
            this.worker.DoWork -= new DoWorkEventHandler(this.WaitForDriveEjection);
            this.worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.Complete);
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 3");
            ManagementEventWatcher watcher1 = new ManagementEventWatcher();
            watcher1.EventArrived += new EventArrivedEventHandler(this.USBEventArrived);
            watcher1.Query = query;
            watcher1.Start();
            watcher1.WaitForNextEvent();
        }
    }
}

