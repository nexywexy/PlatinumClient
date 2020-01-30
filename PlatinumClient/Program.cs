namespace PlatinumClient
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Windows.Forms;

    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                Thread.Sleep(0x3e8);
                File.Delete(string.Join(" ", args));
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            new PrepareDialog().ShowDialog();
            if (!KeyManager.hasPrivateKey())
            {
                new EULADialog().ShowDialog();
            }
            else
            {
                Application.Run(new AuthorizeDialog());
            }
        }
    }
}

