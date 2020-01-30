namespace PlatinumClient
{
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;

    internal abstract class WERProvider : TelemetryProvider
    {
        public const string PARENT_PATH = @"SOFTWARE\Microsoft\Windows\Windows Error Reporting";
        public const string REGISTRY_PATH = @"SOFTWARE\Microsoft\Windows\Windows Error Reporting\LocalDumps";
        public static string DUMP_PATH = (Directory.GetCurrentDirectory() + @"\WERDump\");

        protected WERProvider()
        {
        }

        public List<byte[]> collect()
        {
            List<byte[]> list = new List<byte[]>();
            foreach (string str in Directory.EnumerateFiles(DUMP_PATH))
            {
                if (str.Contains(this.processName()) && str.EndsWith(".dmp"))
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        using (GZipStream stream2 = new GZipStream(stream, CompressionMode.Compress))
                        {
                            using (FileStream stream3 = File.OpenRead(str))
                            {
                                stream3.CopyTo(stream2);
                            }
                        }
                        list.Add(stream.ToArray());
                    }
                    File.Delete(str);
                }
            }
            return list;
        }

        public TelemetryContentType contentType() => 
            TelemetryContentType.BINARY;

        public void prepare(bool status)
        {
            using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Windows\Windows Error Reporting", true))
            {
                if (key.OpenSubKey("LocalDumps") == null)
                {
                    key.CreateSubKey("LocalDumps");
                }
            }
            using (RegistryKey key2 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Windows\Windows Error Reporting\LocalDumps", true))
            {
                if (status)
                {
                    if (!Directory.Exists(DUMP_PATH))
                    {
                        Directory.CreateDirectory(DUMP_PATH);
                    }
                    RegistryKey key1 = key2.CreateSubKey(this.processName());
                    key1.SetValue("DumpCount", 10, RegistryValueKind.DWord);
                    key1.SetValue("DumpType", 0, RegistryValueKind.DWord);
                    key1.SetValue("DumpFolder", DUMP_PATH, RegistryValueKind.String);
                    key1.SetValue("CustomDumpFlags", 80, RegistryValueKind.DWord);
                    key1.Close();
                }
                else
                {
                    key2.DeleteSubKeyTree(this.processName());
                }
            }
            using (RegistryKey key3 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options", true))
            {
                if (status)
                {
                    RegistryKey key4 = key3.CreateSubKey(this.processName());
                    key4.SetValue("GlobalFlag", 0x30, RegistryValueKind.DWord);
                    key4.Close();
                }
                else
                {
                    key3.DeleteSubKeyTree(this.processName());
                }
            }
        }

        public abstract string processName();
        public bool status()
        {
            using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Windows\Windows Error Reporting\LocalDumps", true))
            {
                if (key == null)
                {
                    return false;
                }
                if (key.OpenSubKey(this.processName(), false) != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

