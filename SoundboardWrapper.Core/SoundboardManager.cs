using SoundboardWrapper.Core.Default;
using SoundboardWrapper.Core.Passthrough;
using SoundboardWrapper.Core.Play;
using SoundboardWrapper.Core.Record;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SYWPipeNetworkManager;

namespace SoundboardWrapper.Core
{
    public class SoundboardManager
    {
        internal static Preferences preferences { get; private set; }

        private IEnumerable<string> validatedSources = new List<string>()
        {
            "MidiDomotica"
        };

        public bool Setup()
        {
            PipeMessageControl.Init("Soundboard");
            PipeMessageControl.StartClient(
                (sourceName, message) =>
                {
                    if (ValidateSource(sourceName))
                    {
                        return $"{message} -> " + ProcessMessage(message);
                    }
                    else return $"{message} -> NO";
                }
            );

            preferences = new Preferences();
            preferences.Load();

            SoundboardData.RecordedOutputDevice = preferences.DefaultRecordedOutputDevice;
            SoundboardData.RecordingDevice = preferences.DefaultRecordingDevice;
            SoundboardData.OutputDevice = preferences.DefaultOutputDevice;
            SoundboardData.PassthroughInputDevice = preferences.DefaultPassthroughInputDevice;
            SoundboardData.PassthroughOutputDevice = preferences.DefaultPassthroughOutputDevice;
            SoundboardData.SoundEffectFolderPaths.AddRange(preferences.SoundEffectFolderPaths);

            SoundboardData.InitDevices();

            PassthroughControl.Init(SoundboardData.PassthroughInputDevice, SoundboardData.PassthroughOutputDevice, preferences.PassthroughAudioByDefault);
            RecordControl.Init(SoundboardData.RecordedOutputDevice);

            return true;
        }

        private bool ValidateSource(string source)
        {
            return validatedSources.Contains(source);
        }

        public string ProcessMessage(string message)
        {
            IEnumerable<string> parts = new Regex(@"(::\[|\]::|::)|]$").Split(message).Where(x => !String.IsNullOrWhiteSpace(x) && !x.Contains("::")).Select(x => x.Trim());

            return parts.FirstOrDefault() switch
            {
                "Get" => ProcessGetCommand(parts.Skip(1)),
                "Set" => SoundboardControl.Set(parts.Skip(1)),
                "Record" => RecordControl.Process(parts.Skip(1)),
                "Passthrough" => PassthroughControl.Process(parts.Skip(1)),
                "Player" => PlayerControl.Process(parts.Skip(1)),
                _ => "INVALID DATA",
            };
        }

        private string ProcessGetCommand(IEnumerable<string> parts)
        {
            return parts.FirstOrDefault() switch
            {
                "Sounds" => PlayerControl.GetSounds(parts.Skip(1)),
                "Id" => SoundboardControl.GetName(parts.Skip(1)?.FirstOrDefault()),
                "All" => SoundboardControl.GetModes(),
                _ => "UNKNOWN GET",
            };
        }
    }
}
