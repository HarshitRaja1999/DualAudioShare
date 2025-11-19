using System.Windows;
using ModernWPF;


namespace DualAudioShare
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Fix: Use the correct overload of ApplyTheme with both theme and predefined accent name
            ModernWPF.ModernTheme.ApplyTheme(ModernTheme.Theme.Dark, "Blue");
        }
    }

}
