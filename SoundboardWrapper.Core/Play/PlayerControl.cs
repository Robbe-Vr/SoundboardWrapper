using SoundboardWrapper.Core.Default;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundboardWrapper.Core.Play
{
    internal static partial class PlayerControl
    {
        internal static string Process(IEnumerable<string> parts)
        {
            string type = parts.FirstOrDefault();
            parts = parts.Skip(1);
            bool all = !parts.Any();

            int id;
            switch (type)
            {
                case "Play":
                    return all ? "INVALID DATA" : PlayerManager.Play(parts.FirstOrDefault()).ToString();

                case "Pause":
                    if (all)
                    {
                        PlayerManager.Pause(101);
                    }
                    else
                    {
                        if (int.TryParse(parts.FirstOrDefault(), out id))
                        {
                            PlayerManager.Pause(id);
                        }
                    }
                    break;

                case "Resume":
                    if (all)
                    {
                        PlayerManager.Resume(101);
                    }
                    else
                    {
                        if (int.TryParse(parts.FirstOrDefault(), out id))
                        {
                            PlayerManager.Resume(id);
                        }
                    }
                    break;

                case "Stop":
                    if (all)
                    {
                        PlayerManager.Stop(101);
                    }
                    else
                    {
                        if (int.TryParse(parts.FirstOrDefault(), out id))
                        {
                            PlayerManager.Stop(id);
                        }
                    }
                    break;

                case "Volume":
                    int volume = -1;
                    if (int.TryParse(parts.ElementAtOrDefault(1), out volume) && volume <= 100 && volume > -1 && all)
                    {
                        PlayerManager.SetVolume(101, volume);
                    }
                    else
                    {
                        if (volume > -1 && int.TryParse(parts.FirstOrDefault(), out id))
                        {
                            PlayerManager.SetVolume(id, volume);
                        }
                    }
                    break;

                default:
                    return "INVALID DATA";
            }

            return "EXECUTED";
        }
    }
}
