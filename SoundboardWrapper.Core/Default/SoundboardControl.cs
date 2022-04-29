using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SYWCentralLogging;

namespace SoundboardWrapper.Core.Default
{
    internal static partial class SoundboardControl
    {
        public static string Set(IEnumerable<string> parts)
        {
            string value = parts.ElementAtOrDefault(1);

            if (String.IsNullOrWhiteSpace(value)) return "INVALID DATA";

            switch (parts.FirstOrDefault())
            {
                case "Output":
                    SoundboardData.OutputDevice = value;
                    SoundboardManager.preferences.DefaultOutputDevice = value;
                    SoundboardManager.preferences.Store();
                    break;

                case "Input":
                    SoundboardData.RecordingDevice = value;
                    SoundboardManager.preferences.DefaultRecordingDevice = value;
                    SoundboardManager.preferences.Store();
                    break;

                case "PassthroughInput":
                    SoundboardData.PassthroughInputDevice = value;
                    SoundboardManager.preferences.DefaultPassthroughInputDevice = value;
                    SoundboardManager.preferences.Store();
                    break;

                case "PassthroughOutput":
                    SoundboardData.PassthroughOutputDevice = value;
                    SoundboardManager.preferences.DefaultPassthroughOutputDevice = value;
                    SoundboardManager.preferences.Store();
                    break;

                case "RecordedOutput":
                    SoundboardData.RecordedOutputDevice = value;
                    SoundboardManager.preferences.DefaultRecordedOutputDevice = value;
                    SoundboardManager.preferences.Store();
                    break;

                case "PassthroughDefault":
                    bool newPassthroughDefault;
                    if (bool.TryParse(value, out newPassthroughDefault))
                    SoundboardManager.preferences.PassthroughAudioByDefault = newPassthroughDefault;
                    SoundboardManager.preferences.Store();
                    break;

                case "RecordDefault":
                    bool newRecordDefault;
                    if (bool.TryParse(value, out newRecordDefault))
                        SoundboardManager.preferences.RecordByDefault = newRecordDefault;
                    SoundboardManager.preferences.Store();
                    break;

                case "SoundFolder":
                    if (Directory.Exists(value))
                    {
                        switch (parts.ElementAtOrDefault(2))
                        {
                            case "Remove":
                                SoundboardData.SoundEffectFolderPaths.RemoveAll(x => x == value);
                                SoundboardManager.preferences.SoundEffectFolderPaths.RemoveAll(x => x == value);
                                break;

                            default:
                                if (!SoundboardData.SoundEffectFolderPaths.Contains(value))
                                    SoundboardData.SoundEffectFolderPaths.Add(value);

                                if (!SoundboardManager.preferences.SoundEffectFolderPaths.Contains(value))
                                    SoundboardManager.preferences.SoundEffectFolderPaths.Add(value);
                                break;
                        }
                        SoundboardManager.preferences.Store();
                    }
                    break;

                default:
                    return "INVALID DATA";
            }

            return "EXECUTED";
        }
    }
}
