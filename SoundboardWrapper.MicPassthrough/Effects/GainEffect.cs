using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundboardWrapper.MicPassthrough.Effects
{
    public static class GainEffect
    {
        public static int DefaultValue { get; } = 0;
        public static int Value { get; set; } = DefaultValue;

        public static void Perform(float[] buffer, int index, int samples)
        {
            
        }
    }
}
