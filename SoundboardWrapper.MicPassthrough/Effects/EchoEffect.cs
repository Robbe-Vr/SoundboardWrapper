using NAudio.Wave;
using System;
using System.Collections.Generic;

namespace SoundboardWrapper.MicPassthrough.Effects
{
    public static class EchoEffect
    {
        public static int DefaultWetLevel { get; } = 0;
        public static int DefaultDecay { get; } = 10;
        public static int DefaultDelay { get; } = 75;

        public static int WetLevel { get; set; } = DefaultWetLevel;
        private static int _decay;
        public static int Decay { get { return _decay; } set { Init(decay: value); } }
        private static int _delay;
        public static int Delay { get { return _delay; } set { Init(delay: value); } }

        private static List<Queue<float>> samples = new List<Queue<float>>();

        public static void Init(int delay = -1, int decay = -1)
        {
            if (delay == -1)
            {
                delay = DefaultDelay;
            }
            _delay = delay;

            int sampleCount = (int)((2_000 * (_delay / 100.00f)) * 44.1f);

            if (sampleCount == samples.Count) return;

            if (decay == -1)
            {
                decay = DefaultDecay;
            }
            _decay = decay;

            List<Queue<float>> newSamples = new List<Queue<float>>(_decay);

            if (samples.Count > 0)
            {
                for (int q = 0; q < _decay; q++)
                {
                    newSamples.Add(new Queue<float>(sampleCount));
                    for (int i = 0; i < sampleCount; i++)
                    {
                        newSamples[q].Enqueue(samples.Count > q && samples[q].Count > 0 ? samples[q].Dequeue() : 0f);
                    }
                }
            }
            else
            {
                for (int q = 0; q < _decay; q++)
                {
                    newSamples.Add(new Queue<float>(sampleCount));
                    for (int i = 0; i < sampleCount; i++)
                    {
                        newSamples[q].Enqueue(0f);
                    }
                }
            }

            samples = newSamples;
        }

        public static void Perform(float[] buffer, int index)
        {
            float sample = buffer[index];

            float depthDivider = 0.75f;

            foreach (Queue<float> queue in samples)
            {
                queue.Enqueue(sample);

                if (queue.Count > 0)
                {
                    sample = queue.Dequeue();

                    buffer[index] = Math.Min(1, Math.Max(-1, buffer[index] + ((WetLevel / 100.00f) * depthDivider) * sample));
                }

                depthDivider -= 0.75f / _decay;
            }
        }
    }
}
