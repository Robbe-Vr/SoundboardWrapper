using SoundboardWrapper.MicPassthrough;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundboardWrapper.Core.Passthrough
{
    internal class PassthroughControl
    {
        private static PassthroughManager manager;

        internal static void Init(string input, string output, bool enable)
        {
            manager = new PassthroughManager(input, output);

            if (enable)
            {
                manager.EnablePassthrough();
            }
        }

        internal static string Process(IEnumerable<string> parts)
        {
            switch (parts.FirstOrDefault())
            {
                case "Enable":
                    bool enable;
                    if (bool.TryParse(parts.ElementAtOrDefault(1), out enable))
                    {
                        if (enable != PassthroughManager.PassthroughEnabled)
                        {
                            if (enable)
                            {
                                manager.EnablePassthrough();
                            }
                            else
                            {
                                manager.DisablePassthrough();
                            }
                        }
                    }
                    else return "INVALID DATA";
                    break;

                default:
                    Effect effect;
                    int value;
                    if (Enum.TryParse(parts.FirstOrDefault(), out effect) && int.TryParse(parts.ElementAtOrDefault(1), out value))
                    {
                        PassthroughManager.SetEffectValue(effect, value);
                    }
                    break;
            }

            return "EXECUTED";
        }

        internal static void Init(string passthroughInputDevice, object passthroughOutputDevice)
        {
            throw new NotImplementedException();
        }
    }
}
