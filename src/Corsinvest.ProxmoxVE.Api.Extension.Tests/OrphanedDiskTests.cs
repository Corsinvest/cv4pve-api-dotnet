/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Net;
using Corsinvest.ProxmoxVE.Api.Extension.Utils;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests;

/// <summary>
/// Unit tests for orphaned disk volume discovery and cleanup in <see cref="DiskHelper"/>.
/// </summary>
public class OrphanedDiskTests
{
    private const string ClusterVmsJson = """
    {
        "data": [
            { "type": "qemu", "vmid": 100, "node": "pve01", "name": "active-vm", "status": "running" },
            { "type": "lxc", "vmid": 200, "node": "pve01", "name": "active-ct", "status": "stopped" }
        ]
    }
    """;

    private const string ClusterNodesJson = """
    {
        "data": [
            { "type": "node", "node": "pve01", "status": "online" }
        ]
    }
    """;

    private const string NodeStoragesJson = """
    {
        "data": [
            { "storage": "local-lvm", "active": 1, "content": "images,rootdir", "enabled": 1 },
            { "storage": "backups", "active": 1, "content": "backup", "enabled": 1 }
        ]
    }
    """;

    private const string StorageContentJson = """
    {
        "data": [
            { "volid": "local-lvm:vm-100-disk-0", "vmid": 100, "content": "images", "format": "raw", "size": 10737418240 },
            { "volid": "local-lvm:vm-999-disk-0", "vmid": 999, "content": "images", "format": "raw", "size": 21474836480 },
            { "volid": "local-lvm:subvol-888-disk-0", "vmid": 0, "content": "rootdir", "format": "subvol", "size": 8589934592 },
            { "volid": "local-lvm:iso/debian.iso", "content": "iso", "format": "iso", "size": 524288000 }
        ]
    }
    """;

    private static HttpResponseMessage HandleClusterOrStorage(HttpRequestMessage req)
    {
        var path = req.RequestUri!.AbsolutePath;
        var query = req.RequestUri.Query;

        if (path.Contains("/cluster/resources"))
        {
            if (query.Contains("type=node"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, ClusterNodesJson);
            }
            return FakeHandler.Json(HttpStatusCode.OK, ClusterVmsJson);
        }
        if (path.EndsWith("/storage"))
        {
            return FakeHandler.Json(HttpStatusCode.OK, NodeStoragesJson);
        }
        if (path.EndsWith("/content"))
        {
            return FakeHandler.Json(HttpStatusCode.OK, StorageContentJson);
        }
        return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
    }

    [Fact]
    public async Task GetOrphanedDisksAsync_FindsOrphanedDisks_ExcludesActiveAndNonDisks()
    {
        var handler = new FakeHandler(HandleClusterOrStorage);
        var client = FakeHandler.Client(handler);
        var orphans = await DiskHelper.GetOrphanedDisksAsync(client);

        Assert.NotNull(orphans);
        Assert.Equal(2, orphans.Count);

        // VM 999 orphaned disk
        var vmOrphan = orphans.FirstOrDefault(o => o.VmId == 999);
        Assert.NotNull(vmOrphan);
        Assert.Equal("local-lvm:vm-999-disk-0", vmOrphan.Volume);
        Assert.Equal("vm-999-disk-0", vmOrphan.FileName);
        Assert.Equal("local-lvm", vmOrphan.Storage);
        Assert.Equal("images", vmOrphan.Content);
        Assert.Equal("raw", vmOrphan.Format);
        Assert.Equal(21474836480, vmOrphan.Size);
        Assert.Equal("21.47 GB", vmOrphan.SizeInfo);

        // CT 888 orphaned disk (vmid extracted via regex fallback)
        var ctOrphan = orphans.FirstOrDefault(o => o.VmId == 888);
        Assert.NotNull(ctOrphan);
        Assert.Equal("local-lvm:subvol-888-disk-0", ctOrphan.Volume);
        Assert.Equal("subvol-888-disk-0", ctOrphan.FileName);
        Assert.Equal("rootdir", ctOrphan.Content);
        Assert.Equal("subvol", ctOrphan.Format);

        // Active VM 100 disk must NOT be reported
        Assert.DoesNotContain(orphans, o => o.VmId == 100);

        // ISO file must NOT be reported
        Assert.DoesNotContain(orphans, o => o.Content == "iso");
    }

    [Fact]
    public async Task GetOrphanedDisksAsync_UnknownVmOnOfflineNode_IsCountedAsActive_DiskNotOrphaned()
    {
        const string unknownVmsJson = """
        {
            "data": [
                { "type": "qemu", "vmid": 500, "node": "pve02", "name": "offline-node-vm", "status": "unknown" }
            ]
        }
        """;

        const string sharedStorageContentJson = """
        {
            "data": [
                { "volid": "local-lvm:vm-500-disk-0", "vmid": 500, "content": "images", "format": "raw", "size": 10737418240 }
            ]
        }
        """;

        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            var query = req.RequestUri.Query;
            if (path.Contains("/cluster/resources"))
            {
                if (query.Contains("type=node"))
                {
                    return FakeHandler.Json(HttpStatusCode.OK, ClusterNodesJson);
                }
                return FakeHandler.Json(HttpStatusCode.OK, unknownVmsJson);
            }
            if (path.EndsWith("/storage")) { return FakeHandler.Json(HttpStatusCode.OK, NodeStoragesJson); }
            if (path.EndsWith("/content")) { return FakeHandler.Json(HttpStatusCode.OK, sharedStorageContentJson); }
            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var orphans = await DiskHelper.GetOrphanedDisksAsync(client);

        // VM 500 is unknown on offline node pve02, but must count as active so its disk is not reported as orphan
        Assert.Empty(orphans);
    }

    [Fact]
    public async Task GetOrphanedDisksAsync_CrossStorage_BaseImageProtectedByLinkedCloneOnAnotherStorage()
    {
        const string twoStoragesJson = """
        {
            "data": [
                { "storage": "local-zfs", "active": 1, "content": "images", "enabled": 1 },
                { "storage": "ceph", "active": 1, "content": "images", "enabled": 1 }
            ]
        }
        """;

        const string localZfsContentJson = """
        {
            "data": [
                { "volid": "local-zfs:base-777-disk-0", "vmid": 777, "content": "images", "format": "raw", "size": 10737418240 }
            ]
        }
        """;

        const string cephContentJson = """
        {
            "data": [
                { "volid": "ceph:vm-100-disk-1", "vmid": 100, "content": "images", "format": "raw", "size": 10737418240, "parent": "local-zfs:base-777-disk-0" }
            ]
        }
        """;

        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            var query = req.RequestUri.Query;
            if (path.Contains("/cluster/resources"))
            {
                if (query.Contains("type=node")) { return FakeHandler.Json(HttpStatusCode.OK, ClusterNodesJson); }
                return FakeHandler.Json(HttpStatusCode.OK, ClusterVmsJson);
            }
            if (path.EndsWith("/storage")) { return FakeHandler.Json(HttpStatusCode.OK, twoStoragesJson); }
            if (path.Contains("/storage/local-zfs/content")) { return FakeHandler.Json(HttpStatusCode.OK, localZfsContentJson); }
            if (path.Contains("/storage/ceph/content")) { return FakeHandler.Json(HttpStatusCode.OK, cephContentJson); }
            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var orphans = await DiskHelper.GetOrphanedDisksAsync(client);

        // Base disk 777 on local-zfs is protected by clone on ceph across different storages
        Assert.Empty(orphans);
    }

    [Fact]
    public async Task RemoveOrphanedDiskAsync_CallsDeleteEndpoint_WithCorrectVolume()
    {
        HttpRequestMessage deleteRequest = null;
        var handler = new FakeHandler(req =>
        {
            if (req.Method == HttpMethod.Delete)
            {
                deleteRequest = req;
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":"UPID:pve01:0001:pvesm:root@pam:"}""");
            }
            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var result = await DiskHelper.RemoveOrphanedDiskAsync(client, "pve01", "local-lvm", "local-lvm:vm-999-disk-0");

        Assert.True(result.IsSuccessStatusCode);
        Assert.NotNull(deleteRequest);
        Assert.Equal(HttpMethod.Delete, deleteRequest.Method);
        Assert.Equal("/api2/json/nodes/pve01/storage/local-lvm/content/local-lvm:vm-999-disk-0", deleteRequest.RequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task PurgeOrphanedDisksAsync_DryRun_DoesNotCallDelete()
    {
        var deleteCalled = false;
        var handler = new FakeHandler(req =>
        {
            if (req.Method == HttpMethod.Delete)
            {
                deleteCalled = true;
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":"UPID:..."}""");
            }
            return HandleClusterOrStorage(req);
        });

        var client = FakeHandler.Client(handler);
        var purgeResults = await DiskHelper.PurgeOrphanedDisksAsync(client, dryRun: true);

        Assert.Equal(2, purgeResults.Count);
        Assert.All(purgeResults, r =>
        {
            Assert.False(r.Removed);
            Assert.Null(r.Result);
        });
        Assert.False(deleteCalled);
    }

    [Fact]
    public async Task PurgeOrphanedDisksAsync_NotDryRun_CallsDeleteForEachOrphan()
    {
        var deleteCount = 0;
        var handler = new FakeHandler(req =>
        {
            if (req.Method == HttpMethod.Delete)
            {
                deleteCount++;
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":"UPID:pve01:0001:pvesm:root@pam:"}""");
            }
            return HandleClusterOrStorage(req);
        });

        var client = FakeHandler.Client(handler);
        var purgeResults = await DiskHelper.PurgeOrphanedDisksAsync(client, dryRun: false);

        Assert.Equal(2, purgeResults.Count);
        Assert.All(purgeResults, r =>
        {
            Assert.True(r.Removed);
            Assert.NotNull(r.Result);
            Assert.True(r.Result.IsSuccessStatusCode);
        });
        Assert.Equal(2, deleteCount);
    }
}
