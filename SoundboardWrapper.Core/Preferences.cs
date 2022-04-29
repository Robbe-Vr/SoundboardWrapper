using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundboardWrapper.Core
{
    internal class Preferences
    {
        public string DefaultRecordedOutputDevice { get; set; }
        public string DefaultRecordingDevice { get; set; }
        public string DefaultOutputDevice { get; set; }
        public string DefaultPassthroughInputDevice { get; set; }
        public string DefaultPassthroughOutputDevice { get; set; }
        public List<string> SoundEffectFolderPaths { get; set; }
        public bool RecordByDefault { get; set; }
        public bool PassthroughAudioByDefault { get; set; }

        internal void Load()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MidiDomotica-SoundboardWrapper", "preferences.json");

            if (File.Exists(path))
            {
                string jsonString = File.ReadAllText(path);

                Preferences pref = System.Text.Json.JsonSerializer.Deserialize<Preferences>(jsonString);

                this.DefaultRecordedOutputDevice = pref.DefaultRecordedOutputDevice;
                this.DefaultRecordingDevice = pref.DefaultRecordingDevice;
                this.DefaultOutputDevice = pref.DefaultOutputDevice;
                this.DefaultPassthroughInputDevice = pref.DefaultPassthroughInputDevice;
                this.DefaultPassthroughOutputDevice = pref.DefaultPassthroughOutputDevice;
                this.SoundEffectFolderPaths = pref.SoundEffectFolderPaths;
                this.RecordByDefault = pref.RecordByDefault;
                this.PassthroughAudioByDefault = pref.PassthroughAudioByDefault;
            }
            else
            {
                this.DefaultRecordedOutputDevice = "Default";
                this.DefaultRecordingDevice = "";
                this.DefaultOutputDevice = "Default";
                this.DefaultPassthroughInputDevice = "Analogue 1 + 2 (3- Focusrite Usb Audio)";
                this.DefaultPassthroughOutputDevice = "CABLE Input (VB-Audio Virtual Cable)";
                this.SoundEffectFolderPaths = new List<string>()
                {
                    "D:\\Windows\\MidiDomotica\\Soundboard",
                };
                this.RecordByDefault = false;
                this.PassthroughAudioByDefault = false;

                Store();
            }
        }

        internal void Store()
        {
            string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MidiDomotica-SoundboardWrapper");

            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            string path = Path.Combine(dir, "preferences.json");

            string content = System.Text.Json.JsonSerializer.Serialize(this);

            File.WriteAllText(path, content);
        }
    }
}
