namespace PlatinumClient
{
    using MaterialSkin.Controls;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    internal class MaterialMessage : MaterialLabel
    {
        private bool error;

        public MaterialMessage()
        {
            base.Padding = new Padding(10, 10, 10, 10);
            if (!this.error)
            {
                this.BackColor = SystemColors.ControlLight;
            }
            else
            {
                this.BackColor = Color.MistyRose;
            }
        }

        [Category("Appearance"), Description("Error state of the message")]
        public bool Error
        {
            get => 
                this.error;
            set
            {
                this.error = value;
                if (!this.error)
                {
                    this.BackColor = SystemColors.ControlLight;
                }
                else
                {
                    this.BackColor = Color.MistyRose;
                }
                base.Invalidate();
            }
        }
    }
}

