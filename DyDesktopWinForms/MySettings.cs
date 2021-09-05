using System.IO;
using System.Xml.Serialization;

namespace DyDesktopWinForms
{
    public class MySettings
    {
        private static MySettings mySettings;

        private MySettings()
        {
        }

        public bool FirstRun { get; set; } = true;
        public bool AutoPlay { get; set; }
        public bool AutoPause { get; set; }
        public bool IsMuted { get; set; }
        public int Volume { get; set; } = 3;

        const string filepath = "settings.xml";
        public static MySettings Load()
        {
            if (mySettings == null)
            {
                if (File.Exists(filepath))
                {
                    using (FileStream fileStream = File.OpenRead(filepath))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(MySettings));
                        mySettings = (MySettings)xmlSerializer.Deserialize(fileStream);
                    }
                }
                else
                {
                    mySettings = new MySettings();
                }
            }
            return mySettings;
        }

        public static void Save()
        {
            using (FileStream fileStream = File.Create(filepath))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(MySettings));
                xmlSerializer.Serialize(fileStream, mySettings);
            }
        }
    }
}
