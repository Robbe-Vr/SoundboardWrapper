using NAudio.Wave;
using System;
using System.Collections.Generic;

namespace SoundboardWrapper.MicPassthrough.Effects
{
    public static class ReverbEffect
    {
        public static int DefaultWetLevel { get; } = 0;
        public static int DefaultDecay { get; } = 10;
        public static int DefaultDelay { get; } = 10;

        public static int WetLevel { get; set; } = DefaultWetLevel;
        private static int _decay;
        public static int Decay { get { return _decay; } set { Init(decay: value); } }
        private static int _delay;
        public static int Delay { get { return _delay; } set { Init(delay: value); } }

        private static int sampleDelay = 0;

        public static void Init(int delay = -1, int decay = -1)
        {
            if (delay == -1)
            {
                delay = DefaultDelay;
            }
            _delay = delay;

            sampleDelay = (int)((2_000 * (_delay / 100.00f)) * 44.1f);

            if (decay == -1)
            {
                decay = DefaultDecay;
            }
            _decay = decay;
        }

        public static void Perform(float[] buffer, int index, int samples)
        {
            if (index + sampleDelay > samples) return;

            buffer[index + sampleDelay] += (short)(buffer[index] * ((_decay / 100.00f) * (WetLevel / 100.00f)));
        }
    }
}
