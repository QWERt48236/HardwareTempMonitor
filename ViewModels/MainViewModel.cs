using HardwareTempMonitor.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HardwareTempMonitor.ViewModels
{
    class MainViewModel
    {


        public MainViewModel()
        {
            CPUTemperatureCommand = new RelayCommand(ShowCPUTemperature);
            CPULoadCommand = new RelayCommand(ShowCPULoad);
        }

        public ICommand CPUTemperatureCommand { get; }
        public ICommand CPULoadCommand { get; }
        public ICommand NetworkLoadCommand { get; }

        private void ShowCPUTemperature(object obj)
        {

        }

        private void ShowCPULoad(object obj)
        {

        }
    }
}
