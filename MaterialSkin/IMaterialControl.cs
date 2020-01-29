namespace MaterialSkin
{
    using System;

    internal interface IMaterialControl
    {
        int Depth { get; set; }

        MaterialSkinManager SkinManager { get; }

        MaterialSkin.MouseState MouseState { get; set; }
    }
}

