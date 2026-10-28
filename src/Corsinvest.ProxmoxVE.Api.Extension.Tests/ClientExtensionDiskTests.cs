/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Net;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests;

/// <summary>
/// Unit tests for Storage Volume and VM/LXC Disk lifecycle extensions.
/// </summary>
public class ClientExtensionDiskTests
{
    private static JObject ParseRequestBody(HttpRequestMessage request)
    {
        var raw = request.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
        return string.IsNullOrWhiteSpace(raw) ? [] : JObject.Parse(raw);
    }

    [Fact]
    public async Task CreateDiskAsync_AllocatesVolume_WithGigabytesSize()
    {
        var handler = new FakeHandler(req =>
        {
            Assert.Equal(HttpMethod.Post, req.Method);
            Assert.Equal("/api2/json/nodes/pve01/storage/local-zfs/content", req.RequestUri!.AbsolutePath);

            var body = ParseRequestBody(req);
            Assert.Equal(100, (int)body["vmid"]!);
            Assert.Equal("20G", (string)body["size"]!);
            Assert.False(body.ContainsKey("filename"));
            Assert.False(body.ContainsKey("format"));

            return FakeHandler.Json(HttpStatusCode.OK, """{"data":"local-zfs:vm-100-disk-0"}""");
        });

        var client = FakeHandler.Client(handler);
        var result = await client.CreateDiskAsync("pve01", "local-zfs", 100, 20);

        Assert.True(result.IsSuccessStatusCode);
        Assert.Single(handler.Requests);
    }

    [Fact]
    public async Task CreateDiskAsync_AllocatesVolume_WithCustomParameters()
    {
        var handler = new FakeHandler(req =>
        {
            Assert.Equal(HttpMethod.Post, req.Method);
            Assert.Equal("/api2/json/nodes/pve01/storage/local/content", req.RequestUri!.AbsolutePath);

            var body = ParseRequestBody(req);
            Assert.Equal(101, (int)body["vmid"]!);
            Assert.Equal("32G", (string)body["size"]!);
            Assert.Equal("vm-101-disk-1.qcow2", (string)body["filename"]!);
            Assert.Equal("qcow2", (string)body["format"]!);

            return FakeHandler.Json(HttpStatusCode.OK, """{"data":"local:101/vm-101-disk-1.qcow2"}""");
        });

        var client = FakeHandler.Client(handler);
        var result = await client.CreateDiskAsync("pve01", "local", 101, "32G", "vm-101-disk-1.qcow2", "qcow2");

        Assert.True(result.IsSuccessStatusCode);
        Assert.Single(handler.Requests);
    }

    [Fact]
    public async Task DeleteDiskVolumeAsync_SendsDeleteRequest()
    {
        var handler = new FakeHandler(req =>
        {
            Assert.Equal(HttpMethod.Delete, req.Method);
            Assert.Equal("/api2/json/nodes/pve01/storage/local-zfs/content/vm-100-disk-0", req.RequestUri!.AbsolutePath);

            return FakeHandler.Json(HttpStatusCode.OK, """{"data":"UPID:pve01:00001:task"}""");
        });

        var client = FakeHandler.Client(handler);
        var result = await client.DeleteDiskVolumeAsync("pve01", "local-zfs", "vm-100-disk-0");

        Assert.True(result.IsSuccessStatusCode);
        Assert.Single(handler.Requests);
    }

    [Fact]
    public async Task FindFirstFreeScsiSlotAsync_FindsLowestUnoccupiedSlot()
    {
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (path.EndsWith("/config"))
            {
                // scsi0 and scsi1 are occupied in config
                return FakeHandler.Json(HttpStatusCode.OK, """
                    {"data":{
                        "scsi0":"local-zfs:vm-100-disk-0,size=20G",
                        "scsi1":"local-zfs:vm-100-disk-1,size=30G",
                        "digest":"abcdef123456"
                    }}
                    """);
            }
            if (path.EndsWith("/pending"))
            {
                // scsi2 is occupied in pending changes
                return FakeHandler.Json(HttpStatusCode.OK, """
                    {"data":[
                        {"key":"scsi2","value":"local-zfs:vm-100-disk-2,size=10G"}
                    ]}
                    """);
            }

            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var slot = await client.FindFirstFreeScsiSlotAsync("pve01", 100);

        Assert.Equal("scsi3", slot);
    }

    [Fact]
    public async Task FindFirstFreeScsiSlotAsync_Throws_WhenAllSlotsFull()
    {
        var handler = new FakeHandler(req =>
        {
            var configData = new JObject();
            for (var i = 0; i < 31; i++)
            {
                configData[$"scsi{i}"] = $"local-zfs:vm-100-disk-{i},size=10G";
            }
            return FakeHandler.Json(HttpStatusCode.OK, new JObject { ["data"] = configData }.ToString());
        });

        var client = FakeHandler.Client(handler);
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => client.FindFirstFreeScsiSlotAsync("pve01", 100));

        Assert.Contains("No free SCSI slots available on VM 100", ex.Message);
    }

    [Fact]
    public async Task FindFirstFreeMountPointAsync_FindsLowestUnoccupiedMountPoint()
    {
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (path.EndsWith("/config"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """
                    {"data":{
                        "mp0":"local-zfs:subvol-200-disk-0,mp=/mnt/data",
                        "digest":"fedcba654321"
                    }}
                    """);
            }
            if (path.EndsWith("/pending"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """
                    {"data":[
                        {"key":"mp1","value":"local-zfs:subvol-200-disk-1,mp=/mnt/backup"}
                    ]}
                    """);
            }

            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var slot = await client.FindFirstFreeMountPointAsync("pve01", 200);

        Assert.Equal("mp2", slot);
    }

    [Fact]
    public async Task FindFirstFreeMountPointAsync_Throws_WhenAllMountPointsFull()
    {
        var handler = new FakeHandler(req =>
        {
            var configData = new JObject();
            for (var i = 0; i < 32; i++)
            {
                configData[$"mp{i}"] = $"local-zfs:subvol-200-disk-{i},mp=/mnt/dir{i}";
            }
            return FakeHandler.Json(HttpStatusCode.OK, new JObject { ["data"] = configData }.ToString());
        });

        var client = FakeHandler.Client(handler);
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => client.FindFirstFreeMountPointAsync("pve01", 200));

        Assert.Contains("No free mount points available on Container 200", ex.Message);
    }

    [Fact]
    public async Task AttachDiskToVmAsync_AutoAssignsFreeSlot_AndPostsPayload()
    {
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (req.Method == HttpMethod.Get && path.EndsWith("/config"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """
                    {"data":{
                        "scsi0":"local-zfs:vm-100-disk-0,size=20G"
                    }}
                    """);
            }
            if (req.Method == HttpMethod.Get && path.EndsWith("/pending"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":[]}""");
            }
            if (req.Method == HttpMethod.Post && path.EndsWith("/config"))
            {
                var body = ParseRequestBody(req);
                Assert.Equal("local-zfs:vm-100-disk-1,ssd=1", (string)body["scsi1"]!);
                Assert.Equal("digest123", (string)body["digest"]!);

                return FakeHandler.Json(HttpStatusCode.OK, """{"data":"UPID:pve01:task"}""");
            }

            return FakeHandler.Json(HttpStatusCode.BadRequest, "{}");
        });

        var client = FakeHandler.Client(handler);
        var result = await client.AttachDiskToVmAsync("pve01", 100, "local-zfs:vm-100-disk-1", options: "ssd=1", digest: "digest123");

        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async Task AttachDiskToVmAsync_ExplicitSlot_SendsPayloadDirectly()
    {
        var handler = new FakeHandler(req =>
        {
            Assert.Equal(HttpMethod.Post, req.Method);
            Assert.Equal("/api2/json/nodes/pve01/qemu/100/config", req.RequestUri!.AbsolutePath);

            var body = ParseRequestBody(req);
            Assert.Equal("local-zfs:vm-100-disk-5", (string)body["sata2"]!);

            return FakeHandler.Json(HttpStatusCode.OK, """{"data":"UPID:pve01:task"}""");
        });

        var client = FakeHandler.Client(handler);
        var result = await client.AttachDiskToVmAsync("pve01", 100, "local-zfs:vm-100-disk-5", busSlot: "sata2");

        Assert.True(result.IsSuccessStatusCode);
        Assert.Single(handler.Requests);
    }

    [Fact]
    public async Task AttachDiskToVmAsync_Throws_OnInvalidBusSlot()
    {
        var handler = new FakeHandler(req => FakeHandler.Json(HttpStatusCode.OK, "{}"));
        var client = FakeHandler.Client(handler);

        await Assert.ThrowsAsync<ArgumentException>(() => client.AttachDiskToVmAsync("pve01", 100, "local-zfs:disk", busSlot: "nvme0"));
    }

    [Fact]
    public async Task DetachDiskFromVmAsync_SendsDeleteParameter()
    {
        var handler = new FakeHandler(req =>
        {
            Assert.Equal(HttpMethod.Post, req.Method);
            Assert.Equal("/api2/json/nodes/pve01/qemu/100/config", req.RequestUri!.AbsolutePath);

            var body = ParseRequestBody(req);
            Assert.Equal("scsi1", (string)body["delete"]!);
            Assert.Equal("digest999", (string)body["digest"]!);

            return FakeHandler.Json(HttpStatusCode.OK, """{"data":"UPID:pve01:task"}""");
        });

        var client = FakeHandler.Client(handler);
        var result = await client.DetachDiskFromVmAsync("pve01", 100, "scsi1", digest: "digest999");

        Assert.True(result.IsSuccessStatusCode);
        Assert.Single(handler.Requests);
    }

    [Fact]
    public async Task AttachDiskToLxcAsync_AutoAssignsMountPoint_AndPutsPayload()
    {
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (req.Method == HttpMethod.Get && path.EndsWith("/config"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":{"mp0":"local-zfs:subvol-200-disk-0,mp=/mnt/a"}}""");
            }
            if (req.Method == HttpMethod.Get && path.EndsWith("/pending"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":[]}""");
            }
            if (req.Method == HttpMethod.Put && path.EndsWith("/config"))
            {
                var body = ParseRequestBody(req);
                Assert.Equal("local-zfs:subvol-200-disk-1,mp=/mnt/data,backup=1", (string)body["mp1"]!);
                Assert.Equal("lxcDigest", (string)body["digest"]!);

                return FakeHandler.Json(HttpStatusCode.OK, """{"data":"UPID:pve01:task"}""");
            }

            return FakeHandler.Json(HttpStatusCode.BadRequest, "{}");
        });

        var client = FakeHandler.Client(handler);
        var result = await client.AttachDiskToLxcAsync("pve01", 200, "local-zfs:subvol-200-disk-1", "/mnt/data", options: "backup=1", digest: "lxcDigest");

        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async Task DetachDiskFromLxcAsync_SendsDeleteParameter()
    {
        var handler = new FakeHandler(req =>
        {
            Assert.Equal(HttpMethod.Put, req.Method);
            Assert.Equal("/api2/json/nodes/pve01/lxc/200/config", req.RequestUri!.AbsolutePath);

            var body = ParseRequestBody(req);
            Assert.Equal("mp1", (string)body["delete"]!);
            Assert.Equal("digestLxc", (string)body["digest"]!);

            return FakeHandler.Json(HttpStatusCode.OK, """{"data":"UPID:pve01:task"}""");
        });

        var client = FakeHandler.Client(handler);
        var result = await client.DetachDiskFromLxcAsync("pve01", 200, "mp1", digest: "digestLxc");

        Assert.True(result.IsSuccessStatusCode);
        Assert.Single(handler.Requests);
    }

    [Fact]
    public async Task PurgeUnusedDiskAsync_Qemu_MatchesExactVolume_AndDoesNotMatchSubstring()
    {
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (req.Method == HttpMethod.Get && path.EndsWith("/config"))
            {
                // unused0 is vm-100-disk-10 (must NOT be matched!)
                // unused1 is vm-100-disk-1 (exact match!)
                return FakeHandler.Json(HttpStatusCode.OK, """
                    {"data":{
                        "unused0":"local-zfs:vm-100-disk-10",
                        "unused1":"local-zfs:vm-100-disk-1"
                    }}
                    """);
            }
            if (req.Method == HttpMethod.Post && path.EndsWith("/config"))
            {
                var body = ParseRequestBody(req);
                Assert.Equal("unused1", (string)body["delete"]!);
                Assert.Equal("digestPurge", (string)body["digest"]!);

                return FakeHandler.Json(HttpStatusCode.OK, """{"data":"UPID:pve01:task"}""");
            }

            return FakeHandler.Json(HttpStatusCode.BadRequest, "{}");
        });

        var client = FakeHandler.Client(handler);
        var result = await client.PurgeUnusedDiskAsync("pve01", 100, VmType.Qemu, "vm-100-disk-1", digest: "digestPurge");

        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async Task PurgeUnusedDiskAsync_Lxc_MatchesExactFullVolumeId()
    {
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (req.Method == HttpMethod.Get && path.EndsWith("/config"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """
                    {"data":{
                        "unused0":"local-zfs:subvol-200-disk-0"
                    }}
                    """);
            }
            if (req.Method == HttpMethod.Put && path.EndsWith("/config"))
            {
                var body = ParseRequestBody(req);
                Assert.Equal("unused0", (string)body["delete"]!);

                return FakeHandler.Json(HttpStatusCode.OK, """{"data":"UPID:pve01:task"}""");
            }

            return FakeHandler.Json(HttpStatusCode.BadRequest, "{}");
        });

        var client = FakeHandler.Client(handler);
        var result = await client.PurgeUnusedDiskAsync("pve01", 200, VmType.Lxc, "local-zfs:subvol-200-disk-0");

        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async Task PurgeUnusedDiskAsync_Throws_WhenVolumeNotFound()
    {
        var handler = new FakeHandler(req =>
        {
            return FakeHandler.Json(HttpStatusCode.OK, """
                {"data":{
                    "unused0":"local-zfs:vm-100-disk-5"
                }}
                """);
        });

        var client = FakeHandler.Client(handler);
        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            client.PurgeUnusedDiskAsync("pve01", 100, VmType.Qemu, "vm-100-disk-1"));

        Assert.Contains("Unused disk 'vm-100-disk-1' not found", ex.Message);
    }
}
