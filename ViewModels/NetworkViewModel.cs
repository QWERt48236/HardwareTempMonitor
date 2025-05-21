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
        private PlotModel _network = new PlotModel();

        private DateTime startTime = DateTime.Now;

        public event PropertyChangedEventHandler? PropertyChanged;

        public NetworkViewModel()
        {
            InitializePlot();

            MainMonitoringModel.OnMonitoringDataUpdate += BuildNetworkPlot;
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

        private void BuildNetworkPlot(LinkedList<MonitoringDataModel> monitoringDataModels)
        {
            float downlodSpeed = monitoringDataModels.Last().Network.DownloadSpeed / 1000_000;
            float uploadSpeed = monitoringDataModels.Last().Network.UploadSpeed / 1000_000;

            Application.Current.Dispatcher.Invoke(new Action(() => {

                var downloadLineSeries = NetworkPlot.Series[0] as LineSeries;
                var uploadLineSeries = NetworkPlot.Series[1] as LineSeries;

                if (downloadLineSeries == null || uploadLineSeries == null)
                    return;

                if (downloadLineSeries.Points.Count <= 30)
                {
                    downloadLineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), (double)downlodSpeed));
                    uploadLineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), (double)uploadSpeed));

                    NetworkPlot.InvalidatePlot(true);
                }
                else
                {
                    downloadLineSeries.Points.Remove(downloadLineSeries.Points.First());
                    uploadLineSeries.Points.Remove(uploadLineSeries.Points.First());

                    downloadLineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), (double)downlodSpeed));
                    uploadLineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), (double)uploadSpeed));

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

            NetworkPlot.Series.Add(new LineSeries()
            {
                StrokeThickness = 2,
                Color = OxyColors.DarkGreen
            });

            NetworkPlot.Subtitle = "Network";

            NetworkPlot.Axes.Add(new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
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
                Title = "Downlad Mb",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColors.LightGray,
                MinorGridlineColor = OxyColor.FromAColor(80, OxyColors.LightGray),
                MajorGridlineThickness = 1,
                MinorGridlineThickness = 0.5,
                Minimum = 0
            });
            NetworkPlot.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Right,
                Title = "Upload Mb",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColors.LightGray,
                MinorGridlineColor = OxyColor.FromAColor(80, OxyColors.LightGray),
                MajorGridlineThickness = 1,
                MinorGridlineThickness = 0.5,
                Minimum = 0
            });

            NetworkPlot.InvalidatePlot(true);

            var downloadLineSeries = NetworkPlot.Series[0] as LineSeries;
            var uploadLineSeries = NetworkPlot.Series[1] as LineSeries;

            for(int i = 30; i > 0; i--)
            {
                downloadLineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(startTime.Subtract(TimeSpan.FromSeconds(i))), 0));
                uploadLineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(startTime.Subtract(TimeSpan.FromSeconds(i))), 0));
            }
        }
    }
}
