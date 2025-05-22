using HardwareTempMonitor.Models;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HardwareTempMonitor.ViewModels
{
    class RAMViewModel : INotifyPropertyChanged
    {
        private PlotModel _ramLoad = new PlotModel();

        private DateTime startTime = DateTime.Now;

        public event PropertyChangedEventHandler? PropertyChanged;

        public RAMViewModel()
        {
            InitializeLoadPlot();

            MainMonitoringModel.OnMonitoringDataUpdate += BuildRAMLoadPlot;
        }

        public PlotModel RAMLoadPlot
        {
            get
            {
                return _ramLoad;
            }
            set
            {
                _ramLoad = value;
                OnPropertyChanged("RAMLoadPlot");
            }
        }

        protected virtual void OnPropertyChanged(string Name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }

        private void BuildRAMLoadPlot(LinkedList<MonitoringDataModel> monitoringDataModels)
        {
            float ramLoad = monitoringDataModels.Last().RAM.MemoryUsed;

            Application.Current.Dispatcher.Invoke(new Action(() => {

                var lineSeries = RAMLoadPlot.Series.FirstOrDefault() as LineSeries;

                if (lineSeries == null)
                    return;

                if (lineSeries.Points.Count <= 30)
                {
                    lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), ramLoad));

                    RAMLoadPlot.InvalidatePlot(true);
                }
                else
                {
                    lineSeries.Points.Remove(lineSeries.Points.First());
                    lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), ramLoad));

                    RAMLoadPlot.InvalidatePlot(true);
                }
            }));
        }

        private void InitializeLoadPlot()
        {
            RAMLoadPlot = new PlotModel();
            RAMLoadPlot.Series.Add(new LineSeries()
            {
                StrokeThickness = 3,
                Color = OxyColors.DarkOliveGreen
            });

            RAMLoadPlot.Subtitle = "RAM load";
            RAMLoadPlot.DefaultFont = "Cascadia Code Extra";
            RAMLoadPlot.SubtitleFontSize = 20;

            RAMLoadPlot.Axes.Add(new DateTimeAxis
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
            RAMLoadPlot.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Load, Gb",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColors.LightGray,
                MinorGridlineColor = OxyColor.FromAColor(80, OxyColors.LightGray),
                MajorGridlineThickness = 1,
                MinorGridlineThickness = 0.5,
                Minimum = 0,
                Maximum = 16,
                TitleFontSize = 16,
                FontSize = 12
            });

            RAMLoadPlot.InvalidatePlot(true);

            for (int i = 30; i > 0; i--)
            {
                (RAMLoadPlot.Series.FirstOrDefault() as LineSeries).Points.Add(new DataPoint(DateTimeAxis.ToDouble(startTime.Subtract(TimeSpan.FromSeconds(i))), 0));
            }
        }
    }
}
