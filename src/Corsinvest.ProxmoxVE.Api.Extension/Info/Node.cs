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

using System.Collections.Generic;

namespace Corsinvest.ProxmoxVE.Api.Extension.Info
{
    /// <summary>
    /// Node Info
    /// </summary>
    public class Node
    {
        /// <summary>
        /// Status
        /// </summary>
        /// <value></value>
        public dynamic Status { get; set; }

        /// <summary>
        /// Apt Update
        /// </summary>
        /// <value></value>
        public IEnumerable<dynamic> AptUpdate { get; set; }

        /// <summary>
        /// Disks
        /// </summary>
        /// <returns></returns>
        public NodeDisks Disks { get; set; } = new NodeDisks();

        /// <summary>
        /// Ceph
        /// </summary>
        /// <returns></returns>
        public NodeCeph Ceph { get; set; } = new NodeCeph();

        /// <summary>
        /// Dns
        /// </summary>
        /// <value></value>
        public dynamic Dns { get; set; }

        /// <summary>
        /// Hosts
        /// </summary>
        /// <value></value>
        public dynamic Hosts { get; set; }

        /// <summary>
        /// Netstat
        /// </summary>
        /// <value></value>
        public IEnumerable<dynamic> Netstat { get; set; }

        /// <summary>
        /// Network
        /// </summary>
        /// <value></value>
        public IEnumerable<dynamic> Network { get; set; }

        /// <summary>
        /// Services
        /// </summary>
        /// <value></value>
        public IEnumerable<dynamic> Services { get; set; }

        /// <summary>
        /// Subscription
        /// </summary>
        /// <value></value>
        public dynamic Subscription { get; set; }

        /// <summary>
        /// Version
        /// </summary>
        /// <value></value>
        public dynamic Version { get; set; }

        /// <summary>
        /// Tasks
        /// </summary>
        /// <value></value>
        public IEnumerable<dynamic> Tasks { get; set; }

        /// <summary>
        /// Report
        /// </summary>
        /// <value></value>
        public dynamic Report { get; set; }

        /// <summary>
        /// Package Versions
        /// </summary>
        /// <value></value>
        public IEnumerable<dynamic> PackageVersions { get; set; }

        /// <summary>
        /// Time zone
        /// </summary>
        /// <value></value>
        public string Timezone { get; set; }

        /// <summary>
        /// RrdData
        /// </summary>
        public RrdData RrdData { get; set; } = new RrdData();

        /// <summary>
        /// Firewall
        /// </summary>
        public BaseFirewall Firewall { get; set; } = new BaseFirewall();

        /// <summary>
        /// Resources
        /// </summary>
        /// <value></value>
        public IEnumerable<dynamic> Resources { get; set; }

        /// <summary>
        /// Replication
        /// </summary>
        /// <value></value>
        public IEnumerable<dynamic> Replication { get; set; }

        /// <summary>
        /// Hardware
        /// </summary>
        /// <returns></returns>
        public NodeHardware Hardware { get; set; } = new NodeHardware();

        /// <summary>
        /// Certificates
        /// </summary>
        /// <value></value>
        public IEnumerable<dynamic> Certificates { get; set; }
    }
}