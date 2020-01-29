namespace MaterialSkin.Controls
{
    using MaterialSkin;
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    public sealed class MaterialDivider : Control, IMaterialControl
    {
        public MaterialDivider()
        {
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            base.Height = 1;
            this.BackColor = this.SkinManager.GetDividersColor();
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

