using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;

namespace SoundboardWrapper.MicPassthrough.Passthrough
{
    public class MicPassthroughReceiver
    {
        private WaveInEvent _inputDevice;

        private PassthroughProviderManager provider;

        private PassthroughOutputter _output;

        public bool Initialized { get; private set; }

        public MicPassthroughReceiver(string inputDevice, string outputDevice)
        {
            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                WaveInCapabilities cap = WaveIn.GetCapabilities(i);

                if (cap.ProductName.Contains(inputDevice) || cap.ProductGuid.ToString().Contains(inputDevice) || inputDevice.Contains(cap.ProductGuid.ToString()))
                {
                    _inputDevice = new WaveInEvent();
                    _inputDevice.DeviceNumber = i;
                    _inputDevice.WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(44_100, 1);
                    break;
                }
            }
            if (_inputDevice == null)   // if we're unable to find a recording device, the process can't run, so we do not continue.
            {
                return;
            }

            _output = new PassthroughOutputter(outputDevice);
            if (_output.Initialized == true)  // once we found an output device aswell, we're initialized
            {
                Initialized = true;
            }
        }

        public void StartPassthrough()
        {
            if (_output.Active)
            {
                provider.Resume();

                _output.Resume();
            }
            else
            {
                Task.Run(() =>
                {
                    provider = new PassthroughProviderManager(_inputDevice);

                    _output.Start(provider);
                });
            }
        }

        public void StopPassthrough()
        {
            provider.Pause();

            _output.Stop();
        }
    }
}
