using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using LibreHardwareMonitor.Hardware;
using HardwareTempMonitor.ViewModels;
using System.Windows;

namespace HardwareTempMonitor.Models
{
    class CPUInfo
    {
        public float? GetCPUTemperature()
        {
            Computer _computer = new Computer();

            if (_computer == null)
                return 0;

            _computer.IsCpuEnabled = true;
            _computer.Open();

            List<float?> cpuTemps = new List<float?>();

            foreach(Hardware hardwareItem in _computer.Hardware)
            {
                hardwareItem.Update();

                if (hardwareItem.HardwareType == HardwareType.Cpu)
                {
                    foreach (ISensor sensor in hardwareItem.Sensors)
                    {
                        if(sensor.SensorType == SensorType.Temperature)
                        {
                            cpuTemps.Add(sensor.Value);
                        }
                    }
                }

            }
            _computer.Close();

            float? CPUTemperature = cpuTemps.Sum() / cpuTemps.Count;
            return CPUTemperature;
        }
    }
}
