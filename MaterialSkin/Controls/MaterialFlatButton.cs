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

    public class MaterialFlatButton : Button, IMaterialControl
    {
        private readonly AnimationManager animationManager;
        private readonly AnimationManager hoverAnimationManager;
        private SizeF textSize;

        public MaterialFlatButton()
        {
            AnimationManager.AnimationProgress progress = null;
            AnimationManager.AnimationProgress progress2 = null;
            this.Primary = false;
            AnimationManager manager = new AnimationManager(false) {
                Increment = 0.03,
                AnimationType = AnimationType.EaseOut
            };
            this.animationManager = manager;
            AnimationManager manager2 = new AnimationManager(true) {
                Increment = 0.07,
                AnimationType = AnimationType.Linear
            };
            this.hoverAnimationManager = manager2;
            if (progress == null)
            {
                progress = sender => base.Invalidate();
            }
            this.hoverAnimationManager.OnAnimationProgress += progress;
            if (progress2 == null)
            {
                progress2 = sender => base.Invalidate();
            }
            this.animationManager.OnAnimationProgress += progress2;
            base.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.AutoSize = true;
            base.Margin = new Padding(4, 6, 4, 6);
            base.Padding = new Padding(0);
        }

        private Size GetPreferredSize() => 
            this.GetPreferredSize(new Size(0, 0));

        public override Size GetPreferredSize(Size proposedSize) => 
            new Size(((int) this.textSize.Width) + 8, 0x24);

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (!base.DesignMode)
            {
                this.MouseState = MaterialSkin.MouseState.OUT;
                base.MouseEnter += delegate (object sender, EventArgs args) {
                    this.MouseState = MaterialSkin.MouseState.HOVER;
                    this.hoverAnimationManager.StartNewAnimation(AnimationDirection.In, null);
                    base.Invalidate();
                };
                base.MouseLeave += delegate (object sender, EventArgs args) {
                    this.MouseState = MaterialSkin.MouseState.OUT;
                    this.hoverAnimationManager.StartNewAnimation(AnimationDirection.Out, null);
                    base.Invalidate();
                };
                base.MouseDown += delegate (object sender, MouseEventArgs args) {
                    if (args.Button == MouseButtons.Left)
                    {
                        this.MouseState = MaterialSkin.MouseState.DOWN;
                        this.animationManager.StartNewAnimation(AnimationDirection.In, args.Location, null);
                        base.Invalidate();
                    }
                };
                base.MouseUp += delegate (object sender, MouseEventArgs args) {
                    this.MouseState = MaterialSkin.MouseState.HOVER;
                    base.Invalidate();
                };
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics graphics = pevent.Graphics;
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            graphics.Clear(base.Parent.BackColor);
            Color flatButtonHoverBackgroundColor = this.SkinManager.GetFlatButtonHoverBackgroundColor();
            using (Brush brush = new SolidBrush(Color.FromArgb((int) (this.hoverAnimationManager.GetProgress() * flatButtonHoverBackgroundColor.A), flatButtonHoverBackgroundColor.RemoveAlpha())))
            {
                graphics.FillRectangle(brush, base.ClientRectangle);
            }
            if (this.animationManager.IsAnimating())
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                for (int i = 0; i < this.animationManager.GetAnimationCount(); i++)
                {
                    double progress = this.animationManager.GetProgress(i);
                    Point source = this.animationManager.GetSource(i);
                    using (Brush brush2 = new SolidBrush(Color.FromArgb((int) (101.0 - (progress * 100.0)), Color.Black)))
                    {
                        int width = (int) ((progress * base.Width) * 2.0);
                        graphics.FillEllipse(brush2, new Rectangle(source.X - (width / 2), source.Y - (width / 2), width, width));
                    }
                }
                graphics.SmoothingMode = SmoothingMode.None;
            }
            StringFormat format = new StringFormat {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            graphics.DrawString(this.Text.ToUpper(), this.SkinManager.ROBOTO_MEDIUM_10, base.Enabled ? (this.Primary ? this.SkinManager.ColorScheme.PrimaryBrush : this.SkinManager.GetMainTextBrush()) : this.SkinManager.GetFlatButtonDisabledTextBrush(), base.ClientRectangle, format);
        }

        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager =>
            MaterialSkinManager.Instance;

        [Browsable(false)]
        public MaterialSkin.MouseState MouseState { get; set; }

        public bool Primary { get; set; }

        public override string Text
        {
            get => 
                base.Text;
            set
            {
                base.Text = value;
                this.textSize = base.CreateGraphics().MeasureString(value.ToUpper(), this.SkinManager.ROBOTO_MEDIUM_10);
                if (this.AutoSize)
                {
                    base.Size = this.GetPreferredSize();
                }
                base.Invalidate();
            }
        }
    }
}

