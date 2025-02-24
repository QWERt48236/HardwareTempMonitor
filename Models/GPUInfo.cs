using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareTempMonitor.Models
{
    class GPUInfo
    {
        public float GetGPUTemperature()
        {
            Computer computer = new Computer();

            computer.IsGpuEnabled = true;
            computer.Open();

            List<float?> gpuTemps = new List<float?>();

            foreach (Hardware hardwareItem in computer.Hardware)
            {
                hardwareItem.Update();

                if (hardwareItem.HardwareType == HardwareType.GpuIntel 
                    || hardwareItem.HardwareType == HardwareType.GpuAmd 
                    || hardwareItem.HardwareType == HardwareType.GpuNvidia)
                {
                    foreach (ISensor sensor in hardwareItem.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            gpuTemps.Add(sensor.Value);
                        }
                    }
                }
            }

            computer.Close();

            return (gpuTemps.Sum() / gpuTemps.Count) == null ? 0 : (float)(gpuTemps.Sum() / gpuTemps.Count);
        }
    }
}
