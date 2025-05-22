using System.ComponentModel;
using LibreHardwareMonitor.Hardware;

namespace HardwareTempMonitor.Models
{
    class CPUMeasureModel
    {
        public float GetCPUTemperature(Computer computer)
        {
            float? cpuAverageValue = 0;

            foreach (Hardware hardwareItem in computer.Hardware)
            {
                hardwareItem.Update();

                if (hardwareItem.HardwareType == HardwareType.Cpu)
                {
                    cpuAverageValue = (from sencor in hardwareItem.Sensors
                                               where sencor.Name.Contains("CPU Core") && !sencor.Name.Contains("Distance to TjMax") && sencor.SensorType == SensorType.Temperature
                                               select sencor.Value).Average();
                }
            }

            return cpuAverageValue == null ? 0 : (float)cpuAverageValue;
        }

        public float GetCPULoad(Computer computer)
        {
            float? cpuAverageValue = 0;

            foreach (Hardware hardwareItem in computer.Hardware)
            {
                hardwareItem.Update();

                if (hardwareItem.HardwareType == HardwareType.Cpu)
                {
                    cpuAverageValue = (from sencor in hardwareItem.Sensors
                                                where sencor.Name.Contains("CPU Core") && sencor.SensorType == SensorType.Load
                                                select sencor.Value).Average();
                }
            }

            return cpuAverageValue == null ? 0 : (float)cpuAverageValue;
        }

        public string GetCPUCharacteristics(Computer computer)
        {
            string name = string.Empty;
            string cores = string.Empty;

            foreach (Hardware hardwareItem in computer.Hardware)
            {
                hardwareItem.Update();
                if (hardwareItem.HardwareType == HardwareType.Cpu)
                {
                    name =  hardwareItem.Name;
                }
            }

            cores = computer.SMBios.Processors.ToList().LastOrDefault().CoreCount.ToString();
            string maxFrequency = (computer.SMBios.Processors.ToList().LastOrDefault().MaxSpeed/1000.0).ToString();

            return $"{name} \n" +
                $"  Cores: {cores} \n" +
                $"  Max frequency: {maxFrequency} GHz";
        }
    }
}
