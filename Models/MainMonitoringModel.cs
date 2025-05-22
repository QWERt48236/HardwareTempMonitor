using HardwareTempMonitor.Models.CPU;
using HardwareTempMonitor.Models.Network;
using HardwareTempMonitor.Models.RAM;
using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace HardwareTempMonitor.Models
{
    static class MainMonitoringModel
    {
        public static LinkedList<MonitoringDataModel> MonitoringData { get; private set; }

        public static event Action<LinkedList<MonitoringDataModel>> OnMonitoringDataUpdate;

        private static PeriodicTimer _periodicTimer = SetTimer();
        private static Computer _computer = new Computer();

        private static CPUMeasureModel _cpuMeasureModel = new CPUMeasureModel();
        private static NetworkMeasureModel _networkMeasureModel = new NetworkMeasureModel();
        private static RAMMeasureModel _ramMeasureModel = new RAMMeasureModel();

        public static async Task StartMonitoring()
        {
            MonitoringData = new LinkedList<MonitoringDataModel>();

            while(await _periodicTimer.WaitForNextTickAsync())
            {
                await MeasureDataAsync();
                object obj= MonitoringData;

                OnMonitoringDataUpdate(MonitoringData);
            }
        }

        private static PeriodicTimer SetTimer()
        {
            PeriodicTimer periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(1));
            return periodicTimer;
        }

        private static void MeasureData()
        {
            _computer = new Computer()
            {
                IsCpuEnabled = true,
                IsNetworkEnabled = true,
                IsMemoryEnabled = true
            };

            _computer.Open();

            CPUDataModel CPUDataModel = new CPUDataModel()
            {
                Temperature = _cpuMeasureModel.GetCPUTemperature(_computer),
                Load = _cpuMeasureModel.GetCPULoad(_computer),
                Characteriscs = _cpuMeasureModel.GetCPUCharacteristics(_computer)
            };

            NetworkDataModel networkDataModel = new NetworkDataModel()
            {
                UploadSpeed = _networkMeasureModel.GetUploadSpeed(_computer),
                DownloadSpeed = _networkMeasureModel.GetDownloadSpeed(_computer),
            };

            RAMDataModel rAMDataModel = new RAMDataModel()
            {
                MemoryUsed = _ramMeasureModel.GetMemoryUsed(_computer),
                MemoryAvailable = _ramMeasureModel.GetMemoryAvailable(_computer)
            };

            _computer.Close();

            MonitoringDataModel dataModel = new MonitoringDataModel()
            {
                MeasureTime = DateTime.Now,
                CPU = CPUDataModel,
                Network = networkDataModel,
                RAM = rAMDataModel
            };

            MonitoringData.AddLast(dataModel);
        }

        private static async Task MeasureDataAsync()
        {
            await Task.Run(() => MeasureData());
        }

    }
}
