namespace PlatinumClient
{
    using MaterialSkin;
    using MaterialSkin.Controls;
    using PlatinumClient.Properties;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class MaterialMessageBox : MaterialForm
    {
        private string message;
        private string title;
        private MessageBoxIcon icon;
        private IContainer components;
        private PictureBox pictureBox1;
        private MaterialRaisedButton materialRaisedButton1;
        private MaterialLabel lblPrompt;
        private PictureBox pictureBox2;

        public MaterialMessageBox(string message, string title, MessageBoxIcon icon)
        {
            MaterialSkinManager instance = MaterialSkinManager.Instance;
            instance.Theme = MaterialSkinManager.Themes.LIGHT;
            instance.ColorScheme = new ColorScheme(Primary.Grey900, Primary.Grey800, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
            instance.AddFormToManage(this);
            this.Text = title;
            this.title = title;
            this.message = message;
            this.icon = icon;
            this.InitializeComponent();
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
            this.materialRaisedButton1 = new MaterialRaisedButton();
            this.lblPrompt = new MaterialLabel();
            this.pictureBox2 = new PictureBox();
            this.pictureBox1 = new PictureBox();
            ((ISupportInitialize) this.pictureBox2).BeginInit();
            ((ISupportInitialize) this.pictureBox1).BeginInit();
            base.SuspendLayout();
            this.materialRaisedButton1.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.materialRaisedButton1.Depth = 0;
            this.materialRaisedButton1.Location = new Point(0x123, 0xa5);
            this.materialRaisedButton1.MouseState = MouseState.HOVER;
            this.materialRaisedButton1.Name = "materialRaisedButton1";
            this.materialRaisedButton1.Primary = true;
            this.materialRaisedButton1.Size = new Size(0xb6, 0x27);
            this.materialRaisedButton1.TabIndex = 3;
            this.materialRaisedButton1.Text = "OKAY";
            this.materialRaisedButton1.UseVisualStyleBackColor = true;
            this.materialRaisedButton1.Click += new EventHandler(this.materialRaisedButton1_Click);
            this.lblPrompt.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.lblPrompt.AutoSize = true;
            this.lblPrompt.BackColor = Color.Transparent;
            this.lblPrompt.Depth = 0;
            this.lblPrompt.Font = new Font("Roboto", 11f);
            this.lblPrompt.ForeColor = Color.FromArgb(0xde, 0, 0, 0);
            this.lblPrompt.Location = new Point(0x60, 0x4a);
            this.lblPrompt.MaximumSize = new Size(0x179, 0);
            this.lblPrompt.MouseState = MouseState.HOVER;
            this.lblPrompt.Name = "lblPrompt";
            this.lblPrompt.Padding = new Padding(10);
            this.lblPrompt.Size = new Size(20, 0x27);
            this.lblPrompt.TabIndex = 1;
            this.pictureBox2.BackColor = Color.Transparent;
            this.pictureBox2.Image = Resources.platinumcheats_wide_compressed_white;
            this.pictureBox2.Location = new Point(0x131, 0x1c);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new Size(0xa8, 0x20);
            this.pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 4;
            this.pictureBox2.TabStop = false;
            this.pictureBox1.BackColor = Color.Transparent;
            this.pictureBox1.Image = Resources.shield;
            this.pictureBox1.Location = new Point(12, 0x4a);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Size(0x4e, 80);
            this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x1e5, 0xd8);
            base.Controls.Add(this.pictureBox2);
            base.Controls.Add(this.materialRaisedButton1);
            base.Controls.Add(this.lblPrompt);
            base.Controls.Add(this.pictureBox1);
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "MaterialMessageBox";
            base.Sizable = false;
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Error";
            base.TopMost = true;
            base.Load += new EventHandler(this.MaterialMessageBox_Load);
            ((ISupportInitialize) this.pictureBox2).EndInit();
            ((ISupportInitialize) this.pictureBox1).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void MaterialMessageBox_Load(object sender, EventArgs e)
        {
            this.Text = this.title;
            this.lblPrompt.Text = this.message;
            this.AutoSize = true;
            base.Height = this.lblPrompt.Height + 140;
            if ((this.icon == MessageBoxIcon.Asterisk) || (this.icon == MessageBoxIcon.Asterisk))
            {
                this.pictureBox1.Image = Resources.info_icon;
            }
            else if ((this.icon == MessageBoxIcon.Hand) || (this.icon == MessageBoxIcon.Exclamation))
            {
                this.pictureBox1.Image = Resources.warning_icon;
            }
            else if (this.icon == MessageBoxIcon.None)
            {
                this.pictureBox1.Image = Resources.shield;
            }
        }

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            base.Close();
        }
    }
}

