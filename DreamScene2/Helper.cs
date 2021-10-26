using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace DreamScene2
{
    public static class Helper
    {
        const string registryStartupLocation = @"Software\Microsoft\Windows\CurrentVersion\Run";
        const string keyname = "DreamScene2";
        static string str;

        public static string met()
        {
            if (str == null)
            {
                str = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                   $".{keyname}");
            }
            return str;
        }

        public static bool CheckStartOnBoot()
        {
            RegistryKey startupKey = Registry.CurrentUser.OpenSubKey(registryStartupLocation);
            bool startOnBoot = startupKey.GetValue(keyname) != null;
            startupKey.Close();
            return startOnBoot;
        }

        public static void StToggleStartOnBoot()
        {
            RegistryKey startupKey = Registry.CurrentUser.OpenSubKey(registryStartupLocation, true);
            startupKey.SetValue(keyname, Application.ExecutablePath + " -c");
            startupKey.Close();
        }

        public static void DelToggleStartOnBoot()
        {
            RegistryKey startupKey = Registry.CurrentUser.OpenSubKey(registryStartupLocation, true);
            startupKey.DeleteValue(keyname);
            startupKey.Close();
        }
    }
}
