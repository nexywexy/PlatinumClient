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

    public class MaterialCheckBox : CheckBox, IMaterialControl
    {
        private const int CHECKBOX_SIZE = 0x12;
        private const int CHECKBOX_SIZE_HALF = 9;
        private const int CHECKBOX_INNER_BOX_SIZE = 14;
        private const int TEXT_OFFSET = 0x16;
        private bool ripple;
        private readonly AnimationManager animationManager;
        private readonly AnimationManager rippleAnimationManager;
        private int boxOffset;
        private Rectangle boxRectangle;
        private static readonly Point[] CHECKMARK_LINE = new Point[] { new Point(3, 8), new Point(7, 12), new Point(14, 5) };

        public MaterialCheckBox()
        {
            AnimationManager.AnimationProgress progress = null;
            AnimationManager.AnimationProgress progress2 = null;
            EventHandler handler = null;
            AnimationManager manager = new AnimationManager(true) {
                AnimationType = AnimationType.EaseInOut,
                Increment = 0.05
            };
            this.animationManager = manager;
            AnimationManager manager2 = new AnimationManager(false) {
                AnimationType = AnimationType.Linear,
                Increment = 0.1,
                SecondaryIncrement = 0.08
            };
            this.rippleAnimationManager = manager2;
            if (progress == null)
            {
                progress = sender => base.Invalidate();
            }
            this.animationManager.OnAnimationProgress += progress;
            if (progress2 == null)
            {
                progress2 = sender => base.Invalidate();
            }
            this.rippleAnimationManager.OnAnimationProgress += progress2;
            if (handler == null)
            {
                handler = (sender, args) => this.animationManager.StartNewAnimation(base.Checked ? AnimationDirection.In : AnimationDirection.Out, null);
            }
            base.CheckedChanged += handler;
            this.Ripple = true;
            this.MouseLocation = new Point(-1, -1);
        }

        private Bitmap DrawCheckMarkBitmap()
        {
            Bitmap image = new Bitmap(0x12, 0x12);
            Graphics graphics = Graphics.FromImage(image);
            graphics.Clear(Color.Transparent);
            using (Pen pen = new Pen(base.Parent.BackColor, 2f))
            {
                graphics.DrawLines(pen, CHECKMARK_LINE);
            }
            return image;
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            int width = ((this.boxOffset + 0x12) + 2) + ((int) base.CreateGraphics().MeasureString(this.Text, this.SkinManager.ROBOTO_MEDIUM_10).Width);
            return (this.Ripple ? new Size(width, 30) : new Size(width, 20));
        }

        private bool IsMouseInCheckArea() => 
            this.boxRectangle.Contains(this.MouseLocation);

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            this.Font = this.SkinManager.ROBOTO_MEDIUM_10;
            if (!base.DesignMode)
            {
                this.MouseState = MaterialSkin.MouseState.OUT;
                base.MouseEnter += (sender, args) => (this.MouseState = MaterialSkin.MouseState.HOVER);
                base.MouseLeave += delegate (object sender, EventArgs args) {
                    this.MouseLocation = new Point(-1, -1);
                    this.MouseState = MaterialSkin.MouseState.OUT;
                };
                base.MouseDown += delegate (object sender, MouseEventArgs args) {
                    this.MouseState = MaterialSkin.MouseState.DOWN;
                    if ((this.Ripple && (args.Button == MouseButtons.Left)) && this.IsMouseInCheckArea())
                    {
                        this.rippleAnimationManager.SecondaryIncrement = 0.0;
                        this.rippleAnimationManager.StartNewAnimation(AnimationDirection.InOutIn, new object[] { base.Checked });
                    }
                };
                base.MouseUp += delegate (object sender, MouseEventArgs args) {
                    this.MouseState = MaterialSkin.MouseState.HOVER;
                    this.rippleAnimationManager.SecondaryIncrement = 0.08;
                };
                base.MouseMove += delegate (object sender, MouseEventArgs args) {
                    this.MouseLocation = args.Location;
                    this.Cursor = this.IsMouseInCheckArea() ? Cursors.Hand : Cursors.Default;
                };
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics graphics = pevent.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            graphics.Clear(base.Parent.BackColor);
            int x = (this.boxOffset + 9) - 1;
            double progress = this.animationManager.GetProgress();
            int alpha = base.Enabled ? ((int) (progress * 255.0)) : this.SkinManager.GetCheckBoxOffDisabledColor().A;
            int num4 = base.Enabled ? ((int) (this.SkinManager.GetCheckboxOffColor().A * (1.0 - progress))) : this.SkinManager.GetCheckBoxOffDisabledColor().A;
            SolidBrush brush = new SolidBrush(Color.FromArgb(alpha, base.Enabled ? this.SkinManager.ColorScheme.AccentColor : this.SkinManager.GetCheckBoxOffDisabledColor()));
            SolidBrush brush2 = new SolidBrush(base.Enabled ? this.SkinManager.ColorScheme.AccentColor : this.SkinManager.GetCheckBoxOffDisabledColor());
            Pen pen = new Pen(brush.Color);
            if (this.Ripple && this.rippleAnimationManager.IsAnimating())
            {
                for (int i = 0; i < this.rippleAnimationManager.GetAnimationCount(); i++)
                {
                    double num6 = this.rippleAnimationManager.GetProgress(i);
                    Point point = new Point(x, x);
                    SolidBrush brush3 = new SolidBrush(Color.FromArgb((int) (num6 * 40.0), ((bool) this.rippleAnimationManager.GetData(i)[0]) ? Color.Black : brush.Color));
                    int num7 = ((base.Height % 2) == 0) ? (base.Height - 3) : (base.Height - 2);
                    int num8 = (this.rippleAnimationManager.GetDirection(i) == AnimationDirection.InOutIn) ? ((int) (num7 * (0.8 + (0.2 * num6)))) : num7;
                    using (GraphicsPath path = DrawHelper.CreateRoundRect((float) (point.X - (num8 / 2)), (float) (point.Y - (num8 / 2)), (float) num8, (float) num8, (float) (num8 / 2)))
                    {
                        graphics.FillPath(brush3, path);
                    }
                    brush3.Dispose();
                }
            }
            brush2.Dispose();
            Rectangle rect = new Rectangle(this.boxOffset, this.boxOffset, (int) (17.0 * progress), 0x11);
            using (GraphicsPath path2 = DrawHelper.CreateRoundRect((float) this.boxOffset, (float) this.boxOffset, 17f, 17f, 1f))
            {
                SolidBrush brush4 = new SolidBrush(DrawHelper.BlendColor(base.Parent.BackColor, base.Enabled ? this.SkinManager.GetCheckboxOffColor() : this.SkinManager.GetCheckBoxOffDisabledColor(), (double) num4));
                Pen pen2 = new Pen(brush4.Color);
                graphics.FillPath(brush4, path2);
                graphics.DrawPath(pen2, path2);
                graphics.FillRectangle(new SolidBrush(base.Parent.BackColor), this.boxOffset + 2, this.boxOffset + 2, 13, 13);
                graphics.DrawRectangle(new Pen(base.Parent.BackColor), this.boxOffset + 2, this.boxOffset + 2, 13, 13);
                brush4.Dispose();
                pen2.Dispose();
                if (base.Enabled)
                {
                    graphics.FillPath(brush, path2);
                    graphics.DrawPath(pen, path2);
                }
                else if (base.Checked)
                {
                    graphics.SmoothingMode = SmoothingMode.None;
                    graphics.FillRectangle(brush, this.boxOffset + 2, this.boxOffset + 2, 14, 14);
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                }
                graphics.DrawImageUnscaledAndClipped(this.DrawCheckMarkBitmap(), rect);
            }
            SizeF ef = graphics.MeasureString(this.Text, this.SkinManager.ROBOTO_MEDIUM_10);
            graphics.DrawString(this.Text, this.SkinManager.ROBOTO_MEDIUM_10, base.Enabled ? this.SkinManager.GetMainTextBrush() : this.SkinManager.GetDisabledOrHintBrush(), (float) (this.boxOffset + 0x16), (base.Height / 2) - (ef.Height / 2f));
            pen.Dispose();
            brush.Dispose();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.boxOffset = (base.Height / 2) - 9;
            this.boxRectangle = new Rectangle(this.boxOffset, this.boxOffset, 0x11, 0x11);
        }

        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager =>
            MaterialSkinManager.Instance;

        [Browsable(false)]
        public MaterialSkin.MouseState MouseState { get; set; }

        [Browsable(false)]
        public Point MouseLocation { get; set; }

        [Category("Behavior")]
        public bool Ripple
        {
            get => 
                this.ripple;
            set
            {
                this.ripple = value;
                this.AutoSize = this.AutoSize;
                if (value)
                {
                    base.Margin = new Padding(0);
                }
                base.Invalidate();
            }
        }

        public override bool AutoSize
        {
            get => 
                base.AutoSize;
            set
            {
                base.AutoSize = value;
                if (value)
                {
                    base.Size = new Size(10, 10);
                }
            }
        }
    }
}

