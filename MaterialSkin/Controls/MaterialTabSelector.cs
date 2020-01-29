namespace MaterialSkin.Controls
{
    using MaterialSkin;
    using MaterialSkin.Animations;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Text;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    public class MaterialTabSelector : Control, IMaterialControl
    {
        private const int TAB_HEADER_PADDING = 0x18;
        private const int TAB_INDICATOR_HEIGHT = 2;
        private MaterialTabControl baseTabControl;
        private int previousSelectedTabIndex;
        private Point animationSource;
        private readonly AnimationManager animationManager;
        private List<Rectangle> tabRects;

        public MaterialTabSelector()
        {
            AnimationManager.AnimationProgress progress = null;
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.DoubleBuffer, true);
            base.Height = 0x30;
            AnimationManager manager = new AnimationManager(true) {
                AnimationType = AnimationType.EaseOut,
                Increment = 0.04
            };
            this.animationManager = manager;
            if (progress == null)
            {
                progress = sender => base.Invalidate();
            }
            this.animationManager.OnAnimationProgress += progress;
        }

        private int CalculateTextAlpha(int tabIndex, double animationProgress)
        {
            int a = this.SkinManager.ACTION_BAR_TEXT.A;
            int num2 = this.SkinManager.ACTION_BAR_TEXT_SECONDARY.A;
            if (!((tabIndex != this.baseTabControl.SelectedIndex) || this.animationManager.IsAnimating()))
            {
                return a;
            }
            if ((tabIndex != this.previousSelectedTabIndex) && (tabIndex != this.baseTabControl.SelectedIndex))
            {
                return num2;
            }
            if (tabIndex == this.previousSelectedTabIndex)
            {
                return (a - ((int) ((a - num2) * animationProgress)));
            }
            return (num2 + ((int) ((a - num2) * animationProgress)));
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (this.tabRects == null)
            {
                this.UpdateTabRects();
            }
            for (int i = 0; i < this.tabRects.Count; i++)
            {
                Rectangle rectangle = this.tabRects[i];
                if (rectangle.Contains(e.Location))
                {
                    this.baseTabControl.SelectedIndex = i;
                }
            }
            this.animationSource = e.Location;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            graphics.Clear(this.SkinManager.ColorScheme.PrimaryColor);
            if (this.baseTabControl != null)
            {
                if ((!this.animationManager.IsAnimating() || (this.tabRects == null)) || (this.tabRects.Count != this.baseTabControl.TabCount))
                {
                    this.UpdateTabRects();
                }
                double progress = this.animationManager.GetProgress();
                if (this.animationManager.IsAnimating())
                {
                    SolidBrush brush = new SolidBrush(Color.FromArgb((int) (51.0 - (progress * 50.0)), Color.White));
                    Rectangle rectangle = this.tabRects[this.baseTabControl.SelectedIndex];
                    int num2 = (int) ((progress * rectangle.Width) * 1.75);
                    graphics.SetClip(this.tabRects[this.baseTabControl.SelectedIndex]);
                    graphics.FillEllipse(brush, new Rectangle(this.animationSource.X - (num2 / 2), this.animationSource.Y - (num2 / 2), num2, num2));
                    graphics.ResetClip();
                    brush.Dispose();
                }
                foreach (TabPage page in this.baseTabControl.TabPages)
                {
                    int tabIndex = page.TabIndex;
                    Brush brush = new SolidBrush(Color.FromArgb(this.CalculateTextAlpha(tabIndex, progress), this.SkinManager.ColorScheme.TextColor));
                    StringFormat format = new StringFormat {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    graphics.DrawString(page.Text.ToUpper(), this.SkinManager.ROBOTO_MEDIUM_10, brush, this.tabRects[tabIndex], format);
                    brush.Dispose();
                }
                int num4 = (this.previousSelectedTabIndex == -1) ? this.baseTabControl.SelectedIndex : this.previousSelectedTabIndex;
                Rectangle rectangle2 = this.tabRects[num4];
                Rectangle rectangle3 = this.tabRects[this.baseTabControl.SelectedIndex];
                int y = rectangle3.Bottom - 2;
                int x = rectangle2.X + ((int) ((rectangle3.X - rectangle2.X) * progress));
                int width = rectangle2.Width + ((int) ((rectangle3.Width - rectangle2.Width) * progress));
                graphics.FillRectangle(this.SkinManager.ColorScheme.AccentBrush, x, y, width, 2);
            }
        }

        private void UpdateTabRects()
        {
            this.tabRects = new List<Rectangle>();
            if ((this.baseTabControl != null) && (this.baseTabControl.TabCount != 0))
            {
                using (Bitmap bitmap = new Bitmap(1, 1))
                {
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        this.tabRects.Add(new Rectangle(this.SkinManager.FORM_PADDING, 0, 0x30 + ((int) graphics.MeasureString(this.baseTabControl.TabPages[0].Text, this.SkinManager.ROBOTO_MEDIUM_10).Width), base.Height));
                        for (int i = 1; i < this.baseTabControl.TabPages.Count; i++)
                        {
                            Rectangle rectangle = this.tabRects[i - 1];
                            this.tabRects.Add(new Rectangle(rectangle.Right, 0, 0x30 + ((int) graphics.MeasureString(this.baseTabControl.TabPages[i].Text, this.SkinManager.ROBOTO_MEDIUM_10).Width), base.Height));
                        }
                    }
                }
            }
        }

        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager =>
            MaterialSkinManager.Instance;

        [Browsable(false)]
        public MaterialSkin.MouseState MouseState { get; set; }

        public MaterialTabControl BaseTabControl
        {
            get => 
                this.baseTabControl;
            set
            {
                this.baseTabControl = value;
                if (this.baseTabControl != null)
                {
                    this.previousSelectedTabIndex = this.baseTabControl.SelectedIndex;
                    this.baseTabControl.Deselected += (sender, args) => (this.previousSelectedTabIndex = this.baseTabControl.SelectedIndex);
                    this.baseTabControl.SelectedIndexChanged += delegate (object sender, EventArgs args) {
                        this.animationManager.SetProgress(0.0);
                        this.animationManager.StartNewAnimation(AnimationDirection.In, null);
                    };
                    this.baseTabControl.ControlAdded += (param0, param1) => base.Invalidate();
                    this.baseTabControl.ControlRemoved += (param0, param1) => base.Invalidate();
                }
            }
        }
    }
}

