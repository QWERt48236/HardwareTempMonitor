using HardwareTempMonitor.Models.CPU;
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

        public static event Action<LinkedList<MonitoringDataModel>>? OnMonitoringDataUpdate;

        private static PeriodicTimer _periodicTimer;
        private static CPUMeasureModel _cpuMeasureModel;
        private static Computer _computer;

        public static async Task StartMonitoring()
        {
            MonitoringData = new LinkedList<MonitoringDataModel>();
            _periodicTimer = SetTimer();
            _cpuMeasureModel = new CPUMeasureModel();

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
            _computer = new Computer();
            _computer.IsCpuEnabled = true;
            _computer.Open();

            CPUDataModel CPUDataModel = new CPUDataModel()
            {
                Temperature = _cpuMeasureModel.GetCPUTemperature(_computer),
                Load = _cpuMeasureModel.GetCPULoad(_computer)
            };

            MonitoringDataModel dataModel = new MonitoringDataModel()
            {
                MeasureTime = DateTime.Now,
                CPU = CPUDataModel
            };

            MonitoringData.AddLast(dataModel);

        }

        private static async Task MeasureDataAsync()
        {
            await Task.Run(() => MeasureData());
        }

    }
}
