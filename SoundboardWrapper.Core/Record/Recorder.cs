using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using SYWCentralLogging;

namespace SoundboardWrapper.Core.Record
{
    internal static class Recorder
    {
        private static string _recordingsFolder  { get { return Path.Combine("D:", "Windows", "MidiDomotica", "Soundboard", "Recordings"); } }
        private static string _samplesTempFolder  { get { return Path.Combine("D:", "Windows", "MidiDomotica", "Temp", "Recordings"); } }

        private static int _sampleFileCount = 2;
        private static int _sampleDuration = 5_000;

        private static IEnumerable<string> _sampleFileNames { get { for (int i = 0; i < _sampleFileCount; i++) yield return $"TempRec-{i + 1}.wav"; } }

        private static string _deviceName;
        public static string DeviceName {
            get
            {
                return _deviceName;
            }
            set
            {
                _deviceName = value;

                MMDeviceEnumerator enumerator = new MMDeviceEnumerator();

                MMDevice device = _deviceName == "Default" ? enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia) : enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active).FirstOrDefault(d => d.DeviceFriendlyName == _deviceName);

                capture = new WasapiLoopbackCapture(device);
            }
        }

        private static WasapiLoopbackCapture capture;

        private static MemoryStream memoryStream;
        private static int capacity = 0;

        private static Timer memoryTimer;

        internal static void Start()
        {
            if (capture == null) return;

            if (capture.CaptureState != CaptureState.Stopped) Stop();

            memoryTimer = new Timer(_sampleDuration);
            memoryTimer.AutoReset = true;
            memoryTimer.Elapsed += memoryTimerElapsed;

            if (capacity == 0) capacity = (int)(capture.WaveFormat.AverageBytesPerSecond * (_sampleDuration / 1000.00));

            memoryStream = new MemoryStream(capacity);

            capture.DataAvailable += onDataAvailable;

            memoryTimer.Start();
            capture.StartRecording();
        }

        private static void memoryTimerElapsed(object sender, ElapsedEventArgs e)
        {
            SaveTempFile();
        }

        private static void onDataAvailable(object sender, WaveInEventArgs e)
        {
            if (memoryStream.CanWrite)
            {
                memoryStream.Write(e.Buffer, 0, e.BytesRecorded);
            }
        }

        private static string StopAndSaveAudioRecording()
        {
            if (capture.CaptureState != CaptureState.Capturing) return "NOT RECORDING";

            memoryTimer.Enabled = false;

            capture.DataAvailable -= onDataAvailable;

            SaveTempFile();

            memoryStream.Close();

            memoryStream = new MemoryStream(capacity);

            memoryStream.Position = 0;

            WaveStream waveStream1 = null;

            if (File.Exists(Path.Combine(_samplesTempFolder, _sampleFileNames.Last())))
            {
                using WaveFileReader waveReader1 = new WaveFileReader(Path.Combine(_samplesTempFolder, _sampleFileNames.Last()));

                waveReader1.Position = 0;

                waveStream1 = new RawSourceWaveStream(waveReader1, waveReader1.WaveFormat);
            }

            using WaveFileReader waveReader2 = new WaveFileReader(Path.Combine(_samplesTempFolder, _sampleFileNames.First()));

            waveReader2.Position = 0;

            using WaveStream waveStream2 = new RawSourceWaveStream(waveReader2, waveReader2.WaveFormat);

            if (waveStream1 != null)
            {
                double millisecondsLeft = (_sampleDuration - waveStream2.TotalTime.TotalMilliseconds);
                double start = _sampleDuration - millisecondsLeft;

                Tuple<int, int> tuple = Tuple.Create(TimeSpanToOffset(TimeSpan.FromMilliseconds(start), waveStream1.WaveFormat), TimeSpanToOffset(TimeSpan.FromMilliseconds(millisecondsLeft), waveStream1.WaveFormat));

                byte[] buffer = new byte[waveStream1.Length];
                ReadSegment(buffer, tuple, waveStream1);

                memoryStream.Write(buffer, 0, buffer.Length);
            };

            waveStream2.CopyTo(memoryStream);

            string fileName = String.Format("Recording-({0}).wav", DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH_mm_ss"));
            string filePath = Path.Combine(_recordingsFolder, fileName);

            SaveToFile(filePath);

            waveStream1.Dispose();

            capture.DataAvailable += onDataAvailable;

            memoryTimer.Enabled = true;

            return filePath;
        }

        private static void SaveTempFile()
        {
            if (File.Exists(Path.Combine(_samplesTempFolder, _sampleFileNames.Last())))
            {
                try
                {
                    File.Delete(Path.Combine(_samplesTempFolder, _sampleFileNames.Last()));
                }
                catch (Exception e)
                {
                    Logger.Log("Error while trying to delete temporary recording file! Error: " + e.Message);
                }
            }

            if (File.Exists(Path.Combine(_samplesTempFolder, _sampleFileNames.First())))
            {
                try
                {
                    File.Move(Path.Combine(_samplesTempFolder, _sampleFileNames.First()), Path.Combine(_samplesTempFolder, _sampleFileNames.Last()));
                }
                catch (Exception e)
                {
                    Logger.Log("Error while trying to rename temporary recording file! Error: " + e.Message);
                }
            }

            try
            {
                string filePath = Path.Combine(_samplesTempFolder, _sampleFileNames.First());

                SaveToFile(filePath);
            }
            catch (Exception e)
            {
                Logger.Log("Error while trying to save new temporary recording file! Error: " + e.Message);
            }
        }

        private static void SaveToFile(string filePath)
        {
            using (RawSourceWaveStream waveStream = new RawSourceWaveStream(memoryStream, capture.WaveFormat))
            {
                waveStream.Position = 0;
                using (WaveFileWriter writer = new WaveFileWriter(filePath, capture.WaveFormat))
                {
                    waveStream.CopyTo(writer);
                    memoryStream.Close();
                    memoryStream = new MemoryStream(capacity);
                };
            };
        }

        private static int ReadSegment(byte[] buffer, Tuple<int, int> tuple, WaveStream sourceStream)
        {
            var (segmentStart, segmentLength) = tuple;
            var bytesAvailable = (int)(segmentStart + segmentLength - sourceStream.Position);

            return sourceStream.Read(buffer, 0, bytesAvailable);
        }

        private static int TimeSpanToOffset(TimeSpan ts, WaveFormat waveFormat)
        {
            var bytes = (int)(waveFormat.AverageBytesPerSecond * ts.TotalSeconds);
            bytes -= (bytes % waveFormat.BlockAlign);
            return bytes;
        }

        internal static string Clip()
        {
            return StopAndSaveAudioRecording();
        }

        internal static void Stop()
        {
            if (memoryTimer != null)
            {
                memoryTimer.Stop();
                memoryTimer.Close();
                memoryTimer.Dispose();
                memoryTimer = null;
            }

            if (capture != null)
            {
                capture.StopRecording();
                capture.Dispose();
                capture = null;
            }

            if (memoryStream != null)
            {
                memoryStream.Close();
                memoryStream.Dispose();
                memoryStream = null;
            }
        }
    }
}
