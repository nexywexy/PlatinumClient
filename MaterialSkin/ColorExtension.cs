namespace MaterialSkin
{
    using System;
    using System.Drawing;
    using System.Runtime.CompilerServices;

    public static class ColorExtension
    {
        public static int PercentageToColorComponent(this int percentage) => 
            ((int) ((((double) percentage) / 100.0) * 255.0));

        public static Color RemoveAlpha(this Color color) => 
            Color.FromArgb(color.R, color.G, color.B);

        public static Color ToColor(this int argb) => 
            Color.FromArgb((argb & 0xff0000) >> 0x10, (argb & 0xff00) >> 8, argb & 0xff);
    }
}

