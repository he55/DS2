using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace DreamScene2
{
    public static class Helper
    {
        const string registryStartupLocation = @"Software\Microsoft\Windows\CurrentVersion\Run";
        const string projectName = "DreamScene2";
        static string s_extPath;

        public static string ExtPath()
        {
            if (s_extPath == null)
            {
                s_extPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    $".{projectName}");
            }
            return s_extPath;
        }

        public static bool CheckStartOnBoot()
        {
            RegistryKey startupKey = Registry.CurrentUser.OpenSubKey(registryStartupLocation);
            bool startOnBoot = startupKey.GetValue(projectName) != null;
            startupKey.Close();
            return startOnBoot;
        }

        public static void StToggleStartOnBoot()
        {
            RegistryKey startupKey = Registry.CurrentUser.OpenSubKey(registryStartupLocation, true);
            startupKey.SetValue(projectName, Application.ExecutablePath + " -c");
            startupKey.Close();
        }

        public static void DelToggleStartOnBoot()
        {
            RegistryKey startupKey = Registry.CurrentUser.OpenSubKey(registryStartupLocation, true);
            startupKey.DeleteValue(projectName);
            startupKey.Close();
        }
    }
}
