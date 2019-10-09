/*
 * This file is part of the cv4pve-api-dotnet https://github.com/Corsinvest/cv4pve-api-dotnet,
 * Copyright (C) 2016 Corsinvest Srl
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
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