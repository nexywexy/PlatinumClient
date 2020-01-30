namespace PlatinumClient
{
    using MaterialSkin;
    using MaterialSkin.Controls;
    using PlatinumClient.Properties;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class SubscriptionTab : UserControl
    {
        private string shortName;
        private long subID;
        private IContainer components;
        private WebBrowser newsBrowser;
        private MaterialRaisedButton materialRaisedButton1;
        private PictureBox spinner;
        private PictureBox btnOptions;
        private ContextMenuStrip menuStrip;
        private ToolStripMenuItem branchToolStripMenuItem;
        private ToolStripMenuItem mainlineToolStripMenuItem;
        private ToolStripMenuItem earlyAccessToolStripMenuItem;

        public SubscriptionTab(string shortName, long subID)
        {
            this.shortName = shortName;
            this.subID = subID;
            this.Dock = DockStyle.Fill;
            CookieCollection cookies = API.client.CookieContainer.GetCookies(new Uri(API.API_PATH));
            InternetSetCookie(API.API_PATH, "JSESSIONID", cookies["JSESSIONID"].Value);
            this.InitializeComponent();
            try
            {
                this.newsBrowser.Navigate(API.API_PATH + "dashboard/" + subID);
            }
            catch (COMException)
            {
                this.newsBrowser.Hide();
                this.spinner.Hide();
            }
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            this.btnOptions.ContextMenuStrip = this.menuStrip;
            Point p = new Point(0, this.btnOptions.Height);
            p = this.btnOptions.PointToScreen(p);
            this.menuStrip.Show(p);
        }

        public TabPage createTabPage() => 
            new TabPage(this.shortName) { Controls = { this } };

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
            this.components = new Container();
            this.newsBrowser = new WebBrowser();
            this.materialRaisedButton1 = new MaterialRaisedButton();
            this.spinner = new PictureBox();
            this.btnOptions = new PictureBox();
            this.menuStrip = new ContextMenuStrip(this.components);
            this.branchToolStripMenuItem = new ToolStripMenuItem();
            this.mainlineToolStripMenuItem = new ToolStripMenuItem();
            this.earlyAccessToolStripMenuItem = new ToolStripMenuItem();
            ((ISupportInitialize) this.spinner).BeginInit();
            ((ISupportInitialize) this.btnOptions).BeginInit();
            this.menuStrip.SuspendLayout();
            base.SuspendLayout();
            this.newsBrowser.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.newsBrowser.Location = new Point(0, 0);
            this.newsBrowser.MinimumSize = new Size(20, 20);
            this.newsBrowser.Name = "newsBrowser";
            this.newsBrowser.ScriptErrorsSuppressed = true;
            this.newsBrowser.Size = new Size(0x248, 0x16e);
            this.newsBrowser.TabIndex = 3;
            this.newsBrowser.Url = new Uri("", UriKind.Relative);
            this.newsBrowser.Navigated += new WebBrowserNavigatedEventHandler(this.newsBrowser_Navigated);
            this.newsBrowser.Navigating += new WebBrowserNavigatingEventHandler(this.newsBrowser_Navigating);
            this.materialRaisedButton1.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            this.materialRaisedButton1.Depth = 0;
            this.materialRaisedButton1.Location = new Point(0, 0x174);
            this.materialRaisedButton1.MouseState = MouseState.HOVER;
            this.materialRaisedButton1.Name = "materialRaisedButton1";
            this.materialRaisedButton1.Primary = true;
            this.materialRaisedButton1.Size = new Size(0x248, 0x22);
            this.materialRaisedButton1.TabIndex = 4;
            this.materialRaisedButton1.Text = "Launch";
            this.materialRaisedButton1.UseVisualStyleBackColor = true;
            this.materialRaisedButton1.Click += new EventHandler(this.materialRaisedButton1_Click);
            this.spinner.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.spinner.Image = Resources.ajax_loader;
            this.spinner.Location = new Point(0, 0);
            this.spinner.Name = "spinner";
            this.spinner.Size = new Size(0x248, 0x16e);
            this.spinner.SizeMode = PictureBoxSizeMode.CenterImage;
            this.spinner.TabIndex = 5;
            this.spinner.TabStop = false;
            this.btnOptions.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.btnOptions.BackColor = Color.FromArgb(0x21, 0x21, 0x21);
            this.btnOptions.Image = Resources.chevron_arrow_down;
            this.btnOptions.Location = new Point(0x22e, 0x179);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new Size(0x10, 0x1a);
            this.btnOptions.SizeMode = PictureBoxSizeMode.Zoom;
            this.btnOptions.TabIndex = 6;
            this.btnOptions.TabStop = false;
            this.btnOptions.Click += new EventHandler(this.btnOptions_Click);
            ToolStripItem[] toolStripItems = new ToolStripItem[] { this.branchToolStripMenuItem };
            this.menuStrip.Items.AddRange(toolStripItems);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new Size(0x70, 0x1a);
            ToolStripItem[] itemArray2 = new ToolStripItem[] { this.mainlineToolStripMenuItem, this.earlyAccessToolStripMenuItem };
            this.branchToolStripMenuItem.DropDownItems.AddRange(itemArray2);
            this.branchToolStripMenuItem.Name = "branchToolStripMenuItem";
            this.branchToolStripMenuItem.Size = new Size(0x6f, 0x16);
            this.branchToolStripMenuItem.Text = "Branch";
            this.mainlineToolStripMenuItem.Name = "mainlineToolStripMenuItem";
            this.mainlineToolStripMenuItem.Size = new Size(0x98, 0x16);
            this.mainlineToolStripMenuItem.Text = "Mainline";
            this.earlyAccessToolStripMenuItem.Name = "earlyAccessToolStripMenuItem";
            this.earlyAccessToolStripMenuItem.Size = new Size(0x98, 0x16);
            this.earlyAccessToolStripMenuItem.Text = "Early Access";
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            base.Controls.Add(this.btnOptions);
            base.Controls.Add(this.spinner);
            base.Controls.Add(this.materialRaisedButton1);
            base.Controls.Add(this.newsBrowser);
            base.Margin = new Padding(5);
            base.Name = "SubscriptionTab";
            base.Size = new Size(0x248, 0x196);
            base.Load += new EventHandler(this.SubscriptionTab_Load);
            ((ISupportInitialize) this.spinner).EndInit();
            ((ISupportInitialize) this.btnOptions).EndInit();
            this.menuStrip.ResumeLayout(false);
            base.ResumeLayout(false);
        }

        [DllImport("wininet.dll", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern bool InternetSetCookie(string lpszUrlName, string lpszCookieName, string lpszCookieData);
        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            base.ParentForm.Hide();
            new LoadingDialog(this.subID).ShowDialog();
        }

        private void newsBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            this.spinner.Hide();
        }

        private void newsBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            this.spinner.Show();
        }

        private void SubscriptionTab_Load(object sender, EventArgs e)
        {
        }
    }
}

