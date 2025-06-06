﻿using HardwareTempMonitor.Models;
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

        private PlotModel _cpuTemperature = new PlotModel();
        private PlotModel _cpuLoad = new PlotModel();

        private string _characteristics = string.Empty;

        private DateTime startTime = DateTime.Now;

        public event PropertyChangedEventHandler? PropertyChanged;

        public CPUViewModel()
        {
            InitializeTemperaturePlot();
            InitializeLoadPlot();

            LoadVisibility = Visibility.Collapsed;
            TemperatureVisibility = Visibility.Collapsed;

            MainMonitoringModel.OnMonitoringDataUpdate += BuildCPUTemperaturePlot;
            MainMonitoringModel.OnMonitoringDataUpdate += BuildCPULoadPlot;
            MainMonitoringModel.OnMonitoringDataUpdate += SetCharacteristics;

            CPUTemperatureCommand = new RelayCommand(ShowCPUTemperature);
            CPULoadCommand = new RelayCommand(ShowCPULoad);
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

        public string Characteristics
        {
            get
            {
                return _characteristics;
            }
            set
            {
                _characteristics = value;
                OnPropertyChanged("Characteristics");
            }
        }

        public ICommand CPUTemperatureCommand { get; }
        public ICommand CPULoadCommand { get; }

        protected virtual void OnPropertyChanged(string Name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }

        public void ShowCPUTemperature(object obj)
        {
            TemperatureVisibility = Visibility.Visible;
            LoadVisibility = Visibility.Collapsed;
        }

        public void ShowCPULoad(object obj)
        {
            TemperatureVisibility = Visibility.Collapsed;
            LoadVisibility = Visibility.Visible;
        }

        private void SetCharacteristics(LinkedList<MonitoringDataModel> monitoringDataModels)
        {
            var cpu = monitoringDataModels.Last().CPU.Characteriscs;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Characteristics = cpu;
            }));
        }

        private void BuildCPUTemperaturePlot(LinkedList<MonitoringDataModel> monitoringDataModels)
        {
            float cpuTemp = monitoringDataModels.Last().CPU.Temperature;

            Application.Current.Dispatcher.Invoke(new Action(() => {

                var lineSeries = CPUTemperaturePlot.Series.FirstOrDefault() as LineSeries;

                if (lineSeries == null)
                    return;

                if (lineSeries.Points.Count <= 30)
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
            float cpuLoad = monitoringDataModels.Last().CPU.Load;

            Application.Current.Dispatcher.Invoke(new Action(() => {

                var lineSeries = CPULoadPlot.Series.FirstOrDefault() as LineSeries;

                if (lineSeries == null)
                    return;

                if (lineSeries.Points.Count <= 30)
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
            CPUTemperaturePlot.DefaultFont = "Cascadia Code Extra";
            CPUTemperaturePlot.SubtitleFontSize = 20;

            CPUTemperaturePlot.Axes.Add(new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                LabelFormatter = val =>
                {
                    var span = DateTimeAxis.ToDateTime(val) - startTime;
                    return span.TotalMinutes < 1
                        ? $"{span.Seconds}s"
                        : $"{span.Minutes}m {span.Seconds}s"; 
                },
                IntervalType = DateTimeIntervalType.Seconds,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColors.LightGray,
                MinorGridlineColor = OxyColor.FromAColor(80, OxyColors.LightGray),
                MajorGridlineThickness = 1,
                MinorGridlineThickness = 0.5,
                TitleFontSize = 16,
                FontSize = 12
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
                MinorGridlineThickness = 0.5,
                Minimum = 0,
                TitleFontSize = 16,
                FontSize = 12
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
            CPULoadPlot.DefaultFont = "Cascadia Code Extra";
            CPULoadPlot.SubtitleFontSize = 20;

            CPULoadPlot.Axes.Add(new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                LabelFormatter = val =>
                {
                    var span = DateTimeAxis.ToDateTime(val) - startTime;
                    return span.TotalMinutes < 1
                        ? $"{span.Seconds}s"
                        : $"{span.Minutes}m {span.Seconds}s";
                },
                IntervalType = DateTimeIntervalType.Seconds,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColors.LightGray,
                MinorGridlineColor = OxyColor.FromAColor(80, OxyColors.LightGray),
                MajorGridlineThickness = 1,
                MinorGridlineThickness = 0.5,
                TitleFontSize = 16,
                FontSize = 12
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
                MinorGridlineThickness = 0.5,
                Minimum = 0,
                Maximum = 100,
                TitleFontSize = 16,
                FontSize = 12
            });

            CPULoadPlot.InvalidatePlot(true);

            var temperatureLineSeries = CPUTemperaturePlot.Series.FirstOrDefault() as LineSeries;
            var loadLineSeries = CPULoadPlot.Series.FirstOrDefault() as LineSeries;

            for (int i = 30; i > 0; i--)
            {
                temperatureLineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(startTime.Subtract(TimeSpan.FromSeconds(i))), 0));
                loadLineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(startTime.Subtract(TimeSpan.FromSeconds(i))), 0));

            }
        }
    }
}
