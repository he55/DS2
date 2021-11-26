﻿using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace DreamScene2
{
    public static class Helper
    {
        const string registryStartupLocation = @"Software\Microsoft\Windows\CurrentVersion\Run";
        static string s_extPath;
        static string s_appPath;

        public static void OpenUrl(string str)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = str;
            processStartInfo.UseShellExecute = true;

            Process.Start(processStartInfo);
        }

        public static string GetPath(string str)
        {
            if (s_appPath == null)
            {
                s_appPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constant.projectName);
                if (!Directory.Exists(s_appPath))
                {
                    Directory.CreateDirectory(s_appPath);
                }
            }

            if (string.IsNullOrEmpty(str))
            {
                return s_appPath;
            }
            return Path.Combine(s_appPath, str);
        }

        public static string ExtPath()
        {
            if (s_extPath == null)
            {
                s_extPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                                         $".{Constant.projectName}");
            }
            return s_extPath;
        }

        public static bool CheckStartOnBoot()
        {
            RegistryKey startupKey = Registry.CurrentUser.OpenSubKey(registryStartupLocation);
            bool startOnBoot = startupKey.GetValue(Constant.projectName) != null;
            startupKey.Close();
            return startOnBoot;
        }

        public static void SetStartOnBoot()
        {
            RegistryKey startupKey = Registry.CurrentUser.OpenSubKey(registryStartupLocation, true);
            startupKey.SetValue(Constant.projectName, $"{Application.ExecutablePath} {Constant.cmd}");
            startupKey.Close();
        }

        public static void RemoveStartOnBoot()
        {
            RegistryKey startupKey = Registry.CurrentUser.OpenSubKey(registryStartupLocation, true);
            startupKey.DeleteValue(Constant.projectName);
            startupKey.Close();
        }
    }
}
