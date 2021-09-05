using Microsoft.Win32;
using System.Windows.Forms;

namespace DyDesktopWinForms
{
    public static class DesktopHelper
    {
        const string registryStartupLocation = @"Software\Microsoft\Windows\CurrentVersion\Run";
        static bool startOnBoot;

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
