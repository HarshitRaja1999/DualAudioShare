using NAudio.CoreAudioApi;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DualAudioShare
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AudioRouter _router;
        private MMDeviceCollection? _devices;

        public MainWindow()
        {
            _router = new AudioRouter();
            InitializeComponent();

            LoadDevices();
        }

        private void LoadDevices()
        {
            try
            {
                _devices = _router.GetRenderDevices();

                SourceDeviceCombo.ItemsSource = _devices;
                MirrorDeviceCombo.ItemsSource = _devices;

                // Default: source = default device, mirror = first non-default (if any)
                if (_devices != null && _devices.Count > 0)
                {
                    var defaultDevice = _devices.FirstOrDefault(d => d.ID == GetDefaultRenderDeviceId());
                    if (defaultDevice != null)
                    {
                        SourceDeviceCombo.SelectedItem = defaultDevice;
                    }
                    else
                    {
                        SourceDeviceCombo.SelectedIndex = 0;
                    }

                    // Mirror device: pick another device if available
                    if (_devices.Count > 1)
                    {
                        var mirrorDevice = _devices.FirstOrDefault(d => d != SourceDeviceCombo.SelectedItem);
                        MirrorDeviceCombo.SelectedItem = mirrorDevice;
                    }
                }

                if (SourceDeviceCombo.SelectedItem is MMDevice srcDev)
                {
                    try
                    {
                        float vol = srcDev.AudioEndpointVolume.MasterVolumeLevelScalar;
                        SourceVolumeSlider.Value = vol;
                        SourceVolumeLabel.Text = $"{(int)(vol * 100)}%";
                    }
                    catch
                    {
                        // Some devices might not expose endpoint volume;
                        // leave slider as-is or disable it if you want.
                        SourceVolumeLabel.Text = "N/A";
                    }
                }


                UpdateStatus("Devices loaded. Configure and press Start.");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error loading devices: {ex.Message}");
            }
        }

        private string? GetDefaultRenderDeviceId()
        {
            using var enumerator = new MMDeviceEnumerator();
            var defaultDev = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            return defaultDev?.ID;
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_router.IsRunning)
            {
                StartSharing();
            }
            else
            {
                StopSharing();
            }
        }


        private void StartSharing()
        {
            if (_devices == null || _devices.Count == 0)
            {
                UpdateStatus("No playback devices found.");
                return;
            }

            var source = SourceDeviceCombo.SelectedItem as MMDevice;
            var mirror = MirrorDeviceCombo.SelectedItem as MMDevice;

            if (source == null || mirror == null)
            {
                UpdateStatus("Select both source and mirror devices.");
                return;
            }

            if (source.ID == mirror.ID)
            {
                UpdateStatus("Source and mirror devices must be different.");
                return;
            }

            try
            {
                _router.SetDevices(source, mirror);
                _router.Volume = (float)VolumeSlider.Value;
                _router.Start();

                ToggleButton.Content = "Stop Sharing";
                UpdateStatus($"Sharing audio from \"{source.FriendlyName}\" to \"{mirror.FriendlyName}\"...");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Failed to start sharing: {ex.Message}");
            }
        }

        private void StopSharing()
        {
            try
            {
                _router.Stop();
                ToggleButton.Content = "Start Sharing";
                UpdateStatus("Stopped. Idle.");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error stopping: {ex.Message}");
            }
        }

        //private void VolumeSlider_ValueChanged(object sender,
        //                                       RoutedPropertyChangedEventArgs<double> e)
        //{
        //    if (!IsLoaded || VolumeLabel == null || _router == null)
        //        return;

        //    var vol = (float)e.NewValue;
        //    _router.Volume = vol;
        //    VolumeLabel.Text = $"{(int)(vol * 100)}%";
        //}

        //private void SourceVolumeSlider_ValueChanged(object sender,
        //                                     RoutedPropertyChangedEventArgs<double> e)
        //{
        //    // Ignore events during initialization
        //    if (!IsLoaded || SourceVolumeLabel == null)
        //        return;

        //    var srcDev = SourceDeviceCombo.SelectedItem as MMDevice;
        //    if (srcDev == null)
        //        return;

        //    float vol = (float)e.NewValue;

        //    try
        //    {
        //        srcDev.AudioEndpointVolume.MasterVolumeLevelScalar = vol;
        //        SourceVolumeLabel.Text = $"{(int)(vol * 100)}%";
        //    }
        //    catch
        //    {
        //        SourceVolumeLabel.Text = "N/A";
        //    }
        //}
        private void SourceDeviceCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!IsLoaded || SourceVolumeSlider == null || SourceVolumeLabel == null)
                return;

            if (SourceDeviceCombo.SelectedItem is MMDevice srcDev)
            {
                try
                {
                    float vol = srcDev.AudioEndpointVolume.MasterVolumeLevelScalar;
                    SourceVolumeSlider.Value = vol;
                    SourceVolumeLabel.Text = $"{(int)(vol * 100)}%";
                }
                catch
                {
                    SourceVolumeLabel.Text = "N/A";
                }
            }
        }

        private void SourceVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded || SourceVolumeLabel == null)
                return;

            var srcDev = SourceDeviceCombo.SelectedItem as MMDevice;
            if (srcDev == null) return;

            float vol = (float)e.NewValue;
            try
            {
                srcDev.AudioEndpointVolume.MasterVolumeLevelScalar = vol;
                SourceVolumeLabel.Text = $"{(int)(vol * 100)}%";
            }
            catch
            {
                SourceVolumeLabel.Text = "N/A";
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded || VolumeLabel == null || _router == null)
                return;

            var vol = (float)e.NewValue;
            _router.Volume = vol;
            VolumeLabel.Text = $"{(int)(vol * 100)}%";
        }

        private void UpdateStatus(string msg)
        {
            StatusText.Text = msg;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _router.Dispose();
        }
    }
}