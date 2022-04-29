using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundboardWrapper.MicPassthrough.Effects
{
    public static class PanningGainEffect
    {
        public static int DefaultValue { get; } = 0;

        public static int Value { get; set; } = DefaultValue;

        public static void Perform(float[] buffer, int index, int samples)
        {
            if (index % 2 == 0)
            {
                buffer[index] *= ((Value < 0 ? 100.00f : 100 - Value) / 100.00f) * ((GainEffect.Value / 100.00f) + 1.0f);
            }
            else
            {
                buffer[index] *= ((Value > 0 ? 100.00f : -100 - Value) / 100.00f) * ((GainEffect.Value / 100.00f) + 1.0f);
            }
        }
    }
}
