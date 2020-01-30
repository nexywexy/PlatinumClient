namespace PlatinumClient
{
    using MaterialSkin;
    using MaterialSkin.Controls;
    using PlatinumClient.Properties;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class EULADialog : MaterialForm
    {
        private IContainer components;
        private RichTextBox richTextBox1;
        private MaterialLabel materialLabel1;
        private PictureBox pictureBox1;
        private MaterialRaisedButton btnContinue;
        private MaterialCheckBox cbxAgree;

        public EULADialog()
        {
            this.InitializeComponent();
            MaterialSkinManager instance = MaterialSkinManager.Instance;
            instance.Theme = MaterialSkinManager.Themes.LIGHT;
            instance.ColorScheme = new ColorScheme(Primary.Grey900, Primary.Grey800, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
            instance.AddFormToManage(this);
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            base.Hide();
            new AuthorizeDialog().ShowDialog();
            base.Close();
        }

        private void cbxAgree_CheckedChanged(object sender, EventArgs e)
        {
            this.btnContinue.Enabled = this.cbxAgree.Checked;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void EULADialog_Load(object sender, EventArgs e)
        {
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(EULADialog));
            this.richTextBox1 = new RichTextBox();
            this.materialLabel1 = new MaterialLabel();
            this.pictureBox1 = new PictureBox();
            this.btnContinue = new MaterialRaisedButton();
            this.cbxAgree = new MaterialCheckBox();
            ((ISupportInitialize) this.pictureBox1).BeginInit();
            base.SuspendLayout();
            this.richTextBox1.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.richTextBox1.BackColor = Color.White;
            this.richTextBox1.BorderStyle = BorderStyle.None;
            this.richTextBox1.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.richTextBox1.Location = new Point(12, 0x6d);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new Size(0x281, 0x145);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = manager.GetString("richTextBox1.Text");
            this.materialLabel1.AutoSize = true;
            this.materialLabel1.BackColor = Color.Transparent;
            this.materialLabel1.Depth = 0;
            this.materialLabel1.Font = new Font("Roboto", 11f);
            this.materialLabel1.ForeColor = Color.FromArgb(0xde, 0, 0, 0);
            this.materialLabel1.Location = new Point(12, 0x4e);
            this.materialLabel1.MouseState = MouseState.HOVER;
            this.materialLabel1.Name = "materialLabel1";
            this.materialLabel1.Size = new Size(0x1e5, 0x13);
            this.materialLabel1.TabIndex = 1;
            this.materialLabel1.Text = "You must agree to the Platinum Cheats Service Agreement to continue.";
            this.pictureBox1.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.pictureBox1.BackColor = Color.Transparent;
            this.pictureBox1.Image = Resources.platinumcheats_wide_compressed_white;
            this.pictureBox1.Location = new Point(0x1e5, 0x1b);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Size(0xa8, 0x20);
            this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.btnContinue.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.btnContinue.Depth = 0;
            this.btnContinue.Enabled = false;
            this.btnContinue.Location = new Point(0x1d7, 440);
            this.btnContinue.MouseState = MouseState.HOVER;
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Primary = true;
            this.btnContinue.Size = new Size(0xb6, 0x27);
            this.btnContinue.TabIndex = 4;
            this.btnContinue.Text = "Continue";
            this.btnContinue.UseVisualStyleBackColor = true;
            this.btnContinue.Click += new EventHandler(this.btnContinue_Click);
            this.cbxAgree.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.cbxAgree.AutoSize = true;
            this.cbxAgree.Depth = 0;
            this.cbxAgree.Font = new Font("Roboto", 10f);
            this.cbxAgree.Location = new Point(9, 0x1bd);
            this.cbxAgree.Margin = new Padding(0);
            this.cbxAgree.MouseLocation = new Point(-1, -1);
            this.cbxAgree.MouseState = MouseState.HOVER;
            this.cbxAgree.Name = "cbxAgree";
            this.cbxAgree.Ripple = true;
            this.cbxAgree.Size = new Size(0x1ad, 30);
            this.cbxAgree.TabIndex = 5;
            this.cbxAgree.Text = "I have read and agree to the Platinum Cheats Service Agreement.";
            this.cbxAgree.UseVisualStyleBackColor = true;
            this.cbxAgree.CheckedChanged += new EventHandler(this.cbxAgree_CheckedChanged);
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x299, 0x1eb);
            base.Controls.Add(this.cbxAgree);
            base.Controls.Add(this.btnContinue);
            base.Controls.Add(this.pictureBox1);
            base.Controls.Add(this.materialLabel1);
            base.Controls.Add(this.richTextBox1);
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "EULADialog";
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Service Agreement";
            base.TopMost = true;
            base.Load += new EventHandler(this.EULADialog_Load);
            ((ISupportInitialize) this.pictureBox1).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }
    }
}

