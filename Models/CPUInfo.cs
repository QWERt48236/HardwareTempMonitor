using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using LibreHardwareMonitor.Hardware;
using HardwareTempMonitor.ViewModels;

namespace HardwareTempMonitor.Models
{
    class CPUInfo : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public int GetCPUTemperature()
        {
            Computer _computer = new Computer();
            _computer.IsCpuEnabled = true;
            _computer.Open();

            List<int> cpuTemps = new List<int>();

            foreach(Hardware hardwareItem in _computer.Hardware)
            {
                hardwareItem.Update();
                if (hardwareItem.HardwareType == HardwareType.Cpu)
                {
                    foreach (ISensor sensor in hardwareItem.Sensors)
                    {
                        if(sensor.SensorType == SensorType.Temperature)
                        {
                            cpuTemps.Add((int)sensor.Value);
                        }
                    }
                }

            }
            _computer.Close();

            int CPUTemperature = cpuTemps.Sum() / cpuTemps.Count;
            return CPUTemperature;
        }
    }
}
