using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareTempMonitor.Models.Network
{
    internal struct NetworkDataModel
    {
        public float UploadSpeed {  get; set; }
        public float DownloadSpeed { get; set; }
    }
}
