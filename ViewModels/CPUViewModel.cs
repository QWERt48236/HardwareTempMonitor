using HardwareTempMonitor.Models;
using System.ComponentModel;
using OxyPlot.Series;
using OxyPlot;
using OxyPlot.Axes;
using System.Windows;
using System.Windows.Threading;

namespace HardwareTempMonitor.ViewModels
{
    public class CPUViewModel : INotifyPropertyChanged
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. 
        private static DispatcherTimer _dispatcherTimer;
#pragma warning restore CS8618 // It is containing a non-null value when exiting constructor.
        private PlotModel _cpuTemperature = new PlotModel();
        private CPUModel _cpuInfo = new();

        public CPUViewModel()
        {
            SetTimer();
            InitializePlot();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

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

        private void BuildCPUTemperaturePlot(object? o, EventArgs e)
        {
            float cpuTemp = _cpuInfo.GetCPUTemperature();

            var lineSeries = CPUTemperature.Series.FirstOrDefault() as LineSeries;

            if (lineSeries != null)
            {
                lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), (double)cpuTemp));

                CPUTemperature.InvalidatePlot(true);
            }
        }

        private void InitializePlot()
        {
            CPUTemperature = new PlotModel();
            CPUTemperature.Series.Add(new LineSeries());

            CPUTemperature.Subtitle = "CPU temperature";

            CPUTemperature.Axes.Add(new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Time",
            });
            CPUTemperature.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Temperature °C",
            });

            CPUTemperature.InvalidatePlot(true);
        }
    }
}
