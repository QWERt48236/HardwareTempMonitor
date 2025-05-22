using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareTempMonitor.Models.CPU
{
    internal struct CPUDataModel
    {
        public float Temperature { get; set; }
        public float Load { get; set; }
        public string Characteriscs { get; set; }
    }
}
