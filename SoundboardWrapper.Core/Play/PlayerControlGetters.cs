using SoundboardWrapper.Core.Default;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundboardWrapper.Core.Play
{
    internal static partial class PlayerControl
    {
        internal static string GetSounds(IEnumerable<string> parts)
        {
            switch (parts.FirstOrDefault())
            {
                case null:
                    return GetAvailableSoundFiles();

                default:
                    return GetSoundFilesFromFolder(parts.Skip(1).FirstOrDefault());
            }
        }

        private static string GetSoundFilesFromFolder(string directoryName)
        {
            if (String.IsNullOrWhiteSpace(directoryName)) return string.Empty;

            List<string> files = new List<string>();

            foreach (string folderPath in SoundboardData.SoundEffectFolderPaths.Where(x => x.Contains(directoryName)))
            {
                files.AddRange(GetFolderContents(folderPath));
            }

            return String.Join(',', files);
        }

        private static string GetAvailableSoundFiles()
        {
            List<string> files = new List<string>();

            foreach (string folderPath in SoundboardData.SoundEffectFolderPaths)
            {
                files.AddRange(GetFolderContents(folderPath));
            }

            return String.Join(',', files);
        }

        private static IEnumerable<string> GetFolderContents(string path)
        {
            return new string[] { "*.mp3", "*.wav" }.AsParallel().SelectMany(x => Directory.EnumerateFiles(path, x, SearchOption.AllDirectories));
        }
    }
}
