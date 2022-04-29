using NAudio.Wave;
using SoundboardWrapper.MicPassthrough.Effects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundboardWrapper.MicPassthrough.Passthrough
{
    public class PassthroughOutputter
    {
        private WaveOut _outputDevice;

        private readonly WaveFormat format = WaveFormat.CreateIeeeFloatWaveFormat(44_100, 2);

        public bool Initialized { get; private set; }
        public bool Active { get; private set; }

        public PassthroughOutputter(string outputDevice)
        {
            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                WaveOutCapabilities cap = WaveOut.GetCapabilities(i);

                if (cap.ProductName.Contains(outputDevice) || cap.ProductGuid.ToString().Contains(outputDevice) || outputDevice.Contains(cap.ProductGuid.ToString()))
                {
                    _outputDevice = new WaveOut();
                    _outputDevice.DeviceNumber = i;
                    _outputDevice.Volume = 1.0f;

                    break;
                }
            }
            if (_outputDevice != null)
            {
                Initialized = true;
            }
        }

        public void Start(PassthroughProviderManager provider)
        {
            Active = true;

            _outputDevice.Init(provider.UseFormat(format));

            provider.Start();

            _outputDevice.Play();
        }

        public void Resume()
        {
            Active = true;

            _outputDevice.Play();
        }

        public void Stop()
        {
            _outputDevice.Stop();

            Active = false;
        }
    }
}
