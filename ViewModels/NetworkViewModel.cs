using HardwareTempMonitor.Commands;
using HardwareTempMonitor.Models;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace HardwareTempMonitor.ViewModels
{
    class NetworkViewModel: INotifyPropertyChanged
    {
        public Visibility _visibility;

        private PlotModel _network = new PlotModel();

        private DateTime startTime = DateTime.Now;

        public event PropertyChangedEventHandler? PropertyChanged;

        public NetworkViewModel()
        {
            InitializePlot();

            Visibility = Visibility.Collapsed;

            MainMonitoringModel.OnMonitoringDataUpdate += BuildCPUTemperaturePlot;
        }

        public Visibility Visibility
        {
            get
            {
                return _visibility;
            }
            set
            {
                _visibility = value;
                OnPropertyChanged("Visibility");
            }
        }

        public PlotModel NetworkPlot
        {
            get
            {
                return _network;
            }
            set
            {
                _network = value;
                OnPropertyChanged("NetworkPlot");
            }
        }

        public ICommand NetworkCommand { get; }

        protected virtual void OnPropertyChanged(string Name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }

        private void BuildCPUTemperaturePlot(LinkedList<MonitoringDataModel> monitoringDataModels)
        {
            float cpuTemp = monitoringDataModels.Last().CPU.Temperature;

            Application.Current.Dispatcher.Invoke(new Action(() => {

                var lineSeries = NetworkPlot.Series.FirstOrDefault() as LineSeries;

                if (lineSeries == null)
                    return;

                if (lineSeries.Points.Count <= 120)
                {
                    lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), (double)cpuTemp));

                    NetworkPlot.InvalidatePlot(true);
                }
                else
                {
                    lineSeries.Points.Remove(lineSeries.Points.First());
                    lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), (double)cpuTemp));

                    NetworkPlot.InvalidatePlot(true);
                }
            }));
        }


        private void InitializePlot()
        {
            NetworkPlot = new PlotModel();
            NetworkPlot.Series.Add(new LineSeries()
            {
                StrokeThickness = 2,
                Color = OxyColors.Crimson
            });

            NetworkPlot.Subtitle = "CPU temperature";

            NetworkPlot.Axes.Add(new DateTimeAxis
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
            NetworkPlot.Axes.Add(new LinearAxis
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

            NetworkPlot.InvalidatePlot(true);
        }
    }
}
