using System.ComponentModel;
using LibreHardwareMonitor.Hardware;

namespace HardwareTempMonitor.Models
{
    class CPUModel
    {
        public float GetCPUTemperature()
        {
            Computer computer = new Computer();

            computer.IsCpuEnabled = true;
            computer.Open();

            List<float?> cpuTemps = new List<float?>();

            foreach (Hardware hardwareItem in computer.Hardware)
            {
                hardwareItem.Update();

                if (hardwareItem.HardwareType == HardwareType.Cpu)
                {
                    foreach (ISensor sensor in hardwareItem.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            cpuTemps.Add(sensor.Value);
                        }
                    }
                }
            }

            computer.Close();

            return (cpuTemps.Sum() / cpuTemps.Count) == null ? 0 : (float)(cpuTemps.Sum() / cpuTemps.Count);
        }

        public float GetCPULoad()
        {
            Computer computer = new Computer();

            computer.IsCpuEnabled = true;
            computer.Open();

            List<float?> cpuLoads = new List<float?>();

            foreach (Hardware hardwareItem in computer.Hardware)
            {
                hardwareItem.Update();

                if (hardwareItem.HardwareType == HardwareType.Cpu)
                {
                    foreach (ISensor sensor in hardwareItem.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Load)
                        {
                            cpuLoads.Add(sensor.Value);
                        }
                    }
                }
            }

            computer.Close();

            return (cpuLoads.Sum() / cpuLoads.Count) == null ? 0 : (float)(cpuLoads.Sum() / cpuLoads.Count);
        }
    }
}
