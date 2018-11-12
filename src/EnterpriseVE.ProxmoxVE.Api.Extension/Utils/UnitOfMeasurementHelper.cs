using System;

namespace EnterpriseVE.ProxmoxVE.Api.Extension.Utils
{
    internal class UnitOfMeasurementHelper
    {
        public static string CPUUsageToStirng(double CPU, dynamic maxcpu)
        {
            return CPU == 0 ? "" : (Math.Round(CPU * 100, 1)) + $"% of {maxcpu}CUP";
        }

        public static string MbToString(long @byte) { return @byte == 0 ? "" : (@byte / 1024 / 1024) + ""; }
        public static string GbToString(long @byte) { return @byte == 0 ? "" : (@byte / 1024 / 1024 / 1024) + ""; }
        public static string UpTimeToString(TimeSpan? upTime) { return upTime?.ToString(@"d\.hh\:mm"); }
    }
}