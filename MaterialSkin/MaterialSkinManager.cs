namespace MaterialSkin
{
    using MaterialSkin.Controls;
    using MaterialSkin.Properties;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Text;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class MaterialSkinManager
    {
        private static MaterialSkinManager instance;
        private readonly List<MaterialForm> formsToManage = new List<MaterialForm>();
        private Themes theme;
        private MaterialSkin.ColorScheme colorScheme;
        private static readonly Color MAIN_TEXT_BLACK = Color.FromArgb(0xde, 0, 0, 0);
        private static readonly Brush MAIN_TEXT_BLACK_BRUSH = new SolidBrush(MAIN_TEXT_BLACK);
        public static Color SECONDARY_TEXT_BLACK = Color.FromArgb(0x8a, 0, 0, 0);
        public static Brush SECONDARY_TEXT_BLACK_BRUSH = new SolidBrush(SECONDARY_TEXT_BLACK);
        private static readonly Color DISABLED_OR_HINT_TEXT_BLACK = Color.FromArgb(0x42, 0, 0, 0);
        private static readonly Brush DISABLED_OR_HINT_TEXT_BLACK_BRUSH = new SolidBrush(DISABLED_OR_HINT_TEXT_BLACK);
        private static readonly Color DIVIDERS_BLACK = Color.FromArgb(0x1f, 0, 0, 0);
        private static readonly Brush DIVIDERS_BLACK_BRUSH = new SolidBrush(DIVIDERS_BLACK);
        private static readonly Color MAIN_TEXT_WHITE = Color.FromArgb(0xff, 0xff, 0xff, 0xff);
        private static readonly Brush MAIN_TEXT_WHITE_BRUSH = new SolidBrush(MAIN_TEXT_WHITE);
        public static Color SECONDARY_TEXT_WHITE = Color.FromArgb(0xb3, 0xff, 0xff, 0xff);
        public static Brush SECONDARY_TEXT_WHITE_BRUSH = new SolidBrush(SECONDARY_TEXT_WHITE);
        private static readonly Color DISABLED_OR_HINT_TEXT_WHITE = Color.FromArgb(0x4d, 0xff, 0xff, 0xff);
        private static readonly Brush DISABLED_OR_HINT_TEXT_WHITE_BRUSH = new SolidBrush(DISABLED_OR_HINT_TEXT_WHITE);
        private static readonly Color DIVIDERS_WHITE = Color.FromArgb(0x1f, 0xff, 0xff, 0xff);
        private static readonly Brush DIVIDERS_WHITE_BRUSH = new SolidBrush(DIVIDERS_WHITE);
        private static readonly Color CHECKBOX_OFF_LIGHT = Color.FromArgb(0x8a, 0, 0, 0);
        private static readonly Brush CHECKBOX_OFF_LIGHT_BRUSH = new SolidBrush(CHECKBOX_OFF_LIGHT);
        private static readonly Color CHECKBOX_OFF_DISABLED_LIGHT = Color.FromArgb(0x42, 0, 0, 0);
        private static readonly Brush CHECKBOX_OFF_DISABLED_LIGHT_BRUSH = new SolidBrush(CHECKBOX_OFF_DISABLED_LIGHT);
        private static readonly Color CHECKBOX_OFF_DARK = Color.FromArgb(0xb3, 0xff, 0xff, 0xff);
        private static readonly Brush CHECKBOX_OFF_DARK_BRUSH = new SolidBrush(CHECKBOX_OFF_DARK);
        private static readonly Color CHECKBOX_OFF_DISABLED_DARK = Color.FromArgb(0x4d, 0xff, 0xff, 0xff);
        private static readonly Brush CHECKBOX_OFF_DISABLED_DARK_BRUSH = new SolidBrush(CHECKBOX_OFF_DISABLED_DARK);
        private static readonly Color RAISED_BUTTON_BACKGROUND = Color.FromArgb(0xff, 0xff, 0xff, 0xff);
        private static readonly Brush RAISED_BUTTON_BACKGROUND_BRUSH = new SolidBrush(RAISED_BUTTON_BACKGROUND);
        private static readonly Color RAISED_BUTTON_TEXT_LIGHT = MAIN_TEXT_WHITE;
        private static readonly Brush RAISED_BUTTON_TEXT_LIGHT_BRUSH = new SolidBrush(RAISED_BUTTON_TEXT_LIGHT);
        private static readonly Color RAISED_BUTTON_TEXT_DARK = MAIN_TEXT_BLACK;
        private static readonly Brush RAISED_BUTTON_TEXT_DARK_BRUSH = new SolidBrush(RAISED_BUTTON_TEXT_DARK);
        private static readonly Color FLAT_BUTTON_BACKGROUND_HOVER_LIGHT = Color.FromArgb(20.PercentageToColorComponent(), 0x999999.ToColor());
        private static readonly Brush FLAT_BUTTON_BACKGROUND_HOVER_LIGHT_BRUSH = new SolidBrush(FLAT_BUTTON_BACKGROUND_HOVER_LIGHT);
        private static readonly Color FLAT_BUTTON_BACKGROUND_PRESSED_LIGHT = Color.FromArgb(40.PercentageToColorComponent(), 0x999999.ToColor());
        private static readonly Brush FLAT_BUTTON_BACKGROUND_PRESSED_LIGHT_BRUSH = new SolidBrush(FLAT_BUTTON_BACKGROUND_PRESSED_LIGHT);
        private static readonly Color FLAT_BUTTON_DISABLEDTEXT_LIGHT = Color.FromArgb(0x1a.PercentageToColorComponent(), 0.ToColor());
        private static readonly Brush FLAT_BUTTON_DISABLEDTEXT_LIGHT_BRUSH = new SolidBrush(FLAT_BUTTON_DISABLEDTEXT_LIGHT);
        private static readonly Color FLAT_BUTTON_BACKGROUND_HOVER_DARK = Color.FromArgb(15.PercentageToColorComponent(), 0xcccccc.ToColor());
        private static readonly Brush FLAT_BUTTON_BACKGROUND_HOVER_DARK_BRUSH = new SolidBrush(FLAT_BUTTON_BACKGROUND_HOVER_DARK);
        private static readonly Color FLAT_BUTTON_BACKGROUND_PRESSED_DARK = Color.FromArgb(0x19.PercentageToColorComponent(), 0xcccccc.ToColor());
        private static readonly Brush FLAT_BUTTON_BACKGROUND_PRESSED_DARK_BRUSH = new SolidBrush(FLAT_BUTTON_BACKGROUND_PRESSED_DARK);
        private static readonly Color FLAT_BUTTON_DISABLEDTEXT_DARK = Color.FromArgb(30.PercentageToColorComponent(), 0xffffff.ToColor());
        private static readonly Brush FLAT_BUTTON_DISABLEDTEXT_DARK_BRUSH = new SolidBrush(FLAT_BUTTON_DISABLEDTEXT_DARK);
        private static readonly Color CMS_BACKGROUND_LIGHT_HOVER = Color.FromArgb(0xff, 0xee, 0xee, 0xee);
        private static readonly Brush CMS_BACKGROUND_HOVER_LIGHT_BRUSH = new SolidBrush(CMS_BACKGROUND_LIGHT_HOVER);
        private static readonly Color CMS_BACKGROUND_DARK_HOVER = Color.FromArgb(0x26, 0xcc, 0xcc, 0xcc);
        private static readonly Brush CMS_BACKGROUND_HOVER_DARK_BRUSH = new SolidBrush(CMS_BACKGROUND_DARK_HOVER);
        private static readonly Color BACKGROUND_LIGHT = Color.FromArgb(0xff, 0xff, 0xff, 0xff);
        private static Brush BACKGROUND_LIGHT_BRUSH = new SolidBrush(BACKGROUND_LIGHT);
        private static readonly Color BACKGROUND_DARK = Color.FromArgb(0xff, 0x33, 0x33, 0x33);
        private static Brush BACKGROUND_DARK_BRUSH = new SolidBrush(BACKGROUND_DARK);
        public readonly Color ACTION_BAR_TEXT = Color.FromArgb(0xff, 0xff, 0xff, 0xff);
        public readonly Brush ACTION_BAR_TEXT_BRUSH = new SolidBrush(Color.FromArgb(0xff, 0xff, 0xff, 0xff));
        public readonly Color ACTION_BAR_TEXT_SECONDARY = Color.FromArgb(0x99, 0xff, 0xff, 0xff);
        public readonly Brush ACTION_BAR_TEXT_SECONDARY_BRUSH = new SolidBrush(Color.FromArgb(0x99, 0xff, 0xff, 0xff));
        public Font ROBOTO_MEDIUM_12;
        public Font ROBOTO_REGULAR_11;
        public Font ROBOTO_MEDIUM_11;
        public Font ROBOTO_MEDIUM_10;
        public int FORM_PADDING = 14;
        private readonly PrivateFontCollection privateFontCollection = new PrivateFontCollection();

        private MaterialSkinManager()
        {
            this.ROBOTO_MEDIUM_12 = new Font(this.LoadFont(Resources.Roboto_Medium), 12f);
            this.ROBOTO_MEDIUM_10 = new Font(this.LoadFont(Resources.Roboto_Medium), 10f);
            this.ROBOTO_REGULAR_11 = new Font(this.LoadFont(Resources.Roboto_Regular), 11f);
            this.ROBOTO_MEDIUM_11 = new Font(this.LoadFont(Resources.Roboto_Medium), 11f);
            this.Theme = Themes.LIGHT;
            this.ColorScheme = new MaterialSkin.ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
        }

        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pvd, [In] ref uint pcFonts);
        public void AddFormToManage(MaterialForm materialForm)
        {
            this.formsToManage.Add(materialForm);
            this.UpdateBackgrounds();
        }

        public Color GetApplicationBackgroundColor() => 
            ((this.Theme == Themes.LIGHT) ? BACKGROUND_LIGHT : BACKGROUND_DARK);

        public Brush GetCheckboxOffBrush() => 
            ((this.Theme == Themes.LIGHT) ? CHECKBOX_OFF_LIGHT_BRUSH : CHECKBOX_OFF_DARK_BRUSH);

        public Color GetCheckboxOffColor() => 
            ((this.Theme == Themes.LIGHT) ? CHECKBOX_OFF_LIGHT : CHECKBOX_OFF_DARK);

        public Brush GetCheckBoxOffDisabledBrush() => 
            ((this.Theme == Themes.LIGHT) ? CHECKBOX_OFF_DISABLED_LIGHT_BRUSH : CHECKBOX_OFF_DISABLED_DARK_BRUSH);

        public Color GetCheckBoxOffDisabledColor() => 
            ((this.Theme == Themes.LIGHT) ? CHECKBOX_OFF_DISABLED_LIGHT : CHECKBOX_OFF_DISABLED_DARK);

        public Brush GetCmsSelectedItemBrush() => 
            ((this.Theme == Themes.LIGHT) ? CMS_BACKGROUND_HOVER_LIGHT_BRUSH : CMS_BACKGROUND_HOVER_DARK_BRUSH);

        public Brush GetDisabledOrHintBrush() => 
            ((this.Theme == Themes.LIGHT) ? DISABLED_OR_HINT_TEXT_BLACK_BRUSH : DISABLED_OR_HINT_TEXT_WHITE_BRUSH);

        public Color GetDisabledOrHintColor() => 
            ((this.Theme == Themes.LIGHT) ? DISABLED_OR_HINT_TEXT_BLACK : DISABLED_OR_HINT_TEXT_WHITE);

        public Brush GetDividersBrush() => 
            ((this.Theme == Themes.LIGHT) ? DIVIDERS_BLACK_BRUSH : DIVIDERS_WHITE_BRUSH);

        public Color GetDividersColor() => 
            ((this.Theme == Themes.LIGHT) ? DIVIDERS_BLACK : DIVIDERS_WHITE);

        public Brush GetFlatButtonDisabledTextBrush() => 
            ((this.Theme == Themes.LIGHT) ? FLAT_BUTTON_DISABLEDTEXT_LIGHT_BRUSH : FLAT_BUTTON_DISABLEDTEXT_DARK_BRUSH);

        public Brush GetFlatButtonHoverBackgroundBrush() => 
            ((this.Theme == Themes.LIGHT) ? FLAT_BUTTON_BACKGROUND_HOVER_LIGHT_BRUSH : FLAT_BUTTON_BACKGROUND_HOVER_DARK_BRUSH);

        public Color GetFlatButtonHoverBackgroundColor() => 
            ((this.Theme == Themes.LIGHT) ? FLAT_BUTTON_BACKGROUND_HOVER_LIGHT : FLAT_BUTTON_BACKGROUND_HOVER_DARK);

        public Brush GetFlatButtonPressedBackgroundBrush() => 
            ((this.Theme == Themes.LIGHT) ? FLAT_BUTTON_BACKGROUND_PRESSED_LIGHT_BRUSH : FLAT_BUTTON_BACKGROUND_PRESSED_DARK_BRUSH);

        public Color GetFlatButtonPressedBackgroundColor() => 
            ((this.Theme == Themes.LIGHT) ? FLAT_BUTTON_BACKGROUND_PRESSED_LIGHT : FLAT_BUTTON_BACKGROUND_PRESSED_DARK);

        public Brush GetMainTextBrush() => 
            ((this.Theme == Themes.LIGHT) ? MAIN_TEXT_BLACK_BRUSH : MAIN_TEXT_WHITE_BRUSH);

        public Color GetMainTextColor() => 
            ((this.Theme == Themes.LIGHT) ? MAIN_TEXT_BLACK : MAIN_TEXT_WHITE);

        public Brush GetRaisedButtonBackgroundBrush() => 
            RAISED_BUTTON_BACKGROUND_BRUSH;

        public Brush GetRaisedButtonTextBrush(bool primary) => 
            (primary ? RAISED_BUTTON_TEXT_LIGHT_BRUSH : RAISED_BUTTON_TEXT_DARK_BRUSH);

        private FontFamily LoadFont(byte[] fontResource)
        {
            int length = fontResource.Length;
            IntPtr destination = Marshal.AllocCoTaskMem(length);
            Marshal.Copy(fontResource, 0, destination, length);
            uint pcFonts = 0;
            AddFontMemResourceEx(destination, (uint) fontResource.Length, IntPtr.Zero, ref pcFonts);
            this.privateFontCollection.AddMemoryFont(destination, length);
            return this.privateFontCollection.Families.Last<FontFamily>();
        }

        public void RemoveFormToManage(MaterialForm materialForm)
        {
            this.formsToManage.Remove(materialForm);
        }

        private void UpdateBackgrounds()
        {
            Color applicationBackgroundColor = this.GetApplicationBackgroundColor();
            foreach (MaterialForm form in this.formsToManage)
            {
                form.BackColor = applicationBackgroundColor;
                this.UpdateControl(form, applicationBackgroundColor);
            }
        }

        private void UpdateControl(Control controlToUpdate, Color newBackColor)
        {
            if (controlToUpdate != null)
            {
                if (controlToUpdate.ContextMenuStrip != null)
                {
                    this.UpdateToolStrip(controlToUpdate.ContextMenuStrip, newBackColor);
                }
                MaterialTabControl control = controlToUpdate as MaterialTabControl;
                if (control != null)
                {
                    foreach (TabPage page in control.TabPages)
                    {
                        page.BackColor = newBackColor;
                    }
                }
                if (controlToUpdate is MaterialDivider)
                {
                    controlToUpdate.BackColor = this.GetDividersColor();
                }
                foreach (Control control2 in controlToUpdate.Controls)
                {
                    this.UpdateControl(control2, newBackColor);
                }
                controlToUpdate.Invalidate();
            }
        }

        private void UpdateToolStrip(ToolStrip toolStrip, Color newBackColor)
        {
            if (toolStrip != null)
            {
                toolStrip.BackColor = newBackColor;
                foreach (ToolStripItem item in toolStrip.Items)
                {
                    item.BackColor = newBackColor;
                    if ((item is MaterialToolStripMenuItem) && (item as MaterialToolStripMenuItem).HasDropDown)
                    {
                        this.UpdateToolStrip((item as MaterialToolStripMenuItem).DropDown, newBackColor);
                    }
                }
            }
        }

        public Themes Theme
        {
            get => 
                this.theme;
            set
            {
                this.theme = value;
                this.UpdateBackgrounds();
            }
        }

        public MaterialSkin.ColorScheme ColorScheme
        {
            get => 
                this.colorScheme;
            set
            {
                this.colorScheme = value;
                this.UpdateBackgrounds();
            }
        }

        public static MaterialSkinManager Instance
        {
            get
            {
                MaterialSkinManager instance;
                if (MaterialSkinManager.instance != null)
                {
                    instance = MaterialSkinManager.instance;
                }
                else
                {
                    instance = MaterialSkinManager.instance = new MaterialSkinManager();
                }
                return instance;
            }
        }

        public enum Themes : byte
        {
            LIGHT = 0,
            DARK = 1
        }
    }
}

