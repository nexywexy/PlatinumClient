namespace PlatinumClient
{
    using MaterialSkin.Controls;
    using PlatinumClient.Properties;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public class LoadMaterialForm : MaterialForm
    {
        private bool spinning;
        private IContainer components;
        private PictureBox spinner;
        private Panel panel1;

        public LoadMaterialForm()
        {
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
            this.panel1 = new Panel();
            this.spinner = new PictureBox();
            this.panel1.SuspendLayout();
            ((ISupportInitialize) this.spinner).BeginInit();
            base.SuspendLayout();
            this.panel1.BackColor = Color.Red;
            this.panel1.Controls.Add(this.spinner);
            this.panel1.Dock = DockStyle.Fill;
            this.panel1.Location = new Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(0x11c, 0x105);
            this.panel1.TabIndex = 1;
            this.panel1.Paint += new PaintEventHandler(this.panel1_Paint);
            this.spinner.BackColor = Color.Transparent;
            this.spinner.Dock = DockStyle.Fill;
            this.spinner.Image = Resources.ajax_loader;
            this.spinner.Location = new Point(0, 0);
            this.spinner.Name = "spinner";
            this.spinner.Size = new Size(0x11c, 0x105);
            this.spinner.SizeMode = PictureBoxSizeMode.CenterImage;
            this.spinner.TabIndex = 0;
            this.spinner.TabStop = false;
            this.spinner.Click += new EventHandler(this.spinner_Click);
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x11c, 0x105);
            base.Controls.Add(this.panel1);
            base.Name = "LoadMaterialForm";
            this.Text = "LoadMaterialForm";
            base.Load += new EventHandler(this.LoadMaterialForm_Load);
            this.panel1.ResumeLayout(false);
            ((ISupportInitialize) this.spinner).EndInit();
            base.ResumeLayout(false);
        }

        public bool IsSpinning() => 
            this.spinning;

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void LoadMaterialForm_Load(object sender, EventArgs e)
        {
            this.panel1.BackColor = Color.FromArgb(50, 100, 100, 100);
            this.spinner.Hide();
            this.panel1.Hide();
            this.panel1.BringToFront();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        public void Process(Action action)
        {
            Task.Factory.StartNew(delegate {
                try
                {
                    this.Invoke(new MethodInvoker(this.<Process>b__3_1));
                    action();
                    this.Invoke(new MethodInvoker(this.<Process>b__3_2));
                }
                catch (Exception)
                {
                }
            });
        }

        public void RecenterSpinner()
        {
            this.spinner.Left = (base.ClientSize.Width - this.spinner.Width) / 2;
            this.spinner.Top = (base.ClientSize.Height - this.spinner.Height) / 2;
        }

        public void SetSpinning(bool state)
        {
            this.RecenterSpinner();
            this.spinning = state;
            if (this.spinning)
            {
                this.spinner.Show();
                this.panel1.Show();
            }
            else
            {
                this.spinner.Hide();
                this.panel1.Hide();
            }
            this.panel1.BringToFront();
        }

        private void spinner_Click(object sender, EventArgs e)
        {
        }
    }
}

