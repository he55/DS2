using System.IO;
using System.Xml.Serialization;

namespace DyDesktopWinForms
{
    public class DSSettings
    {
        const string filepath = "settings.xml";
        static DSSettings s_settings;

        private DSSettings()
        {
        }

        public bool FirstRun { get; set; } = true;
        public bool AutoPlay { get; set; }
        public bool AutoPause { get; set; }
        public bool IsMuted { get; set; }
        public int Volume { get; set; } = 3;

        public static DSSettings Load()
        {
            if (s_settings == null)
            {
                if (File.Exists(filepath))
                {
                    using (FileStream fileStream = File.OpenRead(filepath))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(DSSettings));
                        s_settings = (DSSettings)xmlSerializer.Deserialize(fileStream);
                    }
                }
                else
                {
                    s_settings = new DSSettings();
                }
            }
            return s_settings;
        }

        public static void Save()
        {
            using (FileStream fileStream = File.Create(filepath))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(DSSettings));
                xmlSerializer.Serialize(fileStream, s_settings);
            }
        }
    }
}
