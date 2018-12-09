using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using ColdTranslation.Model;

namespace ColdTranslation.Legacy
{
    public class Settings
    {
        public static readonly string SettingsPath =
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ColdTranslation.xml");
        public List<LastRow> LastRows { get; set; } = new List<LastRow>();
        public string LastTranslationSheet { get; set; } = "";
        public Point Location { get; set; } = new Point(0, 0);
        public bool HideSpeaker { get; set; }
        public bool Sen4Mode { get; set; }

        public static void Serialize(string path, Settings settings)
        {
            var serializer = new XmlSerializer(typeof(Settings));
            using (TextWriter writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, settings);
            }
        }

        public static Settings Deserialize(string path)
        {
            var deserializer = new XmlSerializer(typeof(Settings));
            using (TextReader reader = new StreamReader(path))
            {
                var obj = deserializer.Deserialize(reader);
                var settings = obj as Settings;
                return settings;
            }
        }
        

        public static Settings OldSettings
        {
            get
            {
                try
                {
                    if (!File.Exists(SettingsPath)) return null;
                    var settings = Deserialize(SettingsPath);
                    File.Move(SettingsPath, $"{SettingsPath}.old");
                    return settings;
                }
                catch (Exception)
                {
                    return null;
                }



            }
        }
    }
}
