namespace MaterialSkin.Controls
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public class MaterialToolStripMenuItem : ToolStripMenuItem
    {
        public MaterialToolStripMenuItem()
        {
            base.AutoSize = false;
            this.Size = new Size(120, 30);
        }

        protected override ToolStripDropDown CreateDefaultDropDown()
        {
            ToolStripDropDown down = base.CreateDefaultDropDown();
            if (base.DesignMode)
            {
                return down;
            }
            MaterialContextMenuStrip strip = new MaterialContextMenuStrip();
            strip.Items.AddRange(down.Items);
            return strip;
        }
    }
}

