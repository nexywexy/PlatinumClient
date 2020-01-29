namespace MaterialSkin.Controls
{
    using MaterialSkin;
    using MaterialSkin.Animations;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    public class MaterialRaisedButton : Button, IMaterialControl
    {
        private readonly AnimationManager animationManager;

        public MaterialRaisedButton()
        {
            AnimationManager.AnimationProgress progress = null;
            this.Primary = true;
            AnimationManager manager = new AnimationManager(false) {
                Increment = 0.03,
                AnimationType = AnimationType.EaseOut
            };
            this.animationManager = manager;
            if (progress == null)
            {
                progress = sender => base.Invalidate();
            }
            this.animationManager.OnAnimationProgress += progress;
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            this.animationManager.StartNewAnimation(AnimationDirection.In, mevent.Location, null);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics graphics = pevent.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            graphics.Clear(base.Parent.BackColor);
            using (GraphicsPath path = DrawHelper.CreateRoundRect((float) base.ClientRectangle.X, (float) base.ClientRectangle.Y, (float) (base.ClientRectangle.Width - 1), (float) (base.ClientRectangle.Height - 1), 1f))
            {
                graphics.FillPath(this.Primary ? this.SkinManager.ColorScheme.PrimaryBrush : this.SkinManager.GetRaisedButtonBackgroundBrush(), path);
            }
            if (this.animationManager.IsAnimating())
            {
                for (int i = 0; i < this.animationManager.GetAnimationCount(); i++)
                {
                    double progress = this.animationManager.GetProgress(i);
                    Point source = this.animationManager.GetSource(i);
                    SolidBrush brush = new SolidBrush(Color.FromArgb((int) (51.0 - (progress * 50.0)), Color.White));
                    int width = (int) ((progress * base.Width) * 2.0);
                    graphics.FillEllipse(brush, new Rectangle(source.X - (width / 2), source.Y - (width / 2), width, width));
                }
            }
            StringFormat format = new StringFormat {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            graphics.DrawString(this.Text.ToUpper(), this.SkinManager.ROBOTO_MEDIUM_10, this.SkinManager.GetRaisedButtonTextBrush(this.Primary), base.ClientRectangle, format);
        }

        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager =>
            MaterialSkinManager.Instance;

        [Browsable(false)]
        public MaterialSkin.MouseState MouseState { get; set; }

        public bool Primary { get; set; }
    }
}

