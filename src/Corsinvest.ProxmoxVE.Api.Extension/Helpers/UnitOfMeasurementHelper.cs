/*
 * This file is part of the cv4pve-api-dotnet https://github.com/Corsinvest/cv4pve-api-dotnet,
 *
 * This source file is available under two different licenses:
 * - GNU General Public License version 3 (GPLv3)
 * - Corsinvest Enterprise License (CEL)
 * Full copyright and license information is available in
 * LICENSE.md which is distributed with this source code.
 *
 * Copyright (C) 2016 Corsinvest Srl	GPLv3 and CEL
 */

using System;

namespace Corsinvest.ProxmoxVE.Api.Extension.Helpers
{
    internal class UnitOfMeasurementHelper
    {
        public static string CPUUsageToString(double cpu, dynamic maxCpu) 
            => cpu == 0 ? "" : (Math.Round(cpu * 100, 1)) + $"% of {maxCpu} CUP";

        public static string MbToString(long value) 
            => value == 0 ? "" : Math.Round(value / 1024.0 / 1024.0, 2) + "";

        public static string GbToString(long value) 
            => value == 0 ? "" : Math.Round(value / 1024.0 / 1024.0 / 1024.0, 2) + "";

        public static string UpTimeToString(TimeSpan? upTime) 
            => upTime == null ? "" : upTime?.ToString(@"d\.hh\:mm");
    }
}