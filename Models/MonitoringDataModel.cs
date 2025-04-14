using HardwareTempMonitor.Models.CPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareTempMonitor.Models
{
    class MonitoringDataModel
    {
        public DateTime MeasureTime { get; set; }

        public CPUDataModel CPU { get; set; }
    }
}
