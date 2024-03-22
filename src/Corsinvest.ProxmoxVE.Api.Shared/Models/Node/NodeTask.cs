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
public class NodeTask : IStatusItem, INodeItem
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
    [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatUnixTime + "}")]
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
    [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatUnixTime + "}")]
    public long EndTime { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    public string Description
        => Type switch
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
            "cephfscreate" => "CephFS Create",
            "cephsetpool" => "Ceph Pool Edit",
            "cephsetflags" => "Change global Ceph flags",
            "clustercreate" => "Create Cluster",
            "clusterjoin" => "Join Cluster",
            "qmclone" => "Clone",
            "qmconfig" => "Configure",
            "qmcreate" => "Create",
            "qmdelsnapshot" => "Delete Snapshot",
            "qmdestroy" => "Destroy",
            "qmigrate" => "Migrate",
            "qmmove" => "Move disk",
            "qmpause" => "Pause",
            "qmreboot" => "Reboot",
            "qmreset" => "Reset",
            "qmrestore" => "Restore",
            "qmresume" => "Resume",
            "qmrollback" => "Rollback",
            "qmshutdown" => "Shutdown",
            "qmsnapshot" => "Snapshot",
            "qmstart" => "Start",
            "qmstop" => "Stop",
            "qmsuspend" => "Hibernate",
            "move_volume" => "Move Volume",
            "vzmigrate" => "Migrate",
            "vzmount" => "Mount",
            "vzreboot" => "Reboot",
            "vzrestore" => "Restore",
            "vzresume" => "Resume",
            "vzrollback" => "Rollback",
            "vzshutdown" => "Shutdown",
            "vzsnapshot" => "Snapshot",
            "vzstart" => "Start",
            "vzstop" => "Stop",
            "vzsuspend" => "Suspend",
            "vztemplate" => "Convert to template",
            "vzumount" => "Unmount",
            "pull_file" => "Pull file",
            "push_file" => "Push file",
            "qmtemplate" => "Convert to template",
            "spiceproxy" => "Console (Spice)",
            "spiceshell" => "Shell (Spice)",
            "startall" => "Start all VMs and Containers",
            "stopall" => "Stop all VMs and Containers",
            "unknownimgdel" => "Destroy image from unknown guest",
            "vncproxy" => "Console",
            "vncshell" => "Shell",
            "vzclone" => "Clone",
            "vzcreate" => "Create",
            "vzdelsnapshot" => "Delete Snapshot",
            "vzdestroy" => "Destroy",
            "dircreate" => "Directory Storage Create",
            "dirremove" => "Directory Remove",
            "download" => "Download",
            "hamigrate" => "HA Migrate",
            "hashutdown" => "HA Shutdown",
            "hastart" => "HA Start",
            "hastop" => "HA Stop",
            "imgcopy" => "Copy data",
            "imgdel" => "Erase data",
            "lvmcreate" => "LVM Storage Create",
            "lvmthincreate" => "LVM-Thin Storage Create",
            "migrateall" => "Migrate all VMs and Containers",
            "pbs-download" => "VM/CT File Restore Download",
            "zfscreate" => "ZFS Storage Create",
            "vzdump" => "Backup",
            _ => Type
        };

    /// <summary>
    /// Type decode
    /// </summary>
    public string DescriptionFull => (string.IsNullOrWhiteSpace(VmId) ? "" : $"VM/CT {VmId} - ") + Description;

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