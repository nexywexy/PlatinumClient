namespace MaterialSkin
{
    using System;
    using System.Drawing;

    public class ColorScheme
    {
        public readonly Color PrimaryColor;
        public readonly Color DarkPrimaryColor;
        public readonly Color LightPrimaryColor;
        public readonly Color AccentColor;
        public readonly Color TextColor;
        public readonly Pen PrimaryPen;
        public readonly Pen DarkPrimaryPen;
        public readonly Pen LightPrimaryPen;
        public readonly Pen AccentPen;
        public readonly Pen TextPen;
        public readonly Brush PrimaryBrush;
        public readonly Brush DarkPrimaryBrush;
        public readonly Brush LightPrimaryBrush;
        public readonly Brush AccentBrush;
        public readonly Brush TextBrush;

        public ColorScheme(Primary primary, Primary darkPrimary, Primary lightPrimary, Accent accent, TextShade textShade)
        {
            this.PrimaryColor = ((int) primary).ToColor();
            this.DarkPrimaryColor = ((int) darkPrimary).ToColor();
            this.LightPrimaryColor = ((int) lightPrimary).ToColor();
            this.AccentColor = ((int) accent).ToColor();
            this.TextColor = ((int) textShade).ToColor();
            this.PrimaryPen = new Pen(this.PrimaryColor);
            this.DarkPrimaryPen = new Pen(this.DarkPrimaryColor);
            this.LightPrimaryPen = new Pen(this.LightPrimaryColor);
            this.AccentPen = new Pen(this.AccentColor);
            this.TextPen = new Pen(this.TextColor);
            this.PrimaryBrush = new SolidBrush(this.PrimaryColor);
            this.DarkPrimaryBrush = new SolidBrush(this.DarkPrimaryColor);
            this.LightPrimaryBrush = new SolidBrush(this.LightPrimaryColor);
            this.AccentBrush = new SolidBrush(this.AccentColor);
            this.TextBrush = new SolidBrush(this.TextColor);
        }
    }
}

