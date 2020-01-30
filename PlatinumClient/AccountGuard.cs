namespace PlatinumClient
{
    using MaterialSkin;
    using MaterialSkin.Controls;
    using PlatinumClient.Properties;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class AccountGuard : MaterialForm
    {
        private IContainer components;
        private PictureBox pictureBox1;
        private MaterialLabel materialLabel1;
        private Label label1;
        private LinkLabel linkLabel1;
        private PictureBox pictureBox2;
        private MaterialRaisedButton materialRaisedButton1;
        private MaterialSingleLineTextField txtCode;

        public AccountGuard()
        {
            this.InitializeComponent();
            MaterialSkinManager instance = MaterialSkinManager.Instance;
            instance.Theme = MaterialSkinManager.Themes.LIGHT;
            instance.ColorScheme = new ColorScheme(Primary.Grey900, Primary.Grey800, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
            instance.AddFormToManage(this);
        }

        private void AccountGuard_Load(object sender, EventArgs e)
        {
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
            this.materialLabel1 = new MaterialLabel();
            this.label1 = new Label();
            this.linkLabel1 = new LinkLabel();
            this.materialRaisedButton1 = new MaterialRaisedButton();
            this.txtCode = new MaterialSingleLineTextField();
            this.pictureBox2 = new PictureBox();
            this.pictureBox1 = new PictureBox();
            ((ISupportInitialize) this.pictureBox2).BeginInit();
            ((ISupportInitialize) this.pictureBox1).BeginInit();
            base.SuspendLayout();
            this.materialLabel1.BackColor = SystemColors.ControlLight;
            this.materialLabel1.Depth = 0;
            this.materialLabel1.Font = new Font("Roboto", 11f);
            this.materialLabel1.ForeColor = Color.FromArgb(0xde, 0, 0, 0);
            this.materialLabel1.Location = new Point(12, 0x49);
            this.materialLabel1.MouseState = MouseState.HOVER;
            this.materialLabel1.Name = "materialLabel1";
            this.materialLabel1.Padding = new Padding(10);
            this.materialLabel1.Size = new Size(310, 0x4d);
            this.materialLabel1.TabIndex = 2;
            this.materialLabel1.Text = "To help protect your Platinum Cheats account, we've sent a code to your email. Please enter the code below to continue. ";
            this.label1.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.label1.AutoSize = true;
            this.label1.Location = new Point(9, 0xe8);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x3e, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Need help?";
            this.linkLabel1.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new Point(70, 0xe8);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new Size(130, 13);
            this.linkLabel1.TabIndex = 4;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Contact customer service.";
            this.materialRaisedButton1.Depth = 0;
            this.materialRaisedButton1.Location = new Point(12, 0xc3);
            this.materialRaisedButton1.MouseState = MouseState.HOVER;
            this.materialRaisedButton1.Name = "materialRaisedButton1";
            this.materialRaisedButton1.Primary = true;
            this.materialRaisedButton1.Size = new Size(440, 0x20);
            this.materialRaisedButton1.TabIndex = 0x10;
            this.materialRaisedButton1.Text = "SUBMIT";
            this.materialRaisedButton1.UseVisualStyleBackColor = true;
            this.materialRaisedButton1.Click += new EventHandler(this.materialRaisedButton1_Click);
            this.txtCode.Depth = 0;
            this.txtCode.Hint = "######";
            this.txtCode.Location = new Point(12, 0xa6);
            this.txtCode.MouseState = MouseState.HOVER;
            this.txtCode.Name = "txtCode";
            this.txtCode.PasswordChar = '\0';
            this.txtCode.SelectedText = "";
            this.txtCode.SelectionLength = 0;
            this.txtCode.SelectionStart = 0;
            this.txtCode.Size = new Size(310, 0x17);
            this.txtCode.TabIndex = 0x11;
            this.txtCode.TabStop = false;
            this.txtCode.UseSystemPasswordChar = false;
            this.pictureBox2.BackColor = Color.Transparent;
            this.pictureBox2.Image = Resources.cloud_security;
            this.pictureBox2.Location = new Point(0x148, 0x49);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new Size(0x7c, 0x74);
            this.pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 15;
            this.pictureBox2.TabStop = false;
            this.pictureBox1.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.pictureBox1.BackColor = Color.Transparent;
            this.pictureBox1.Image = Resources.platinumcheats_wide_compressed_white;
            this.pictureBox1.Location = new Point(0x11c, 0x1b);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Size(0xa8, 0x20);
            this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x1d0, 0xfe);
            base.Controls.Add(this.txtCode);
            base.Controls.Add(this.materialRaisedButton1);
            base.Controls.Add(this.pictureBox2);
            base.Controls.Add(this.linkLabel1);
            base.Controls.Add(this.label1);
            base.Controls.Add(this.materialLabel1);
            base.Controls.Add(this.pictureBox1);
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "AccountGuard";
            base.Sizable = false;
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Account Guard";
            base.TopMost = true;
            base.Load += new EventHandler(this.AccountGuard_Load);
            ((ISupportInitialize) this.pictureBox2).EndInit();
            ((ISupportInitialize) this.pictureBox1).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            if (API.submitGuardCode(this.txtCode.Text))
            {
                base.Close();
            }
        }
    }
}

