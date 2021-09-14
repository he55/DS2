using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace DyDesktopWinForms
{
    public static class DSHelper
    {
        const string registryStartupLocation = @"Software\Microsoft\Windows\CurrentVersion\Run";
        static bool startOnBoot;
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
            startOnBoot = startupKey.GetValue("DyDesktopWinForms") != null;
            startupKey.Close();
            return startOnBoot;
        }

        public static bool ToggleStartOnBoot()
        {
            RegistryKey startupKey = Registry.CurrentUser.OpenSubKey(registryStartupLocation, true);

            if (!startOnBoot)
            {
                startupKey.SetValue("DyDesktopWinForms", Application.ExecutablePath + " -c");
                startOnBoot = true;
            }
            else
            {
                startupKey.DeleteValue("DyDesktopWinForms");
                startOnBoot = false;
            }
            startupKey.Close();
            return startOnBoot;
        }
    }
}
