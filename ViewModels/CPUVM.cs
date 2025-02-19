using HardwareTempMonitor.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot.Wpf;
using OxyPlot.Series;
using System.Timers;
using HardwareTempMonitor.Views;
using OxyPlot;
using OxyPlot.Axes;

namespace HardwareTempMonitor.ViewModels
{
    public class CPUVM : INotifyPropertyChanged
    {
        private float? _temperature;
        private PlotModel _cpuTemperature = new PlotModel();

        private CPUInfo CPUInfo = new();
        private static System.Timers.Timer _timer;

        //public float? Temperature 
        //{
        //    get
        //    {
        //        float? cputemp = CPUInfo.GetCPUTemperature();

        //        if (cputemp != null)
        //        {
        //            return float.Round((float)cputemp, 2);
        //        }
        //        return 0;
        //    }
        //    set
        //    {
        //        _temperature = value;
        //        OnPropertyChanged("Temperature");
        //    }
        //}
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

        public event PropertyChangedEventHandler PropertyChanged;

        public CPUVM()
        {
            SetTimer();
        } 

        private void SetTimer()
        {
            _timer = new System.Timers.Timer(3000);
            _timer.Elapsed += BuildCPUTemperaturePlot;

            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        protected virtual void OnPropertyChanged(string Name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }

        private void BuildCPUTemperaturePlot(object? s, ElapsedEventArgs e)
        {
            float? cputemp = CPUInfo.GetCPUTemperature();

            if (!CPUTemperature.Series.Any())
            {
                CPUTemperature.Series.Add(new LineSeries());
            }

            var lineSeries = CPUTemperature.Series.First() as LineSeries;
            lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), (double)cputemp));

            CPUTemperature.InvalidatePlot(true);
        }
    }
}
