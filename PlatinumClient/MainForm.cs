namespace PlatinumClient
{
    using MaterialSkin;
    using MaterialSkin.Controls;
    using PlatinumClient.Properties;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class MainForm : LoadMaterialForm
    {
        private IContainer components;
        private PictureBox pictureBox2;
        private MaterialTabControl subTabs;
        private MaterialTabSelector subTabSelector;
        private TabPage dummyTab;
        private BackgroundWorker subWorker;
        private MaterialRaisedButton btnSettings;

        public MainForm()
        {
            this.InitializeComponent();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            new SettingsDialog().ShowDialog();
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
            this.subTabs = new MaterialTabControl();
            this.dummyTab = new TabPage();
            this.subTabSelector = new MaterialTabSelector();
            this.subWorker = new BackgroundWorker();
            this.btnSettings = new MaterialRaisedButton();
            ((ISupportInitialize) this.pictureBox2).BeginInit();
            this.subTabs.SuspendLayout();
            base.SuspendLayout();
            this.pictureBox2.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.pictureBox2.BackColor = Color.Transparent;
            this.pictureBox2.Image = Resources.platinumcheats_wide_compressed_white;
            this.pictureBox2.Location = new Point(660, 0x1c);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new Size(0xa8, 0x20);
            this.pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 5;
            this.pictureBox2.TabStop = false;
            this.subTabs.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.subTabs.Controls.Add(this.dummyTab);
            this.subTabs.Depth = 0;
            this.subTabs.Location = new Point(12, 0x7d);
            this.subTabs.MouseState = MouseState.HOVER;
            this.subTabs.Name = "subTabs";
            this.subTabs.SelectedIndex = 0;
            this.subTabs.Size = new Size(0x330, 0x1b7);
            this.subTabs.TabIndex = 6;
            this.dummyTab.Location = new Point(4, 0x16);
            this.dummyTab.Name = "dummyTab";
            this.dummyTab.Size = new Size(0x328, 0x19d);
            this.dummyTab.TabIndex = 0;
            this.dummyTab.Text = "Null";
            this.dummyTab.UseVisualStyleBackColor = true;
            this.subTabSelector.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.subTabSelector.BaseTabControl = this.subTabs;
            this.subTabSelector.Depth = 0;
            this.subTabSelector.Location = new Point(-1, 0x3f);
            this.subTabSelector.MouseState = MouseState.HOVER;
            this.subTabSelector.Name = "subTabSelector";
            this.subTabSelector.Size = new Size(0x34b, 0x2e);
            this.subTabSelector.TabIndex = 7;
            this.subTabSelector.Text = "materialTabSelector1";
            this.subWorker.DoWork += new DoWorkEventHandler(this.subWorker_DoWork);
            this.subWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.subWorker_RunWorkerCompleted);
            this.btnSettings.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.btnSettings.Depth = 0;
            this.btnSettings.Location = new Point(0x2db, 0x44);
            this.btnSettings.MouseState = MouseState.HOVER;
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Primary = true;
            this.btnSettings.Size = new Size(0x61, 0x20);
            this.btnSettings.TabIndex = 0x11;
            this.btnSettings.Text = "Settings";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new EventHandler(this.btnSettings_Click);
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(840, 0x240);
            base.Controls.Add(this.btnSettings);
            base.Controls.Add(this.subTabSelector);
            base.Controls.Add(this.subTabs);
            base.Controls.Add(this.pictureBox2);
            base.Name = "MainForm";
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Select a Subscription";
            base.Load += new EventHandler(this.MainForm_Load);
            base.Controls.SetChildIndex(this.pictureBox2, 0);
            base.Controls.SetChildIndex(this.subTabs, 0);
            base.Controls.SetChildIndex(this.subTabSelector, 0);
            base.Controls.SetChildIndex(this.btnSettings, 0);
            ((ISupportInitialize) this.pictureBox2).EndInit();
            this.subTabs.ResumeLayout(false);
            base.ResumeLayout(false);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            MaterialSkinManager.Instance.AddFormToManage(this);
            base.SetSpinning(true);
            if (Telemetry.status())
            {
                Telemetry.prepare(true);
                new TelemetryDialog().ShowDialog();
            }
            this.subWorker.RunWorkerAsync();
        }

        private void materialTabSelector1_Click(object sender, EventArgs e)
        {
        }

        private void subWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            API.submitPlatformIDs();
            e.Result = API.getSubs();
        }

        private void subWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                this.subTabs.TabPages.Clear();
                foreach (Subscription subscription in ((Subs) e.Result).subs)
                {
                    this.subTabs.TabPages.Add(new SubscriptionTab(subscription.productShortName, (long) subscription.id).createTabPage());
                }
                base.SetSpinning(false);
            }
        }
    }
}

