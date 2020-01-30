namespace PlatinumClient
{
    using MaterialSkin;
    using MaterialSkin.Controls;
    using PlatinumClient.Properties;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class TelemetryDialog : MaterialForm
    {
        private IContainer components;
        private PictureBox pictureBox2;
        private Panel panel;
        private PictureBox promptIcon;
        private MaterialLabel lblProgress;
        private ProgressBar progress;
        private BackgroundWorker collectWorker;

        public TelemetryDialog()
        {
            this.InitializeComponent();
        }

        private void collectWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<TelemetryReport> list = Telemetry.collect();
            int count = list.Count;
            for (int i = 1; i <= count; i++)
            {
                this.collectWorker.ReportProgress(i);
                API.submitTelemetryReport(list[i - 1], this);
            }
        }

        private void collectWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.lblProgress.Text = "Uploading report #" + e.ProgressPercentage;
        }

        private void collectWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            base.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pictureBox2 = new PictureBox();
            this.panel = new Panel();
            this.promptIcon = new PictureBox();
            this.lblProgress = new MaterialLabel();
            this.progress = new ProgressBar();
            this.collectWorker = new BackgroundWorker();
            ((ISupportInitialize) this.pictureBox2).BeginInit();
            this.panel.SuspendLayout();
            ((ISupportInitialize) this.promptIcon).BeginInit();
            base.SuspendLayout();
            this.pictureBox2.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.pictureBox2.BackColor = Color.Transparent;
            this.pictureBox2.Image = Resources.platinumcheats_wide_compressed_white;
            this.pictureBox2.Location = new Point(0xde, 0x1c);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new Size(0xa8, 0x20);
            this.pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 4;
            this.pictureBox2.TabStop = false;
            this.panel.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.panel.Controls.Add(this.promptIcon);
            this.panel.Controls.Add(this.lblProgress);
            this.panel.Controls.Add(this.progress);
            this.panel.Location = new Point(-1, 0x40);
            this.panel.Name = "panel";
            this.panel.Size = new Size(0x193, 0x3b);
            this.panel.TabIndex = 6;
            this.promptIcon.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.promptIcon.BackColor = Color.Transparent;
            this.promptIcon.Image = Resources.ajax_loader;
            this.promptIcon.Location = new Point(12, 11);
            this.promptIcon.Name = "promptIcon";
            this.promptIcon.Size = new Size(0x20, 0x20);
            this.promptIcon.SizeMode = PictureBoxSizeMode.Zoom;
            this.promptIcon.TabIndex = 0;
            this.promptIcon.TabStop = false;
            this.lblProgress.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.lblProgress.AutoSize = true;
            this.lblProgress.BackColor = Color.Transparent;
            this.lblProgress.Depth = 0;
            this.lblProgress.Font = new Font("Roboto", 11f);
            this.lblProgress.ForeColor = Color.FromArgb(0xde, 0, 0, 0);
            this.lblProgress.Location = new Point(50, 11);
            this.lblProgress.MouseState = MouseState.HOVER;
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new Size(0xc1, 0x13);
            this.lblProgress.TabIndex = 1;
            this.lblProgress.Text = "Preparing incident reports...";
            this.progress.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            this.progress.Location = new Point(0x36, 0x21);
            this.progress.MarqueeAnimationSpeed = 20;
            this.progress.Name = "progress";
            this.progress.Size = new Size(0x14d, 10);
            this.progress.Style = ProgressBarStyle.Marquee;
            this.progress.TabIndex = 3;
            this.collectWorker.WorkerReportsProgress = true;
            this.collectWorker.DoWork += new DoWorkEventHandler(this.collectWorker_DoWork);
            this.collectWorker.ProgressChanged += new ProgressChangedEventHandler(this.collectWorker_ProgressChanged);
            this.collectWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.collectWorker_RunWorkerCompleted);
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x192, 0x79);
            base.Controls.Add(this.panel);
            base.Controls.Add(this.pictureBox2);
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "TelemetryDialog";
            base.Sizable = false;
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Incident Report";
            base.TopMost = true;
            base.FormClosing += new FormClosingEventHandler(this.TelemetryDialog_FormClosing);
            base.Load += new EventHandler(this.TelemetryDialog_Load);
            ((ISupportInitialize) this.pictureBox2).EndInit();
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            ((ISupportInitialize) this.promptIcon).EndInit();
            base.ResumeLayout(false);
        }

        private void TelemetryDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void TelemetryDialog_Load(object sender, EventArgs e)
        {
            this.collectWorker.RunWorkerAsync();
        }
    }
}

