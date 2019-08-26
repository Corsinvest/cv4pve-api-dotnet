using System;

namespace Corsinvest.ProxmoxVE.Api.Extension.Utils
{
    internal class UnitOfMeasurementHelper
    {
        public static string CPUUsageToStirng(double cpu, dynamic maxCpu) => cpu == 0 ? "" : (Math.Round(cpu * 100, 1)) + $"% of {maxCpu} CUP";
        public static string MbToString(long value) => value == 0 ? "" : Math.Round(value / 1024.0 / 1024.0, 2) + "";
        public static string GbToString(long value) => value == 0 ? "" : Math.Round(value / 1024.0 / 1024.0 / 1024.0, 2) + "";
        public static string UpTimeToString(TimeSpan? upTime) => upTime == null ? "" : upTime?.ToString(@"d\.hh\:mm");
    }
}