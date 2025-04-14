using HardwareTempMonitor.Models;
using System.ComponentModel;
using OxyPlot.Series;
using OxyPlot;
using OxyPlot.Axes;
using System.Windows;
using System.Windows.Threading;
using HardwareTempMonitor.Commands;
using System.Windows.Input;

namespace HardwareTempMonitor.ViewModels
{
    public class CPUViewModel : INotifyPropertyChanged
    {
        public Visibility _temperatureVisibility;
        public Visibility _loadVisibility;

        public ICommand CPUTemperatureCommand { get; }
        public ICommand CPULoadCommand { get; }

        private void ShowCPUTemperature(object obj)
        {
            TemperatureVisibility = Visibility.Visible;
            LoadVisibility = Visibility.Collapsed;
        }

        public void ShowCPULoad(object obj)
        {
            TemperatureVisibility = Visibility.Collapsed;
            LoadVisibility = Visibility.Visible;
        }

        public Visibility TemperatureVisibility
        {
            get
            {
                return _temperatureVisibility;
            }
            set
            {
                _temperatureVisibility = value;
                OnPropertyChanged("TemperatureVisibility");
            }
        }

        public Visibility LoadVisibility
        {
            get
            {
                return _loadVisibility;
            }
            set
            {
                _loadVisibility = value;
                OnPropertyChanged("LoadVisibility");
            }
        }

        private PlotModel _cpuTemperature = new PlotModel();
        private PlotModel _cpuLoad = new PlotModel();

        private DateTime startTime = DateTime.Now;

        public CPUViewModel()
        {
            InitializeTemperaturePlot();
            InitializeLoadPlot();

            LoadVisibility = Visibility.Collapsed;
            TemperatureVisibility = Visibility.Collapsed;


            MainMonitoringModel.OnMonitoringDataUpdate += BuildCPUTemperaturePlot;
            MainMonitoringModel.OnMonitoringDataUpdate += BuildCPULoadPlot;

            CPUTemperatureCommand = new RelayCommand(ShowCPUTemperature);
            CPULoadCommand = new RelayCommand(ShowCPULoad);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public PlotModel CPUTemperaturePlot
        {
            get
            {
                return _cpuTemperature;
            }
            set
            {
                _cpuTemperature = value;
                OnPropertyChanged("CPUTemperaturePlot");
            }
        }

        public PlotModel CPULoadPlot
        {
            get
            {
                return _cpuLoad;
            }
            set
            {
                _cpuLoad = value;
                OnPropertyChanged("CPULoadPlot");
            }
        }

        protected virtual void OnPropertyChanged(string Name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }

        private void BuildCPUTemperaturePlot(LinkedList<MonitoringDataModel> monitoringDataModels)
        {
            float cpuTemp = monitoringDataModels.Last().CPU.Temperature;

            Application.Current.Dispatcher.Invoke(new Action(() => {

                var lineSeries = CPUTemperaturePlot.Series.FirstOrDefault() as LineSeries;

                if (lineSeries == null)
                    return;

                if (lineSeries.Points.Count <= 120)
                {
                    lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), (double)cpuTemp));

                    CPUTemperaturePlot.InvalidatePlot(true);
                }
                else
                {
                    lineSeries.Points.Remove(lineSeries.Points.First());
                    lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), (double)cpuTemp));

                    CPUTemperaturePlot.InvalidatePlot(true);
                }
            }));
        }

        private void BuildCPULoadPlot(LinkedList<MonitoringDataModel> monitoringDataModels)
        {
            int cpuLoad = (int)(monitoringDataModels.Last().CPU.Load*100);

            Application.Current.Dispatcher.Invoke(new Action(() => {

                var lineSeries = CPULoadPlot.Series.FirstOrDefault() as LineSeries;

                if (lineSeries == null)
                    return;

                if (lineSeries.Points.Count <= 120)
                {
                    lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), cpuLoad));

                    CPULoadPlot.InvalidatePlot(true);
                }
                else
                {
                    lineSeries.Points.Remove(lineSeries.Points.First());
                    lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), cpuLoad));

                    CPULoadPlot.InvalidatePlot(true);
                }
            }));
        }

        private void InitializeTemperaturePlot()
        {
            CPUTemperaturePlot = new PlotModel();
            CPUTemperaturePlot.Series.Add(new LineSeries()
            {
                StrokeThickness = 2,
                Color = OxyColors.Crimson
            });

            CPUTemperaturePlot.Subtitle = "CPU temperature";

            CPUTemperaturePlot.Axes.Add(new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Time",
                LabelFormatter = val =>
                {
                    var span = DateTimeAxis.ToDateTime(val) - startTime;
                    return span.TotalMinutes < 1
                        ? $"+{span.Seconds}s"
                        : $"+{span.Minutes}m {span.Seconds}s"; 
                },
                IntervalType = DateTimeIntervalType.Seconds,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColors.LightGray,
                MinorGridlineColor = OxyColor.FromAColor(80, OxyColors.LightGray),
                MajorGridlineThickness = 1,
                MinorGridlineThickness = 0.5
            });
            CPUTemperaturePlot.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Temperature °C",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColors.LightGray,
                MinorGridlineColor = OxyColor.FromAColor(80, OxyColors.LightGray),
                MajorGridlineThickness = 1,
                MinorGridlineThickness = 0.5
            });

            CPUTemperaturePlot.InvalidatePlot(true);
        }

        private void InitializeLoadPlot()
        {
            CPULoadPlot = new PlotModel();
            CPULoadPlot.Series.Add(new LineSeries()
            {
                StrokeThickness = 2,
                Color = OxyColors.Aquamarine
            });

            CPULoadPlot.Subtitle = "CPU load";

            CPULoadPlot.Axes.Add(new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Time",
                LabelFormatter = val =>
                {
                    var span = DateTimeAxis.ToDateTime(val) - startTime;
                    return span.TotalMinutes < 1
                        ? $"+{span.Seconds}s"
                        : $"+{span.Minutes}m {span.Seconds}s";
                },
                IntervalType = DateTimeIntervalType.Seconds,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColors.LightGray,
                MinorGridlineColor = OxyColor.FromAColor(80, OxyColors.LightGray),
                MajorGridlineThickness = 1,
                MinorGridlineThickness = 0.5
            });
            CPULoadPlot.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Load %",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColors.LightGray,
                MinorGridlineColor = OxyColor.FromAColor(80, OxyColors.LightGray),
                MajorGridlineThickness = 1,
                MinorGridlineThickness = 0.5
            });

            CPULoadPlot.InvalidatePlot(true);
        }
    }
}
