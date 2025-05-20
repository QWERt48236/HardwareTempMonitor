using HardwareTempMonitor.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HardwareTempMonitor.ViewModels
{
    class MainViewModel:INotifyPropertyChanged
    {
        private Visibility _CPUVisibility;
        private Visibility _networkVisibility;

        public CPUViewModel CPUViewModel { get; } = new CPUViewModel();
        public NetworkViewModel NetworkViewModel { get; } = new NetworkViewModel();

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainViewModel()
        {
            CPUTemperatureCommand = new RelayCommand(ShowCPUTemperature);
            CPULoadCommand = new RelayCommand(ShowCPULoad);
            NetworkLoadCommand = new RelayCommand(ShowNetwork);

            CPUVisibility = Visibility.Collapsed;
            NetworkVisibility = Visibility.Collapsed;
        }

        public Visibility CPUVisibility
        {
            get
            {
                return _CPUVisibility;
            }
            set
            {
                _CPUVisibility = value;
                OnPropertyChanged("CPUVisibility");
            }
        }

        public Visibility NetworkVisibility
        {
            get
            {
                return _networkVisibility;
            }
            set
            {
                _networkVisibility = value;
                OnPropertyChanged("NetworkVisibility");
            }
        }

        public ICommand CPUTemperatureCommand { get; }
        public ICommand CPULoadCommand { get; }
        public ICommand NetworkLoadCommand { get; }

        protected virtual void OnPropertyChanged(string Name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }

        private void ShowCPUTemperature(object obj)
        {
            Debug.WriteLine("1");

            CPUVisibility = Visibility.Visible;
            NetworkVisibility = Visibility.Collapsed;

            CPUViewModel.ShowCPUTemperature(obj);
        }

        private void ShowCPULoad(object obj)
        {
            CPUVisibility = Visibility.Visible;
            NetworkVisibility = Visibility.Collapsed;

            CPUViewModel.ShowCPULoad(obj);
        }

        private void ShowNetwork(object obj)
        {
            Debug.WriteLine("2");

            CPUVisibility = Visibility.Collapsed;
            NetworkVisibility = Visibility.Visible;
        }
    }
}
