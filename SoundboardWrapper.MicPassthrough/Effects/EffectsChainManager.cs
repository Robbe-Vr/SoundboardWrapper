using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SYWCentralLogging;

namespace SoundboardWrapper.MicPassthrough.Effects
{
    public static class EffectsChainManager
    {
        public static void Manipulate(float[] buffer, int offset, int samples, int sampleRate)
        {
            try
            {
                if (NoiseGateEffect.Threshold > 0 && !NoiseGateEffect.Perform(buffer, samples))
                {
                    for (int i = offset; i < offset + samples; i++)
                    {
                        //if (GainEffect.Value > 0)
                        //{
                        //    GainEffect.Perform(buffer, i, samples);
                        //}

                        if (EchoEffect.WetLevel > 0 && EchoEffect.Delay > 0 && EchoEffect.Decay > 0)
                        {
                            EchoEffect.Perform(buffer, i);
                        }

                        if (ReverbEffect.WetLevel > 0 && ReverbEffect.Delay > 0 && ReverbEffect.Decay > 0)
                        {
                            ReverbEffect.Perform(buffer, i, samples);
                        }

                        if (PanningGainEffect.Value != 0)
                        {
                            PanningGainEffect.Perform(buffer, i, samples);
                        }
                    }

                    if (PitchEffect.Value != 100)
                    {
                        PitchEffect.Perform(buffer, sampleRate, samples);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log($"Audio Manipulation Failed!\nError: {e.Message}\nStackTrace: {e.StackTrace}", SeverityLevel.Low);
            }
        }

        /// <summary>
        /// values from 0 (normal) to 1000 (max gain -> x10) (%)
        /// </summary>
        /// <param name="value"></param>
        public static bool SetGain(int value)
        {
            if (value < 0 || value > 1000) return false;

            try
            {
                GainEffect.Value = value;
            }
            catch { return false; }

            return true;
        }

        /// <summary>
        /// values from -100 (left) to 0 (normal) to 100 (right) (%)
        /// </summary>
        /// <param name="value"></param>
        public static bool SetPanning(int value)
        {
            if (value < -100 || value > 100) return false;

            try
            {
                PanningGainEffect.Value = value;
            }
            catch { return false; }

            return true;
        }

        /// <summary>
        /// values from 1 to 100 (normal) to 200+ (octave up) (%)
        /// </summary>
        /// <param name="value"></param>
        public static bool SetPitch(int value)
        {
            if (value < 1 || value > 500) return false;

            try
            {
                PitchEffect.Value = value;
            }
            catch { return false; }

            return true;

            return true;
        }

        /// <summary>
        /// values from 0 (normal) to 100 (2 seconds echo) (%)
        /// </summary>
        /// <param name="value"></param>
        public static bool SetEchoDelay(int value)
        {
            if (value < 0 || value > 100) return false;

            try
            {
                EchoEffect.Delay = value;
            }
            catch { return false; }

            return true;
        }

        /// <summary>
        /// values from 0 (normal) to 100 (max amount echos) (%)
        /// </summary>
        /// <param name="value"></param>
        public static bool SetEchoDecay(int value)
        {
            if (value < 0 || value > 100) return false;

            try
            {
                EchoEffect.Decay = value;
            }
            catch { return false; }

            return true;
        }

        /// <summary>
        /// values from 0 (no effect) to 100 (%)
        /// </summary>
        /// <param name="value"></param>
        public static bool SetEchoMix(int value)
        {
            if (value < 0 || value > 100) return false;

            try
            {
                EchoEffect.WetLevel = value;
            }
            catch { return false; }

            return true;
        }

        /// <summary>
        /// values from 0 (normal) to 100 (2 seconds reverb) (%)
        /// </summary>
        /// <param name="value"></param>
        public static bool SetReverbDelay(int value)
        {
            if (value < 0 || value > 100) return false;

            try
            {
                ReverbEffect.Delay = value;
            }
            catch { return false; }

            return true;
        }

        /// <summary>
        /// values from 0 (normal) to 100 (max amount reverb) (%)
        /// </summary>
        /// <param name="value"></param>
        public static bool SetReverbDecay(int value)
        {
            if (value < 0 || value > 100) return false;

            try
            {
                ReverbEffect.Decay = value;
            }
            catch { return false; }

            return true;
        }

        /// <summary>
        /// values from 0 (no effect) to 100 (%)
        /// </summary>
        /// <param name="value"></param>
        public static bool SetReverbMix(int value)
        {
            if (value < 0 || value > 100) return false;

            try
            {
                ReverbEffect.WetLevel = value;
            }
            catch { return false; }

            return true;
        }

        /// <summary>
        /// values from 0 (no effect) to 100 (mute all) (%)
        /// </summary>
        /// <param name="value"></param>
        public static bool SetNoiseGateThreshold(int value)
        {
            if (value < 0 || value > 100) return false;

            try
            {
                NoiseGateEffect.Threshold = value;
            }
            catch { return false; }

            return true;
        }
    }
}
