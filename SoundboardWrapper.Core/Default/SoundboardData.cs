using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundboardWrapper.Core.Default
{
    internal static class SoundboardData
    {
        public static string PassthroughOutputDevice { get; set; }
        internal static string RecordedOutputDevice { get; set; }
        internal static string RecordingDevice { get; set; }
        internal static string OutputDevice { get; set; }
        internal static string PassthroughInputDevice { get; set; }

        internal static List<string> SoundEffectFolderPaths { get; } = new List<string>();

        internal static List<string> OutputDevicesInNumberedOrder { get; } = new List<string>();
        internal static List<string> InputDevicesInNumberedOrder { get; } = new List<string>();

        internal static int GetOutputDeviceNumber(string deviceName)
        {
            return OutputDevicesInNumberedOrder.FindIndex(x => x.Contains(deviceName));
        }

        internal static int GetInputDeviceNumber(string deviceName)
        {
            return InputDevicesInNumberedOrder.FindIndex(x => x.Contains(deviceName));
        }

        internal static void InitDevices()
        {
            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                WaveInCapabilities cap = WaveIn.GetCapabilities(i);

                InputDevicesInNumberedOrder.Add(cap.ProductName);
            }

            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                WaveOutCapabilities cap = WaveOut.GetCapabilities(i);

                OutputDevicesInNumberedOrder.Add(cap.ProductName);
            }
        }
    }
}
