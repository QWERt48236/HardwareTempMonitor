using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibreHardwareMonitor.Hardware;

namespace HardwareTempMonitor.Models.RAM
{
    class RAMMeasureModel
    {
        public float GetMemoryUsed(Computer computer)
        {
            float? memoryUsed = 0;
            foreach (IHardware hardwareItem in computer.Hardware)
            {
                hardwareItem.Update();
                if (hardwareItem.HardwareType == HardwareType.Memory)
                {
                    memoryUsed = (from sencor in hardwareItem.Sensors
                                  where sencor.Name == "Memory Used" && sencor.SensorType == SensorType.Data
                                  select sencor.Value).Single();
                }
            }
            return memoryUsed == null ? 0 : (float)memoryUsed;
        }

        public float GetMemoryAvailable(Computer computer)
        {
            float? memoryAvailable = 0;
            foreach (IHardware hardwareItem in computer.Hardware)
            {
                hardwareItem.Update();
                if (hardwareItem.HardwareType == HardwareType.Memory)
                {
                    memoryAvailable = (from sencor in hardwareItem.Sensors
                                       where sencor.Name == "Memory Available" && sencor.SensorType == SensorType.Data
                                       select sencor.Value).Single();
                }
            }
            return memoryAvailable == null ? 0 : (float)memoryAvailable;
        }
    }
}
