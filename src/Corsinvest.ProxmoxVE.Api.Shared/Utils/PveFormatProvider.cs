/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using System;

namespace Corsinvest.ProxmoxVE.Api.Shared.Utils
{
    /// <summary>
    /// Pve FormatProvider
    /// </summary>
    public class PveFormatProvider : IFormatProvider, ICustomFormatter
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="formatType"></param>
        /// <returns></returns>
        public object GetFormat(Type formatType)
            => formatType == typeof(ICustomFormatter)
                ? this
                : null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            // Check whether this is an appropriate callback
            if (!this.Equals(formatProvider)) { return null; }
            //return FormatHelper.Format(format, arg);

            return format switch
            {
                FormatHelper.FormatBytes => FormatHelper.FromBytes(Convert.ToDouble(arg)),
                FormatHelper.FormatBits => FormatHelper.FromBits(Convert.ToInt64(arg)),
                FormatHelper.FormatUnixTime => DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(arg)).ToString("R"),
                FormatHelper.FormatUptimeUnixTime => FormatHelper.UptimeInfo(Convert.ToDouble(arg)),
                _ => string.Format("{0:" + format + "}", arg),
            };
        }
    }
}