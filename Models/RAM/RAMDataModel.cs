using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareTempMonitor.Models.RAM
{
    internal struct RAMDataModel
    {
        public float MemoryUsed { get; set; }
        public float MemoryAvailable { get; set; }
    }
}
