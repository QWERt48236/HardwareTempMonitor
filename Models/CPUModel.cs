using System.ComponentModel;
using LibreHardwareMonitor.Hardware;

namespace HardwareTempMonitor.Models
{
    class CPUModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public float? GetCPUTemperature()
        {
            Computer _computer = new Computer();

            _computer.IsCpuEnabled = true;
            _computer.Open();

            List<float?> cpuTemps = new List<float?>();

            foreach (Hardware hardwareItem in _computer.Hardware)
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
            _computer.Close();

            return cpuTemps.Sum() / cpuTemps.Count;
        }
    }
}
