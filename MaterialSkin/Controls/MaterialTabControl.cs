namespace MaterialSkin.Controls
{
    using MaterialSkin;
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    public class MaterialTabControl : TabControl, IMaterialControl
    {
        protected override void WndProc(ref Message m)
        {
            if (!((m.Msg != 0x1328) || base.DesignMode))
            {
                m.Result = (IntPtr) 1;
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager =>
            MaterialSkinManager.Instance;

        [Browsable(false)]
        public MaterialSkin.MouseState MouseState { get; set; }
    }
}

