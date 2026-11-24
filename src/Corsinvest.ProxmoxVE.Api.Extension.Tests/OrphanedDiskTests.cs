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
    private const string PermissionsJson = """{"data":{"/vms":{"VM.Audit":1}}}""";

    private const string ResourcesJson = """
    {
        "data": [
            { "type": "qemu", "vmid": 100, "node": "pve01", "name": "active-vm", "status": "running" },
            { "type": "lxc", "vmid": 200, "node": "pve01", "name": "active-ct", "status": "stopped" },
            { "type": "storage", "storage": "local-lvm", "node": "pve01", "status": "available", "shared": 0, "content": "images,rootdir" },
            { "type": "storage", "storage": "backups", "node": "pve01", "status": "available", "shared": 0, "content": "backup" }
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

    private static FakeHandler Handler(string resourcesJson,
                                       Func<string, string> contentByPath,
                                       Func<HttpRequestMessage, HttpResponseMessage> other = null)
        => new(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (path.EndsWith("/access/permissions")) { return FakeHandler.Json(HttpStatusCode.OK, PermissionsJson); }
            if (path.EndsWith("/cluster/resources")) { return FakeHandler.Json(HttpStatusCode.OK, resourcesJson); }
            if (req.Method == HttpMethod.Get && path.EndsWith("/content") && contentByPath(path) is { } content)
            {
                return FakeHandler.Json(HttpStatusCode.OK, content);
            }
            return other?.Invoke(req) ?? FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

    [Fact]
    public async Task GetOrphanedDisksAsync_FindsOrphanedDisks_ExcludesActiveAndNonDisks()
    {
        var client = FakeHandler.Client(Handler(ResourcesJson, _ => StorageContentJson));
        var orphans = await DiskHelper.GetOrphanedDisksAsync(client);

        Assert.Equal(2, orphans.Count);

        // VM 999 orphaned disk
        var vmOrphan = orphans.FirstOrDefault(o => o.VmId == 999);
        Assert.NotNull(vmOrphan);
        Assert.Equal("local-lvm:vm-999-disk-0", vmOrphan.Volume);
        Assert.Equal("vm-999-disk-0", vmOrphan.FileName);
        Assert.Equal("local-lvm", vmOrphan.Storage);
        Assert.Equal("pve01", vmOrphan.Node);
        Assert.Equal("images", vmOrphan.Content);
        Assert.Equal("raw", vmOrphan.Format);
        Assert.Equal(21474836480, vmOrphan.Size);

        // CT 888 orphaned disk (vmid extracted via regex fallback)
        var ctOrphan = orphans.FirstOrDefault(o => o.VmId == 888);
        Assert.NotNull(ctOrphan);
        Assert.Equal("local-lvm:subvol-888-disk-0", ctOrphan.Volume);
        Assert.Equal("subvol-888-disk-0", ctOrphan.FileName);
        Assert.Equal("rootdir", ctOrphan.Content);
        Assert.Equal("subvol", ctOrphan.Format);

        Assert.DoesNotContain(orphans, o => o.VmId == 100);
        Assert.DoesNotContain(orphans, o => o.Content == "iso");
    }

    [Fact]
    public async Task GetOrphanedDisksAsync_UnknownVmOnOfflineNode_IsCountedAsActive_DiskNotOrphaned()
    {
        const string resourcesJson = """
        {
            "data": [
                { "type": "qemu", "vmid": 500, "node": "pve02", "name": "offline-node-vm", "status": "unknown" },
                { "type": "storage", "storage": "ceph", "node": "pve01", "status": "available", "shared": 1, "content": "images" }
            ]
        }
        """;

        const string contentJson = """
        {
            "data": [
                { "volid": "ceph:vm-500-disk-0", "vmid": 500, "content": "images", "format": "raw", "size": 10737418240 }
            ]
        }
        """;

        var client = FakeHandler.Client(Handler(resourcesJson, _ => contentJson));

        Assert.Empty(await DiskHelper.GetOrphanedDisksAsync(client));
    }

    [Fact]
    public async Task GetOrphanedDisksAsync_WithoutVmAudit_Throws_AndReadsNoStorage()
    {
        var contentRead = false;
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (path.EndsWith("/access/permissions")) { return FakeHandler.Json(HttpStatusCode.OK, """{"data":{"/vms":{}}}"""); }
            if (path.EndsWith("/content")) { contentRead = true; }
            return FakeHandler.Json(HttpStatusCode.OK, ResourcesJson);
        });

        var client = FakeHandler.Client(handler);

        await Assert.ThrowsAsync<InvalidOperationException>(() => DiskHelper.GetOrphanedDisksAsync(client));
        Assert.False(contentRead);
    }

    [Fact]
    public async Task GetOrphanedDisksAsync_BaseVolumeWithLinkedClone_IsNotOrphaned()
    {
        // Volume names as listed by the ZFS plugin: the clone volume starts with its base volume.
        // Template 777 and clone owner 778 are both gone: the clone is orphaned, its base is still in use.
        const string resourcesJson = """
        {
            "data": [
                { "type": "storage", "storage": "local-zfs", "node": "pve01", "status": "available", "shared": 0, "content": "images,rootdir" }
            ]
        }
        """;

        const string contentJson = """
        {
            "data": [
                { "volid": "local-zfs:base-777-disk-0", "vmid": 777, "content": "images", "format": "raw", "size": 10737418240 },
                { "volid": "local-zfs:base-777-disk-0/vm-778-disk-0", "vmid": 0, "content": "images", "format": "raw", "size": 10737418240, "parent": "base-777-disk-0@__base__" },
                { "volid": "local-zfs:base-779-disk-0", "vmid": 779, "content": "images", "format": "raw", "size": 10737418240 }
            ]
        }
        """;

        var client = FakeHandler.Client(Handler(resourcesJson, _ => contentJson));
        var orphans = await DiskHelper.GetOrphanedDisksAsync(client);

        Assert.Equal(["local-zfs:base-777-disk-0/vm-778-disk-0", "local-zfs:base-779-disk-0"],
                     orphans.Select(o => o.Volume).Order());

        // VMID of the clone is read from its own name, not from its base
        Assert.Equal(778, orphans.Single(o => o.Volume.EndsWith("vm-778-disk-0")).VmId);
    }

    [Fact]
    public async Task GetOrphanedDisksAsync_LocalStorageOnTwoNodes_ReportsEachNode_SharedStorageReadOnce()
    {
        const string resourcesJson = """
        {
            "data": [
                { "type": "storage", "storage": "local-zfs", "node": "pve01", "status": "available", "shared": 0, "content": "images" },
                { "type": "storage", "storage": "local-zfs", "node": "pve02", "status": "available", "shared": 0, "content": "images" },
                { "type": "storage", "storage": "ceph", "node": "pve01", "status": "available", "shared": 1, "content": "images" },
                { "type": "storage", "storage": "ceph", "node": "pve02", "status": "available", "shared": 1, "content": "images" }
            ]
        }
        """;

        var cephReads = 0;
        var client = FakeHandler.Client(Handler(resourcesJson, path =>
        {
            if (path.Contains("/storage/ceph/"))
            {
                cephReads++;
                return """{"data":[{ "volid": "ceph:vm-998-disk-0", "vmid": 998, "content": "images", "size": 1 }]}""";
            }
            return """{"data":[{ "volid": "local-zfs:vm-999-disk-0", "vmid": 999, "content": "images", "size": 1 }]}""";
        }));

        var orphans = await DiskHelper.GetOrphanedDisksAsync(client);

        Assert.Equal(["pve01", "pve02"], orphans.Where(o => o.Storage == "local-zfs").Select(o => o.Node).Order());
        Assert.Single(orphans, o => o.Storage == "ceph");
        Assert.Equal(1, cephReads);
    }

    [Fact]
    public async Task GetOrphanedDisksAsync_StorageFilter_ReadsOnlyNodesThatHaveIt()
    {
        const string resourcesJson = """
        {
            "data": [
                { "type": "storage", "storage": "local-zfs", "node": "pve01", "status": "available", "shared": 0, "content": "images" },
                { "type": "storage", "storage": "local-lvm", "node": "pve02", "status": "available", "shared": 0, "content": "images" }
            ]
        }
        """;

        var readPaths = new List<string>();
        var client = FakeHandler.Client(Handler(resourcesJson, path =>
        {
            readPaths.Add(path);
            return """{"data":[]}""";
        }));

        await DiskHelper.GetOrphanedDisksAsync(client, storage: "local-zfs");

        Assert.Equal(["/api2/json/nodes/pve01/storage/local-zfs/content"], readPaths);
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
        Assert.Equal("/api2/json/nodes/pve01/storage/local-lvm/content/local-lvm:vm-999-disk-0", deleteRequest.RequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task PurgeOrphanedDisksAsync_DryRun_DoesNotCallDelete()
    {
        var deleteCalled = false;
        var client = FakeHandler.Client(Handler(ResourcesJson, _ => StorageContentJson, req =>
        {
            if (req.Method == HttpMethod.Delete) { deleteCalled = true; }
            return null;
        }));

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
    public async Task PurgeOrphanedDisksAsync_NotDryRun_RemovedOnlyWhenDeleteTaskEndsOk()
    {
        var deleteCount = 0;
        var client = FakeHandler.Client(Handler(ResourcesJson, _ => StorageContentJson, req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (req.Method == HttpMethod.Delete)
            {
                deleteCount++;
                var id = path.Contains("vm-999") ? "ok" : "ko";
                return FakeHandler.Json(HttpStatusCode.OK, $$"""{"data":"UPID:pve01:0001:imgdel:{{id}}:root@pam:"}""");
            }
            if (path.Contains("/tasks/"))
            {
                var exitStatus = path.Contains(":ok:") ? "OK" : "volume busy";
                return FakeHandler.Json(HttpStatusCode.OK, $$$"""{"data":{"status":"stopped","exitstatus":"{{{exitStatus}}}"}}""");
            }
            return null;
        }));

        var purgeResults = await DiskHelper.PurgeOrphanedDisksAsync(client, dryRun: false);

        Assert.Equal(2, deleteCount);
        Assert.True(purgeResults.Single(r => r.Disk.VmId == 999).Removed);
        Assert.False(purgeResults.Single(r => r.Disk.VmId == 888).Removed);
    }
}
