namespace MaterialSkin.Controls
{
    using MaterialSkin;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Text;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class MaterialForm : Form, IMaterialControl
    {
        public const int WM_NCLBUTTONDOWN = 0xa1;
        public const int HT_CAPTION = 2;
        public const int WM_MOUSEMOVE = 0x200;
        public const int WM_LBUTTONDOWN = 0x201;
        public const int WM_LBUTTONUP = 0x202;
        public const int WM_LBUTTONDBLCLK = 0x203;
        public const int WM_RBUTTONDOWN = 0x204;
        private const int HTBOTTOMLEFT = 0x10;
        private const int HTBOTTOMRIGHT = 0x11;
        private const int HTLEFT = 10;
        private const int HTRIGHT = 11;
        private const int HTBOTTOM = 15;
        private const int HTTOP = 12;
        private const int HTTOPLEFT = 13;
        private const int HTTOPRIGHT = 14;
        private const int BORDER_WIDTH = 7;
        private const int WMSZ_TOP = 3;
        private const int WMSZ_TOPLEFT = 4;
        private const int WMSZ_TOPRIGHT = 5;
        private const int WMSZ_LEFT = 1;
        private const int WMSZ_RIGHT = 2;
        private const int WMSZ_BOTTOM = 6;
        private const int WMSZ_BOTTOMLEFT = 7;
        private const int WMSZ_BOTTOMRIGHT = 8;
        private const int STATUS_BAR_BUTTON_WIDTH = 0x18;
        private const int STATUS_BAR_HEIGHT = 0x18;
        private const int ACTION_BAR_HEIGHT = 40;
        private const uint TPM_LEFTALIGN = 0;
        private const uint TPM_RETURNCMD = 0x100;
        private const int WM_SYSCOMMAND = 0x112;
        private const int WS_MINIMIZEBOX = 0x20000;
        private const int WS_SYSMENU = 0x80000;
        private const int MONITOR_DEFAULTTONEAREST = 2;
        private ResizeDirection resizeDir;
        private ButtonState buttonState = ButtonState.None;
        private readonly Dictionary<int, int> resizingLocationsToCmd;
        private readonly Cursor[] resizeCursors;
        private Rectangle minButtonBounds;
        private Rectangle maxButtonBounds;
        private Rectangle xButtonBounds;
        private Rectangle actionBarBounds;
        private Rectangle statusBarBounds;
        private bool Maximized;
        private Size previousSize;
        private Point previousLocation;
        private bool headerMouseDown;

        public MaterialForm()
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int> {
                { 
                    12,
                    3
                },
                { 
                    13,
                    4
                },
                { 
                    14,
                    5
                },
                { 
                    10,
                    1
                },
                { 
                    11,
                    2
                },
                { 
                    15,
                    6
                },
                { 
                    0x10,
                    7
                },
                { 
                    0x11,
                    8
                }
            };
            this.resizingLocationsToCmd = dictionary;
            this.resizeCursors = new Cursor[] { Cursors.SizeNESW, Cursors.SizeWE, Cursors.SizeNWSE, Cursors.SizeWE, Cursors.SizeNS };
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Sizable = true;
            this.DoubleBuffered = true;
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Application.AddMessageFilter(new MouseMessageFilter());
            MouseMessageFilter.MouseMove += new MouseEventHandler(this.OnGlobalMouseMove);
        }

        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern bool GetMonitorInfo(HandleRef hmonitor, [In, Out] MONITORINFOEX info);
        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        private void MaximizeWindow(bool maximize)
        {
            if (base.MaximizeBox && base.ControlBox)
            {
                this.Maximized = maximize;
                if (maximize)
                {
                    IntPtr handle = MonitorFromWindow(base.Handle, 2);
                    MONITORINFOEX info = new MONITORINFOEX();
                    GetMonitorInfo(new HandleRef(null, handle), info);
                    this.previousSize = base.Size;
                    this.previousLocation = base.Location;
                    int width = info.rcWork.Width();
                    base.Size = new Size(width, info.rcWork.Height());
                    base.Location = new Point(info.rcWork.left, info.rcWork.top);
                }
                else
                {
                    base.Size = this.previousSize;
                    base.Location = this.previousLocation;
                }
            }
        }

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);
        protected void OnGlobalMouseMove(object sender, MouseEventArgs e)
        {
            if (!base.IsDisposed)
            {
                Point point = base.PointToClient(e.Location);
                MouseEventArgs args = new MouseEventArgs(MouseButtons.None, 0, point.X, point.Y, 0);
                this.OnMouseMove(args);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!base.DesignMode)
            {
                this.UpdateButtons(e, false);
                if (!((e.Button != MouseButtons.Left) || this.Maximized))
                {
                    this.ResizeForm(this.resizeDir);
                }
                base.OnMouseDown(e);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (!base.DesignMode)
            {
                this.buttonState = ButtonState.None;
                base.Invalidate();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!base.DesignMode)
            {
                if (this.Sizable)
                {
                    bool flag2 = base.GetChildAtPoint(e.Location) != null;
                    if (!((((e.Location.X >= 7) || (e.Location.Y <= (base.Height - 7))) || flag2) || this.Maximized))
                    {
                        this.resizeDir = ResizeDirection.BottomLeft;
                        this.Cursor = Cursors.SizeNESW;
                    }
                    else if (!(((e.Location.X >= 7) || flag2) || this.Maximized))
                    {
                        this.resizeDir = ResizeDirection.Left;
                        this.Cursor = Cursors.SizeWE;
                    }
                    else if (!((((e.Location.X <= (base.Width - 7)) || (e.Location.Y <= (base.Height - 7))) || flag2) || this.Maximized))
                    {
                        this.resizeDir = ResizeDirection.BottomRight;
                        this.Cursor = Cursors.SizeNWSE;
                    }
                    else if (!(((e.Location.X <= (base.Width - 7)) || flag2) || this.Maximized))
                    {
                        this.resizeDir = ResizeDirection.Right;
                        this.Cursor = Cursors.SizeWE;
                    }
                    else if (!(((e.Location.Y <= (base.Height - 7)) || flag2) || this.Maximized))
                    {
                        this.resizeDir = ResizeDirection.Bottom;
                        this.Cursor = Cursors.SizeNS;
                    }
                    else
                    {
                        this.resizeDir = ResizeDirection.None;
                        if (this.resizeCursors.Contains<Cursor>(this.Cursor))
                        {
                            this.Cursor = Cursors.Default;
                        }
                    }
                }
                this.UpdateButtons(e, false);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!base.DesignMode)
            {
                this.UpdateButtons(e, true);
                base.OnMouseUp(e);
                ReleaseCapture();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            graphics.Clear(this.SkinManager.GetApplicationBackgroundColor());
            graphics.FillRectangle(this.SkinManager.ColorScheme.DarkPrimaryBrush, this.statusBarBounds);
            graphics.FillRectangle(this.SkinManager.ColorScheme.PrimaryBrush, this.actionBarBounds);
            using (Pen pen = new Pen(this.SkinManager.GetDividersColor(), 1f))
            {
                graphics.DrawLine(pen, new Point(0, this.actionBarBounds.Bottom), new Point(0, base.Height - 2));
                graphics.DrawLine(pen, new Point(base.Width - 1, this.actionBarBounds.Bottom), new Point(base.Width - 1, base.Height - 2));
                graphics.DrawLine(pen, new Point(0, base.Height - 1), new Point(base.Width - 1, base.Height - 1));
            }
            bool flag2 = base.MinimizeBox && base.ControlBox;
            bool flag3 = base.MaximizeBox && base.ControlBox;
            Brush flatButtonHoverBackgroundBrush = this.SkinManager.GetFlatButtonHoverBackgroundBrush();
            Brush flatButtonPressedBackgroundBrush = this.SkinManager.GetFlatButtonPressedBackgroundBrush();
            if ((this.buttonState == ButtonState.MinOver) && flag2)
            {
                graphics.FillRectangle(flatButtonHoverBackgroundBrush, flag3 ? this.minButtonBounds : this.maxButtonBounds);
            }
            if ((this.buttonState == ButtonState.MinDown) && flag2)
            {
                graphics.FillRectangle(flatButtonPressedBackgroundBrush, flag3 ? this.minButtonBounds : this.maxButtonBounds);
            }
            if ((this.buttonState == ButtonState.MaxOver) && flag3)
            {
                graphics.FillRectangle(flatButtonHoverBackgroundBrush, this.maxButtonBounds);
            }
            if ((this.buttonState == ButtonState.MaxDown) && flag3)
            {
                graphics.FillRectangle(flatButtonPressedBackgroundBrush, this.maxButtonBounds);
            }
            if ((this.buttonState == ButtonState.XOver) && base.ControlBox)
            {
                graphics.FillRectangle(flatButtonHoverBackgroundBrush, this.xButtonBounds);
            }
            if ((this.buttonState == ButtonState.XDown) && base.ControlBox)
            {
                graphics.FillRectangle(flatButtonPressedBackgroundBrush, this.xButtonBounds);
            }
            using (Pen pen2 = new Pen(this.SkinManager.ACTION_BAR_TEXT_SECONDARY, 2f))
            {
                if (flag2)
                {
                    int num = flag3 ? this.minButtonBounds.X : this.maxButtonBounds.X;
                    int num2 = flag3 ? this.minButtonBounds.Y : this.maxButtonBounds.Y;
                    graphics.DrawLine(pen2, (int) (num + ((int) (this.minButtonBounds.Width * 0.33))), (int) (num2 + ((int) (this.minButtonBounds.Height * 0.66))), (int) (num + ((int) (this.minButtonBounds.Width * 0.66))), (int) (num2 + ((int) (this.minButtonBounds.Height * 0.66))));
                }
                if (flag3)
                {
                    graphics.DrawRectangle(pen2, this.maxButtonBounds.X + ((int) (this.maxButtonBounds.Width * 0.33)), this.maxButtonBounds.Y + ((int) (this.maxButtonBounds.Height * 0.36)), (int) (this.maxButtonBounds.Width * 0.39), (int) (this.maxButtonBounds.Height * 0.31));
                }
                if (base.ControlBox)
                {
                    graphics.DrawLine(pen2, (int) (this.xButtonBounds.X + ((int) (this.xButtonBounds.Width * 0.33))), (int) (this.xButtonBounds.Y + ((int) (this.xButtonBounds.Height * 0.33))), (int) (this.xButtonBounds.X + ((int) (this.xButtonBounds.Width * 0.66))), (int) (this.xButtonBounds.Y + ((int) (this.xButtonBounds.Height * 0.66))));
                    graphics.DrawLine(pen2, (int) (this.xButtonBounds.X + ((int) (this.xButtonBounds.Width * 0.66))), (int) (this.xButtonBounds.Y + ((int) (this.xButtonBounds.Height * 0.33))), (int) (this.xButtonBounds.X + ((int) (this.xButtonBounds.Width * 0.33))), (int) (this.xButtonBounds.Y + ((int) (this.xButtonBounds.Height * 0.66))));
                }
            }
            StringFormat format = new StringFormat {
                LineAlignment = StringAlignment.Center
            };
            graphics.DrawString(this.Text, this.SkinManager.ROBOTO_MEDIUM_12, this.SkinManager.ColorScheme.TextBrush, new Rectangle(this.SkinManager.FORM_PADDING, 0x18, base.Width, 40), format);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.minButtonBounds = new Rectangle((base.Width - (this.SkinManager.FORM_PADDING / 2)) - 0x48, 0, 0x18, 0x18);
            this.maxButtonBounds = new Rectangle((base.Width - (this.SkinManager.FORM_PADDING / 2)) - 0x30, 0, 0x18, 0x18);
            this.xButtonBounds = new Rectangle((base.Width - (this.SkinManager.FORM_PADDING / 2)) - 0x18, 0, 0x18, 0x18);
            this.statusBarBounds = new Rectangle(0, 0, base.Width, 0x18);
            this.actionBarBounds = new Rectangle(0, 0x18, base.Width, 40);
        }

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        private void ResizeForm(ResizeDirection direction)
        {
            if (!base.DesignMode)
            {
                int wParam = -1;
                switch (direction)
                {
                    case ResizeDirection.BottomLeft:
                        wParam = 0x10;
                        break;

                    case ResizeDirection.Left:
                        wParam = 10;
                        break;

                    case ResizeDirection.Right:
                        wParam = 11;
                        break;

                    case ResizeDirection.BottomRight:
                        wParam = 0x11;
                        break;

                    case ResizeDirection.Bottom:
                        wParam = 15;
                        break;
                }
                ReleaseCapture();
                if (wParam != -1)
                {
                    SendMessage(base.Handle, 0xa1, wParam, 0);
                }
            }
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern int TrackPopupMenuEx(IntPtr hmenu, uint fuFlags, int x, int y, IntPtr hwnd, IntPtr lptpm);
        private void UpdateButtons(MouseEventArgs e, bool up = false)
        {
            if (!base.DesignMode)
            {
                ButtonState buttonState = this.buttonState;
                bool flag2 = base.MinimizeBox && base.ControlBox;
                bool flag3 = base.MaximizeBox && base.ControlBox;
                if ((e.Button == MouseButtons.Left) && !up)
                {
                    if ((flag2 && !flag3) && this.maxButtonBounds.Contains(e.Location))
                    {
                        this.buttonState = ButtonState.MinDown;
                    }
                    else if ((flag2 && flag3) && this.minButtonBounds.Contains(e.Location))
                    {
                        this.buttonState = ButtonState.MinDown;
                    }
                    else if (flag3 && this.maxButtonBounds.Contains(e.Location))
                    {
                        this.buttonState = ButtonState.MaxDown;
                    }
                    else if (base.ControlBox && this.xButtonBounds.Contains(e.Location))
                    {
                        this.buttonState = ButtonState.XDown;
                    }
                    else
                    {
                        this.buttonState = ButtonState.None;
                    }
                }
                else if ((flag2 && !flag3) && this.maxButtonBounds.Contains(e.Location))
                {
                    this.buttonState = ButtonState.MinOver;
                    if (buttonState == ButtonState.MinDown)
                    {
                        base.WindowState = FormWindowState.Minimized;
                    }
                }
                else if ((flag2 && flag3) && this.minButtonBounds.Contains(e.Location))
                {
                    this.buttonState = ButtonState.MinOver;
                    if (buttonState == ButtonState.MinDown)
                    {
                        base.WindowState = FormWindowState.Minimized;
                    }
                }
                else if ((base.MaximizeBox && base.ControlBox) && this.maxButtonBounds.Contains(e.Location))
                {
                    this.buttonState = ButtonState.MaxOver;
                    if (buttonState == ButtonState.MaxDown)
                    {
                        this.MaximizeWindow(!this.Maximized);
                    }
                }
                else if (base.ControlBox && this.xButtonBounds.Contains(e.Location))
                {
                    this.buttonState = ButtonState.XOver;
                    if (buttonState == ButtonState.XDown)
                    {
                        base.Close();
                    }
                }
                else
                {
                    this.buttonState = ButtonState.None;
                }
                if (buttonState != this.buttonState)
                {
                    base.Invalidate();
                }
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (!base.DesignMode && !base.IsDisposed)
            {
                if (m.Msg == 0x203)
                {
                    this.MaximizeWindow(!this.Maximized);
                }
                else if ((((m.Msg == 0x200) && this.Maximized) && (this.statusBarBounds.Contains(base.PointToClient(Cursor.Position)) || this.actionBarBounds.Contains(base.PointToClient(Cursor.Position)))) && ((!this.minButtonBounds.Contains(base.PointToClient(Cursor.Position)) && !this.maxButtonBounds.Contains(base.PointToClient(Cursor.Position))) && !this.xButtonBounds.Contains(base.PointToClient(Cursor.Position))))
                {
                    if (this.headerMouseDown)
                    {
                        this.Maximized = false;
                        this.headerMouseDown = false;
                        Point point = base.PointToClient(Cursor.Position);
                        if (point.X < (base.Width / 2))
                        {
                            base.Location = (point.X < (this.previousSize.Width / 2)) ? new Point(Cursor.Position.X - point.X, Cursor.Position.Y - point.Y) : new Point(Cursor.Position.X - (this.previousSize.Width / 2), Cursor.Position.Y - point.Y);
                        }
                        else
                        {
                            base.Location = ((base.Width - point.X) < (this.previousSize.Width / 2)) ? new Point(((Cursor.Position.X - this.previousSize.Width) + base.Width) - point.X, Cursor.Position.Y - point.Y) : new Point(Cursor.Position.X - (this.previousSize.Width / 2), Cursor.Position.Y - point.Y);
                        }
                        base.Size = this.previousSize;
                        ReleaseCapture();
                        SendMessage(base.Handle, 0xa1, 2, 0);
                    }
                }
                else if (((m.Msg == 0x201) && (this.statusBarBounds.Contains(base.PointToClient(Cursor.Position)) || this.actionBarBounds.Contains(base.PointToClient(Cursor.Position)))) && ((!this.minButtonBounds.Contains(base.PointToClient(Cursor.Position)) && !this.maxButtonBounds.Contains(base.PointToClient(Cursor.Position))) && !this.xButtonBounds.Contains(base.PointToClient(Cursor.Position))))
                {
                    if (!this.Maximized)
                    {
                        ReleaseCapture();
                        SendMessage(base.Handle, 0xa1, 2, 0);
                    }
                    else
                    {
                        this.headerMouseDown = true;
                    }
                }
                else if (m.Msg == 0x204)
                {
                    Point pt = base.PointToClient(Cursor.Position);
                    if (!(((!this.statusBarBounds.Contains(pt) || this.minButtonBounds.Contains(pt)) || this.maxButtonBounds.Contains(pt)) || this.xButtonBounds.Contains(pt)))
                    {
                        int wParam = TrackPopupMenuEx(GetSystemMenu(base.Handle, false), 0x100, Cursor.Position.X, Cursor.Position.Y, base.Handle, IntPtr.Zero);
                        SendMessage(base.Handle, 0x112, wParam, 0);
                    }
                }
                else if (m.Msg == 0xa1)
                {
                    if (this.Sizable)
                    {
                        byte num2 = 0;
                        if (this.resizingLocationsToCmd.ContainsKey((int) m.WParam))
                        {
                            num2 = (byte) this.resizingLocationsToCmd[(int) m.WParam];
                        }
                        if (num2 != 0)
                        {
                            SendMessage(base.Handle, 0x112, 0xf000 | num2, (int) m.LParam);
                        }
                    }
                }
                else if (m.Msg == 0x202)
                {
                    this.headerMouseDown = false;
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

        public System.Windows.Forms.FormBorderStyle FormBorderStyle
        {
            get => 
                base.FormBorderStyle;
            set => 
                (base.FormBorderStyle = value);
        }

        public bool Sizable { get; set; }

        protected override System.Windows.Forms.CreateParams CreateParams
        {
            get
            {
                System.Windows.Forms.CreateParams createParams = base.CreateParams;
                createParams.Style = (createParams.Style | 0x20000) | 0x80000;
                return createParams;
            }
        }

        private enum ButtonState
        {
            XOver,
            MaxOver,
            MinOver,
            XDown,
            MaxDown,
            MinDown,
            None
        }

        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto, Pack=4)]
        public class MONITORINFOEX
        {
            public int cbSize = Marshal.SizeOf(typeof(MaterialForm.MONITORINFOEX));
            public MaterialForm.RECT rcMonitor = new MaterialForm.RECT();
            public MaterialForm.RECT rcWork = new MaterialForm.RECT();
            public int dwFlags = 0;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=0x20)]
            public char[] szDevice = new char[0x20];
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
            public int Width() => 
                (this.right - this.left);

            public int Height() => 
                (this.bottom - this.top);
        }

        private enum ResizeDirection
        {
            BottomLeft,
            Left,
            Right,
            BottomRight,
            Bottom,
            None
        }
    }
}

