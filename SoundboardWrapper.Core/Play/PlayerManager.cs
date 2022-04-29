using SoundboardWrapper.Core.Default;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundboardWrapper.Core.Play
{
    internal static class PlayerManager
    {
        private readonly static List<SoundPlayer> players = new List<SoundPlayer>();
        private static Random random = new Random();

        internal static int Play(string file)
        {
            if (!File.Exists(file)) return -1;

            SoundPlayer player = new SoundPlayer();
            players.Add(player);

            player.Init(file, SoundboardData.GetOutputDeviceNumber(SoundboardData.OutputDevice));

            player.Id = CreateNewId();

            player.SetVolume(100);
            player.Play();

            return player.Id;
        }

        internal static void Pause(int id)
        {
            if (id == 101) players.ForEach(player => player.Pause());
            else players.FirstOrDefault(x => x.Id == id)?.Pause();
        }

        internal static void Resume(int id)
        {
            if (id == 101) players.ForEach(player => player.Resume());
            else players.FirstOrDefault(x => x.Id == id)?.Resume();
        }

        internal static void Stop(int id)
        {
            if (id == 101) players.ForEach(player => player.Stop());
            else players.FirstOrDefault(x => x.Id == id)?.Stop();
        }

        internal static void SetVolume(int id, int value)
        {
            if (id == 101) players.ForEach(player => player.SetVolume(value));
            else players.FirstOrDefault(x => x.Id == id)?.SetVolume(value);
        }

        internal static void Clean(SoundPlayer player)
        {
            players.Remove(player);
        }

        private static int CreateNewId()
        {
            int id = 0;
            while (players.Any(x => x.Id == (id = random.Next(1, 100)))) { }

            return id;
        }
    }
}
