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

    public class MaterialRadioButton : RadioButton, IMaterialControl
    {
        private const int RADIOBUTTON_SIZE = 0x13;
        private const int RADIOBUTTON_SIZE_HALF = 9;
        private const int RADIOBUTTON_OUTER_CIRCLE_WIDTH = 2;
        private const int RADIOBUTTON_INNER_CIRCLE_SIZE = 15;
        private bool ripple;
        private readonly AnimationManager animationManager;
        private readonly AnimationManager rippleAnimationManager;
        private Rectangle radioButtonBounds;
        private int boxOffset;

        public MaterialRadioButton()
        {
            AnimationManager.AnimationProgress progress = null;
            AnimationManager.AnimationProgress progress2 = null;
            EventHandler handler = null;
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.DoubleBuffer, true);
            AnimationManager manager = new AnimationManager(true) {
                AnimationType = AnimationType.EaseInOut,
                Increment = 0.06
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
            base.SizeChanged += new EventHandler(this.OnSizeChanged);
            this.Ripple = true;
            this.MouseLocation = new Point(-1, -1);
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            int width = (this.boxOffset + 20) + ((int) base.CreateGraphics().MeasureString(this.Text, this.SkinManager.ROBOTO_MEDIUM_10).Width);
            return (this.Ripple ? new Size(width, 30) : new Size(width, 20));
        }

        private bool IsMouseInCheckArea() => 
            this.radioButtonBounds.Contains(this.MouseLocation);

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
            GraphicsPath path;
            Graphics graphics = pevent.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            graphics.Clear(base.Parent.BackColor);
            int x = this.boxOffset + 9;
            double progress = this.animationManager.GetProgress();
            int alpha = base.Enabled ? ((int) (progress * 255.0)) : this.SkinManager.GetCheckBoxOffDisabledColor().A;
            int num4 = base.Enabled ? ((int) (this.SkinManager.GetCheckboxOffColor().A * (1.0 - progress))) : this.SkinManager.GetCheckBoxOffDisabledColor().A;
            float width = (float) (progress * 8.0);
            float num6 = width / 2f;
            width = (float) (progress * 9.0);
            SolidBrush brush = new SolidBrush(Color.FromArgb(alpha, base.Enabled ? this.SkinManager.ColorScheme.AccentColor : this.SkinManager.GetCheckBoxOffDisabledColor()));
            Pen pen = new Pen(brush.Color);
            if (this.Ripple && this.rippleAnimationManager.IsAnimating())
            {
                for (int i = 0; i < this.rippleAnimationManager.GetAnimationCount(); i++)
                {
                    double num8 = this.rippleAnimationManager.GetProgress(i);
                    Point point = new Point(x, x);
                    SolidBrush brush2 = new SolidBrush(Color.FromArgb((int) (num8 * 40.0), ((bool) this.rippleAnimationManager.GetData(i)[0]) ? Color.Black : brush.Color));
                    int num9 = ((base.Height % 2) == 0) ? (base.Height - 3) : (base.Height - 2);
                    int num10 = (this.rippleAnimationManager.GetDirection(i) == AnimationDirection.InOutIn) ? ((int) (num9 * (0.8 + (0.2 * num8)))) : num9;
                    using (path = DrawHelper.CreateRoundRect((float) (point.X - (num10 / 2)), (float) (point.Y - (num10 / 2)), (float) num10, (float) num10, (float) (num10 / 2)))
                    {
                        graphics.FillPath(brush2, path);
                    }
                    brush2.Dispose();
                }
            }
            Color color = DrawHelper.BlendColor(base.Parent.BackColor, base.Enabled ? this.SkinManager.GetCheckboxOffColor() : this.SkinManager.GetCheckBoxOffDisabledColor(), (double) num4);
            using (path = DrawHelper.CreateRoundRect((float) this.boxOffset, (float) this.boxOffset, 19f, 19f, 9f))
            {
                graphics.FillPath(new SolidBrush(color), path);
                if (base.Enabled)
                {
                    graphics.FillPath(brush, path);
                }
            }
            graphics.FillEllipse(new SolidBrush(base.Parent.BackColor), 2 + this.boxOffset, 2 + this.boxOffset, 15, 15);
            if (base.Checked)
            {
                using (path = DrawHelper.CreateRoundRect(x - num6, x - num6, width, width, 4f))
                {
                    graphics.FillPath(brush, path);
                }
            }
            SizeF ef = graphics.MeasureString(this.Text, this.SkinManager.ROBOTO_MEDIUM_10);
            graphics.DrawString(this.Text, this.SkinManager.ROBOTO_MEDIUM_10, base.Enabled ? this.SkinManager.GetMainTextBrush() : this.SkinManager.GetDisabledOrHintBrush(), (float) (this.boxOffset + 0x16), (base.Height / 2) - (ef.Height / 2f));
            brush.Dispose();
            pen.Dispose();
        }

        private void OnSizeChanged(object sender, EventArgs eventArgs)
        {
            this.boxOffset = (base.Height / 2) - ((int) Math.Ceiling((double) 9.5));
            this.radioButtonBounds = new Rectangle(this.boxOffset, this.boxOffset, 0x13, 0x13);
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
    }
}

