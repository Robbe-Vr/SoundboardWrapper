using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SoundboardWrapper.MicPassthrough.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SYWCentralLogging;

namespace SoundboardWrapper.MicPassthrough.Passthrough
{
    public class PassthroughProviderManager : ISampleProvider
    {
        private WaveInEvent _waveIn;

        private BufferedWaveProvider inputProvider;
        private MonoToStereoSampleProvider dataProvider;

        public WaveFormat WaveFormat { get; private set; }

        public PassthroughProviderManager(WaveFormat format, WaveInEvent waveIn)
        {
            _waveIn = waveIn;

            WaveFormat = format;

            inputProvider = new BufferedWaveProvider(_waveIn.WaveFormat);

            VolumeSampleProvider volumeProvider = new VolumeSampleProvider(inputProvider.ToSampleProvider());
            volumeProvider.Volume = 2.0f;

            dataProvider = new MonoToStereoSampleProvider(volumeProvider);
        }

        public PassthroughProviderManager(WaveInEvent waveIn)
        {
            _waveIn = waveIn;
        }

        public ISampleProvider UseFormat(WaveFormat format)
        {
            WaveFormat = format;

            inputProvider = new BufferedWaveProvider(_waveIn.WaveFormat);

            VolumeSampleProvider volumeProvider = new VolumeSampleProvider(inputProvider.ToSampleProvider());
            volumeProvider.Volume = 2.0f;

            dataProvider = new MonoToStereoSampleProvider(volumeProvider);

            return this;
        }

        public void Start()
        {
            _waveIn.DataAvailable += (sender, e) =>
            {
                try
                {
                    inputProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);
                }
                catch
                {
                    Logger.Log("Failed to add samples to input BufferedWaveProvider!", SeverityLevel.Low);
                }
            };

            _waveIn.StartRecording();
        }

        public void Pause()
        {
            _waveIn.StopRecording();
        }

        public void Resume()
        {
            _waveIn.StartRecording();
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int read = dataProvider.Read(buffer, offset, count);

            EffectsChainManager.Manipulate(buffer, offset, read, _waveIn.WaveFormat.SampleRate);
            
            return read;
        }
    }
}
