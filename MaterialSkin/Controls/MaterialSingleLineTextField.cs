namespace MaterialSkin.Controls
{
    using MaterialSkin;
    using MaterialSkin.Animations;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class MaterialSingleLineTextField : Control, IMaterialControl
    {
        private readonly AnimationManager animationManager;
        private readonly BaseTextBox baseTextBox;

        public event EventHandler AcceptsTabChanged
        {
            add
            {
                this.baseTextBox.AcceptsTabChanged += value;
            }
            remove
            {
                this.baseTextBox.AcceptsTabChanged -= value;
            }
        }

        public event EventHandler AutoSizeChanged
        {
            add
            {
                this.baseTextBox.AutoSizeChanged += value;
            }
            remove
            {
                this.baseTextBox.AutoSizeChanged -= value;
            }
        }

        public event EventHandler BackgroundImageChanged
        {
            add
            {
                this.baseTextBox.BackgroundImageChanged += value;
            }
            remove
            {
                this.baseTextBox.BackgroundImageChanged -= value;
            }
        }

        public event EventHandler BackgroundImageLayoutChanged
        {
            add
            {
                this.baseTextBox.BackgroundImageLayoutChanged += value;
            }
            remove
            {
                this.baseTextBox.BackgroundImageLayoutChanged -= value;
            }
        }

        public event EventHandler BindingContextChanged
        {
            add
            {
                this.baseTextBox.BindingContextChanged += value;
            }
            remove
            {
                this.baseTextBox.BindingContextChanged -= value;
            }
        }

        public event EventHandler BorderStyleChanged
        {
            add
            {
                this.baseTextBox.BorderStyleChanged += value;
            }
            remove
            {
                this.baseTextBox.BorderStyleChanged -= value;
            }
        }

        public event EventHandler CausesValidationChanged
        {
            add
            {
                this.baseTextBox.CausesValidationChanged += value;
            }
            remove
            {
                this.baseTextBox.CausesValidationChanged -= value;
            }
        }

        public event UICuesEventHandler ChangeUICues
        {
            add
            {
                this.baseTextBox.ChangeUICues += value;
            }
            remove
            {
                this.baseTextBox.ChangeUICues -= value;
            }
        }

        public event EventHandler Click
        {
            add
            {
                this.baseTextBox.Click += value;
            }
            remove
            {
                this.baseTextBox.Click -= value;
            }
        }

        public event EventHandler ClientSizeChanged
        {
            add
            {
                this.baseTextBox.ClientSizeChanged += value;
            }
            remove
            {
                this.baseTextBox.ClientSizeChanged -= value;
            }
        }

        public event EventHandler ContextMenuChanged
        {
            add
            {
                this.baseTextBox.ContextMenuChanged += value;
            }
            remove
            {
                this.baseTextBox.ContextMenuChanged -= value;
            }
        }

        public event EventHandler ContextMenuStripChanged
        {
            add
            {
                this.baseTextBox.ContextMenuStripChanged += value;
            }
            remove
            {
                this.baseTextBox.ContextMenuStripChanged -= value;
            }
        }

        public event ControlEventHandler ControlAdded
        {
            add
            {
                this.baseTextBox.ControlAdded += value;
            }
            remove
            {
                this.baseTextBox.ControlAdded -= value;
            }
        }

        public event ControlEventHandler ControlRemoved
        {
            add
            {
                this.baseTextBox.ControlRemoved += value;
            }
            remove
            {
                this.baseTextBox.ControlRemoved -= value;
            }
        }

        public event EventHandler CursorChanged
        {
            add
            {
                this.baseTextBox.CursorChanged += value;
            }
            remove
            {
                this.baseTextBox.CursorChanged -= value;
            }
        }

        public event EventHandler Disposed
        {
            add
            {
                this.baseTextBox.Disposed += value;
            }
            remove
            {
                this.baseTextBox.Disposed -= value;
            }
        }

        public event EventHandler DockChanged
        {
            add
            {
                this.baseTextBox.DockChanged += value;
            }
            remove
            {
                this.baseTextBox.DockChanged -= value;
            }
        }

        public event EventHandler DoubleClick
        {
            add
            {
                this.baseTextBox.DoubleClick += value;
            }
            remove
            {
                this.baseTextBox.DoubleClick -= value;
            }
        }

        public event DragEventHandler DragDrop
        {
            add
            {
                this.baseTextBox.DragDrop += value;
            }
            remove
            {
                this.baseTextBox.DragDrop -= value;
            }
        }

        public event DragEventHandler DragEnter
        {
            add
            {
                this.baseTextBox.DragEnter += value;
            }
            remove
            {
                this.baseTextBox.DragEnter -= value;
            }
        }

        public event EventHandler DragLeave
        {
            add
            {
                this.baseTextBox.DragLeave += value;
            }
            remove
            {
                this.baseTextBox.DragLeave -= value;
            }
        }

        public event DragEventHandler DragOver
        {
            add
            {
                this.baseTextBox.DragOver += value;
            }
            remove
            {
                this.baseTextBox.DragOver -= value;
            }
        }

        public event EventHandler EnabledChanged
        {
            add
            {
                this.baseTextBox.EnabledChanged += value;
            }
            remove
            {
                this.baseTextBox.EnabledChanged -= value;
            }
        }

        public event EventHandler Enter
        {
            add
            {
                this.baseTextBox.Enter += value;
            }
            remove
            {
                this.baseTextBox.Enter -= value;
            }
        }

        public event EventHandler FontChanged
        {
            add
            {
                this.baseTextBox.FontChanged += value;
            }
            remove
            {
                this.baseTextBox.FontChanged -= value;
            }
        }

        public event EventHandler ForeColorChanged
        {
            add
            {
                this.baseTextBox.ForeColorChanged += value;
            }
            remove
            {
                this.baseTextBox.ForeColorChanged -= value;
            }
        }

        public event GiveFeedbackEventHandler GiveFeedback
        {
            add
            {
                this.baseTextBox.GiveFeedback += value;
            }
            remove
            {
                this.baseTextBox.GiveFeedback -= value;
            }
        }

        public event EventHandler GotFocus
        {
            add
            {
                this.baseTextBox.GotFocus += value;
            }
            remove
            {
                this.baseTextBox.GotFocus -= value;
            }
        }

        public event EventHandler HandleCreated
        {
            add
            {
                this.baseTextBox.HandleCreated += value;
            }
            remove
            {
                this.baseTextBox.HandleCreated -= value;
            }
        }

        public event EventHandler HandleDestroyed
        {
            add
            {
                this.baseTextBox.HandleDestroyed += value;
            }
            remove
            {
                this.baseTextBox.HandleDestroyed -= value;
            }
        }

        public event HelpEventHandler HelpRequested
        {
            add
            {
                this.baseTextBox.HelpRequested += value;
            }
            remove
            {
                this.baseTextBox.HelpRequested -= value;
            }
        }

        public event EventHandler HideSelectionChanged
        {
            add
            {
                this.baseTextBox.HideSelectionChanged += value;
            }
            remove
            {
                this.baseTextBox.HideSelectionChanged -= value;
            }
        }

        public event EventHandler ImeModeChanged
        {
            add
            {
                this.baseTextBox.ImeModeChanged += value;
            }
            remove
            {
                this.baseTextBox.ImeModeChanged -= value;
            }
        }

        public event InvalidateEventHandler Invalidated
        {
            add
            {
                this.baseTextBox.Invalidated += value;
            }
            remove
            {
                this.baseTextBox.Invalidated -= value;
            }
        }

        public event KeyEventHandler KeyDown
        {
            add
            {
                this.baseTextBox.KeyDown += value;
            }
            remove
            {
                this.baseTextBox.KeyDown -= value;
            }
        }

        public event KeyPressEventHandler KeyPress
        {
            add
            {
                this.baseTextBox.KeyPress += value;
            }
            remove
            {
                this.baseTextBox.KeyPress -= value;
            }
        }

        public event KeyEventHandler KeyUp
        {
            add
            {
                this.baseTextBox.KeyUp += value;
            }
            remove
            {
                this.baseTextBox.KeyUp -= value;
            }
        }

        public event LayoutEventHandler Layout
        {
            add
            {
                this.baseTextBox.Layout += value;
            }
            remove
            {
                this.baseTextBox.Layout -= value;
            }
        }

        public event EventHandler Leave
        {
            add
            {
                this.baseTextBox.Leave += value;
            }
            remove
            {
                this.baseTextBox.Leave -= value;
            }
        }

        public event EventHandler LocationChanged
        {
            add
            {
                this.baseTextBox.LocationChanged += value;
            }
            remove
            {
                this.baseTextBox.LocationChanged -= value;
            }
        }

        public event EventHandler LostFocus
        {
            add
            {
                this.baseTextBox.LostFocus += value;
            }
            remove
            {
                this.baseTextBox.LostFocus -= value;
            }
        }

        public event EventHandler MarginChanged
        {
            add
            {
                this.baseTextBox.MarginChanged += value;
            }
            remove
            {
                this.baseTextBox.MarginChanged -= value;
            }
        }

        public event EventHandler ModifiedChanged
        {
            add
            {
                this.baseTextBox.ModifiedChanged += value;
            }
            remove
            {
                this.baseTextBox.ModifiedChanged -= value;
            }
        }

        public event EventHandler MouseCaptureChanged
        {
            add
            {
                this.baseTextBox.MouseCaptureChanged += value;
            }
            remove
            {
                this.baseTextBox.MouseCaptureChanged -= value;
            }
        }

        public event MouseEventHandler MouseClick
        {
            add
            {
                this.baseTextBox.MouseClick += value;
            }
            remove
            {
                this.baseTextBox.MouseClick -= value;
            }
        }

        public event MouseEventHandler MouseDoubleClick
        {
            add
            {
                this.baseTextBox.MouseDoubleClick += value;
            }
            remove
            {
                this.baseTextBox.MouseDoubleClick -= value;
            }
        }

        public event MouseEventHandler MouseDown
        {
            add
            {
                this.baseTextBox.MouseDown += value;
            }
            remove
            {
                this.baseTextBox.MouseDown -= value;
            }
        }

        public event EventHandler MouseEnter
        {
            add
            {
                this.baseTextBox.MouseEnter += value;
            }
            remove
            {
                this.baseTextBox.MouseEnter -= value;
            }
        }

        public event EventHandler MouseHover
        {
            add
            {
                this.baseTextBox.MouseHover += value;
            }
            remove
            {
                this.baseTextBox.MouseHover -= value;
            }
        }

        public event EventHandler MouseLeave
        {
            add
            {
                this.baseTextBox.MouseLeave += value;
            }
            remove
            {
                this.baseTextBox.MouseLeave -= value;
            }
        }

        public event MouseEventHandler MouseMove
        {
            add
            {
                this.baseTextBox.MouseMove += value;
            }
            remove
            {
                this.baseTextBox.MouseMove -= value;
            }
        }

        public event MouseEventHandler MouseUp
        {
            add
            {
                this.baseTextBox.MouseUp += value;
            }
            remove
            {
                this.baseTextBox.MouseUp -= value;
            }
        }

        public event MouseEventHandler MouseWheel
        {
            add
            {
                this.baseTextBox.MouseWheel += value;
            }
            remove
            {
                this.baseTextBox.MouseWheel -= value;
            }
        }

        public event EventHandler Move
        {
            add
            {
                this.baseTextBox.Move += value;
            }
            remove
            {
                this.baseTextBox.Move -= value;
            }
        }

        public event EventHandler MultilineChanged
        {
            add
            {
                this.baseTextBox.MultilineChanged += value;
            }
            remove
            {
                this.baseTextBox.MultilineChanged -= value;
            }
        }

        public event EventHandler PaddingChanged
        {
            add
            {
                this.baseTextBox.PaddingChanged += value;
            }
            remove
            {
                this.baseTextBox.PaddingChanged -= value;
            }
        }

        public event PaintEventHandler Paint
        {
            add
            {
                this.baseTextBox.Paint += value;
            }
            remove
            {
                this.baseTextBox.Paint -= value;
            }
        }

        public event EventHandler ParentChanged
        {
            add
            {
                this.baseTextBox.ParentChanged += value;
            }
            remove
            {
                this.baseTextBox.ParentChanged -= value;
            }
        }

        public event PreviewKeyDownEventHandler PreviewKeyDown
        {
            add
            {
                this.baseTextBox.PreviewKeyDown += value;
            }
            remove
            {
                this.baseTextBox.PreviewKeyDown -= value;
            }
        }

        public event QueryAccessibilityHelpEventHandler QueryAccessibilityHelp
        {
            add
            {
                this.baseTextBox.QueryAccessibilityHelp += value;
            }
            remove
            {
                this.baseTextBox.QueryAccessibilityHelp -= value;
            }
        }

        public event QueryContinueDragEventHandler QueryContinueDrag
        {
            add
            {
                this.baseTextBox.QueryContinueDrag += value;
            }
            remove
            {
                this.baseTextBox.QueryContinueDrag -= value;
            }
        }

        public event EventHandler ReadOnlyChanged
        {
            add
            {
                this.baseTextBox.ReadOnlyChanged += value;
            }
            remove
            {
                this.baseTextBox.ReadOnlyChanged -= value;
            }
        }

        public event EventHandler RegionChanged
        {
            add
            {
                this.baseTextBox.RegionChanged += value;
            }
            remove
            {
                this.baseTextBox.RegionChanged -= value;
            }
        }

        public event EventHandler Resize
        {
            add
            {
                this.baseTextBox.Resize += value;
            }
            remove
            {
                this.baseTextBox.Resize -= value;
            }
        }

        public event EventHandler RightToLeftChanged
        {
            add
            {
                this.baseTextBox.RightToLeftChanged += value;
            }
            remove
            {
                this.baseTextBox.RightToLeftChanged -= value;
            }
        }

        public event EventHandler SizeChanged
        {
            add
            {
                this.baseTextBox.SizeChanged += value;
            }
            remove
            {
                this.baseTextBox.SizeChanged -= value;
            }
        }

        public event EventHandler StyleChanged
        {
            add
            {
                this.baseTextBox.StyleChanged += value;
            }
            remove
            {
                this.baseTextBox.StyleChanged -= value;
            }
        }

        public event EventHandler SystemColorsChanged
        {
            add
            {
                this.baseTextBox.SystemColorsChanged += value;
            }
            remove
            {
                this.baseTextBox.SystemColorsChanged -= value;
            }
        }

        public event EventHandler TabIndexChanged
        {
            add
            {
                this.baseTextBox.TabIndexChanged += value;
            }
            remove
            {
                this.baseTextBox.TabIndexChanged -= value;
            }
        }

        public event EventHandler TabStopChanged
        {
            add
            {
                this.baseTextBox.TabStopChanged += value;
            }
            remove
            {
                this.baseTextBox.TabStopChanged -= value;
            }
        }

        public event EventHandler TextAlignChanged
        {
            add
            {
                this.baseTextBox.TextAlignChanged += value;
            }
            remove
            {
                this.baseTextBox.TextAlignChanged -= value;
            }
        }

        public event EventHandler TextChanged
        {
            add
            {
                this.baseTextBox.TextChanged += value;
            }
            remove
            {
                this.baseTextBox.TextChanged -= value;
            }
        }

        public event EventHandler Validated
        {
            add
            {
                this.baseTextBox.Validated += value;
            }
            remove
            {
                this.baseTextBox.Validated -= value;
            }
        }

        public event CancelEventHandler Validating
        {
            add
            {
                this.baseTextBox.Validating += value;
            }
            remove
            {
                this.baseTextBox.Validating -= value;
            }
        }

        public event EventHandler VisibleChanged
        {
            add
            {
                this.baseTextBox.VisibleChanged += value;
            }
            remove
            {
                this.baseTextBox.VisibleChanged -= value;
            }
        }

        public MaterialSingleLineTextField()
        {
            AnimationManager.AnimationProgress progress = null;
            EventHandler handler = null;
            EventHandler handler2 = null;
            EventHandler handler3 = null;
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.DoubleBuffer, true);
            AnimationManager manager = new AnimationManager(true) {
                Increment = 0.06,
                AnimationType = AnimationType.EaseInOut,
                InterruptAnimation = false
            };
            this.animationManager = manager;
            if (progress == null)
            {
                progress = sender => base.Invalidate();
            }
            this.animationManager.OnAnimationProgress += progress;
            BaseTextBox box = new BaseTextBox {
                BorderStyle = BorderStyle.None,
                Font = this.SkinManager.ROBOTO_REGULAR_11,
                ForeColor = this.SkinManager.GetMainTextColor(),
                Location = new Point(0, 0),
                Width = base.Width,
                Height = base.Height - 5
            };
            this.baseTextBox = box;
            if (!(base.Controls.Contains(this.baseTextBox) || base.DesignMode))
            {
                base.Controls.Add(this.baseTextBox);
            }
            if (handler == null)
            {
                handler = (sender, args) => this.animationManager.StartNewAnimation(AnimationDirection.In, null);
            }
            this.baseTextBox.GotFocus += handler;
            if (handler2 == null)
            {
                handler2 = (sender, args) => this.animationManager.StartNewAnimation(AnimationDirection.Out, null);
            }
            this.baseTextBox.LostFocus += handler2;
            if (handler3 == null)
            {
                handler3 = delegate (object sender, EventArgs args) {
                    this.baseTextBox.BackColor = this.BackColor;
                    this.baseTextBox.ForeColor = this.SkinManager.GetMainTextColor();
                };
            }
            base.BackColorChanged += handler3;
        }

        public void Clear()
        {
            this.baseTextBox.Clear();
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            this.baseTextBox.BackColor = base.Parent.BackColor;
            this.baseTextBox.ForeColor = this.SkinManager.GetMainTextColor();
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics graphics = pevent.Graphics;
            graphics.Clear(base.Parent.BackColor);
            int y = this.baseTextBox.Bottom + 3;
            if (!this.animationManager.IsAnimating())
            {
                graphics.FillRectangle(this.baseTextBox.Focused ? this.SkinManager.ColorScheme.PrimaryBrush : this.SkinManager.GetDividersBrush(), this.baseTextBox.Location.X, y, this.baseTextBox.Width, this.baseTextBox.Focused ? 2 : 1);
            }
            else
            {
                int width = (int) (this.baseTextBox.Width * this.animationManager.GetProgress());
                int num3 = width / 2;
                int num4 = this.baseTextBox.Location.X + (this.baseTextBox.Width / 2);
                graphics.FillRectangle(this.SkinManager.GetDividersBrush(), this.baseTextBox.Location.X, y, this.baseTextBox.Width, 1);
                graphics.FillRectangle(this.SkinManager.ColorScheme.PrimaryBrush, num4 - num3, y, width, 2);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.baseTextBox.Location = new Point(0, 0);
            this.baseTextBox.Width = base.Width;
            base.Height = this.baseTextBox.Height + 5;
        }

        public void SelectAll()
        {
            this.baseTextBox.SelectAll();
        }

        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager =>
            MaterialSkinManager.Instance;

        [Browsable(false)]
        public MaterialSkin.MouseState MouseState { get; set; }

        public override string Text
        {
            get => 
                this.baseTextBox.Text;
            set => 
                (this.baseTextBox.Text = value);
        }

        public object Tag
        {
            get => 
                this.baseTextBox.Tag;
            set => 
                (this.baseTextBox.Tag = value);
        }

        public string SelectedText
        {
            get => 
                this.baseTextBox.SelectedText;
            set => 
                (this.baseTextBox.SelectedText = value);
        }

        public string Hint
        {
            get => 
                this.baseTextBox.Hint;
            set => 
                (this.baseTextBox.Hint = value);
        }

        public int SelectionStart
        {
            get => 
                this.baseTextBox.SelectionStart;
            set => 
                (this.baseTextBox.SelectionStart = value);
        }

        public int SelectionLength
        {
            get => 
                this.baseTextBox.SelectionLength;
            set => 
                (this.baseTextBox.SelectionLength = value);
        }

        public int TextLength =>
            this.baseTextBox.TextLength;

        public bool UseSystemPasswordChar
        {
            get => 
                this.baseTextBox.UseSystemPasswordChar;
            set => 
                (this.baseTextBox.UseSystemPasswordChar = value);
        }

        public char PasswordChar
        {
            get => 
                this.baseTextBox.PasswordChar;
            set => 
                (this.baseTextBox.PasswordChar = value);
        }

        private class BaseTextBox : TextBox
        {
            private const int EM_SETCUEBANNER = 0x1501;
            private const char EmptyChar = '\0';
            private const char VisualStylePasswordChar = '●';
            private const char NonVisualStylePasswordChar = '*';
            private string hint = string.Empty;
            private char passwordChar = '\0';
            private char useSystemPasswordChar = '\0';

            public BaseTextBox()
            {
                MaterialContextMenuStrip strip = new MaterialSingleLineTextField.TextBoxContextMenuStrip();
                strip.Opening += new CancelEventHandler(this.ContextMenuStripOnOpening);
                strip.OnItemClickStart += new MaterialContextMenuStrip.ItemClickStart(this.ContextMenuStripOnItemClickStart);
                this.ContextMenuStrip = strip;
            }

            private void ContextMenuStripOnItemClickStart(object sender, ToolStripItemClickedEventArgs toolStripItemClickedEventArgs)
            {
                switch (toolStripItemClickedEventArgs.ClickedItem.Text)
                {
                    case "Undo":
                        base.Undo();
                        break;

                    case "Cut":
                        base.Cut();
                        break;

                    case "Copy":
                        base.Copy();
                        break;

                    case "Paste":
                        base.Paste();
                        break;

                    case "Delete":
                        this.SelectedText = string.Empty;
                        break;

                    case "Select All":
                        this.SelectAll();
                        break;
                }
            }

            private void ContextMenuStripOnOpening(object sender, CancelEventArgs cancelEventArgs)
            {
                MaterialSingleLineTextField.TextBoxContextMenuStrip strip = sender as MaterialSingleLineTextField.TextBoxContextMenuStrip;
                if (strip != null)
                {
                    strip.undo.Enabled = base.CanUndo;
                    strip.cut.Enabled = !string.IsNullOrEmpty(this.SelectedText);
                    strip.copy.Enabled = !string.IsNullOrEmpty(this.SelectedText);
                    strip.paste.Enabled = Clipboard.ContainsText();
                    strip.delete.Enabled = !string.IsNullOrEmpty(this.SelectedText);
                    strip.selectAll.Enabled = !string.IsNullOrEmpty(this.Text);
                }
            }

            public void SelectAll()
            {
                base.BeginInvoke(delegate {
                    base.Focus();
                    base.SelectAll();
                });
            }

            [DllImport("user32.dll", CharSet=CharSet.Auto)]
            private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);
            private void SetBasePasswordChar()
            {
                base.PasswordChar = this.UseSystemPasswordChar ? this.useSystemPasswordChar : this.passwordChar;
            }

            public string Hint
            {
                get => 
                    this.hint;
                set
                {
                    this.hint = value;
                    SendMessage(base.Handle, 0x1501, (int) IntPtr.Zero, this.Hint);
                }
            }

            public char PasswordChar
            {
                get => 
                    this.passwordChar;
                set
                {
                    this.passwordChar = value;
                    this.SetBasePasswordChar();
                }
            }

            public bool UseSystemPasswordChar
            {
                get => 
                    (this.useSystemPasswordChar != '\0');
                set
                {
                    if (value)
                    {
                        this.useSystemPasswordChar = Application.RenderWithVisualStyles ? '●' : '*';
                    }
                    else
                    {
                        this.useSystemPasswordChar = '\0';
                    }
                    this.SetBasePasswordChar();
                }
            }
        }

        private class TextBoxContextMenuStrip : MaterialContextMenuStrip
        {
            public readonly ToolStripItem undo;
            public readonly ToolStripItem seperator1;
            public readonly ToolStripItem cut;
            public readonly ToolStripItem copy;
            public readonly ToolStripItem paste;
            public readonly ToolStripItem delete;
            public readonly ToolStripItem seperator2;
            public readonly ToolStripItem selectAll;

            public TextBoxContextMenuStrip()
            {
                MaterialToolStripMenuItem item = new MaterialToolStripMenuItem {
                    Text = "Undo"
                };
                this.undo = item;
                this.seperator1 = new ToolStripSeparator();
                MaterialToolStripMenuItem item2 = new MaterialToolStripMenuItem {
                    Text = "Cut"
                };
                this.cut = item2;
                MaterialToolStripMenuItem item3 = new MaterialToolStripMenuItem {
                    Text = "Copy"
                };
                this.copy = item3;
                MaterialToolStripMenuItem item4 = new MaterialToolStripMenuItem {
                    Text = "Paste"
                };
                this.paste = item4;
                MaterialToolStripMenuItem item5 = new MaterialToolStripMenuItem {
                    Text = "Delete"
                };
                this.delete = item5;
                this.seperator2 = new ToolStripSeparator();
                MaterialToolStripMenuItem item6 = new MaterialToolStripMenuItem {
                    Text = "Select All"
                };
                this.selectAll = item6;
                this.Items.AddRange(new ToolStripItem[] { this.undo, this.seperator1, this.cut, this.copy, this.paste, this.delete, this.seperator2, this.selectAll });
            }
        }
    }
}

