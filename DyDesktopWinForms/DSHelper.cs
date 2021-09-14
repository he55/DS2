using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace DyDesktopWinForms
{
    public static class DSHelper
    {
        const string registryStartupLocation = @"Software\Microsoft\Windows\CurrentVersion\Run";
        static string str;

        public static string met()
        {
            if (str == null)
            {
                str = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".DyDesktopWinForms");
            }
            return str;
        }

        public static bool CheckStartOnBoot()
        {
            RegistryKey startupKey = Registry.CurrentUser.OpenSubKey(registryStartupLocation);
           bool startOnBoot = startupKey.GetValue("DyDesktopWinForms") != null;
            startupKey.Close();
            return startOnBoot;
        }

        public static void DelToggleStartOnBoot()
        {
            RegistryKey startupKey = Registry.CurrentUser.OpenSubKey(registryStartupLocation, true);
            startupKey.DeleteValue("DyDesktopWinForms");
            startupKey.Close();
        }

        public static void StToggleStartOnBoot()
        {
            RegistryKey startupKey = Registry.CurrentUser.OpenSubKey(registryStartupLocation, true);
            startupKey.SetValue("DyDesktopWinForms", Application.ExecutablePath + " -c");
            startupKey.Close();
        }
    }
}
