using LibreHardwareMonitor.Hardware;
using LibreHardwareMonitor.Hardware.Motherboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareTempMonitor.Models
{
    class NetworkModel
    {
        public float GetMotherboardTemperature()
        {
            Computer computer = new Computer
            {
                IsCpuEnabled = true,
                IsMotherboardEnabled = true,
                IsGpuEnabled = true,
                IsStorageEnabled = true,
                IsMemoryEnabled = true,
                IsNetworkEnabled = true
            };

            computer.Open();

            List<float?> motherboardTemps = new List<float?>();

            foreach (IHardware hardwareItem in computer.Hardware)
            {
                hardwareItem.Update();

                if (hardwareItem.HardwareType == HardwareType.Network)
                {
                    foreach (ISensor sensor in hardwareItem.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Data)
                        {
                            motherboardTemps.Add(sensor.Value);
                        }
                    }
                }
            }

            computer.Close();

            return (motherboardTemps.Sum() / motherboardTemps.Count) == null ? 0 : (float)(motherboardTemps.Sum() / motherboardTemps.Count);
        }
    }
}
