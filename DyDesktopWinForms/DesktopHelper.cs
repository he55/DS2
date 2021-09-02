// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using Microsoft.Win32;
using System.IO;
using System.Windows.Forms;

namespace DyDesktopWinForms
{
    public static class DesktopHelper
    {
        private const string registryStartupLocation = @"Software\Microsoft\Windows\CurrentVersion\Run";

        private static bool startOnBoot;

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
                startupKey.SetValue("DyDesktopWinForms", Application.ExecutablePath);
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
