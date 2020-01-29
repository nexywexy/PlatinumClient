namespace MaterialSkin
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;

    internal static class DrawHelper
    {
        public static Color BlendColor(Color backgroundColor, Color frontColor) => 
            BlendColor(backgroundColor, frontColor, (double) frontColor.A);

        public static Color BlendColor(Color backgroundColor, Color frontColor, double blend)
        {
            double num = blend / 255.0;
            double num2 = 1.0 - num;
            int red = (int) ((backgroundColor.R * num2) + (frontColor.R * num));
            int green = (int) ((backgroundColor.G * num2) + (frontColor.G * num));
            int blue = (int) ((backgroundColor.B * num2) + (frontColor.B * num));
            return Color.FromArgb(red, green, blue);
        }

        public static GraphicsPath CreateRoundRect(Rectangle rect, float radius) => 
            CreateRoundRect((float) rect.X, (float) rect.Y, (float) rect.Width, (float) rect.Height, radius);

        public static GraphicsPath CreateRoundRect(float x, float y, float width, float height, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(x + radius, y, (x + width) - (radius * 2f), y);
            path.AddArc((x + width) - (radius * 2f), y, radius * 2f, radius * 2f, 270f, 90f);
            path.AddLine((float) (x + width), (float) (y + radius), (float) (x + width), (float) ((y + height) - (radius * 2f)));
            path.AddArc((float) ((x + width) - (radius * 2f)), (float) ((y + height) - (radius * 2f)), (float) (radius * 2f), (float) (radius * 2f), 0f, 90f);
            path.AddLine((float) ((x + width) - (radius * 2f)), (float) (y + height), (float) (x + radius), (float) (y + height));
            path.AddArc(x, (y + height) - (radius * 2f), radius * 2f, radius * 2f, 90f, 90f);
            path.AddLine(x, (y + height) - (radius * 2f), x, y + radius);
            path.AddArc(x, y, radius * 2f, radius * 2f, 180f, 90f);
            path.CloseFigure();
            return path;
        }
    }
}

