namespace MaterialSkin.Controls
{
    using MaterialSkin;
    using MaterialSkin.Animations;
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    internal class MaterialToolStripRender : ToolStripProfessionalRenderer, IMaterialControl
    {
        private Rectangle GetItemRect(ToolStripItem item) => 
            new Rectangle(0, item.ContentRectangle.Y, item.ContentRectangle.Width + 4, item.ContentRectangle.Height);

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Point point = new Point(e.ArrowRectangle.X + (e.ArrowRectangle.Width / 2), e.ArrowRectangle.Y + (e.ArrowRectangle.Height / 2));
            Brush brush = e.Item.Enabled ? this.SkinManager.GetMainTextBrush() : this.SkinManager.GetDisabledOrHintBrush();
            using (GraphicsPath path = new GraphicsPath())
            {
                Point[] points = new Point[] { new Point(point.X - 4, point.Y - 4), new Point(point.X, point.Y), new Point(point.X - 4, point.Y + 4) };
                path.AddLines(points);
                path.CloseFigure();
                graphics.FillPath(brush, path);
            }
        }

        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            Rectangle itemRect = this.GetItemRect(e.Item);
            Rectangle layoutRectangle = new Rectangle(0x18, itemRect.Y, itemRect.Width - 40, itemRect.Height);
            StringFormat format = new StringFormat {
                LineAlignment = StringAlignment.Center
            };
            graphics.DrawString(e.Text, this.SkinManager.ROBOTO_MEDIUM_10, e.Item.Enabled ? this.SkinManager.GetMainTextBrush() : this.SkinManager.GetDisabledOrHintBrush(), layoutRectangle, format);
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(this.SkinManager.GetApplicationBackgroundColor());
            Rectangle itemRect = this.GetItemRect(e.Item);
            graphics.FillRectangle((e.Item.Selected && e.Item.Enabled) ? this.SkinManager.GetCmsSelectedItemBrush() : new SolidBrush(this.SkinManager.GetApplicationBackgroundColor()), itemRect);
            MaterialContextMenuStrip toolStrip = e.ToolStrip as MaterialContextMenuStrip;
            if (toolStrip != null)
            {
                AnimationManager animationManager = toolStrip.animationManager;
                Point animationSource = toolStrip.animationSource;
                if (toolStrip.animationManager.IsAnimating() && e.Item.Bounds.Contains(animationSource))
                {
                    for (int i = 0; i < animationManager.GetAnimationCount(); i++)
                    {
                        double progress = animationManager.GetProgress(i);
                        SolidBrush brush = new SolidBrush(Color.FromArgb((int) (51.0 - (progress * 50.0)), Color.Black));
                        int width = (int) ((progress * itemRect.Width) * 2.5);
                        graphics.FillEllipse(brush, new Rectangle(animationSource.X - (width / 2), itemRect.Y - itemRect.Height, width, itemRect.Height * 3));
                    }
                }
            }
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.FillRectangle(new SolidBrush(this.SkinManager.GetApplicationBackgroundColor()), e.Item.Bounds);
            graphics.DrawLine(new Pen(this.SkinManager.GetDividersColor()), new Point(e.Item.Bounds.Left, e.Item.Bounds.Height / 2), new Point(e.Item.Bounds.Right, e.Item.Bounds.Height / 2));
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(this.SkinManager.GetDividersColor()), new Rectangle(e.AffectedBounds.X, e.AffectedBounds.Y, e.AffectedBounds.Width - 1, e.AffectedBounds.Height - 1));
        }

        public int Depth { get; set; }

        public MaterialSkinManager SkinManager =>
            MaterialSkinManager.Instance;

        public MaterialSkin.MouseState MouseState { get; set; }
    }
}

