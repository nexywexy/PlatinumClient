namespace MaterialSkin.Controls
{
    using MaterialSkin;
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    public class MaterialLabel : Label, IMaterialControl
    {
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            this.ForeColor = this.SkinManager.GetMainTextColor();
            this.Font = this.SkinManager.ROBOTO_REGULAR_11;
            base.BackColorChanged += (sender, args) => (this.ForeColor = this.SkinManager.GetMainTextColor());
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

