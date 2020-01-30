namespace PlatinumClient
{
    using MaterialSkin;
    using MaterialSkin.Controls;
    using PlatinumClient.Properties;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class SettingsDialog : LoadMaterialForm
    {
        private IContainer components;
        private PictureBox pictureBox2;
        private Panel panel2;
        private Label lblTelemetry;
        private MaterialCheckBox cbxTelemetry;
        private MaterialRaisedButton btnApply;

        public SettingsDialog()
        {
            this.InitializeComponent();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            Telemetry.prepare(this.cbxTelemetry.Checked);
            base.Close();
        }

        private void cbxTelemetry_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbxTelemetry.Checked)
            {
                new MaterialMessageBox("Telemetry should only be enabled if instructed by Customer Service. It is not safe to have it enabled for normal use.\n\nMake sure to add \"-nominidumps\" to the game's launch options in Steam and do not eject your USB/drive at the end.\n\nDisable this setting for use on anticheat-protected servers.", "Telemetry Information", MessageBoxIcon.None).ShowDialog();
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

        private void InitializeComponent()
        {
            this.pictureBox2 = new PictureBox();
            this.panel2 = new Panel();
            this.lblTelemetry = new Label();
            this.cbxTelemetry = new MaterialCheckBox();
            this.btnApply = new MaterialRaisedButton();
            ((ISupportInitialize) this.pictureBox2).BeginInit();
            this.panel2.SuspendLayout();
            base.SuspendLayout();
            this.pictureBox2.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.pictureBox2.BackColor = Color.Transparent;
            this.pictureBox2.Image = Resources.platinumcheats_wide_compressed_white;
            this.pictureBox2.Location = new Point(0x15a, 0x1c);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new Size(0xa8, 0x20);
            this.pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 6;
            this.pictureBox2.TabStop = false;
            this.panel2.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.panel2.Controls.Add(this.lblTelemetry);
            this.panel2.Controls.Add(this.cbxTelemetry);
            this.panel2.Controls.Add(this.btnApply);
            this.panel2.Location = new Point(-1, 0x40);
            this.panel2.Name = "panel2";
            this.panel2.Size = new Size(0x20f, 0x85);
            this.panel2.TabIndex = 0x11;
            this.lblTelemetry.BackColor = SystemColors.Control;
            this.lblTelemetry.Font = new Font("Roboto Lt", 9.75f);
            this.lblTelemetry.Location = new Point(0x21, 0x26);
            this.lblTelemetry.Name = "lblTelemetry";
            this.lblTelemetry.Size = new Size(0x1dc, 30);
            this.lblTelemetry.TabIndex = 0x13;
            this.lblTelemetry.Text = "Crash and error reports will be automatically uploaded to Platinum Cheats. Use only if instructed to by Customer Service.";
            this.cbxTelemetry.AutoSize = true;
            this.cbxTelemetry.BackColor = Color.White;
            this.cbxTelemetry.Depth = 0;
            this.cbxTelemetry.Font = new Font("Roboto", 10f);
            this.cbxTelemetry.Location = new Point(6, 10);
            this.cbxTelemetry.Margin = new Padding(0);
            this.cbxTelemetry.MouseLocation = new Point(-1, -1);
            this.cbxTelemetry.MouseState = MouseState.HOVER;
            this.cbxTelemetry.Name = "cbxTelemetry";
            this.cbxTelemetry.Ripple = true;
            this.cbxTelemetry.Size = new Size(0xc9, 30);
            this.cbxTelemetry.TabIndex = 0x12;
            this.cbxTelemetry.Text = "Enable debugging telemetry";
            this.cbxTelemetry.UseVisualStyleBackColor = false;
            this.cbxTelemetry.CheckedChanged += new EventHandler(this.cbxTelemetry_CheckedChanged);
            this.btnApply.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            this.btnApply.Depth = 0;
            this.btnApply.Location = new Point(13, 0x57);
            this.btnApply.MouseState = MouseState.HOVER;
            this.btnApply.Name = "btnApply";
            this.btnApply.Primary = true;
            this.btnApply.Size = new Size(0x1f6, 0x20);
            this.btnApply.TabIndex = 0x11;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new EventHandler(this.btnApply_Click);
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x20e, 0xc3);
            base.Controls.Add(this.panel2);
            base.Controls.Add(this.pictureBox2);
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "SettingsDialog";
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Settings";
            base.Load += new EventHandler(this.SettingsDialog_Load);
            base.Controls.SetChildIndex(this.pictureBox2, 0);
            base.Controls.SetChildIndex(this.panel2, 0);
            ((ISupportInitialize) this.pictureBox2).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            base.ResumeLayout(false);
        }

        private void SettingsDialog_Load(object sender, EventArgs e)
        {
            this.cbxTelemetry.Checked = Telemetry.status();
        }
    }
}

