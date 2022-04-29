using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundboardWrapper.Core.Play
{
    internal class SoundPlayer
    {
        public int Id { get; set; }

        private WaveOut WaveOut;

        public void Init(string file, int device)
        {
            WaveOut = new WaveOut();
            WaveOut.DeviceNumber = device;

            string extension = Path.GetExtension(file);
            WaveOut.Init(extension == ".mp3" ? new Mp3FileReader(file) :
                         extension == ".wav" ? new WaveFileReader(file) :
                         new AudioFileReader(file));

            WaveOut.PlaybackStopped += (sender, e) =>
            {
                Dispose();
            };
        }

        public void Play()
        {
            WaveOut.Play();
        }

        public void SetVolume(int value)
        {
            WaveOut.Volume = value / 100.00f;
        }

        public void Pause()
        {
            WaveOut.Pause();
        }

        public void Resume()
        {
            WaveOut.Resume();
        }

        public void Stop()
        {
            WaveOut.Stop();
        }

        public void Dispose()
        {
            if (WaveOut != null)
            {
                WaveOut.Stop();
                //waveOut.Dispose();
                WaveOut = null;
            }

            PlayerManager.Clean(this);
        }
    }
}
