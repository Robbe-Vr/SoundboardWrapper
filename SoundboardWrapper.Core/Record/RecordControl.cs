using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundboardWrapper.Core.Record
{
    internal static partial class RecordControl
    {
        internal static void Init(string recorderDevice)
        {
            Recorder.DeviceName = recorderDevice;
        }

        internal static string Process(IEnumerable<string> parts)
        {
            switch (parts.FirstOrDefault())
            {
                case "Start":
                    Recorder.Start();
                    break;

                case "Clip":
                    return Recorder.Clip();

                case "Stop":
                    Recorder.Stop();
                    break;

                default:
                    return "INVALID DATA";
            }

            return "EXECUTED";
        }
    }
}
