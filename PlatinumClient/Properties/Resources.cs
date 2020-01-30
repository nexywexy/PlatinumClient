namespace PlatinumClient.Properties
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Resources;
    using System.Runtime.CompilerServices;

    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"), DebuggerNonUserCode, CompilerGenerated]
    internal class Resources
    {
        private static System.Resources.ResourceManager resourceMan;
        private static CultureInfo resourceCulture;

        internal Resources()
        {
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (resourceMan == null)
                {
                    resourceMan = new System.Resources.ResourceManager("PlatinumClient.Properties.Resources", typeof(Resources).Assembly);
                }
                return resourceMan;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture
        {
            get => 
                resourceCulture;
            set => 
                (resourceCulture = value);
        }

        internal static Bitmap ajax_loader =>
            ((Bitmap) ResourceManager.GetObject("ajax_loader", resourceCulture));

        internal static Bitmap chevron_arrow_down =>
            ((Bitmap) ResourceManager.GetObject("chevron_arrow_down", resourceCulture));

        internal static Bitmap cloud_dl =>
            ((Bitmap) ResourceManager.GetObject("cloud_dl", resourceCulture));

        internal static Bitmap cloud_security =>
            ((Bitmap) ResourceManager.GetObject("cloud_security", resourceCulture));

        internal static Bitmap coffee =>
            ((Bitmap) ResourceManager.GetObject("coffee", resourceCulture));

        internal static Bitmap desktop_computer_locked_screen =>
            ((Bitmap) ResourceManager.GetObject("desktop_computer_locked_screen", resourceCulture));

        internal static Bitmap handyman_tools =>
            ((Bitmap) ResourceManager.GetObject("handyman_tools", resourceCulture));

        internal static Bitmap info_icon =>
            ((Bitmap) ResourceManager.GetObject("info_icon", resourceCulture));

        internal static Bitmap memory_stick =>
            ((Bitmap) ResourceManager.GetObject("memory_stick", resourceCulture));

        internal static Bitmap platinumcheats_wide_compressed_white =>
            ((Bitmap) ResourceManager.GetObject("platinumcheats_wide_compressed_white", resourceCulture));

        internal static Bitmap screwdriver_and_wrench_crossed =>
            ((Bitmap) ResourceManager.GetObject("screwdriver_and_wrench_crossed", resourceCulture));

        internal static Bitmap settings_work_tool =>
            ((Bitmap) ResourceManager.GetObject("settings_work_tool", resourceCulture));

        internal static Bitmap shield =>
            ((Bitmap) ResourceManager.GetObject("shield", resourceCulture));

        internal static Bitmap success =>
            ((Bitmap) ResourceManager.GetObject("success", resourceCulture));

        internal static Bitmap tick_inside_circle =>
            ((Bitmap) ResourceManager.GetObject("tick_inside_circle", resourceCulture));

        internal static Bitmap vintage_hourglass =>
            ((Bitmap) ResourceManager.GetObject("vintage_hourglass", resourceCulture));

        internal static Bitmap warning_icon =>
            ((Bitmap) ResourceManager.GetObject("warning_icon", resourceCulture));
    }
}

