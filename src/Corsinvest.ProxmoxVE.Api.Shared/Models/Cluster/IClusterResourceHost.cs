/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Common;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster
{
    /// <summary>
    /// Base host
    /// </summary>
    public interface IClusterResourceHost : ICpu, IMemory, IClusterResourceBase, IDisk, IUptimeItem
    {
    }
}