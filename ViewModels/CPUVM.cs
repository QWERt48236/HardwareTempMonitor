using HardwareTempMonitor.Models;
using System.ComponentModel;
using OxyPlot.Series;
using OxyPlot;
using OxyPlot.Axes;
using System.Windows;
using System.Windows.Threading;

namespace HardwareTempMonitor.ViewModels
{
    public class CPUVM : INotifyPropertyChanged
    {
        private static DispatcherTimer _dispatcherTimer;
        private PlotModel _cpuTemperature = new PlotModel();
        private CPUModel _cpuInfo = new();

        public CPUVM()
        {
            SetTimer();

            CPUTemperature = new PlotModel();
            CPUTemperature.Series.Add(new LineSeries());
            CPUTemperature.InvalidatePlot(true);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public PlotModel CPUTemperature
        {
            get
            {
                return _cpuTemperature;
            }
            set
            {
                _cpuTemperature = value;
                OnPropertyChanged("CPUTemperature");
            }
        }

        protected virtual void OnPropertyChanged(string Name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }

        private void SetTimer()
        {
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            _dispatcherTimer.Tick += BuildCPUTemperaturePlot;
            _dispatcherTimer.Start();
        }

        private void BuildCPUTemperaturePlot(object o, EventArgs e)
        {
            float? cpuTemp = _cpuInfo.GetCPUTemperature();

            Application.Current.Dispatcher.Invoke(() =>
            {
                var lineSeries = CPUTemperature.Series.FirstOrDefault() as LineSeries;

                if (lineSeries != null)
                {
                    lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), (double)cpuTemp));

                    CPUTemperature.InvalidatePlot(true);
                }
            });
        }
    }
}
