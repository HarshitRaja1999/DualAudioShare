using System;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace DualAudioShare
{
    public class AudioRouter : IDisposable
    {
        private readonly MMDeviceEnumerator _deviceEnumerator;
        private WasapiLoopbackCapture? _capture;
        private WasapiOut? _mirrorOut;
        private BufferedWaveProvider? _mirrorProvider;

        public MMDevice? SourceDevice { get; private set; }
        public MMDevice? MirrorDevice { get; private set; }

        public float Volume
        {
            get => _mirrorOut?.Volume ?? 1.0f;
            set
            {
                if (_mirrorOut != null)
                {
                    _mirrorOut.Volume = value;
                }
            }
        }

        public bool IsRunning { get; private set; }

        public AudioRouter()
        {
            _deviceEnumerator = new MMDeviceEnumerator();
        }

        public MMDeviceCollection GetRenderDevices()
        {
            // All active render (playback) devices
            return _deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
        }

        public void SetDevices(MMDevice source, MMDevice mirror)
        {
            if (IsRunning)
                throw new InvalidOperationException("Cannot change devices while running.");

            SourceDevice = source;
            MirrorDevice = mirror;
        }

        public void Start()
        {
            if (IsRunning)
                return;

            if (SourceDevice == null || MirrorDevice == null)
                throw new InvalidOperationException("Source and mirror devices must be set before starting.");

            // Loopback capture from source
            _capture = new WasapiLoopbackCapture(SourceDevice);
            _capture.DataAvailable += CaptureOnDataAvailable;
            _capture.RecordingStopped += CaptureOnRecordingStopped;

            // Buffered provider for mirror device
            _mirrorProvider = new BufferedWaveProvider(_capture.WaveFormat)
            {
                DiscardOnBufferOverflow = true
            };

            // Output to mirror device
            _mirrorOut = new WasapiOut(MirrorDevice, AudioClientShareMode.Shared, true, 20);
            _mirrorOut.Init(_mirrorProvider);
            _mirrorOut.Volume = 0.8f;

            _mirrorOut.Play();
            _capture.StartRecording();

            IsRunning = true;
        }

        public void Stop()
        {
            if (!IsRunning)
                return;

            IsRunning = false;

            if (_capture != null)
            {
                _capture.StopRecording();
            }
        }

        private void CaptureOnDataAvailable(object? sender, WaveInEventArgs e)
        {
            if (_mirrorProvider != null && e.BytesRecorded > 0)
            {
                _mirrorProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);
            }
        }

        private void CaptureOnRecordingStopped(object? sender, StoppedEventArgs e)
        {
            _capture?.Dispose();
            _capture = null;

            _mirrorOut?.Stop();
            _mirrorOut?.Dispose();
            _mirrorOut = null;

            _mirrorProvider = null;
        }

        public void Dispose()
        {
            Stop();
            _deviceEnumerator.Dispose();
        }
    }
}
