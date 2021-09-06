using System.IO;
using System.Xml.Serialization;

namespace DyDesktopWinForms
{
    public class DSSettings
    {
        static DSSettings mySettings;

        private DSSettings()
        {
        }

        public bool FirstRun { get; set; } = true;
        public bool AutoPlay { get; set; }
        public bool AutoPause { get; set; }
        public bool IsMuted { get; set; }
        public int Volume { get; set; } = 3;

        const string filepath = "settings.xml";
        public static DSSettings Load()
        {
            if (mySettings == null)
            {
                if (File.Exists(filepath))
                {
                    using (FileStream fileStream = File.OpenRead(filepath))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(DSSettings));
                        mySettings = (DSSettings)xmlSerializer.Deserialize(fileStream);
                    }
                }
                else
                {
                    mySettings = new DSSettings();
                }
            }
            return mySettings;
        }

        public static void Save()
        {
            using (FileStream fileStream = File.Create(filepath))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(DSSettings));
                xmlSerializer.Serialize(fileStream, mySettings);
            }
        }
    }
}
