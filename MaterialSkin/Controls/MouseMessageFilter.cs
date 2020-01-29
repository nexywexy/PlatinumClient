namespace MaterialSkin.Controls
{
    using System;
    using System.Threading;
    using System.Windows.Forms;

    public class MouseMessageFilter : IMessageFilter
    {
        private const int WM_MOUSEMOVE = 0x200;

        public static  event MouseEventHandler MouseMove;

        public bool PreFilterMessage(ref Message m)
        {
            if ((m.Msg == 0x200) && (MouseMove != null))
            {
                int x = Control.MousePosition.X;
                int y = Control.MousePosition.Y;
                MouseMove(null, new MouseEventArgs(MouseButtons.None, 0, x, y, 0));
            }
            return false;
        }
    }
}

