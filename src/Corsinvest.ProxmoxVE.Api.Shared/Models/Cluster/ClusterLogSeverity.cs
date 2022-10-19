/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster
{
    /// <summary>
    /// Cluster Log Severity
    /// </summary>
    public enum ClusterLogSeverity
    {
        /// <summary>
        /// Panic
        /// </summary>
        Panic = 0,

        /// <summary>
        /// Alert
        /// </summary>
        Alert = 1,

        /// <summary>
        /// Critical
        /// </summary>
        Critical = 2,

        /// <summary>
        /// Error
        /// </summary>
        Error = 3,

        /// <summary>
        /// Warning
        /// </summary>
        Warning = 4,

        /// <summary>
        /// Notice
        /// </summary>
        Notice = 5,

        /// <summary>
        /// Info
        /// </summary>
        Info = 6,

        /// <summary>
        /// Debug
        /// </summary>
        Debug = 7
    }
}