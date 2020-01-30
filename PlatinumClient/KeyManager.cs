namespace PlatinumClient
{
    using System;
    using System.IO;

    internal class KeyManager
    {
        public static string PRIVATE_KEY_FILE_NAME = "platinumkey.dat";
        public static string PRIVATE_KEY_PATH = (Directory.GetCurrentDirectory() + @"\" + PRIVATE_KEY_FILE_NAME);

        public static void deauthorize()
        {
            File.Delete(PRIVATE_KEY_PATH);
        }

        public static string getEncodedPrivateKey() => 
            Convert.ToBase64String(getPrivateKey());

        public static byte[] getPrivateKey()
        {
            if (hasPrivateKey())
            {
                return File.ReadAllBytes(PRIVATE_KEY_PATH);
            }
            return null;
        }

        public static bool hasPrivateKey() => 
            File.Exists(PRIVATE_KEY_PATH);

        public static void setEncodedPrivateKey(string newKey)
        {
            setPrivateKey(Convert.FromBase64String(newKey));
        }

        public static void setPrivateKey(byte[] newKey)
        {
            hasPrivateKey();
            File.WriteAllBytes(PRIVATE_KEY_PATH, newKey);
        }
    }
}

