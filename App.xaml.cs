using HardwareTempMonitor.Models;
using System.Configuration;
using System.Data;
using System.Windows;

namespace HardwareTempMonitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Task.Run(async () => await MainMonitoringModel.StartMonitoring());
        }
    }

}
