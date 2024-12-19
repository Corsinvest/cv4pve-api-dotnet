/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Common;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Task data
/// </summary>
public class NodeTask : ModelBase, IStatusItem, INodeItem
{
    /// <summary>
    /// Node
    /// </summary>
    [JsonProperty("node")]
    public string Node { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    [JsonProperty("status")]
    public string Status { get; set; }

    /// <summary>
    /// Status Ok
    /// </summary>
    public bool StatusOk => Status == "OK";

    /// <summary>
    /// Vm Id
    /// </summary>
    [JsonProperty("id")]
    public string VmId { get; set; }

    /// <summary>
    /// StartTime unix time
    /// </summary>
    [JsonProperty("starttime")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatUnixTime )]
    public long StartTime { get; set; }

    /// <summary>
    /// StartTime
    /// </summary>
    public DateTime StartTimeDate => DateTimeOffset.FromUnixTimeSeconds(StartTime).DateTime;

    /// <summary>
    /// Endtime
    /// </summary>
    public DateTime EndTimeDate => DateTimeOffset.FromUnixTimeSeconds(EndTime).DateTime;

    /// <summary>
    /// Duration
    /// </summary>
    public TimeSpan? Duration
        => EndTime > 0
            ? EndTimeDate - StartTimeDate
            : null;

    /// <summary>
    /// Duration info
    /// </summary>
    public string DurationInfo
    {
        get
        {
            var durationInfo = "";
            if (Duration.HasValue)
            {
                var duration = Duration.Value;
                if (duration.TotalSeconds < 0.1) { durationInfo = "<0.1s"; }
                else if (duration.TotalMinutes < 1) { durationInfo = $"{duration.TotalSeconds}s"; }
                else { durationInfo = duration.ToString(); }
            }
            return durationInfo;
        }
    }

    /// <summary>
    /// EndTime unix time
    /// </summary>
    [JsonProperty("endtime")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatUnixTime )]
    public long EndTime { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    public string Description =>
        Type switch
        {
            "acmedeactivate" => "ACME Account Deactivate",
            "acmenewcert" => "SRV Order Certificate",
            "acmerefresh" => "ACME Account Refresh",
            "acmeregister" => "ACME Account Register",
            "acmerenew" => "SRV Renew Certificate",
            "acmerevoke" => "SRV Revoke Certificate",
            "acmeupdate" => "ACME Account Update",
            "auth-realm-sync" => "Realm Sync",
            "auth-realm-sync-test" => "Realm Sync Preview",
            "cephcreatemds" => "Ceph Metadata Server Create",
            "cephcreatemgr" => "Ceph Manager Create",
            "cephcreatemon" => "Ceph Monitor Create",
            "cephcreateosd" => "Ceph OSD Create",
            "cephcreatepool" => "Ceph Pool Create",
            "cephdestroymds" => "Ceph Metadata Server Destroy",
            "cephdestroymgr" => "Ceph Manager Destroy",
            "cephdestroymon" => "Ceph Monitor Destroy",
            "cephdestroyosd" => "Ceph OSD Destroy",
            "cephdestroypool" => "Ceph Pool Destroy",
            "cephdestroyfs" => "CephFS Destroy",
            "cephfscreate" => "CephFS Create",
            "cephsetpool" => "Ceph Pool Edit",
            "cephsetflags" => "Change global Ceph flags",
            "clustercreate" => "Create Cluster",
            "clusterjoin" => "Join Cluster",
            "dircreate" => "Directory Storage Create",
            "dirremove" => "Directory Remove",
            "download" => "File Download",
            "hamigrate" => "HA Migrate",
            "hashutdown" => "HA Shutdown",
            "hastart" => "HA Start",
            "hastop" => "HA Stop",
            "imgcopy" => "Copy data",
            "imgdel" => "Erase data",
            "lvmcreate" => "LVM Storage Create",
            "lvmremove" => "Volume Group Remove",
            "lvmthincreate" => "LVM-Thin Storage Create",
            "lvmthinremove" => "Thinpool Remove",
            "migrateall" => "Bulk migrate VMs and Containers",
            "move_volume" => $"CT {VmId} Move Volume",
            "pbs-download" => $"VM/CT {VmId} File Restore Download",
            "pull_file" => $"CT {VmId} Pull file",
            "push_file" => $"CT {VmId} Push file",
            "qmclone" => $"VM {VmId} Clone",
            "qmconfig" => $"VM {VmId} Configure",
            "qmcreate" => $"VM {VmId} Create",
            "qmdelsnapshot" => $"VM {VmId} Delete Snapshot",
            "qmdestroy" => $"VM {VmId} Destroy",
            "qmigrate" => $"VM {VmId} Migrate",
            "qmmove" => $"VM {VmId} Move disk",
            "qmpause" => $"VM {VmId} Pause",
            "qmreboot" => $"VM {VmId} Reboot",
            "qmreset" => $"VM {VmId} Reset",
            "qmrestore" => $"VM {VmId} Restore",
            "qmresume" => $"VM {VmId} Resume",
            "qmrollback" => $"VM {VmId} Rollback",
            "qmshutdown" => $"VM {VmId} Shutdown",
            "qmsnapshot" => $"VM {VmId} Snapshot",
            "qmstart" => $"VM {VmId} Start",
            "qmstop" => $"VM {VmId} Stop",
            "qmsuspend" => $"VM {VmId} Hibernate",
            "qmtemplate" => $"VM {VmId} Convert to template",
            "resize" => $"VM/CT {VmId} Resize",
            "spiceproxy" => $"VM/CT {VmId} Console (Spice)",
            "spiceshell" => "Shell (Spice)",
            "startall" => "Bulk start VMs and Containers",
            "stopall" => "Bulk shutdown VMs and Containers",
            "suspendall" => "Suspend all VMs",
            "unknownimgdel" => "Destroy image from unknown guest",
            "wipedisk" => "Device Wipe Disk",
            "vncproxy" => $"VM/CT {VmId} Console",
            "vncshell" => "Shell",
            "vzclone" => $"CT {VmId} Clone",
            "vzcreate" => $"CT {VmId} Create",
            "vzdelsnapshot" => $"CT {VmId} Delete Snapshot",
            "vzdestroy" => $"CT {VmId} Destroy",
            "vzdump" => !string.IsNullOrEmpty(VmId) ? $"VM/CT {VmId} - Backup" : "Backup Job",
            "vzmigrate" => $"CT {VmId} Migrate",
            "vzmount" => $"CT {VmId} Mount",
            "vzreboot" => $"CT {VmId} Reboot",
            "vzrestore" => $"CT {VmId} Restore",
            "vzresume" => $"CT {VmId} Resume",
            "vzrollback" => $"CT {VmId} Rollback",
            "vzshutdown" => $"CT {VmId} Shutdown",
            "vzsnapshot" => $"CT {VmId} Snapshot",
            "vzstart" => $"CT {VmId} Start",
            "vzstop" => $"CT {VmId} Stop",
            "vzsuspend" => $"CT {VmId} Suspend",
            "vztemplate" => $"CT {VmId} Convert to template",
            "vzumount" => $"CT {VmId} Unmount",
            "zfscreate" => "ZFS Storage Create",
            "zfsremove" => "ZFS Pool Remove",
            _ => Type
        };

    /// <summary>
    /// Upid
    /// </summary>
    [JsonProperty("upid")]
    public string UniqueTaskId { get; set; }

    /// <summary>
    /// User
    /// </summary>
    [JsonProperty("user")]
    public string User { get; set; }

    /// <summary>
    /// Process Id
    /// </summary>
    [JsonProperty("pid")]
    public int Pid { get; set; }
}