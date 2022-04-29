using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundboardWrapper.MicPassthrough.Effects
{
    public static class NoiseGateEffect
    {
        private static float _absMax = 1.0f;

        public static int DefaultValue { get; } = 10;

        public static int Threshold { get; set; } = DefaultValue;

        public static bool Perform(float[] buffer, int samples)
        {
            float currentMax = buffer.Max();

            if (currentMax / _absMax < (Threshold / 100.00f) * _absMax)
            {
                for (int i = 0; i < samples; i++)
                {
                    buffer[i] = 0;
                }

                return true;
            }

            return false;
        }
    }
}
