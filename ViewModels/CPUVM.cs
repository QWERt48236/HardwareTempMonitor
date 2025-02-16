using HardwareTempMonitor.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareTempMonitor.ViewModels
{
    class CPUVM : INotifyPropertyChanged
    {
        private CPUInfo CPUInfo = new();

        private static System.Timers.Timer timer;

        private int _temperature;
        public int Temperature 
        {
            get
            {
                if (!timer.Enabled)
                {
                    SetTimer();
                    return CPUInfo.GetCPUTemperature();
                }

                return _temperature;
            }
            set
            {
                _temperature = value;
                OnPropertyChanged("Temperature");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public CPUVM()
        {
            SetTimer();
        } 

        private void SetTimer()
        {
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += (s, e) => { Temperature = CPUInfo.GetCPUTemperature(); };
            timer.AutoReset = true;
            timer.Enabled = true;
            Temperature = CPUInfo.GetCPUTemperature();
        }

        protected virtual void OnPropertyChanged(string Name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }
    }
}
