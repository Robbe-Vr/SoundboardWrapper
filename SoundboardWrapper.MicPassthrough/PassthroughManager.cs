using System;
using SoundboardWrapper.MicPassthrough.Passthrough;
using SoundboardWrapper.MicPassthrough.Effects;

namespace SoundboardWrapper.MicPassthrough
{
    public class PassthroughManager
    {
        private MicPassthroughReceiver passthrough;

        public static bool PassthroughEnabled { get; private set; }

        public PassthroughManager(string input, string output)
        {
            passthrough = new MicPassthroughReceiver(input, output);
        }

        public void EnablePassthrough()
        {
            if (passthrough.Initialized)
            {
                passthrough.StartPassthrough();
                PassthroughEnabled = true;

                EchoEffect.Init();
                ReverbEffect.Init();
            }
        }

        public void DisablePassthrough()
        {
            if (passthrough.Initialized)
            {
                passthrough.StopPassthrough();
                PassthroughEnabled = false;
            }
        }

        public static bool SetEffectValue(Effect effect, int value)
        {
            switch (effect)
            {
                case Effect.Gain:
                    return EffectsChainManager.SetGain(value);

                case Effect.EchoDelay:
                    return EffectsChainManager.SetEchoDelay(value);

                case Effect.EchoDecay:
                    return EffectsChainManager.SetEchoDecay(value);

                case Effect.EchoMix:
                    return EffectsChainManager.SetEchoMix(value);

                case Effect.ReverbDelay:
                    return EffectsChainManager.SetReverbDelay(value);

                case Effect.ReverbDecay:
                    return EffectsChainManager.SetReverbDecay(value);

                case Effect.ReverbMix:
                    return EffectsChainManager.SetReverbMix(value);
                    
                case Effect.Pitch:
                    return EffectsChainManager.SetPitch(value);

                case Effect.Panning:
                    return EffectsChainManager.SetPanning(value);

                case Effect.NoiseGate:
                    return EffectsChainManager.SetNoiseGateThreshold(value);
            }

            return false;
        }
    }

    public enum Effect
    {
        Gain,
        EchoDelay,
        EchoDecay,
        EchoMix,
        ReverbDelay,
        ReverbDecay,
        ReverbMix,
        Pitch,
        Panning,
        NoiseGate,
    }
}
