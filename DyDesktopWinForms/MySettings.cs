using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DyDesktopWinForms
{
    public class MySettings
    {
        private static MySettings mySettings = new MySettings();

        private MySettings()
        {

        }

        public bool FirstRun { get; set; }
        public bool AutoPlay { get; set; }
        public bool AutoPause { get; set; }
        public bool IsMuted { get; set; }
        public int Volume { get; set; } = 3;

        public static MySettings Load()
        {
            return mySettings;
        }

        public static void Save()
        {

        }
    }
}
