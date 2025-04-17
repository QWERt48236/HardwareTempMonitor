using LibreHardwareMonitor.Hardware;
using LibreHardwareMonitor.Hardware.Motherboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareTempMonitor.Models
{
    class NetworkMeasureModel
    {
        public float GetUploadSpeed(Computer computer)
        {
            float? uploadSpeed = 0;

            foreach (IHardware hardwareItem in computer.Hardware)
            {
                hardwareItem.Update();

                if (hardwareItem.HardwareType == HardwareType.Network)
                {
                    uploadSpeed = (from sencor in hardwareItem.Sensors
                                   where sencor.Name == "Upload Speed" && sencor.SensorType == SensorType.Throughput
                                   select sencor.Value).Single();
                }
            }

            return uploadSpeed == null ? 0 : (float)uploadSpeed;
        }

        public float GetDownloadSpeed(Computer computer)
        {
            float? downloadSpeed = 0;

            foreach (IHardware hardwareItem in computer.Hardware)
            {
                hardwareItem.Update();

                if (hardwareItem.HardwareType == HardwareType.Network)
                {
                    downloadSpeed = (from sencor in hardwareItem.Sensors
                                   where sencor.Name == "Download Speed" && sencor.SensorType == SensorType.Throughput
                                     select sencor.Value).Single();
                }
            }

            return downloadSpeed == null ? 0 : (float)downloadSpeed;
        }
    }
}
